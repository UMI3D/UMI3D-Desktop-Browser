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

using System.Collections.Generic;

namespace umi3d.common
{
    /// <summary>
    /// DTO describing a resource, a list of variants for a file.
    /// </summary>
    /// Variants are used to load a file instead of another for optimization purposes.
    [System.Serializable]
    public class ResourceDto : UMI3DDto
    {
        /// <summary>
        /// File variants representing a same resource.
        /// </summary>
        /// Variants are used to load a file instead of another for optimization purposes.
        public List<FileDto> variants { get; set; } = new List<FileDto>();
    }
}
