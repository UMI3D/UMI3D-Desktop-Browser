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

using umi3d.cdk.userCapture.tracking;
using umi3d.common.userCapture.pose;
using UnityEngine;

namespace umi3d.cdk.userCapture.pose
{
    /// <summary>
    /// Wrapper for <see cref="BoneRotationConditionDto"/>.
    /// </summary>
    public class BoneRotationPoseCondition : IPoseCondition
    {
        protected readonly BoneRotationConditionDto boneRotationConditionDto;

        protected ITrackedSubskeleton trackedSkeleton;

        public BoneRotationPoseCondition(BoneRotationConditionDto dto, ITrackedSubskeleton trackedSkeleton)
        {
            this.boneRotationConditionDto = dto ?? throw new System.ArgumentNullException(nameof(dto));
            this.trackedSkeleton = trackedSkeleton;
        }

        /// <inheritdoc/>
        public bool Check()
        {
            if (!trackedSkeleton.TrackedBones.TryGetValue(boneRotationConditionDto.BoneId, out TrackedSubskeletonBone bone))
                return false;

            return Quaternion.Angle(bone.transform.rotation, boneRotationConditionDto.Rotation.Quaternion()) <= boneRotationConditionDto.Threshold;
        }
    }
}