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
        public Dictionary<string, AbstractVolumeCell> cells = new Dictionary<string, AbstractVolumeCell>();

        public void CreateOBJVolume(OBJVolumeDto dto, UnityAction<AbstractVolumeCell> finished)
        {
            ObjMeshDtoLoader loader = new ObjMeshDtoLoader();

            Action<object> success = obj =>
            {

                Debug.Log("OBJ succes");
                OBJVolumeCell cell = new OBJVolumeCell()
                {
                    id = dto.id,
                    meshes = (obj as GameObject).GetComponentsInChildren<MeshFilter>().ToList().ConvertAll(filter => filter.mesh)
                };
            };

            Action<string> failed = s =>
            {
                Debug.LogError("Failed to load obj file : " + s);
            };

            loader.UrlToObject(dto.objFile, ".obj", null, success, failed);
        }

        public void DeleteOBJVolume(string id)
        {
            throw new System.NotImplementedException(); //todo
        }

    }
}