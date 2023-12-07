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
using umi3d.browserRuntime.connection;
using UnityEngine;

public class URLFormatTest
{
    [Test]
    public void IpPortToURL()
    {
        Assert.AreEqual(null, URLFormat.IpPortToURL(null, null));
        UnityEngine.TestTools.LogAssert.Expect(LogType.Error, "Ip is empty or null.");
        Assert.AreEqual(null, URLFormat.IpPortToURL("", ""));
        UnityEngine.TestTools.LogAssert.Expect(LogType.Error, "Ip is empty or null.");
        Assert.AreEqual(null, URLFormat.IpPortToURL(null, "2"));
        UnityEngine.TestTools.LogAssert.Expect(LogType.Error, "Ip is empty or null.");

        Assert.AreEqual("http://1", URLFormat.IpPortToURL("1", null));
        Assert.AreEqual("http://1", URLFormat.IpPortToURL("1", ""));
        Assert.AreEqual("http://1", URLFormat.IpPortToURL("http://1", ""));
        Assert.AreEqual("http://1:2", URLFormat.IpPortToURL("1", "2"));
        Assert.AreEqual("http://1", URLFormat.IpPortToURL("http://1", ""));
    }

    [Test]
    public void URLToMediaURL()
    {
        Assert.AreEqual(null, URLFormat.URLToMediaURL(null));
        UnityEngine.TestTools.LogAssert.Expect(LogType.Error, "url is null or empty.");
        Assert.AreEqual(null, URLFormat.URLToMediaURL(""));
        UnityEngine.TestTools.LogAssert.Expect(LogType.Error, "url is null or empty.");

        Assert.AreEqual("http://1/media", URLFormat.URLToMediaURL("1"));
    }
}
