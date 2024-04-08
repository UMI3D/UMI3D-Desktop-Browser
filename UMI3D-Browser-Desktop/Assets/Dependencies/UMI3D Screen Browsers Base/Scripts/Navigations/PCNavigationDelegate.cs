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

using umi3d.cdk.navigation;
using umi3d.common;

using UnityEngine;

public class PCNavigationDelegate : INavigationDelegate
{
    #region Dependencies

    public Transform playerTransform;
    public UMI3DCollisionManager collisionManager;

    /// <summary>
    /// Is player active ?
    /// </summary>
    public bool isActive = false;

    #endregion

    public void Activate()
    {
        isActive = true;
    }

    public void Disable()
    {
        isActive = false;
    }

    public NavigationData GetNavigationData()
    {
        throw new System.NotImplementedException();
    }

    public void Navigate(ulong environmentId, NavigateDto data)
    {
        throw new System.NotImplementedException();
    }

    public void Teleport(ulong environmentId, TeleportDto data)
    {
        playerTransform.localPosition = data.position.Struct();
        playerTransform.localRotation = data.rotation.Quaternion();
        collisionManager.ComputeGround();
    }

    public void UpdateFrame(ulong environmentId, FrameRequestDto data)
    {
        throw new System.NotImplementedException();
    }
}
