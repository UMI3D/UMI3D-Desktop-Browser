using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using umi3d.cdk.userCapture;
using umi3d.common.userCapture;
using umi3d.common;
using umi3d.cdk;
using System;


public class AdaptBodyPose : MonoBehaviour
{
    public IKControl IKControl;

    public VirtualObjectBodyInteraction RightHand;
    public VirtualObjectBodyInteraction LeftHand;

    public VirtualObjectBodyInteraction RightFoot;
    public VirtualObjectBodyInteraction LeftFoot;

    public Transform Hips;

    Vector3 RightHandPos;
    Quaternion RightHandRot;

    Vector3 LeftHandPos;
    Quaternion LeftHandRot;

    Vector3 RightAnklePos;
    Quaternion RightAnkleRot;

    Vector3 LeftAnklePos;
    Quaternion LeftAnkleRot;

    ulong currentVehicleId = 0;
    UMI3DBodyPoseDto currentPose = null;

    IEnumerator bodyPlacement = null;


    void Start()
    {
        UMI3DClientUserTracking.Instance.boardedVehicleEvent.AddListener((vehicleId) => currentVehicleId = vehicleId);
        UMI3DClientUserTracking.Instance.bodyPoseEvent.AddListener((dto) => SetupBodyPose(dto));
    }

    IEnumerator LerpJointsQuaternion(float startTime, Transform relativeNode)
    {
        float elapsedTime = startTime;

        while (relativeNode != null)
        {
            elapsedTime = elapsedTime + Time.deltaTime;

            Vector3 aimRightHandPos = Hips.TransformPoint(RightHandPos);
            Quaternion aimRightHandRot = Hips.rotation * RightHandRot;

            Vector3 aimLeftHandPos = Hips.TransformPoint(LeftHandPos);
            Quaternion aimLeftHandRot = Hips.rotation * LeftHandRot;

            Vector3 aimRightFootPos = Hips.TransformPoint(RightAnklePos);
            Quaternion aimRightFootRot = Hips.rotation * RightAnkleRot;

            Vector3 aimLeftFootPos = Hips.TransformPoint(LeftAnklePos);
            Quaternion aimLeftFootRot = Hips.rotation * LeftAnkleRot;

            RightHand.transform.position = Vector3.Lerp(RightHand.transform.position, aimRightHandPos, elapsedTime);
            RightHand.transform.rotation = Quaternion.Slerp(RightHand.transform.rotation, aimRightHandRot, elapsedTime);

            LeftHand.transform.position = Vector3.Lerp(LeftHand.transform.position, aimLeftHandPos, elapsedTime);
            LeftHand.transform.rotation = Quaternion.Slerp(LeftHand.transform.rotation, aimLeftHandRot, elapsedTime);

            RightFoot.transform.position = Vector3.Lerp(RightFoot.transform.position, aimRightFootPos, elapsedTime);
            RightFoot.transform.rotation = Quaternion.Slerp(RightFoot.transform.rotation, aimRightFootRot, elapsedTime);

            LeftFoot.transform.position = Vector3.Lerp(LeftFoot.transform.position, aimLeftFootPos, elapsedTime);
            LeftFoot.transform.rotation = Quaternion.Slerp(LeftFoot.transform.rotation, aimLeftFootRot, elapsedTime);

            yield return null;
        }

        currentPose = null;
        IKControl.leftIkActive = IKControl.rightIkActive = IKControl.overrideFeetIk = false;
    }

    public void SetupBodyPose(UMI3DBodyPoseDto dto)
    {
        if (dto.IsActive)
        {
            if (bodyPlacement != null)
                StopCoroutine(bodyPlacement);

            if (currentPose != null)
                (UMI3DEnvironmentLoader.GetEntity(currentPose.id).dto as UMI3DBodyPoseDto).IsActive = false;

            currentPose = dto;

            LeftAnklePos = dto.TargetTransforms[BoneType.LeftAnkle].Key;
            LeftAnkleRot = Quaternion.Euler(dto.TargetTransforms[BoneType.LeftAnkle].Value);

            RightAnklePos = dto.TargetTransforms[BoneType.RightAnkle].Key;
            RightAnkleRot = Quaternion.Euler(dto.TargetTransforms[BoneType.RightAnkle].Value);

            LeftHandPos = dto.TargetTransforms[BoneType.LeftHand].Key;
            LeftHandRot = Quaternion.Euler(dto.TargetTransforms[BoneType.LeftHand].Value);

            RightHandPos = dto.TargetTransforms[BoneType.RightHand].Key;
            RightHandRot = Quaternion.Euler(dto.TargetTransforms[BoneType.RightHand].Value);

            Transform relativeNode = (UMI3DEnvironmentLoader.GetEntity(currentVehicleId) as UMI3DNodeInstance).transform;

            IKControl.leftIkActive = IKControl.rightIkActive = IKControl.overrideFeetIk = true;

            bodyPlacement = LerpJointsQuaternion(0, relativeNode);
            StartCoroutine(bodyPlacement);
        }
        else
        {
            if (currentPose != null && dto.id.Equals(currentPose.id))
            {
                currentPose = null;
                IKControl.leftIkActive = IKControl.rightIkActive = IKControl.overrideFeetIk = false;
            }
        }
    }
}

