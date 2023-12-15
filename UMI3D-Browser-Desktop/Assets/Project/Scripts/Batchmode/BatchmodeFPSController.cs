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
using umi3d.cdk.interaction;
using System.Collections.Generic;
using System.Linq;
using umi3d.common.interaction;
using umi3d.common.userCapture;

namespace BrowserDesktop
{
    public class BatchmodeFPSController : AbstractNavigation
    {
        public static bool simulateInteraction = false;

        #region Fields

        [SerializeField]
        BaseFPSNavigation fpsController = null;

        [SerializeField]
        LayerMask navmeshLayer, obstacleLayer, layerToConsider;

        #region Movement

        private Coroutine movementCoroutine = null;
        private bool isWalking = false;

        private NavMeshAgent agent;
        private NavMeshSurface surface;

        #endregion

        #region Interaction

        private List<Interactable> interactables = new ();

        #endregion

        #endregion

        #region Methods

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

            BakeInteraction();

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

        private void BakeInteraction()
        {
            if (!simulateInteraction)
                return;

            InteractableContainer[] containers = FindObjectsOfType<InteractableContainer>();

            foreach (var container in containers)
            {
                interactables.Add(container.Interactable);
            }
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
                    Interact();

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

        private async void Interact()
        {
            if (!simulateInteraction || interactables.Count == 0)
                return;

            Interactable interactable = interactables[Random.Range(0, interactables.Count)];
            Debug.Log("Try to interact with " + interactable.name);

            if (interactable.interactions.Count == 0)
                return;

            List<EventDto> interactions = new();

            foreach (var interactionTask in interactable.interactions)
            {
                if (await interactionTask is EventDto ev)
                    interactions.Add(ev);
            }

            if (interactions.Count == 0)
                return;

            EventDto eventDto = interactions[Random.Range(0, interactions.Count)];
            Debug.Log("Interact with " + eventDto.name);

            if (eventDto.hold)
            {
                var answer = new EventStateChangedDto
                {
                    active = true,
                    boneType = BoneType.Viewpoint,
                    id = eventDto.id,
                    toolId = interactable.id,
                    hoveredObjectId = 0,
                    bonePosition = transform.position.Dto(),
                    boneRotation = transform.rotation.Dto()
                };

                UMI3DClientServer.SendRequest(answer, true);

                await UMI3DAsyncManager.Delay(2000);

                answer.active = false;
                UMI3DClientServer.SendRequest(answer, true);
            } else
            {
                var answer = new EventTriggeredDto
                {
                    boneType = BoneType.Viewpoint,
                    id = eventDto.id,
                    toolId = interactable.id,
                    hoveredObjectId = 0,
                    bonePosition = transform.position.Dto(),
                    boneRotation = transform.rotation.Dto()
                };

                UMI3DClientServer.SendRequest(answer, true);
            }
        }

        #endregion
    }
}