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
using UnityEngine;

namespace umi3d.baseBrowser.Navigation
{

    /// <summary>
    /// This class handles the navigation possibility. It is based on Unity navmesh.
    /// </summary>
    public class NavmeshManager : MonoBehaviour
    {
        #region Methods

        #region Navmesh

        [Tooltip("Default layer for traversable objets")]
        public LayerMask defaultLayer;

        [Tooltip("Layer for objects part of the navmesh")]
        public LayerMask navmeshLayer;

        [Tooltip("Layer for non traversable objects")]
        public LayerMask obstacleLayer;

        #endregion

        #region Volume

        public Material invisibleMaterial;

        private Dictionary<ulong, GameObject> cellIdToGameobjects = new Dictionary<ulong, GameObject>();

        #endregion

        #endregion

        private void OnEnable()
        {
            UMI3DEnvironmentLoader.Instance.onNodePartOfNavmeshSet += SetPartOfNavmesh;
            UMI3DEnvironmentLoader.Instance.onNodeTraversableSet += SetTraversable;
            cdk.collaboration.UMI3DCollaborationClientServer.Instance.OnLeaving.AddListener(Reset);

            VolumePrimitiveManager.SubscribeToPrimitiveCreation(OnPrimitiveCreated, false);
            VolumePrimitiveManager.SubscribeToPrimitiveDelete(OnPrimitiveDeleted);
        }

        private void OnDisable()
        {
            if (UMI3DEnvironmentLoader.Exists)
                UMI3DEnvironmentLoader.Instance.onNodePartOfNavmeshSet -= SetPartOfNavmesh;

            if (UMI3DEnvironmentLoader.Exists)
                UMI3DEnvironmentLoader.Instance.onNodeTraversableSet -= SetTraversable;

            if (cdk.collaboration.UMI3DCollaborationClientServer.Exists)
                cdk.collaboration.UMI3DCollaborationClientServer.Instance.OnLeaving.RemoveListener(Reset);

            VolumePrimitiveManager.UnsubscribeToPrimitiveCreation(OnPrimitiveCreated);
            VolumePrimitiveManager.UnsubscribeToPrimitiveDelete(OnPrimitiveDeleted);
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

        /// <summary>
        /// Resets navmesh data.
        /// </summary>
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
        /// Sets <paramref name="node"/> as part of the navmesh.
        /// </summary>
        /// <param name="node"></param>
        private void SetPartOfNavmesh(UMI3DNodeInstance node)
        {
            if (node.IsPartOfNavmesh)
            {
                if (node.GameObject.GetComponent<Collider>() != null)
                {
                    node.GameObject.layer = ToLayer(navmeshLayer);
                }

                foreach (var renderer in node.renderers)
                {
                    if (renderer.TryGetComponent<Collider>(out Collider c))
                    {
                        renderer.gameObject.layer = ToLayer(navmeshLayer);
                    }
                    else
                    {
                        Debug.LogWarning(renderer.gameObject.name + " is set as part of the navmesh but it does not have colliders");
                    }
                }
            }
            else
            {
                if (!node.IsTraversable)
                {
                    SetTraversable(node);
                }
                else
                {

                    SetLayer(node, defaultLayer);
                }
            }
        }

        /// <summary>
        /// Sets <paramref name="node"/> as traversable.
        /// </summary>
        /// <param name="node"></param>
        private void SetTraversable(UMI3DNodeInstance node)
        {
            if (!node.IsPartOfNavmesh)
            {
                if (node.IsTraversable)
                {
                    SetLayer(node, defaultLayer);
                }
                else
                {
                    SetLayer(node, obstacleLayer);
                }
            }
        }

        /// <summary>
        /// Sets a node and its renderers to layer <see cref="mask"/>.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mask"></param>
        private void SetLayer(UMI3DNodeInstance node, LayerMask mask)
        {
            if (node.GameObject.GetComponent<Collider>() != null)
            {
                node.GameObject.layer = ToLayer(navmeshLayer);
            }

            foreach (var renderer in node.renderers)
            {
                renderer.gameObject.layer = ToLayer(mask);
            }
        }

        private void ChangeObjectAndChildrenLayer(GameObject parent, LayerMask mask)
        {
            parent.layer = mask;

            foreach (Transform t in parent.transform)
                ChangeObjectAndChildrenLayer(t.gameObject, mask);
        }

        /// <summary> Converts given bitmask to layer number </summary>
        /// <returns> layer number </returns>
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

    }
}