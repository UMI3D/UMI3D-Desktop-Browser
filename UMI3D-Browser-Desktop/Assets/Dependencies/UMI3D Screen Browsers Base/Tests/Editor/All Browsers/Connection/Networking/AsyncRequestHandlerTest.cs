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
using NUnit.Framework;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using umi3d.browserRuntime.connection;
using umi3d.common;
using umi3d.testsUtils;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

public class AsyncRequestHandlerTest
{
    IAsyncRequestHandler GetRequestHandler(string url)
    {
        return new AsyncRequestedHandler(webRequestFactory: tries =>
        {
            return Task.FromResult(UnityWebRequest.Get(url));
        });
    }

    string ValidURL
    {
        get
        {
            return "https://intraverse-inetum.com";
        }
    }

    [Test, Performance]
    public void Perf_Execution()
    {
        IAsyncRequestHandler requestHandler = null;

        Measure.Method(async () =>
        {
            await requestHandler.Execute();
        })
            .WarmupCount(10)
            .MeasurementCount(30)
            .SetUp(() =>
            {
                requestHandler = GetRequestHandler("failAddress");
            })
            .Run();

        PerformanceTest info = PerformanceTest.Active;
        info.CalculateStatisticalValues();

        var time = info.SampleGroups.Find(s => s.Name == "Time");
        Assert.LessOrEqual(time.Max, .32f);
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
        var watch = System.Diagnostics.Stopwatch.StartNew();

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
            // Request take usually 3s to be performed.
            watch.CheckExecutionTime(3000);
            yield return null;
        }

        watch.Stop();
    }

    [UnityTest]
    public IEnumerator HasBeenCanceled()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        IAsyncRequestHandler requestHandler = GetRequestHandler("failAddress");

        Assert.IsTrue(!requestHandler.HasBeenCanceled);
        requestHandler.Execute();
        requestHandler.Abort();
        Assert.IsTrue(requestHandler.HasBeenCanceled);

        bool completed = false;
        requestHandler.Completed += handler =>
        {
            Assert.Fail("Should not end here.");
        };
        requestHandler.Aborted += handler =>
        {
            completed = true;
            Assert.IsTrue(requestHandler.HasBeenCanceled);
        };

        while (!completed)
        {
            // Request take usually 3s to be performed.
            watch.CheckExecutionTime(3000);
            yield return null;
        }

        watch.Stop();
    }

    [UnityTest]
    public IEnumerator Progress()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

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
            // Request take usually 3s to be performed.
            watch.CheckExecutionTime(3000);
            yield return null;
        }

        watch.Stop();
    }

    [UnityTest]
    public IEnumerator Result()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        IAsyncRequestHandler requestHandler0 = GetRequestHandler("failAddress");
        IAsyncRequestHandler requestHandler1 = GetRequestHandler(URLFormat.URLToMediaURL(ValidURL));

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
            Assert.AreEqual(UnityWebRequest.Result.Success, handler.Result, $"Be sure to have launch a world before testing.");
            Assert.AreEqual(null, handler.Error);
            Assert.AreNotEqual(null, handler.DownloadedText);
            Assert.AreNotEqual(null, handler.GetDownloadedData<MediaDto>());
        };

        while (completed < 2)
        {
            // Request take usually 3s to be performed.
            watch.CheckExecutionTime(3000);
            yield return null;
        }

        watch.Stop();
    }

    [UnityTest]
    public IEnumerator Retry()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        IAsyncRequestHandler requestHandler = GetRequestHandler("failAddress");

        Assert.AreEqual(-1, requestHandler.Retry().Result);
        UnityEngine.TestTools.LogAssert.Expect(LogType.Error, "You cannot retry a request that as not been executed or finished.");

        var ExecuteTask = requestHandler.Execute();
        Assert.AreEqual(-1, requestHandler.Retry().Result);
        UnityEngine.TestTools.LogAssert.Expect(LogType.Error, "You cannot retry a request that as not been executed or finished.");

        while (!ExecuteTask.IsCompleted)
        {
            // Request take usually 3s to be performed.
            watch.CheckExecutionTime(3000);
            yield return null;
        }

        var retryTask = requestHandler.Retry();

        while (!retryTask.IsCompleted)
        {
            // Request take usually 3s to be performed.
            watch.CheckExecutionTime(6000);
            yield return null;
        }
        Assert.AreEqual(2, retryTask.Result);
        Assert.AreEqual(2, requestHandler.TryCount);

        watch.Stop();
    }
}
