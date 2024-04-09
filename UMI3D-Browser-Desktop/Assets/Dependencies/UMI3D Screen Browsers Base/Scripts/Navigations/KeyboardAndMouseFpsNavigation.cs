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
        public BaseFPSNavigation FPSNavigation;
        public BaseFPSData data;

        public bool Update()
        {
            if (!FPSNavigation.OnUpdate()) return false;

            if 
            (
                FPSNavigation.state == BaseFPSNavigation.State.Default 
                && KeyboardNavigation.IsPressed(NavigationEnum.FreeView)
            ) FPSNavigation.state = BaseFPSNavigation.State.FreeHead;
            else if 
            (
                FPSNavigation.state == BaseFPSNavigation.State.FreeHead 
                && !KeyboardNavigation.IsPressed(NavigationEnum.FreeView)
            )
            {
                FPSNavigation.state = BaseFPSNavigation.State.Default;
                FPSNavigation.changeToDefault = true;
            }

            FPSNavigation.HandleMovement();
            HandleView();

            return true;
        }

        #region Movement

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="move"></param>
        /// <param name="height"></param>
        public void Walk(ref Vector2 move, ref float height)
        {


            

            

            FPSNavigation.ComputeGravity(KeyboardNavigation.IsPressed(NavigationEnum.Jump), ref height);
        }

        #endregion

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void HandleView()
        {
            if (FPSNavigation.state == BaseFPSNavigation.State.FreeMousse) return;
            Vector3 angleView = FPSNavigation.viewpoint.rotation.eulerAngles.NormalizeAngle();

            Vector2 angularSpeed = new Vector2
            (
                -1 * Input.GetAxis("Mouse Y") * data.AngularViewSpeed.x,
                Input.GetAxis("Mouse X") * data.AngularViewSpeed.y
            );

            FPSNavigation.BaseHandleView(angleView, angularSpeed);
        }

        public Vector3 HandleUserInput()
        {
            Vector3 result = Vector3.zero;
            if (KeyboardNavigation.IsPressed(NavigationEnum.Forward)) result.z += 1;
            if (KeyboardNavigation.IsPressed(NavigationEnum.Backward)) result.z -= 1;
            if (KeyboardNavigation.IsPressed(NavigationEnum.Right)) result.x += 1;
            if (KeyboardNavigation.IsPressed(NavigationEnum.Left)) result.x -= 1;

            data.WantToJump = KeyboardNavigation.IsPressed(NavigationEnum.Jump);
            data.WantToCrouch = KeyboardNavigation.IsPressed(NavigationEnum.Crouch);
            data.WantToSprint = KeyboardNavigation.IsPressed(NavigationEnum.sprint);

            return result;
        }
    }
}
