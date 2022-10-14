﻿/*
Copyright 2019 - 2021 Inetum

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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using umi3d.common;
using UnityEngine;
using UnityEngine.Networking;

namespace umi3d.cdk
{
    /// <summary>
    /// Resource Loader for Image.
    /// </summary>
    public class ImageDtoLoader : IResourcesLoader
    {
        public List<string> supportedFileExtentions;
        public List<string> ignoredFileExtentions;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ImageDtoLoader()
        {
            this.supportedFileExtentions = new List<string>() { ".jpg", ".bmp", ".dib", ".rle", ".exr", ".gif", ".hdr", ".iff", ".jpeg", ".pict", ".pct", ".png", ".psd", ".tga", ".tif", ".tiff", ".TGA", ".PNG", ".JPG", ".JPEG" };
            this.ignoredFileExtentions = new List<string>();
        }

        /// <see cref="IResourcesLoader.IsSuitableFor"/>
        public bool IsSuitableFor(string extension)
        {
            return supportedFileExtentions.Contains(extension);
        }

        /// <see cref="IResourcesLoader.IsToBeIgnored"/>
        public bool IsToBeIgnored(string extension)
        {
            return ignoredFileExtentions.Contains(extension);
        }

        /// <see cref="IResourcesLoader.UrlToObject"/>
        public virtual async Task<object> UrlToObject(string url, string extension, string authorization, string pathIfObjectInBundle = "")
        {
#if UNITY_ANDROID
            UnityWebRequest www = url.Contains("http") ? UnityWebRequestTexture.GetTexture(url) : UnityWebRequestTexture.GetTexture("file://" + url);
#else
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
#endif

            SetCertificate(www, authorization);
            await UMI3DResourcesManager.DownloadObject(www);
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            www.Dispose();
            return texture;
        }

        /// <see cref="IResourcesLoader.ObjectFromCache"/>
        public virtual async Task<object> ObjectFromCache(object o, string pathIfObjectInBundle)
        {
            return (o);
        }

        /// <summary>
        /// Set Webrequest Certificate
        /// </summary>
        /// <param name="www">web request</param>
        /// <param name="fileAuthorization">authorization</param>
        public virtual void SetCertificate(UnityWebRequest www, string fileAuthorization)
        {
            if (fileAuthorization != null && fileAuthorization != "")
            {
                string authorization = fileAuthorization;

                if (!UMI3DClientServer.Instance.AuthorizationInHeader && www.url.StartsWith("http"))
                {
                    www.url = UMI3DResourcesManager.Instance.SetAuthorisationWithParameter(www.url, fileAuthorization);
                }
                else
                {
                    www.SetRequestHeader(UMI3DNetworkingKeys.Authorization, authorization);
                }
            }
        }

        /// <see cref="IResourcesLoader.DeleteObject"/>
        public void DeleteObject(object objectLoaded, string reason)
        {
            GameObject.Destroy(objectLoaded as UnityEngine.Object);
        }
    }
}