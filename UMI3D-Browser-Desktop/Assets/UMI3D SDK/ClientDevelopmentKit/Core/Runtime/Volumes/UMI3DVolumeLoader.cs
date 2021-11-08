﻿/*
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

namespace umi3d.cdk.volumes
{
    /// <summary>
    /// Loader for volume parts.
    /// </summary>
	static public class UMI3DVolumeLoader 
	{
        static public void ReadUMI3DExtension(AbstractVolumeDescriptorDto dto, Action finished, Action<Umi3dExecption> failed)
        {
            switch (dto)
            {
                case PointDto pointDto:
                    VolumeSliceGroupManager.CreatePoint(pointDto, p =>
                    {
                        UMI3DEnvironmentLoader.RegisterEntityInstance(dto.id, dto, p, () => VolumeSliceGroupManager.DeletePoint(dto.id));
                        finished.Invoke();
                    });                    
                    break;

                case FaceDto faceDto:
                    VolumeSliceGroupManager.CreateFace(faceDto, f =>
                    {
                        UMI3DEnvironmentLoader.RegisterEntityInstance(dto.id, dto, f, () => VolumeSliceGroupManager.DeleteFace(dto.id));
                        finished.Invoke();
                    });                   
                    break;

                case VolumeSliceDto slice:
                    VolumeSliceGroupManager.CreateVolumeSlice(slice, s =>
                    {
                        UMI3DEnvironmentLoader.RegisterEntityInstance(dto.id, dto, s, () => VolumeSliceGroupManager.DeleteVolumeSlice(dto.id));
                        finished.Invoke();
                    });
                    break;

                case VolumeSlicesGroupDto group:
                    VolumeSliceGroupManager.CreateVolumeSliceGroup(group, v =>
                    {
                        UMI3DEnvironmentLoader.RegisterEntityInstance(dto.id, dto, v, () => VolumeSliceGroupManager.DeleteVolumeSliceGroup(dto.id));
                        finished.Invoke();
                    });                    
                    break;

                case AbstractPrimitiveDto prim:
                    VolumePrimitiveManager.CreatePrimitive(prim, p =>
                    {
                        UMI3DEnvironmentLoader.RegisterEntityInstance(dto.id, dto, p, () => VolumePrimitiveManager.DeletePrimitive(dto.id));
                        finished.Invoke();
                    });
                    break;
                case OBJVolumeDto obj:
                    ExternalVolumeDataManager.Instance.CreateOBJVolume(obj, objVolume =>
                    {
                        UMI3DEnvironmentLoader.RegisterEntityInstance(dto.id, dto, objVolume, () => ExternalVolumeDataManager.Instance.DeleteOBJVolume(dto.id));
                        finished.Invoke();
                    });

                    break;
                default:
                    failed(new Umi3dExecption("Unknown Dto Type"));
                    break;
            }
        }

        static public bool SetUMI3DProperty(UMI3DEntityInstance entity, SetEntityPropertyDto property)
        {
            Debug.Log("SetUMI3DProperty : " + property.property);
            switch (property.property)
            {
                case UMI3DPropertyKeys.VolumeCell_RoodNodeId:
                    var cell_1 = entity.Object as AbstractVolumeCell;
                    cell_1.SetRootNode((ulong)property.value);
                    return true;

                case UMI3DPropertyKeys.VolumeCell_RootNodeToLocalMatrix:
                    var cell_2 = entity.Object as AbstractVolumeCell;
                    cell_2.SetTransform((Matrix4x4)property.value);
                    return true;

                case UMI3DPropertyKeys.VolumePrimitive_Box_Center:

                    Debug.Log("Box center update");
                    var box_1 = entity.Object as Box;
                    Vector3 newCenter = (Vector3)property.value;
                    Vector3 size = box_1.bounds.size;
                    box_1.SetBounds(new Bounds(newCenter, size));
                    return true;

                case UMI3DPropertyKeys.VolumePrimitive_Box_Size:
                    var box_2 = entity.Object as Box;
                    Vector3 center = box_2.bounds.center;
                    Vector3 newsize = (Vector3)property.value;
                    box_2.SetBounds(new Bounds(center, newsize));
                    return true;

                case UMI3DPropertyKeys.VolumePrimitive_Cylinder_Height:
                    var cyl_1 = entity.Object as Cylinder;
                    cyl_1.SetHeight((float)property.value);
                    return true;

                case UMI3DPropertyKeys.VolumePrimitive_Cylinder_Radius:
                    var cyl_2 = entity.Object as Cylinder;
                    cyl_2.SetRadius((float)property.value);
                    return true;

                default:
                    return false;
            }
        }
    }
}