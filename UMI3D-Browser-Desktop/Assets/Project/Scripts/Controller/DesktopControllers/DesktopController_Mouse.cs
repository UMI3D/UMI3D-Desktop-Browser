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
using BrowserDesktop.Cursor;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.cdk.interaction;
using UnityEngine;
using UnityEngine.Events;

namespace BrowserDesktop.Controller
{
    public partial class MouseAndKeyboardController
    {
        public enum HoverState 
        { 
            None, //No hovering 
            Hovering, //Mouse is hovering an object
            AutoProjected //The projection is auto.
        }

        public struct MouseData
        {
            public bool ForceProjection;
            public bool ForceProjectionReleasable;
            public HoldableButtonMenuItem ForceProjectionReleasableButton;

            public Interactable LastProjected;
            public Interactable OldHovered;
            public ulong LastHoveredId;
            public Interactable CurrentHovered;
            public Transform CurrentHoveredTransform;
            public ulong CurrentHoveredId;

            public Vector3 point;
            public Vector3 worldPoint;
            public Vector3 centeredWorldPoint;
            public Vector3 normal;

            public Vector3 worldNormal;
            public Vector3 direction;
            public Vector3 worlDirection;
            public Vector3 cursorOffset;

            public Vector3 lastPoint, lastNormal, lastDirection;

            public HoverState HoverState;

            public int saveDelay;

            public void Save()
            {
                if (saveDelay > 0)
                    saveDelay--;
                else
                {
                    if (saveDelay < 0) saveDelay = 0;
                    OldHovered = CurrentHovered;
                    LastHoveredId = CurrentHoveredId;
                    CurrentHovered = null;
                    CurrentHoveredTransform = null;
                    CurrentHoveredId = 0;
                    lastPoint = point;
                    lastNormal = normal;
                    lastDirection = direction;
                }
            }

            public bool isDelaying()
                => saveDelay > 0;
        }

        public class HoverEvent : UnityEvent<ulong> { };

        public MouseData mouseData;

        public static bool IsFreeAndHovering = false;
        public static HoverEvent HoverEnter = new HoverEvent();
        public static HoverEvent HoverUpdate = new HoverEvent();
        public static HoverEvent HoverExit = new HoverEvent();

        private bool m_isCursorMovementFree
            => CursorHandler.Movement == CursorHandler.CursorMovement.Free;
    }

    public partial class MouseAndKeyboardController
    {
        private void MouseHandler()
        {
            mouseData.Save();
            Ray ray = new Ray(CameraTransform.position, CameraTransform.forward);
            Debug.DrawRay(ray.origin, ray.direction.normalized * 100f, Color.red, 0, true);
            RaycastHit[] hits = umi3d.common.Physics.RaycastAll(ray, 100f);

            //1. Cast a ray to find all interactables
            List<(RaycastHit, InteractableContainer)> interactables = new List<(RaycastHit, InteractableContainer)>();
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.GetComponentInParent<UMI3DEnvironmentLoader>() == null)
                    continue;
                var interactable = hit.collider.gameObject.GetComponent<InteractableContainer>();
                if (interactable == null)
                    interactable = hit.collider.gameObject.GetComponentInParent<InteractableContainer>();
                if (interactable != null)
                    interactables.Add((hit, interactable));
            }

            //2. Sort them by hasPriority and distance from user
            interactables.Sort(delegate ((RaycastHit, InteractableContainer) x, (RaycastHit, InteractableContainer) y)
            {
                if (x.Item2.Interactable.HasPriority && !y.Item2.Interactable.HasPriority) return -1;
                else if (!x.Item2.Interactable.HasPriority && y.Item2.Interactable.HasPriority) return 1;
                else
                {
                    if (Vector3.Distance(CameraTransform.position, x.Item1.point) >= Vector3.Distance(CameraTransform.position, y.Item1.point))
                        return 1;
                    else
                        return -1;
                }
            });

            foreach ((RaycastHit, InteractableContainer) entry in interactables)
            {
                InteractableContainer interactableContainer = entry.Item2;
                Interactable interactable = interactableContainer.Interactable;
                RaycastHit hit = entry.Item1;

                if (!interactable.Active)
                    continue;

                mouseData.CurrentHoveredId = UMI3DEnvironmentLoader.GetNodeID(hit.collider);

                mouseData.CurrentHovered = interactable;
                mouseData.CurrentHoveredTransform = interactableContainer.transform;

                mouseData.point = interactableContainer.transform.InverseTransformPoint(hit.point);
                mouseData.worldPoint = hit.point;
                if (Vector3.Distance(mouseData.worldPoint, hit.transform.position) < 0.1f) mouseData.centeredWorldPoint = hit.transform.position;
                else mouseData.centeredWorldPoint = mouseData.worldPoint;

                mouseData.normal = interactableContainer.transform.InverseTransformDirection(hit.normal);
                mouseData.worldNormal = hit.normal;

                mouseData.direction = interactableContainer.transform.InverseTransformDirection(ray.direction);
                mouseData.worlDirection = ray.direction;

                break;
            }

            if (CursorHandler.State != CursorHandler.CursorState.Clicked)
                UpdateTool();

            Hover();
        }

        #region Hover

        private void Hover()
        {
            if (mouseData.CurrentHovered == null)
            {
                OldHoverExit();
                return;
            }

            if (mouseData.CurrentHovered != mouseData.OldHovered)
                OldHoverExitAndCurrentHoverEnter();
            else
            {
                if ((mouseData.LastHoveredId != 0)
                    &&
                    (mouseData.CurrentHoveredId != mouseData.LastHoveredId)
                    &&
                    associatedInputs.ContainsKey(mouseData.CurrentHovered.dto.id))
                {
                    foreach (var input in associatedInputs[mouseData.CurrentHovered.dto.id])
                        input.UpdateHoveredObjectId(mouseData.CurrentHoveredId);
                }
            }
        }

        private void OldHoverExitAndCurrentHoverEnter()
        {
            OldHoverExit();
            CurrentHoverEnter();
        }

        private void OldHoverExit()
        {
            if (mouseData.OldHovered == null)
                return;

            ulong lastHoverId = mouseData.LastHoveredId;
            mouseData.OldHovered
                .HoverExit(hoverBoneType, lastHoverId, mouseData.lastPoint, mouseData.lastNormal, mouseData.lastDirection);

            ulong hoverExitAnimationId = mouseData.OldHovered.dto.HoverExitAnimationId;
            if (hoverExitAnimationId != 0)
            {
                UMI3DNodeAnimation anim = UMI3DNodeAnimation.Get(hoverExitAnimationId);
                HoverExit.Invoke(lastHoverId);
                if (anim != null)
                    anim.Start();
            }
            mouseData.OldHovered = null;
        }

        private void CurrentHoverEnter()
        {
            if (mouseData.CurrentHovered == null)
                return;

            ulong currentHoverId = mouseData.CurrentHoveredId;
            mouseData.CurrentHovered
                .HoverEnter(hoverBoneType, currentHoverId, mouseData.point, mouseData.normal, mouseData.direction);

            ulong hoverEnterAnimationId = mouseData.CurrentHovered.dto.HoverEnterAnimationId;
            if (hoverEnterAnimationId != 0)
            {
                UMI3DNodeAnimation anim = UMI3DNodeAnimation.Get(hoverEnterAnimationId);
                HoverEnter.Invoke(currentHoverId);
                if (anim != null)
                    anim.Start();
            }
        }

        #endregion
    }
}