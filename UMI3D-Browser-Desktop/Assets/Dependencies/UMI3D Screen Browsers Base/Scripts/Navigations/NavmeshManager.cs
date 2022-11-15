/*
Copyright 2019 - 2022 Inetum

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
using umi3d.cdk;
using umi3d.cdk.volumes;
using umi3d.common;
using UnityEngine;
using UnityEngine.Events;

namespace umi3d.baseBrowser.Navigation
{

    /// <summary>
    /// This class handles the navigation possibility. It is based on Unity navmesh.
    /// </summary>
    public class NavmeshManager : MonoBehaviour
    {
        #region Methods

        #region Navmesh

        /// <summary>
        /// Name of the layer where objects of the navmesh will be set.
        /// </summary>
        public string navmeshLayerName = "Navmesh";

        /// <summary>
        /// Name of the layerer where obstacles objects will be set.
        /// </summary>
        public string obstacleLayerName = "Obstacle";

        private LayerMask navmeshLayer;

        private LayerMask obstacleLayer;

        #endregion

        #region Volume

        public Material invisibleMaterial;

        private Dictionary<ulong, GameObject> cellIdToGameobjects = new Dictionary<ulong, GameObject>();

        #endregion

        #endregion

        void Start()
        {
            navmeshLayer = LayerMask.NameToLayer(navmeshLayerName);
            obstacleLayer = LayerMask.NameToLayer(obstacleLayerName);
            Debug.Assert(navmeshLayer != default && obstacleLayerName != default);

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(InitNavMesh);

            cdk.collaboration.UMI3DCollaborationClientServer.Instance.OnLeaving.AddListener(Reset);

            VolumePrimitiveManager.SubscribeToPrimitiveCreation(OnPrimitiveCreated, false);
            VolumePrimitiveManager.SubscribeToPrimitiveDelete(OnPrimitiveDeleted);
        }

        /// <summary>
        /// Creates an obstacle when a new primitive non traversable is created.
        /// </summary>
        /// <param name="primitive"></param>
        private void OnPrimitiveCreated(AbstractVolumeCell primitive)
        {
            if (primitive.isTraversable)
                return;

            GameObject go = new GameObject("Obstacle-Volume-" + primitive.Id());
            go.transform.SetParent((primitive as AbstractPrimitive)?.rootNode?.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            switch (primitive)
            {
                case Box box:
                    BoxCollider boxCollider = go.AddComponent<BoxCollider>();
                    boxCollider.center = box.bounds.center;
                    boxCollider.size = box.bounds.size;
                    break;
                case Cylinder cylinder:
                    CapsuleCollider capsuleCollider = go.AddComponent<CapsuleCollider>();
                    capsuleCollider.height = cylinder.height;
                    capsuleCollider.radius = cylinder.radius;
                    break;
                default:
                    Destroy(go);
                    Debug.LogError("Primitive of type " + primitive?.GetType() + " not supported.");
                    break;
            }

            if (go != null)
                ChangeObjectAndChildrenLayer(go, obstacleLayer);
        }

        /// <summary>
        /// If <paramref name="primitive"/> had a related obstacle, deletes it.
        /// </summary>
        /// <param name="primitive"></param>
        private void OnPrimitiveDeleted(AbstractVolumeCell primitive)
        {
            if (cellIdToGameobjects.ContainsKey(primitive.Id()))
            {
                GameObject go = cellIdToGameobjects[primitive.Id()];
                if (go != null)
                    Destroy(go);
                cellIdToGameobjects.Remove(primitive.Id());
            }
        }

        private void Reset()
        {
            foreach (var primitive in cellIdToGameobjects)
            {
                if (primitive.Value != null)
                    Destroy(primitive.Value);
            }
            cellIdToGameobjects.Clear();
        }

        /// <summary>
        /// Once the environment is loaded, generates the navmesh.
        /// </summary>
        private void InitNavMesh()
        {
            foreach (var entity in UMI3DEnvironmentLoader.Entities())
            {
                if (entity is UMI3DNodeInstance nodeInstance)
                {
                    UMI3DDto dto = (nodeInstance.dto as GlTFNodeDto)?.extensions.umi3d;

                    if (dto is UMI3DMeshNodeDto && !(dto is SubModelDto)) //subModels will be initialized with their associated UMI3DModel.
                        InitModel(nodeInstance);
                }
            }
        }

        /// <summary>
        /// Inits navmesh according to the data stored by nodeInstance and its children.
        /// </summary>
        /// <param name="nodeInstance"></param>
        void InitModel(UMI3DNodeInstance nodeInstance)
        {
            UMI3DMeshNodeDto meshNodeDto = (nodeInstance.dto as GlTFNodeDto)?.extensions.umi3d as UMI3DMeshNodeDto;

            if (meshNodeDto != null)
                SetUpGameObject(nodeInstance, meshNodeDto);
            else
            {
                SubModelDto subModelDto = (nodeInstance.dto as GlTFNodeDto)?.extensions.umi3d as SubModelDto;
                if (subModelDto != null)
                    SetUpGameObject(nodeInstance, subModelDto);
            }

            foreach (var child in nodeInstance.subNodeInstances)
            {
                InitModel(child);
            }
        }

        private void SetUpGameObject(UMI3DNodeInstance nodeInstance, UMI3DMeshNodeDto meshDto)
        {

            SetUpGameObject(nodeInstance, meshDto.isPartOfNavmesh, meshDto.isTraversable);
        }

        private void SetUpGameObject(UMI3DNodeInstance nodeInstance, SubModelDto subModelDto)
        {
            SetUpGameObject(nodeInstance, subModelDto.isPartOfNavmesh, subModelDto.isTraversable);
        }

        /// <summary>
        /// If a gameobject is not traversable or part of the navmesh, sets it up.
        /// </summary>
        /// <param name="nodeInstance"></param>
        /// <param name="dto"></param>
        private void SetUpGameObject(UMI3DNodeInstance nodeInstance, bool isPartOfNavmesh, bool isTraversable)
        {
            GameObject obj = nodeInstance.gameObject;

            if (isPartOfNavmesh)
            {
                ChangeObjectAndChildrenLayer(obj, navmeshLayer);
            }
            else if (!isTraversable)
            {
                ChangeObjectAndChildrenLayer(obj, obstacleLayer);
            }

            if (isPartOfNavmesh || !isTraversable)
            {
                foreach (var r in nodeInstance.renderers)
                {
                    if (r.gameObject.GetComponent<Collider>() == null)
                    {
                        r.gameObject.AddComponent<MeshCollider>();
                    }
                }
            }
        }

        private void ChangeObjectAndChildrenLayer(GameObject parent, LayerMask mask)
        {
            parent.layer = mask;

            foreach (Transform t in parent.transform)
                ChangeObjectAndChildrenLayer(t.gameObject, mask);
        }
    }
}