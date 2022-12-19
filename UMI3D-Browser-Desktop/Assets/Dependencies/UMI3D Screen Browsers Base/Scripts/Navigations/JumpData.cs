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
using UnityEngine;

namespace umi3d.baseBrowser.Navigation
{
    public struct JumpData
    {
        public BaseFPSData data;
        public bool IsJumping;
        public float velocity, maxJumpVelocity;

        public void ComputeVelocity(float time, float deltaTime, bool canJump)
        {
            IsJumping = canJump;
            if (IsJumping) ComputeJumpVelocity(time);
            ComputeFallVelocity(deltaTime);
        }

        private void ComputeJumpVelocity(float time)
            => velocity = maxJumpVelocity;

        private void ComputeFallVelocity(float deltaTime)
            => velocity += data.gravity * deltaTime;
    }
}
