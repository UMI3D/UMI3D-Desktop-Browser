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

using System.Collections.Generic;
using umi3d.baseBrowser.Navigation;
using umi3d.cdk.collaboration.userCapture;
using umi3d.cdk.navigation;
using UnityEngine;

namespace umi3d.baseBrowser
{
    public class UMI3DPCPlayer : MonoBehaviour
    {
        public BaseFPSData fpsData;
        public Transform personalSkeletonContainer;
        public Transform playerTransform;
        public Transform skeleton;

        [Header("Player Collision Component")]
        public Transform topHead;
        /// <summary>
        /// List of point which from rays will be created to check if there is a navmesh under player's feet
        /// </summary>
        public List<Transform> feetRaycastOrigin;

        [Header("Collision layer")]
        public LayerMask obstacleLayer;
        public LayerMask navmeshLayer;

        [Header("Camera")]
        public Transform viewpointPivot;
        public Transform neckPivot;
        public Transform head;

        [HideInInspector] public UMI3DNavigation navigation = new();

        UMI3DCollisionManager collisionManager;
        UMI3DCameraManager cameraManager;
        UMI3DMovementManager movementManager;
        PCNavigationDelegate navigationDelegate;
        UMI3DPlayerCapsuleColliderDelegate colliderDelegate;

        void Awake()
        {
            colliderDelegate = new()
            {
                playerTransform = playerTransform,
                data = fpsData
            };
            colliderDelegate.Init();
            collisionManager = new()
            {
                data = fpsData,
                playerTransform = playerTransform,
                colliderDelegate = colliderDelegate,
                navmeshLayer = navmeshLayer,
                obstacleLayer = obstacleLayer
            };
            cameraManager = new()
            {
                data = fpsData,
                playerTransform = playerTransform,
                viewpointPivot = viewpointPivot,
                neckPivot = neckPivot,
                head = head
            };
            movementManager = new()
            {
                data = fpsData,
                playerTransform = playerTransform,
                skeleton = skeleton,
                collisionManager = collisionManager,
                concreteFPSNavigation = new KeyboardAndMouseFpsNavigation()
                {
                    data = fpsData,
                }
            };
            navigationDelegate = new()
            {
                data = fpsData,
                playerTransform = playerTransform,
                personalSkeletonContainer = personalSkeletonContainer,
                collisionManager = collisionManager,
            };
            navigation.Init(navigationDelegate);

            // SKELETON SERVICE
            CollaborationSkeletonsManager.Instance.navigation = navigationDelegate; //also use to init manager via Instance call

#if !UNITY_EDITOR
            fpsData.navigationMode = E_NavigationMode.Default;
#endif
        }

        private void Update()
        {
            if (!navigationDelegate.isActive)
            {
                return; 
            }

            cameraManager.HandleView();
            colliderDelegate.ComputeCollider();
            movementManager.ComputeMovement();
        }

        private void OnDrawGizmosSelected()
        {
            colliderDelegate.DrawGizmos();
        }
    }
}
