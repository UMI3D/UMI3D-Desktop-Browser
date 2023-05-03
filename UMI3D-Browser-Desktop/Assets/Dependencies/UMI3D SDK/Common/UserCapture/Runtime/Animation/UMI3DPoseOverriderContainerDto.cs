using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.common.userCapture
{
    public class UMI3DPoseOverriderContainerDto : UMI3DDto, IEntity
    {
        /// <summary>
        /// The id of the entity
        /// </summary>
        public ulong id;

        /// <summary>
        /// All the pose ovveriders of the linked container
        /// </summary>
        public PoseOverriderDto[] poseOverriderDtos;
    }
}