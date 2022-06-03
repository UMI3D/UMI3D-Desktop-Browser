using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using umi3d.cdk.userCapture;
using umi3d.common.userCapture;
using umi3d.common;
using umi3d.cdk;
using BrowserDesktop.Controller;
using System;

public class AdaptHandPose : MonoBehaviour
{
    public IKControl IKControl;

    public VirtualObjectBodyInteraction RightHand;
    public VirtualObjectBodyInteraction LeftHand;

    public Transform RightFreeAnchor;
    public Transform LeftFreeAnchor;

    public bool isRightHanded = true;

    Vector3 RightHandPos;
    Quaternion RightHandRot;

    Vector3 LeftHandPos;
    Quaternion LeftHandRot;

    UMI3DHandPoseDto currentPose = null;

    IEnumerator rightHandPlacement = null;
    IEnumerator leftHandPlacement = null;

    ulong currentHoverId = 0;
    UMI3DHandPoseDto passiveHoverPose;

    Dictionary<uint, Quaternion> rightPhalanxRotations;
    Dictionary<uint, Quaternion> leftPhalanxRotations;


    // Start is called before the first frame update
    void Start()
    {
        UMI3DClientUserTracking.Instance.handPoseEvent.AddListener((dto) => SetupHandPose(dto));
        MouseAndKeyboardController.HoverEnter.AddListener(id => { 
            currentHoverId = id;
        });
        MouseAndKeyboardController.HoverExit.AddListener(id => {
            if (currentHoverId != 0 && id.Equals(currentHoverId))
            {
                currentHoverId = 0;
                passiveHoverPose = null;
                RightFreeAnchor.transform.localRotation = Quaternion.identity;
                LeftFreeAnchor.transform.localRotation = Quaternion.identity;
            }
        });
    }

    private void Update()
    {
        if (currentHoverId != 0)
        {
            UMI3DNodeInstance node = (UMI3DEnvironmentLoader.GetEntity(currentHoverId) as UMI3DNodeInstance);
            if (node != null && node.transform != null)
            {
                RightFreeAnchor.LookAt(node.transform);
                LeftFreeAnchor.LookAt(node.transform);
            }
            else
            {
                currentHoverId = 0;
                passiveHoverPose = null;
                RightFreeAnchor.transform.localRotation = Quaternion.identity;
                LeftFreeAnchor.transform.localRotation = Quaternion.identity;
            }
        }
    }

    IEnumerator LerpRightPhalanxQuaternion(float startTime, Transform transform = null)
    {
        float elapsedTime = startTime;

        Transform relativeTransform = transform != null ? transform : RightFreeAnchor;

        while (relativeTransform != null)
        {
            elapsedTime = elapsedTime + Time.deltaTime;

            Vector3 aimPos = transform != null ? relativeTransform.TransformPoint(RightHandPos) : relativeTransform.position;
            Quaternion aimRot = relativeTransform.transform.rotation * RightHandRot;

            RightHand.transform.position = Vector3.Lerp(RightHand.transform.position, aimPos, elapsedTime);
            RightHand.transform.rotation = Quaternion.Slerp(RightHand.transform.rotation, aimRot, elapsedTime);

            foreach (var ibone in RightHand.GetComponentsInChildren<VirtualObjectBodyInteractionBone>())
                ibone.transform.localRotation = Quaternion.Slerp(ibone.transform.localRotation, rightPhalanxRotations[ibone.bone.Convert()], elapsedTime);

            yield return null;
        }

        currentPose = null;
        IKControl.rightIkActive = false;
    }

    IEnumerator LerpLeftPhalanxQuaternion(float startTime, Transform transform = null)
    {
        float elapsedTime = startTime;

        Transform relativeTransform = transform != null ? transform : LeftFreeAnchor;

        while (relativeTransform != null)
        {
            elapsedTime = elapsedTime + Time.deltaTime;

            Vector3 aimPos = transform != null ? relativeTransform.TransformPoint(LeftHandPos) : relativeTransform.position;
            Quaternion aimRot = relativeTransform.transform.rotation * LeftHandRot;

            LeftHand.transform.position = Vector3.Lerp(LeftHand.transform.position, aimPos, elapsedTime);
            LeftHand.transform.rotation = Quaternion.Slerp(LeftHand.transform.rotation, aimRot, elapsedTime);

            foreach (var ibone in LeftHand.GetComponentsInChildren<VirtualObjectBodyInteractionBone>())
                ibone.transform.localRotation = Quaternion.Slerp(ibone.transform.localRotation, leftPhalanxRotations[ibone.bone.Convert()], elapsedTime);

            yield return null;
        }

        currentPose = null;
        IKControl.leftIkActive = false;
    }

    public void SetupHandPose(UMI3DHandPoseDto dto)
    {
        if (dto.IsActive)
        {
            if (currentPose != null && currentPose.HoverPose == false && dto.HoverPose == true)
            {
                passiveHoverPose = dto;
                return;
            }

            if (currentPose != null && currentPose.HoverPose == true && dto.HoverPose == false)
            {
                passiveHoverPose = currentPose;
            }

            if (isRightHanded)
            {
                if (rightHandPlacement != null)
                    StopCoroutine(rightHandPlacement);

                if (currentPose != null)
                    (UMI3DEnvironmentLoader.GetEntity(currentPose.id).dto as UMI3DHandPoseDto).IsActive = false;

                currentPose = dto;

                rightPhalanxRotations = GetHandedRotations(dto.PhalanxRotations);

                RightHandPos = dto.RightHandPosition;
                RightHandRot = Quaternion.Euler(dto.RightHandEulerRotation);

                Transform relativeNode = (currentHoverId != 0 && dto.isRelativeToNode) ? (UMI3DEnvironmentLoader.GetEntity(currentHoverId) as UMI3DNodeInstance).transform : null;

                IKControl.rightIkActive = true;

                rightHandPlacement = LerpRightPhalanxQuaternion(0, relativeNode);
                StartCoroutine(rightHandPlacement);
            }
            else
            {
                if (leftHandPlacement != null)
                    StopCoroutine(leftHandPlacement);

                if (currentPose != null)
                    (UMI3DEnvironmentLoader.GetEntity(currentPose.id).dto as UMI3DHandPoseDto).IsActive = false;

                currentPose = dto;

                leftPhalanxRotations = GetHandedRotations(dto.PhalanxRotations);

                LeftHandPos = dto.LeftHandPosition;
                LeftHandRot = Quaternion.Euler(dto.LeftHandEulerRotation);

                Transform relativeNode = (currentHoverId != 0 && dto.isRelativeToNode) ? (UMI3DEnvironmentLoader.GetEntity(currentHoverId) as UMI3DNodeInstance).transform : null;

                IKControl.leftIkActive = true;

                leftHandPlacement = LerpLeftPhalanxQuaternion(0, relativeNode);
                StartCoroutine(leftHandPlacement);
            }
        }
        else
        {
            if (isRightHanded)
            {
                if (currentPose != null && dto.id.Equals(currentPose.id))
                {
                    if (passiveHoverPose != null)
                    {
                        StopCoroutine(rightHandPlacement);
                        currentPose = passiveHoverPose;

                        rightPhalanxRotations = GetHandedRotations(currentPose.PhalanxRotations);

                        RightHandPos = currentPose.RightHandPosition;
                        RightHandRot = Quaternion.Euler(currentPose.RightHandEulerRotation);

                        Transform relativeNode = (currentHoverId != 0 && currentPose.isRelativeToNode) ? (UMI3DEnvironmentLoader.GetEntity(currentHoverId) as UMI3DNodeInstance).transform : null;

                        rightHandPlacement = LerpRightPhalanxQuaternion(0, relativeNode);
                        StartCoroutine(rightHandPlacement);
                    }
                    else
                    {
                        currentPose = null;
                        IKControl.rightIkActive = false;
                    }
                }
            }
            else
            {
                if (currentPose != null && dto.id.Equals(currentPose.id))
                {
                    if (passiveHoverPose != null)
                    {
                        StopCoroutine(leftHandPlacement);
                        currentPose = passiveHoverPose;

                        leftPhalanxRotations = GetHandedRotations(currentPose.PhalanxRotations);

                        LeftHandPos = currentPose.LeftHandPosition;
                        LeftHandRot = Quaternion.Euler(currentPose.LeftHandEulerRotation);

                        Transform relativeNode = (currentHoverId != 0 && currentPose.isRelativeToNode) ? (UMI3DEnvironmentLoader.GetEntity(currentHoverId) as UMI3DNodeInstance).transform : null;

                        leftHandPlacement = LerpLeftPhalanxQuaternion(0, relativeNode);
                        StartCoroutine(leftHandPlacement);
                    }
                    else
                    {
                        currentPose = null;
                        IKControl.leftIkActive = false;
                    }
                }
            }
        }
    }

    private Dictionary<uint, Quaternion> GetHandedRotations(Dictionary<uint, SerializableVector3> PhalanxRotations)
    {
        Dictionary<uint, Quaternion> handedPhalanxRotations = new Dictionary<uint, Quaternion>();

        if (isRightHanded)
        {
            handedPhalanxRotations.Add((BoneType.RightThumbProximal) , Quaternion.Euler(PhalanxRotations[BoneType.RightThumbProximal]));
            handedPhalanxRotations.Add((BoneType.RightThumbIntermediate), Quaternion.Euler(PhalanxRotations[BoneType.RightThumbIntermediate]));
            handedPhalanxRotations.Add((BoneType.RightThumbDistal), Quaternion.Euler(PhalanxRotations[BoneType.RightThumbDistal]));

            handedPhalanxRotations.Add((BoneType.RightIndexProximal), Quaternion.Euler(PhalanxRotations[BoneType.RightIndexProximal]));
            handedPhalanxRotations.Add((BoneType.RightIndexIntermediate), Quaternion.Euler(PhalanxRotations[BoneType.RightIndexIntermediate]));
            handedPhalanxRotations.Add((BoneType.RightIndexDistal), Quaternion.Euler(PhalanxRotations[BoneType.RightIndexDistal]));

            handedPhalanxRotations.Add((BoneType.RightMiddleProximal), Quaternion.Euler(PhalanxRotations[BoneType.RightMiddleProximal]));
            handedPhalanxRotations.Add((BoneType.RightMiddleIntermediate), Quaternion.Euler(PhalanxRotations[BoneType.RightMiddleIntermediate]));
            handedPhalanxRotations.Add((BoneType.RightMiddleDistal), Quaternion.Euler(PhalanxRotations[BoneType.RightMiddleDistal]));

            handedPhalanxRotations.Add((BoneType.RightRingProximal), Quaternion.Euler(PhalanxRotations[BoneType.RightRingProximal]));
            handedPhalanxRotations.Add((BoneType.RightRingIntermediate), Quaternion.Euler(PhalanxRotations[BoneType.RightRingIntermediate]));
            handedPhalanxRotations.Add((BoneType.RightRingDistal), Quaternion.Euler(PhalanxRotations[BoneType.RightRingDistal]));

            handedPhalanxRotations.Add((BoneType.RightLittleProximal), Quaternion.Euler(PhalanxRotations[BoneType.RightLittleProximal]));
            handedPhalanxRotations.Add((BoneType.RightLittleIntermediate), Quaternion.Euler(PhalanxRotations[BoneType.RightLittleIntermediate]));
            handedPhalanxRotations.Add((BoneType.RightLittleDistal), Quaternion.Euler(PhalanxRotations[BoneType.RightLittleDistal]));
        }
        else
        {
            handedPhalanxRotations.Add((BoneType.LeftThumbProximal), Quaternion.Euler(PhalanxRotations[BoneType.LeftThumbProximal]));
            handedPhalanxRotations.Add((BoneType.LeftThumbIntermediate), Quaternion.Euler(PhalanxRotations[BoneType.LeftThumbIntermediate]));
            handedPhalanxRotations.Add((BoneType.LeftThumbDistal), Quaternion.Euler(PhalanxRotations[BoneType.LeftThumbDistal]));

            handedPhalanxRotations.Add((BoneType.LeftIndexProximal), Quaternion.Euler(PhalanxRotations[BoneType.LeftIndexProximal]));
            handedPhalanxRotations.Add((BoneType.LeftIndexIntermediate), Quaternion.Euler(PhalanxRotations[BoneType.LeftIndexIntermediate]));
            handedPhalanxRotations.Add((BoneType.LeftIndexDistal), Quaternion.Euler(PhalanxRotations[BoneType.LeftIndexDistal]));

            handedPhalanxRotations.Add((BoneType.LeftMiddleProximal), Quaternion.Euler(PhalanxRotations[BoneType.LeftMiddleProximal]));
            handedPhalanxRotations.Add((BoneType.LeftMiddleIntermediate), Quaternion.Euler(PhalanxRotations[BoneType.LeftMiddleIntermediate]));
            handedPhalanxRotations.Add((BoneType.LeftMiddleDistal), Quaternion.Euler(PhalanxRotations[BoneType.LeftMiddleDistal]));

            handedPhalanxRotations.Add((BoneType.LeftRingProximal), Quaternion.Euler(PhalanxRotations[BoneType.LeftRingProximal]));
            handedPhalanxRotations.Add((BoneType.LeftRingIntermediate), Quaternion.Euler(PhalanxRotations[BoneType.LeftRingIntermediate]));
            handedPhalanxRotations.Add((BoneType.LeftRingDistal), Quaternion.Euler(PhalanxRotations[BoneType.LeftRingDistal]));

            handedPhalanxRotations.Add((BoneType.LeftLittleProximal), Quaternion.Euler(PhalanxRotations[BoneType.LeftLittleProximal]));
            handedPhalanxRotations.Add((BoneType.LeftLittleIntermediate), Quaternion.Euler(PhalanxRotations[BoneType.LeftLittleIntermediate]));
            handedPhalanxRotations.Add((BoneType.LeftLittleDistal), Quaternion.Euler(PhalanxRotations[BoneType.LeftLittleDistal]));
        }

        return handedPhalanxRotations;
    }
}