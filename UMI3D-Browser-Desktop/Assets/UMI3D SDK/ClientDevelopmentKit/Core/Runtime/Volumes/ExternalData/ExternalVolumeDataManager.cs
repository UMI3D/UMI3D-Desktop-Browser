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

using umi3d.common.volume;
using umi3d.common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using umi3d.cdk;
using System.Linq;

namespace umi3d.cdk.volumes 
{
    public class ExternalVolumeDataManager : Singleton<ExternalVolumeDataManager>
    {
        private static Dictionary<string, AbstractVolumeCell> cells = new Dictionary<string, AbstractVolumeCell>();

        private class ExternalVolumeEvent : UnityEvent<AbstractVolumeCell> { }
        private static ExternalVolumeEvent onVolumeCreation = new ExternalVolumeEvent();

        /// <summary>
        /// Subscribe an action to a cell reception.
        /// </summary>
        /// <param name="catchUpWithPreviousCells">If true, the action will be called for each already received cells.</param>
        public static void SubscribeToExternalVolumeCreation(UnityAction<AbstractVolumeCell> callback, bool catchUpWithPreviousCells)
        {
            onVolumeCreation.AddListener(callback);

            if (catchUpWithPreviousCells)
                foreach (AbstractVolumeCell cell in cells.Values)
                    callback(cell);
        }
        public static void UnsubscribeToExternalVolumeCreation(UnityAction<AbstractVolumeCell> callback) => onVolumeCreation.RemoveListener(callback);
       
        public void CreateOBJVolume(OBJVolumeDto dto, UnityAction<AbstractVolumeCell> finished)
        {
            ObjMeshDtoLoader loader = new ObjMeshDtoLoader();

            Action<object> success = obj =>
            {
                GameObject sceneNode = UMI3DEnvironmentLoader.GetNode(dto.rootId).gameObject;

                OBJVolumeCell cell = new OBJVolumeCell()
                {
                    id = dto.id,
                    meshes = (obj as GameObject).GetComponentsInChildren<MeshFilter>().ToList().ConvertAll(filter => filter.mesh)
                };

                Matrix4x4 m = Matrix4x4.TRS(sceneNode.transform.TransformPoint(dto.position), Quaternion.Inverse(sceneNode.transform.rotation) * dto.rotation, sceneNode.transform.InverseTransformVector(dto.scale));
                foreach(Mesh mesh in cell.meshes)
                {
                    mesh.vertices = mesh.vertices.ToList().ConvertAll(v => Vector3.Scale(v, new Vector3(-1, 1, -1))).ToArray(); //asimpl right handed coordinate system dirty fix
                    mesh.vertices = mesh.vertices.ToList().ConvertAll(v => m.MultiplyPoint(v)).ToArray(); //apply dto transform
                }

                onVolumeCreation.Invoke(cell);
                finished.Invoke(cell);
            };

            Action<string> failed = s =>
            {
                Debug.LogError("Failed to load obj file : " + s);   
            };

            loader.UrlToObject(dto.objFile, ".obj", UMI3DClientServer.getAuthorization(), success, failed);
        }

        public void DeleteOBJVolume(ulong id)
        {
            throw new System.NotImplementedException(); //todo
        }

        public static List<AbstractVolumeCell> GetCells() => cells.Values.ToList();
    }
}