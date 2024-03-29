﻿/*
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

using UnityEngine;

namespace umi3d.common.userCapture.description
{
    /// <summary>
    /// Indicate that a link should be generated
    /// </summary>
    public abstract class SkeletonMappingLinkMarker : MonoBehaviour
    {
        /// <summary>
        /// Bone type for which to generate the link.
        /// </summary>
        [Tooltip("Bone type for which to generate the link.")]
        [inetum.unityUtils.ConstEnum(typeof(BoneType), typeof(uint))]
        public uint BoneType;

        /// <summary>
        /// Level of articulation of that link.
        /// </summary>
        [inetum.unityUtils.ConstEnum(typeof(LevelOfArticulation), typeof(uint))]
        public uint LevelOfArticulation;

        public abstract ISkeletonMappingLink ToLink();

        public SkeletonMapping ToSkeletonMapping()
        {
            return new SkeletonMapping(BoneType, ToLink());
        }
    }
}