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
using UnityEngine;
using UnityEngine.TestTools;

public class MediaDTOWebRequestTest
{
    IMediaDTOWebRequest mediaDTOWebRequest
    {
        get
        {
            //return new MediaDTOWebRequest();
            return null;
        }
    }

    [Test]
    public void IsUrlFormatValid()
    {
        IMediaDTOWebRequest mediaDTOWebRequest = null;

        Assert.IsFalse(mediaDTOWebRequest.IsUrlFormatValid(null));
        Assert.IsFalse(mediaDTOWebRequest.IsUrlFormatValid("/media"));
        Assert.IsTrue(mediaDTOWebRequest.IsUrlFormatValid("1/media"));
    }

    [Test]
    public void URLToMediaURL()
    {
        IMediaDTOWebRequest mediaDTOWebRequest = null;

        Assert.AreEqual(null, mediaDTOWebRequest.URLToMediaURL(null));
        UnityEngine.TestTools.LogAssert.Expect(LogType.Error, "Execute method can only be called once.");

        Assert.AreEqual("http://1/media", mediaDTOWebRequest.URLToMediaURL("1"));
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator MediaDTOWebRequestTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
