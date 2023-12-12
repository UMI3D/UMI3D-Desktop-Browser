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
using umi3d.testsUtils;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

public class MediaDTOWebRequestTest
{
    IMediaDTOWebRequest GetMediaDTOWebRequest(IConnectionStateData connectionStateData)
    {
        return new MediaDTOWebRequest(connectionStateData);
    }

    string ValidURL
    {
        get
        {
            return "https://intraverse-inetum.com";
        }
    }

    [Test]
    public void IsUrlFormatValid()
    {
        IMediaDTOWebRequest mediaDTOWebRequest = GetMediaDTOWebRequest(new ConnectionStateData());

        Assert.IsFalse(mediaDTOWebRequest.IsUrlFormatValid(null));
        Assert.IsFalse(mediaDTOWebRequest.IsUrlFormatValid("/media"));
        Assert.IsTrue(mediaDTOWebRequest.IsUrlFormatValid("1/media"));
    }

    [Test]
    public void URLToMediaURL()
    {
        IMediaDTOWebRequest mediaDTOWebRequest = GetMediaDTOWebRequest(new ConnectionStateData());

        Assert.AreEqual(null, mediaDTOWebRequest.URLToMediaURL(null));
        UnityEngine.TestTools.LogAssert.Expect(LogType.Error, "url is null or empty.");

        Assert.AreEqual("http://1/media", mediaDTOWebRequest.URLToMediaURL("1"));
    }

    [UnityTest]
    public IEnumerator RequestMedia()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        IConnectionStateData connectionStateData = new ConnectionStateData();
        IMediaDTOWebRequest mediaDTOWebRequest = GetMediaDTOWebRequest(connectionStateData);

        string mediaURL = null;
        IAsyncRequestHandler requestHandler = null;

        //------- First ----------------
        mediaURL = mediaDTOWebRequest.URLToMediaURL("failUrl");
        requestHandler = mediaDTOWebRequest.RequestMediaDto(mediaURL);
        UnityEngine.TestTools.LogAssert.Expect(LogType.Error, "MediaDTO request failed after 3 tries.");

        while (!requestHandler.RequestTask.IsCompleted)
        {
            // Request take usually 3s to be performed.
            watch.CheckExecutionTime(11_000);
            yield return null;
        }
        Assert.AreEqual(3, requestHandler.TryCount);
        Assert.AreEqual(UnityWebRequest.Result.ConnectionError, requestHandler.Result);

        //------- Second ----------------
        mediaURL = mediaDTOWebRequest.URLToMediaURL(ValidURL);
        requestHandler = mediaDTOWebRequest.RequestMediaDto(mediaURL);

        while (!requestHandler.RequestTask.IsCompleted)
        {
            // Request take usually 3s to be performed.
            watch.CheckExecutionTime(15_000);
            yield return null;
        }
        Assert.AreEqual(UnityWebRequest.Result.Success, requestHandler.Result);

        //------- Third ----------------
        mediaURL = mediaDTOWebRequest.URLToMediaURL("failUrl");
        requestHandler = mediaDTOWebRequest.RequestMediaDto(mediaURL);
        connectionStateData.Add(new MasterServerSessionConnectionState(), new MasterServerSessionConnectionState().Id);

        while (!requestHandler.RequestTask.IsCompleted)
        {
            // Request take usually 3s to be performed.
            watch.CheckExecutionTime(15_000);
            yield return null;
        }
        Assert.True(requestHandler.HasBeenCanceled);
        Assert.AreEqual("Request aborted", requestHandler.Error);
        Assert.AreEqual(UnityWebRequest.Result.ConnectionError, requestHandler.Result);
    }
}
