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

namespace umi3d.common
{
    /// <summary>
    /// DTO describing the meta-data measuring an asset.
    /// </summary>
    [System.Serializable]
    public class AssetMetricDto : UMI3DDto
    {
        /// <summary>
        /// Arbitrary level of resolution from low to higher resolution.
        /// </summary>
        public int resolution { get; set; } = 1;

        /// <summary>
        /// File size in Mb.
        /// </summary>
        public float size { get; set; } = 0f;
    }
}
