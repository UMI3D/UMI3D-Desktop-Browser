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
using umi3d.cdk.collaboration;
using umi3d.common;

namespace umi3d.browserRuntime.connection
{
    public class AssetFormat
    {
        public AssetFormat(UMI3DCollabLoadingParameters loadingParameters) 
        {
            loadingParameters.supportedformats.Clear();

            loadingParameters.supportedformats.AddRange(
                new string[]
                {
                    UMI3DAssetFormat.gltf,
                    UMI3DAssetFormat.obj,
                    UMI3DAssetFormat.fbx,
                    UMI3DAssetFormat.png,
                    UMI3DAssetFormat.jpg
                }
            );

#if UNITY_STANDALONE || UNITY_EDITOR
            // For Windows.
            loadingParameters.supportedformats.Add(UMI3DAssetFormat.unity_standalone_urp);
#elif UNITY_ANDROID
            // For Android.
            loadingParameters.supportedformats.Add(UMI3DAssetFormat.unity_android_urp);
#endif
        }
    }
}
