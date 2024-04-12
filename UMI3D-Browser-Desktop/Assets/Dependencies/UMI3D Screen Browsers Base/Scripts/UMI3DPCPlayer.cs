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
        public Transform viewpoint;
        public Transform neckPivot;
        public Transform head;

        [HideInInspector] public UMI3DNavigation navigation = new();

        UMI3DCollisionManager collisionManager;
        UMI3DMovementManager movementManager;
        PCNavigationDelegate navigationDelegate;

        void Awake()
        {
            collisionManager = new()
            {
                data = fpsData,
                playerTransform = playerTransform,
                topHead = topHead,
                feetRaycastOrigin = feetRaycastOrigin,
                navmeshLayer = navmeshLayer,
                obstacleLayer = obstacleLayer
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
        }

        private void Update()
        {
            if (!navigationDelegate.isActive)
            {
                return; 
            }

            movementManager.ComputeMovement();
        }
    }
}