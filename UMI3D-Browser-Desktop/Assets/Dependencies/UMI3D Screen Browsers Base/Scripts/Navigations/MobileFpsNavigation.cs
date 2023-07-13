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
using umi3d.baseBrowser.Navigation;
using UnityEngine;

namespace umi3d.mobileBrowser.Controller
{
    public class MobileFpsNavigation : IConcreteFPSNavigation
    {
        public BaseFPSNavigation FPSNavigation;
        public BaseFPSData data;

        public Func<Vector2> CameraDirection;
        public Func<Vector2> MoveDirection;

        public void HandleView()
        {
            Vector3 angleView = FPSNavigation.viewpoint.rotation.eulerAngles.NormalizeAngle();

            Debug.Assert(CameraDirection != null, "CameraDirection must not be null.");

            var direction = CameraDirection.Invoke();
            Vector2 angularSpeed = new Vector2
            (
                -1 * direction.y * data.AngularViewSpeed.y,
                direction.x * data.AngularViewSpeed.x
            );

            FPSNavigation.BaseHandleView(angleView, angularSpeed);
        }

        public bool Update()
        {
            if (!FPSNavigation.OnUpdate()) return false;

            FPSNavigation.HandleMovement();
            HandleView();

            return true;
        }

        public void UpdateMovement(ref Vector2 move)
        {
            Debug.Assert(MoveDirection != null, "CameraDirection must not be null.");
            Vector2 joystickInput = MoveDirection();
            FPSNavigation.Movement = new Vector2(joystickInput.y, joystickInput.x);
            if (FPSNavigation.Movement != Vector2.zero) FPSNavigation.OnPlayerMoved();
        }

        public void Walk(ref Vector2 move, ref float height)
        {
            FPSNavigation.IsCrouching = FPSNavigation.WantToCrouch || (FPSNavigation.IsCrouching && !FPSNavigation.CanJump());
            if (FPSNavigation.IsCrouching)
            {
                move.x *= (move.x > 0) ? data.forwardSpeed.y : data.backwardSpeed.y;
                move.y *= data.lateralSpeed.z;
            }
            else
            {
                move.x *= (move.x > 0) ? data.forwardSpeed.z : data.backwardSpeed.z;
                move.y *= data.lateralSpeed.z;
            }

            FPSNavigation.skeleton.transform.localPosition = new Vector3(0, Mathf.Lerp(FPSNavigation.skeleton.transform.localPosition.y, (FPSNavigation.IsCrouching) ? data.squatHeight : data.standHeight, data.squatSpeed == 0 ? 1000000 : Time.deltaTime / data.squatSpeed), 0);

            FPSNavigation.ComputeGravity(FPSNavigation.WantToJump, ref height);
        }
    }
}