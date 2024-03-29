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
using umi3d.common.userCapture.description;

namespace umi3d.common.collaboration.dto.signaling
{
    /// <summary>
    /// DTO describing user configuration when joining an environment.
    /// </summary>
    public class JoinDto : UMI3DDto
    {
        /// <summary>
        /// The local poses from the client
        /// </summary>
        public List<PoseDto> clientLocalPoses { get; set; }

        /// <summary>
        /// User size scale relative to the environment.
        /// </summary>
        public Vector3Dto userSize { get; set; }

        /// <summary>
        /// True if the browser uses purely virtual immersion and not any form of Mixed Reality.
        /// </summary>
        public bool hasImmersiveDevice { get; set; }

        /// <summary>
        /// True if the browser uses immersive display.
        /// </summary>
        public bool hasHeadMountedDisplay { get; set; }

        /// <summary>
        /// BoneType of bones with controllers on browser. <br/>
        /// E.g. BoneType.ViewPoint for Desktop, BoneType.LeftHand and BoneType.RightHand for most VR devices.
        /// </summary>
        public List<uint> bonesWithController { get; set; }
    }
}