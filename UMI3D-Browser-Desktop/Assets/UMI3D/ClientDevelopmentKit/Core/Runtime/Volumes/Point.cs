using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.common.volume;

namespace umi3d.cdk.volumes
{
	public class Point : AbstractVolumePart
	{
        public Vector3 position { get; private set; }

    	public void Setup(PointDto dto)
        {
            position = dto.position;
        }

        public void SetPosition(Vector3 newPosition)
        {
            position = newPosition;
        }
	}
}