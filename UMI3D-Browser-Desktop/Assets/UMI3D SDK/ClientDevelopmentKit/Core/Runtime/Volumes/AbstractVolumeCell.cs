/*
Copyright 2019 - 2021 Inetum

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
using UnityEngine;
using UnityEngine.Events;

namespace umi3d.cdk.volumes
{ 
    public abstract class AbstractVolumeCell
    {
        public abstract ulong Id();

        public bool isTraversable = true;

        /// <summary>
        /// Check if a point is inside a cell.
        /// </summary>
        /// <returns></returns>
        public abstract bool IsInside(Vector3 point, Space relativeTo);

        public abstract void GetBase(Action<Mesh> onSuccess, float angleLimit);

        public abstract Mesh GetMesh();

        /// <summary>
        /// Set the cell transform relatively to the root node.
        /// </summary>
        /// <param name="transform"></param>
        public abstract void SetTransform(Matrix4x4 transform);

        /// <summary>
        /// Set the root node which the cell is positioned relatively to.
        /// </summary>
        /// <param name="rootNodeId"></param>
        public abstract void SetRootNode(ulong rootNodeId);


        protected UnityEvent onUpdate = new UnityEvent();
        protected void SubscribeToUpdate(UnityAction callback) => onUpdate.AddListener(callback);
        protected void UnsubscribeToUpdate(UnityAction callback) => onUpdate.RemoveListener(callback);
    }
}