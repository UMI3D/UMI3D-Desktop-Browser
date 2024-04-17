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
using System;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.baseBrowser.Navigation;
using UnityEngine;

namespace umi3d.mobileBrowser.Controller
{
    public class MobileFpsNavigation : IConcreteFPSNavigation
    {
        public BaseFPSData data;

        public Func<Vector2> CameraDirection;
        public Func<Vector2> MoveDirection;

        public void HandleUserInput()
        {
            // Player movement
            Debug.Assert(MoveDirection != null, "MoveDirection must not be null.");
            // x: Left to right.
            // y: back to front. 
            Vector2 joystickInput = MoveDirection?.Invoke() ?? Vector2.zero;
            data.playerTranslationSpeed = new Vector3(
                joystickInput.x, 
                0f, 
                joystickInput.y
            );

            // Camera movement
            Debug.Assert(CameraDirection != null, "CameraDirection must not be null.");
            joystickInput = CameraDirection?.Invoke() ?? Vector2.zero;
            data.cameraRotation = new Vector2(
                -1 * joystickInput.y,
                joystickInput.x
            );
        }
    }
}