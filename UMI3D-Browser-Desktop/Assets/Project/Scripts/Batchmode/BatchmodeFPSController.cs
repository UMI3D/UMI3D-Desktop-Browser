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
using umi3d.baseBrowser.Navigation;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.cdk.collaboration.userCapture;
using umi3d.common;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

namespace BrowserDesktop
{
    public class BatchmodeFPSController : AbstractNavigation
    {
        #region Fields

        [SerializeField]
        BaseFPSNavigation fpsController = null;

        [SerializeField]
        LayerMask navmeshLayer, obstacleLayer, layerToConsider;

        private Coroutine movementCoroutine = null;
        private bool isWalking = false;

        private NavMeshAgent agent;
        private NavMeshSurface surface;

        #endregion

        void Start()
        {
            if (!Application.isBatchMode)
                return;

            Debug.Assert(fpsController != null);
        }

        private void OnEnable()
        {
            if (!Application.isBatchMode)
                return;

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(OnEnvironmentLoaded);
            UMI3DCollaborationClientServer.Instance.OnLeavingEnvironment.AddListener(OnLeave);
        }

        private void OnDisable()
        {
            if (!Application.isBatchMode)
                return;

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.RemoveListener(OnEnvironmentLoaded);
            UMI3DCollaborationClientServer.Instance.OnLeavingEnvironment.RemoveListener(OnLeave);
        }

        private void OnEnvironmentLoaded()
        {
            fpsController.Disable();

            BakeNavmesh();

            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.speed = 2f;

            movementCoroutine = StartCoroutine(UpdateCoroutine());

            CollaborationSkeletonsManager.Instance.navigation = this;
        }

        private void OnLeave()
        {
            StopCoroutine(movementCoroutine);
        }

        private void BakeNavmesh()
        {
            GameObject[] sceneObjects = FindObjectsOfType<GameObject>();

            int layer = ToLayer(navmeshLayer);
            int obstacle = ToLayer(obstacleLayer);

            foreach (var obj in sceneObjects)
            {
                if (obj.isStatic)
                    continue;

                if (obj.layer == layer)
                {
                    var navmeshModifier = obj.AddComponent<NavMeshModifier>();
                    navmeshModifier.overrideArea = true;
                    navmeshModifier.area = 0;
                } else if (obj.layer == obstacle)
                {
                    var navmeshModifier = obj.AddComponent<NavMeshModifier>();
                    navmeshModifier.overrideArea = true;
                    navmeshModifier.area = 1;
                }
            }

            GameObject navmeshSurface = new GameObject("NavmeshSurface");
            surface = navmeshSurface.AddComponent<NavMeshSurface>();
            surface.layerMask = layerToConsider;
            surface.BuildNavMesh();
        }

        [ContextMenu("Bake")]
        private void Bake()
        {
            surface.BuildNavMesh();
        }

        public static int ToLayer(int bitmask)
        {
            int result = bitmask > 0 ? 0 : 31;
            while (bitmask > 1)
            {
                bitmask = bitmask >> 1;
                result++;
            }
            return result;
        }

        private IEnumerator UpdateCoroutine()
        {
            Vector3 target;

            RandomPoint(out target);
            agent.SetDestination(target);

            while (true)
            {
                if (Vector3.Distance(transform.position, agent.destination) > .3f)
                {
                    isWalking = true;

                    yield return null;
                }
                else
                {
                    RandomPoint(out target);
                    agent.SetDestination(target);

                    agent.isStopped = true;
                    isWalking = false;

                    yield return new WaitForSeconds(Random.Range(3f, 7f));

                    isWalking = true;
                    agent.isStopped = false;
                }
            }
        }

        bool RandomPoint(out Vector3 result)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 random = new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));

                NavMeshHit hit;
                if (NavMesh.SamplePosition(random, out hit, 4.0f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }

        public override void Disable()
        {
        }

        public override void Activate()
        {
        }

        public override void Navigate(NavigateDto data)
        {
        }

        public override void Teleport(TeleportDto data)
        {
            transform.localPosition = data.position.Struct();
            transform.localRotation = data.rotation.Quaternion();
        }

        public override void UpdateFrame(FrameRequestDto data)
        {
        }

        public override NavigationData GetNavigationData()
        {
            return new NavigationData
            {
                speed = new Vector3Dto()
                {
                    X = (isWalking ? .5f : 0f) / Time.deltaTime,
                    Z = 0,
                    Y = 0
                },

                crouching = false,
                jumping = false,
                grounded = true,
            };
        }
    }
}