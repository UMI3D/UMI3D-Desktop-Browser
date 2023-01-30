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
using System.Linq;
using umi3d.common;
using umi3d.common.userCapture;

namespace umi3d.cdk.userCapture
{

    public class AnimationSkeleton : ISubSkeleton
    {
        private const DebugScope debugScope =  DebugScope.CDK | DebugScope.Animation | DebugScope.UserCapture;

        public SkeletonMapper mapper;

        public UserCameraPropertiesDto GetCameraDto()
        {
            return null; //to implement only in TrackedAvatar
        }

        public PoseDto GetPose()
        {
            if (!mapper.animations.Select(id => UMI3DAnimatorAnimation.Get(id)).Any(a => a?.IsPlaying() ?? false))
                return null;
            return mapper.GetPose();
        }

        public void Update(UserTrackingFrameDto trackingFrame)
        {
            var animations = from animId in mapper.animations select (id: animId, UMI3DAnimation: UMI3DAnimatorAnimation.Get(animId));

            foreach (var anim in animations)
            {
                if (anim.UMI3DAnimation == null) // animation could not be found
                {
                    UMI3DLogger.LogWarning($"Trying to play/stop a unreferenced animation. ID : {anim.id}", debugScope);
                    continue;
                }
                if (trackingFrame.animationsPlaying.FirstOrDefault(animId=> animId == anim.id) != default)
                {
                    if (!anim.UMI3DAnimation.IsPlaying())
                        anim.UMI3DAnimation.Start();
                }
                else
                {
                    if (anim.UMI3DAnimation.IsPlaying())
                        anim.UMI3DAnimation.Stop();
                }
            }
        }

        public void WriteTrackingFrame(UserTrackingFrameDto trackingFrame, TrackingOption option)
        {
            var activeAnimations = from animId in mapper.animations
                                   select UMI3DAnimatorAnimation.Get(animId)
                                   into anim
                                   where anim.IsPlaying()
                                   select anim;

            foreach (var activeAnimation in activeAnimations)
            {
                trackingFrame.animationsPlaying.Add(activeAnimation.dto.id);
            }
        }
    }
}