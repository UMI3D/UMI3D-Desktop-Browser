/*
Copyright 2019 - 2023 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using umi3d.browserRuntime.connection;
using umi3d.common;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

public class AsyncRequestHandlerTest
{
    IAsyncRequestHandler GetRequestHandler(string url)
    {
        return new AsyncRequestedHandler(UnityWebRequest.Get(url));
    }

    string ValidURL
    {
        get
        {
            return "https://intraverse-inetum.com";
        }
    }

    [Test]
    public void DoubleExecution()
    {
        IAsyncRequestHandler requestHandler = GetRequestHandler("failAddress");
        requestHandler.Execute();
        requestHandler.Execute();
        UnityEngine.TestTools.LogAssert.Expect(LogType.Error, "Execute method can only be called once.");
    }

    [UnityTest]
    public IEnumerator IsDone()
    {
        IAsyncRequestHandler requestHandler = GetRequestHandler("failAddress");

        Assert.IsTrue(!requestHandler.IsDone);
        requestHandler.Execute();
        Assert.IsTrue(!requestHandler.IsDone);

        bool completed = false;
        requestHandler.Completed += handler =>
        {
            completed = true;
            Assert.IsTrue(handler.IsDone);
        };

        while (!completed)
        {
            yield return null;
        }
    }

    [UnityTest]
    public IEnumerator HasBeenCanceled()
    {
        IAsyncRequestHandler requestHandler = GetRequestHandler("failAddress");

        Assert.IsTrue(!requestHandler.HasBeenCanceled);
        requestHandler.Execute();
        requestHandler.Abort();
        Assert.IsTrue(requestHandler.HasBeenCanceled);

        bool completed = false;
        requestHandler.Completed += handler =>
        {
            completed = true;
            Assert.IsTrue(requestHandler.HasBeenCanceled);
        };

        while (!completed)
        {
            yield return null;
        }
    }

    [UnityTest]
    public IEnumerator Progress()
    {
        IAsyncRequestHandler requestHandler = GetRequestHandler(ValidURL);

        Assert.AreEqual(0f, requestHandler.Progress);
        requestHandler.Execute();

        bool completed = false;
        requestHandler.Completed += handler =>
        {
            completed = true;
            Assert.AreEqual(1f, requestHandler.Progress);
        };

        while (!completed)
        {
            yield return null;
        }
    }

    [UnityTest]
    public IEnumerator Result()
    {
        IAsyncRequestHandler requestHandler0 = GetRequestHandler("failAddress");
        IAsyncRequestHandler requestHandler1 = GetRequestHandler($"{URLFormat.URLToMediaURL(ValidURL)}");
        Assert.AreEqual(requestHandler0.Result, UnityWebRequest.Result.InProgress);
        requestHandler0.Execute();
        Assert.AreEqual(requestHandler0.Result, UnityWebRequest.Result.InProgress);
        Assert.AreEqual(requestHandler1.Result, UnityWebRequest.Result.InProgress);
        requestHandler1.Execute();
        Assert.AreEqual(requestHandler1.Result, UnityWebRequest.Result.InProgress);

        int completed = 0;
        requestHandler0.Completed += handler =>
        {
            completed++;
            Assert.AreEqual(UnityWebRequest.Result.ConnectionError, handler.Result);
            Assert.AreEqual("Cannot connect to destination host", handler.Error);
        };
        requestHandler1.Completed += handler =>
        {
            completed++;
            Assert.AreEqual(UnityWebRequest.Result.Success, handler.Result);
        };

        while (completed < 2)
        {
            yield return null;
        }
    }


    //[UnityTest]
    //public IEnumerator RequestSucceeded()
    //{
    //    IAsyncRequestHandler requestHandler = null;

    //    while (!requestHandler.IsDone)
    //    {
    //        yield return null;
    //    }

    //    if (requestHandler.Result == UnityEngine.Networking.UnityWebRequest.Result.Success)
    //    {
    //        Assert.True(requestHandler.DownloadedData != null);
    //    }
    //    else if (requestHandler.Result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError)
    //    {
    //        Assert.Fail("Request has not reached the server");
    //    }
    //    else if (requestHandler.Result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
    //    {
    //        Assert.Fail("The server returned an error response");
    //    }
    //    else
    //    {
    //        Assert.Fail("The data was corrupted or not in the correct format");
    //    }
    //}
}
