/*
Copyright 2019 - 2024 Inetum

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
using umi3d.baseBrowser.cursor;
using umi3d.baseBrowser.Navigation;
using UnityEngine;

public sealed class UMI3DCameraManager
{
    #region Dependencies

    public Transform playerTransform;
    public Transform viewpointPivot;
    public Transform neckPivot;
    public Transform head;
    public IConcreteFPSNavigation concreteFPSNavigation;

    public BaseFPSData data;

    #endregion
    
    public void HandleView()
    {
        if (
                (BaseCursor.Movement == BaseCursor.CursorMovement.Free
                || BaseCursor.Movement == BaseCursor.CursorMovement.FreeHidden)
            )
        {
            return;
        }

        concreteFPSNavigation.HandleUserCamera();

        data.cameraMode = E_CameraMode.Navigation; // Debug only.
        if (data.cameraMode != E_CameraMode.Locked)
        {
            data.cameraMode = data.WantToLookAround
                ? E_CameraMode.NeckMovement
                : E_CameraMode.Navigation;
        }

        Vector2 angularSpeed = data.cameraRotation * data.AngularViewSpeed;

        // ------ Horizontal movement ------
        float viewpointYAxis = 0f;
        switch (data.cameraMode)
        {
            case E_CameraMode.Locked:
                break;
            case E_CameraMode.Navigation:
                // Rotate the player instead of the camera.
                playerTransform.rotation = Quaternion.Euler(
                    0f,
                    (viewpointPivot.rotation.eulerAngles.NormalizeAngle() + ((Vector3)angularSpeed).NormalizeAngle()).y, 
                    0f
                );
                break;
            case E_CameraMode.NeckMovement:
                viewpointYAxis = Mathf.Clamp(
                    (viewpointPivot.localRotation.eulerAngles.NormalizeAngle() + ((Vector3)angularSpeed).NormalizeAngle()).y, 
                    -data.maxYCameraAngle, 
                    data.maxYCameraAngle
                );
                break;
            case E_CameraMode.Free:
                viewpointYAxis = (viewpointPivot.localRotation.eulerAngles.NormalizeAngle() + ((Vector3)angularSpeed).NormalizeAngle()).y;
                break;
            default:
                break;
        }

        // ------ Vertical movement (Rotate the camera) ------
        // Restrict up and down viewpoint movement.
        float viewpointXAxis = Mathf.Clamp(
            (viewpointPivot.localRotation.eulerAngles.NormalizeAngle() + ((Vector3)angularSpeed).NormalizeAngle()).x, 
            -data.maxXCameraAngle, 
            data.maxXCameraAngle
        );

        viewpointPivot.localRotation = Quaternion.Euler(viewpointXAxis, viewpointYAxis, 0f);

        Vector3 headAngle = new Vector3(
            Mathf.Clamp(viewpointXAxis, data.maxXHeadAngle.x, data.maxXHeadAngle.y) / 2,
            viewpointYAxis / 2,
            0f
        );
        head.localRotation = Quaternion.Euler(headAngle);

        Vector3 neckAngle = new Vector3(
            Mathf.Clamp(viewpointXAxis, -data.maxNeckAngle, data.maxNeckAngle) / 2,
            viewpointYAxis / 2,
            0f
        );
        neckPivot.localRotation = Quaternion.Euler(neckAngle);
    }
}
