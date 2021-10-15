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
            switch (property.property)
            {
                case UMI3DPropertyKeys.PointPosition:
                    Point point = entity.Object as Point;
                    if (point == null)
                        throw new Exception("Internal error : entity is not a point");

                    point.SetPosition(property.value as SerializableVector3);
                    return true;

                case UMI3DPropertyKeys.FacePointsIds:
                    Face face = entity.Object as Face;
                    if (face == null)
                        throw new Exception("Internal error : entity is not a face");

                    face.SetPoints(property.value as List<ulong>);
                    return true;

                case UMI3DPropertyKeys.VolumeSlices:
                    VolumeSliceGroup group = entity.Object as VolumeSliceGroup;
                    if (group == null)
                        throw new Exception("Internal error : entity is not a volume slice group");

                    group.SetSlices(property.value as List<VolumeSliceDto>);
                    return true;

                case UMI3DPropertyKeys.VolumeSlicePoints:
                    VolumeSlice slice = entity.Object as VolumeSlice;
                    if (slice == null)
                        throw new Exception("Internal error : entity is not a volume slice");

                    slice.SetPoints(property.value as List<PointDto>);
                    return true;

                case UMI3DPropertyKeys.VolumeSliceEdges:
                    VolumeSlice slice2 = entity.Object as VolumeSlice;
                    if (slice2 == null)
                        throw new Exception("Internal error : entity is not a volume slice");

                    slice2.SetEdges(property.value as List<int>);
                    return true;

                case UMI3DPropertyKeys.VolumeSliceFaces:
                    VolumeSlice slice3 = entity.Object as VolumeSlice;
                    if (slice3 == null)
                        throw new Exception("Internal error : entity is not a volume slice");

                    slice3.SetFaces(property.value as List<FaceDto>);
                    return true;

                //TODO : Primitives

                default:
                    return false;
            }
        }
    }
}