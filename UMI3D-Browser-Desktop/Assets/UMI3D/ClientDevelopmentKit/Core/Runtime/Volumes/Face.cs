using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.common.volume;

namespace umi3d.cdk.volumes
{
	public class Face 
	{
        public Point[] points { get => points_.ToArray(); }
        private List<Point> points_;

        public void Setup(FaceDto dto)
        {
            points_ = dto.pointsIds.ConvertAll(id => VolumeManager.Instance.GetPoint(id));
        }

        public void SetPoints(List<int> newPoints)
        {
            throw new System.NotImplementedException(); //todo
        }

        public GeometryTools.Face3 ToFace3()
        { 
            throw new System.NotImplementedException(); //todo
        }
	}
}