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
    [System.Serializable]

    /// <summary>
    /// Should follow glTF 2.0 specifications.
    /// https://github.com/KhronosGroup/glTF/tree/master/specification/2.0#materials
    /// UMI3D-based textures using ResourceDto should be defined in the umi3d extension.
    /// </summary>
    public class GlTFMaterialDto : UMI3DDto, IEntity
    {
        public string alphaMode { get; set; }
        public bool doubleSided { get; set; } = false;
        public string name { get; set; }
        public PBRMaterialDto pbrMetallicRoughness { get; set; } = new PBRMaterialDto();
        public GlTFMaterialExtensions extensions { get; set; } = null;


        public Vector3Dto emissiveFactor { get; set; }
    }
}
