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
using System.Threading;
using NUnit.Framework;
using umi3d.browserRuntime.connection;
using UnityEngine;
using UnityEngine.TestTools;

public class ConnectionToTest
{
    IConnectionTo MasterServerConnection
    {
        get
        {
            return new ConnectionToMasterServer();
        }
    }

    string ValidURL
    {
        get
        {
            return "https://intraverse-inetum.com";
        }
    }

    [UnityTest]
    public IEnumerator ConnectionToMasterServer()
    {
        IConnectionTo connectionTo = MasterServerConnection;

        connectionTo.Canceled += connectionHandler =>
        {
            UnityEngine.Debug.Log($"{Thread.CurrentThread.ManagedThreadId}");
        };

        var connectionTask = connectionTo.Connect(url: "0:0");

        while (!connectionTask.IsCompleted)
        {
            yield return null;
        }
        //Assert.AreEqual("blabealb", connectionTo.Error);
        //Assert.AreEqual("blabealb", connectionTo.Result);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator ConnectionToTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
