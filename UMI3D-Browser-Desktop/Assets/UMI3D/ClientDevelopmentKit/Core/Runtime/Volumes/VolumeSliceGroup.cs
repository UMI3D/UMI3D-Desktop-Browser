using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.common.volume;

namespace umi3d.cdk.volumes
{
	public class VolumeSliceGroup : AbstractVolumeCell
	{
        public string id { get; private set; }
        private List<VolumeSlice> slices = new List<VolumeSlice>();

        public void Setup(VolumeDto dto)
        {
            id = dto.id;
            slices = VolumeSliceGroupManager.Instance.GetVolumeSlices();
        }	
        
        public override bool IsInside(Vector3 point)
        {
            foreach(VolumeSlice s in slices)
            {
                if (s.isInside(point))
                    return true;
            }
            return false;
        }

        public void SetSlices(List<VolumeSliceDto> newSlices)
        {
            slices = newSlices.ConvertAll(dto => VolumeSliceGroupManager.Instance.GetVolumeSlice(dto.id));
        }

        public VolumeSlice[] GetSlices() => slices.ToArray();
	}
}