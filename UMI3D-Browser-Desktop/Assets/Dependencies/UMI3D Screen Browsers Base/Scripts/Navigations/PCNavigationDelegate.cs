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

using System;
using umi3d.baseBrowser.Navigation;
using umi3d.cdk;
using umi3d.cdk.navigation;
using umi3d.common;

using UnityEngine;

public class PCNavigationDelegate : INavigationDelegate
{
    #region Dependencies

    public Transform playerTransform;
    public Transform personalSkeletonContainer;
    public Transform cameraTransform;
    public UMI3DCollisionManager collisionManager;
    public BaseFPSData data;

    /// <summary>
    /// Is player active ?
    /// </summary>
    public bool isActive = false;
    public UMI3DNodeInstance globalFrame;

    #endregion

    public void Activate()
    {
        // Activating navigation and changing camera mode to Navigation
        isActive = true;
        data.cameraMode = E_CameraMode.Navigation;
    }

    public void Disable()
    {
        // Disabling navigation, changing camera mode to Free, resetting speed values
        isActive = false;
        data.IsJumping = data.IsCrouching = data.IsSprinting = false;
        data.cameraMode = E_CameraMode.Free;
    }

    public NavigationData GetNavigationData()
    {
        var translation = data.playerTranslationSpeed * Time.deltaTime;
        Func<float> yVelocity = () =>
        {
            return collisionManager.IsCloseToGround 
            ? 0f 
            : translation.y / Time.deltaTime;
        };

        return new NavigationData()
        {
            speed = new Vector3Dto()
            {
                X = translation.x / Time.deltaTime,
                Y = yVelocity(),
                Z = translation.z / Time.deltaTime
            },
            crouching = data.IsCrouching,
            jumping = data.IsJumping,
            grounded = collisionManager.IsGrounded,
        };
    }

    public void Navigate(ulong environmentId, NavigateDto data)
    {
        this.data.continuousDestination = playerTransform.parent.position + data.position.Struct();
    }

    public void Teleport(ulong environmentId, TeleportDto data)
    {
        playerTransform.localPosition = data.position.Struct();
        playerTransform.localRotation = data.rotation.Quaternion();
    }

    public void ViewpointTeleport(ulong environmentId, ViewpointTeleportDto data)
    {
        personalSkeletonContainer.rotation = data.rotation.Quaternion();
        if (cameraTransform != null)
        {
            cameraTransform.parent.localRotation = Quaternion.identity;
            float angle = Vector3.SignedAngle(
                personalSkeletonContainer.forward,
                Vector3.ProjectOnPlane(
                    cameraTransform.forward,
                    Vector3.up
                ),
                Vector3.up
            );
            personalSkeletonContainer.Rotate(0, -angle, 0);
        }

        playerTransform.position = data.position.Struct();
        if (cameraTransform != null)
        {
            Vector3 translation = playerTransform.position - cameraTransform.position;
            playerTransform.Translate(translation, Space.World);
        }
    }

    public void UpdateFrame(ulong environmentId, FrameRequestDto data)
    {
        if (data.FrameId == 0)
        {
            // bind the personalSkeletonContainer to the Scene.
            personalSkeletonContainer.SetParent(
                UMI3DLoadingHandler.Instance.transform,
                true
            );
            globalFrame.Delete -= GlobalFrameDeleted;
            globalFrame = null;
        }
        else
        {
            // bind the personalSkeletonContainer to the Frame.
            UMI3DNodeInstance Frame = UMI3DEnvironmentLoader.GetNode(environmentId, data.FrameId);
            if (Frame != null)
            {
                globalFrame = Frame;
                personalSkeletonContainer.SetParent(
                    Frame.transform,
                    true
                );
                globalFrame.Delete += GlobalFrameDeleted;
            }
        }
    }

    void GlobalFrameDeleted()
    {
        personalSkeletonContainer.SetParent(
            UMI3DLoadingHandler.Instance.transform,
            true
        );
    }
}
