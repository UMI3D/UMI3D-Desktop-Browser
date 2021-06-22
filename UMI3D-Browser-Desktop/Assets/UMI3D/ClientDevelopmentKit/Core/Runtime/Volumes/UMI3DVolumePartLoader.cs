using umi3d.common.volume;
using umi3d.common;
using System;
using System.Collections.Generic;

namespace umi3d.cdk.volumes
{
	static public class UMI3DVolumePartLoader 
	{
        static public void ReadUMI3DExtension(VolumePartDto dto, Action finished, Action<string> failed)
        {
            switch (dto)
            {
                case PointDto pointDto:
                    VolumeManager.Instance.CreatePoint(pointDto, p =>
                    {
                        UMI3DEnvironmentLoader.RegisterEntityInstance(dto.id, dto, p, () => VolumeManager.Instance.DeletePoint(dto.id));
                        finished.Invoke();
                    });                    
                    break;

                case FaceDto faceDto:
                    VolumeManager.Instance.CreateFace(faceDto, f =>
                    {
                        UMI3DEnvironmentLoader.RegisterEntityInstance(dto.id, dto, f, () => VolumeManager.Instance.DeleteFace(dto.id));
                        finished.Invoke();
                    });                   
                    break;

                case VolumeSliceDto slice:
                    VolumeManager.Instance.CreateVolumeSlice(slice, s =>
                    {
                        UMI3DEnvironmentLoader.RegisterEntityInstance(dto.id, dto, s, () => VolumeManager.Instance.DeleteVolumeSlice(dto.id));
                    });
                    break;

                case VolumeDto volume:
                    VolumeManager.Instance.CreateVolumeCell(volume, v =>
                    {
                        UMI3DEnvironmentLoader.RegisterEntityInstance(dto.id, dto, v, () => VolumeManager.Instance.DeleteVolumeCell(dto.id));
                        finished.Invoke();
                    });                    
                    break;

                default:
                    failed("Unknown dto type");
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

                    face.SetPoints(property.value as List<string>);
                    return true;

                case UMI3DPropertyKeys.VolumeSlices:
                    VolumeSliceGroup cell = entity.Object as VolumeSliceGroup;
                    if (cell == null)
                        throw new Exception("Internal error : entity is not a volume cell");

                    cell.SetSlices(property.value as List<VolumeSliceDto>);
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

                default:
                    return false;
            }
        }
    }
}