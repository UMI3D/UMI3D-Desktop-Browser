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
using umi3d.baseBrowser.inputs.interactions;
using UnityEngine;

namespace umi3d.baseBrowser.Navigation
{
    public class KeyboardAndMouseFpsNavigation: IConcreteFPSNavigation
    {
        public BaseFPSData data;

        public void HandleUserInput()
        {
            // Player movement
            data.playerMovement = Vector3.zero;
            if (KeyboardNavigation.IsPressed(NavigationEnum.Forward)) data.playerMovement.z += 1;
            if (KeyboardNavigation.IsPressed(NavigationEnum.Backward)) data.playerMovement.z -= 1;
            if (KeyboardNavigation.IsPressed(NavigationEnum.Right)) data.playerMovement.x += 1;
            if (KeyboardNavigation.IsPressed(NavigationEnum.Left)) data.playerMovement.x -= 1;

            data.WantToJump = KeyboardNavigation.IsPressed(NavigationEnum.Jump);
            data.WantToCrouch = KeyboardNavigation.IsPressed(NavigationEnum.Crouch);
            data.WantToSprint = KeyboardNavigation.IsPressed(NavigationEnum.sprint);

            // Camera movement
            data.cameraMovement = new Vector2( 
                -1 * Input.GetAxis("Mouse Y"),
                Input.GetAxis("Mouse X")
            );

            data.WantToLookAround =  KeyboardNavigation.IsPressed(NavigationEnum.FreeView);
        }
    }
}
