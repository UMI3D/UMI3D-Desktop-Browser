/*
Copyright 2019 - 2023 Inetum

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
using System.Collections.Generic;
using umi3d.baseBrowser.cursor;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.cdk;
using umi3d.cdk.interaction;
using umi3d.cdk.menu;
using umi3d.common;
using umi3d.common.interaction;
using umi3d.desktopBrowser.Controller;
using umi3d.mobileBrowser.Controller;
using UnityEngine;

namespace umi3d.baseBrowser.Controller
{
    public partial class BaseController : AbstractController
    {
        #region Fields

        public static bool Exists => s_instance != null;
        public static BaseController Instance
        {
            get => s_instance;
            set
            {
                if (Exists) return;
                s_instance = value;
            }
        }
        protected static BaseController s_instance;

        [SerializeField]
        protected InteractionMapper InteractionMapper;
        [SerializeField]
        protected Transform CameraTransform;

        [Header("Actions' parents")]
        public GameObject ParameterActions;
        public GameObject EventActions;
        public GameObject ManipulationGroupActions;
        public GameObject ManipulationActions;

        [Header("Keyboard' parents")]
        public GameObject KeyboardActions;
        public GameObject KeyboardShortcuts;
        public GameObject KeyboardEmotes;
        public GameObject KeyboardNavigations;
        public GameObject KeyboardManipulations;

        [HideInInspector]
        public MenuAsset ObjectMenu;
        public MenuAsset ManipulationMenu;
        public CursorData mouseData;

        public IConcreteController CurrentController;

        protected List<IConcreteController> m_controllers = new List<IConcreteController>();
        
        [Header("Bone Type")]
        /// <summary>
        /// Avatar bone linked to this input.
        /// </summary>
        [SerializeField]
        [inetum.unityUtils.ConstEnum(typeof(common.userCapture.BoneType), typeof(uint))]
        public uint interactionBoneType = common.userCapture.BoneType.RightHand;
        [SerializeField]
        [inetum.unityUtils.ConstEnum(typeof(common.userCapture.BoneType), typeof(uint))]
        protected uint hoverBoneType = common.userCapture.BoneType.Head;

        protected int m_navigationDirect = 0;
        protected AutoProjectOnHover reason = new AutoProjectOnHover();

        public static event System.Action<ulong> HoverEnter;
        public static event System.Action<ulong> HoverUpdate;
        public static event System.Action<ulong> HoverExit;
        public static bool CanProcess = false;
        #endregion

        #region Monobehaviour Life Cycle
        protected virtual void Awake()
        {
            s_instance = this;

            mouseData.ForceProjectionReleasableButton = new ButtonMenuItem
            {
                Name = "Release",
                IsHoldable = false
            };
            mouseData.ForceProjectionReleasableButton.Subscribe(ReleaseForceProjection);

            mouseData.saveDelay = 0;
            ObjectMenu = Resources.Load<MenuAsset>("Scriptables/GamePanel/ObjectMenu");
            ManipulationMenu = Resources.Load<MenuAsset>("Scriptables/GamePanel/ManipulationMenu");

            ManipulationGroupInputs.AddRange(ManipulationGroupActions.GetComponents<BaseManipulationGroup>());
            //TODO instantiate concrete controllers.
            m_controllers.Add
            (
                new KeyboardAndMouseController() 
                { 
                    Controller = this,
                    ObjectMenu = ObjectMenu,
                    ManipulationGroup = ManipulationGroupInputs.Find(a => a is ManipulationGroupeForDesktop)
                }
            );
            m_controllers.Add
            (
                new MobileController()
            );

            m_controllers.ForEach(controller => controller?.Awake());

            //TODO for now CurrentController is the desktop one.
            CurrentController = m_controllers.Find(controller => controller is KeyboardAndMouseController);
        }

        protected virtual void Start()
        {
            m_controllers.ForEach(controller => controller?.Start());
        }

        protected virtual void LateUpdate()
        {
            if (!CanProcess) return;

            m_navigationDirect = 0;
            MouseHandler();
        }
        protected virtual void Update() 
        {
            CurrentController?.Update();
        }

        private void OnDisable()
        {
            KeyboardInteraction.S_Interactions.Clear();
            KeyboardShortcut.S_Shortcuts.Clear();
            KeyboardEmote.S_Emotes.Clear();
            KeyboardNavigation.S_Navigations.Clear();
            KeyboardManipulation.S_Manipulations.Clear();
        }
        #endregion

        #region Projection
        protected void ReleaseForceProjection(bool _)
        {
            if (!mouseData.ForceProjectionReleasable) return;

            RemoveForceProjectionReleaseButton();
            UnequipeForceProjection();
        }
        private void AddForceProjectionReleaseButton()
        {
            if (mouseData.ForceProjectionReleasableButton == null || !mouseData.ForceProjectionReleasable)
                return;
            if (!ObjectMenu.menu.Contains(mouseData.ForceProjectionReleasableButton))
                ObjectMenu.menu.Add(mouseData.ForceProjectionReleasableButton);
        }
        protected void RemoveForceProjectionReleaseButton()
        {
            if (mouseData.ForceProjectionReleasableButton == null) return;
            ObjectMenu.menu.Remove(mouseData.ForceProjectionReleasableButton);
        }
        protected void UnequipeForceProjection()
        {
            InteractionMapper.ReleaseTool(currentTool.id, new RequestedByUser());
            mouseData.ForceProjection = false;
            mouseData.CurrentHovered = null;
            mouseData.CurrentHoveredTransform = null;
            mouseData.OldHovered = null;
            mouseData.HoverState = HoverState.None;
        }
        private void SetAutoProjection()
        {
            InteractionMapper.SelectTool(mouseData.CurrentHovered.dto.id, true, this, mouseData.CurrentHoveredId, reason);
            mouseData.HoverState = HoverState.AutoProjected;
            BaseCursor.State = BaseCursor.CursorState.Hover;
            mouseData.LastProjected = mouseData.CurrentHovered;
        }
        private void ReleaseAutoProjection()
        {
            if (mouseData.HoverState == HoverState.AutoProjected && currentTool != null)
                InteractionMapper.ReleaseTool(currentTool.id, new RequestedByUser());
            mouseData.HoverState = HoverState.None;
            BaseCursor.State = BaseCursor.CursorState.Default;
            mouseData.LastProjected = null;
        }
        #endregion

        #region Update Tool
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="reason"></param>
        public override void Release(AbstractTool tool, InteractionMappingReason reason)
        {
            base.Release(tool, reason);
            if (reason is ToolNeedToBeUpdated && tool.interactions.Count > 0) return;

            if (mouseData.CurrentHovered != null && mouseData.CurrentHovered.dto.id == tool.id)
            {
                mouseData.CurrentHovered = null;
                mouseData.CurrentHoveredTransform = null;
                mouseData.HoverState = HoverState.None;
                BaseCursor.State = BaseCursor.CursorState.Default;
            }
            if (mouseData.ForceProjection)
            {
                mouseData.ForceProjection = false;
                RemoveForceProjectionReleaseButton();
            }
            tool.onReleased(interactionBoneType);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="releasable"></param>
        /// <param name="reason"></param>
        /// <param name="hoveredObjectId"></param>
        public override void Project(AbstractTool tool, bool releasable, InteractionMappingReason reason, ulong hoveredObjectId)
        {
            base.Project(tool, releasable, reason, hoveredObjectId);
            if (reason is RequestedByEnvironment)
            {
                mouseData.ForceProjection = true;
                mouseData.ForceProjectionReleasable = releasable;
                mouseData.LastProjected = null;
            }
            tool.onProjected(interactionBoneType);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public override bool IsCompatibleWith(AbstractTool tool)
        {
            foreach (var man in tool.interactions.FindAll(x => x is ManipulationDto))
            {
                System.Predicate<DofGroupOptionDto> predicat = (sep) =>
                {
                    foreach (DofGroupDto dof in sep.separations)
                        if (!BaseManipulationGroup.DofGroups.Contains(dof.dofs)) return false;
                    return true;
                };

                if (!(man as ManipulationDto).dofSeparationOptions.Exists(predicat)) return false;
            }
            return true;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public override bool RequiresMenu(AbstractTool tool)
        {
            List<AbstractInteractionDto> interactions = tool.interactions;
            List<AbstractInteractionDto> manips = interactions.FindAll(x => x is ManipulationDto);
            List<AbstractInteractionDto> events = interactions.FindAll(x => x is EventDto);
            //List<AbstractInteractionDto> parameters = tool.Interactions.FindAll(x => x is AbstractParameterDto);
            // return ((events.Count > 7 || manips.Count > 0) && (events.Count > 6 || manips.Count > 1));
            return false; // (/*(parameters.Count > 0) ||*/ (events.Count > 7) || (manips.Count > 1) || ((manips.Count > 0) && (events.Count > 6)));
        }
        protected override ulong GetCurrentHoveredId()
            => mouseData.CurrentHoveredId;
        protected override bool isInteracting()
        {
            throw new System.NotImplementedException();
        }
        protected override bool isNavigating()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Create a menu to access each interactions of a tool separately.
        /// </summary>
        /// <param name="interactions"></param>
        public override void CreateInteractionsMenuFor(AbstractTool tool)
        {
            Debug.Log("oups");
        }
        //bool ShouldAutoProject(InteractableDto tool)
        //{
        //    List<AbstractInteractionDto> manips = tool.interactions.FindAll(x => x is ManipulationDto);
        //    List<AbstractInteractionDto> events = tool.interactions.FindAll(x => x is EventDto);
        //    List<AbstractInteractionDto> parameters = tool.interactions.FindAll(x => x is AbstractParameterDto);
        //    return (((parameters.Count == 0) && (events.Count <= 7) && (manips.Count == 0)));
        //}

        //public bool RequiresParametersMenu(AbstractTool tool)
        //{
        //    List<AbstractInteractionDto> interactions = tool.interactions;
        //    List<AbstractInteractionDto> parameters = interactions.FindAll(x => x is AbstractParameterDto);
        //    // return ((events.Count > 7 || manips.Count > 0) && (events.Count > 6 || manips.Count > 1));
        //    return (parameters.Count > 0);
        //}
        protected void UpdateTool()
        {
            if (mouseData.ForceProjection && mouseData.ForceProjectionReleasable)
            {
                AddForceProjectionReleaseButton();
                //if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.LeaveContextualMenu)))
                //    UnequipeForceProjection();
            }
            UpdateToolForForceProjection();
            UpdateToolForNonForceProjection();
        }

        private void UpdateToolForForceProjection()
        {
            if (!mouseData.ForceProjection) return;

            if (mouseData.CurrentHovered != null)
            {
                if (mouseData.CurrentHovered == mouseData.LastProjected) return;
                mouseData.HoverState = HoverState.Hovering;
            }
            else if (mouseData.LastProjected != null) mouseData.HoverState = HoverState.None;
            mouseData.LastProjected = null;
        }

        private void UpdateToolForNonForceProjection()
        {
            if (mouseData.ForceProjection) return;

            if (mouseData.CurrentHovered != null)
            {
                if (mouseData.CurrentHovered == mouseData.LastProjected) return;
                if (mouseData.LastProjected != null) ReleaseAutoProjection();
                bool isInteractionsEmpty = mouseData.CurrentHovered.dto.interactions.Count == 0;
                bool isCompatible = IsCompatibleWith(mouseData.CurrentHovered);
                if (!isInteractionsEmpty && isCompatible && !baseBrowser.inputs.interactions.EventInteraction.IsInputHold) SetAutoProjection();
            }
            else if (mouseData.LastProjected != null) ReleaseAutoProjection();
        }

        #endregion

        private void MouseHandler()
        {
            mouseData.Save();
            Ray ray = new Ray(CameraTransform.position, CameraTransform.forward);
            RaycastHit[] hits = umi3d.common.Physics.RaycastAll(ray, 100f);

            //1. Cast a ray to find all interactables
            List<(RaycastHit, InteractableContainer)> interactables = new List<(RaycastHit, InteractableContainer)>();
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.GetComponentInParent<cdk.UMI3DEnvironmentLoader>() == null) continue;
                var interactable = hit.collider.gameObject.GetComponent<InteractableContainer>();
                if (interactable == null) interactable = hit.collider.gameObject.GetComponentInParent<InteractableContainer>();
                if (interactable != null) interactables.Add((hit, interactable));
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

                if (!interactable.Active) continue;

                if (interactable.InteractionDistance >= 0 && interactable.InteractionDistance < hit.distance) continue;

                mouseData.CurrentHoveredId = cdk.UMI3DEnvironmentLoader.GetNodeID(hit.collider);

                mouseData.CurrentHovered = interactable;
                mouseData.CurrentHoveredTransform = interactableContainer.transform;

                mouseData.Position = interactableContainer.transform.InverseTransformPoint(hit.point);
                mouseData.WorldPosition = hit.point;
                if (Vector3.Distance(mouseData.WorldPosition, hit.transform.position) < 0.1f) mouseData.CenteredWorldPosition = hit.transform.position;
                else mouseData.CenteredWorldPosition = mouseData.WorldPosition;

                mouseData.Normal = interactableContainer.transform.InverseTransformDirection(hit.normal);
                mouseData.WorldNormal = hit.normal;

                mouseData.Direction = interactableContainer.transform.InverseTransformDirection(ray.direction);
                mouseData.WorlDirection = ray.direction;

                break;
            }
            if (BaseCursor.State != BaseCursor.CursorState.Clicked) UpdateTool();

            Hover();
        }

        #region Hover
        protected void Hover()
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

                mouseData.CurrentHovered.Hovered(hoverBoneType, mouseData.CurrentHoveredId, mouseData.Position, mouseData.Normal, mouseData.Direction);
            }
        }
        private void OldHoverExitAndCurrentHoverEnter()
        {
            OldHoverExit();
            CurrentHoverEnter();
        }
        private void OldHoverExit()
        {
            if (mouseData.OldHovered == null) return;

            ulong lastHoverId = mouseData.LastHoveredId;
            mouseData.OldHovered
                .HoverExit(hoverBoneType, lastHoverId, mouseData.LastPosition, mouseData.LastNormal, mouseData.LastDirection);

            ulong hoverExitAnimationId = mouseData.OldHovered.dto.HoverExitAnimationId;
            if (hoverExitAnimationId != 0)
            {
                cdk.UMI3DAbstractAnimation anim = cdk.UMI3DAbstractAnimation.Get(hoverExitAnimationId);

                anim.SetUMI3DProperty(UMI3DEnvironmentLoader.GetEntity(hoverExitAnimationId), new SetEntityPropertyDto()
                {
                    entityId = hoverExitAnimationId,
                    property = UMI3DPropertyKeys.AnimationPlaying,
                    value = true
                });

                HoverExit.Invoke(lastHoverId);
                if (anim != null) anim.Start();
            }
            mouseData.OldHovered = null;
        }
        private void CurrentHoverEnter()
        {
            if (mouseData.CurrentHovered == null) return;

            ulong currentHoverId = mouseData.CurrentHoveredId;
            mouseData.CurrentHovered
                .HoverEnter(hoverBoneType, currentHoverId, mouseData.Position, mouseData.Normal, mouseData.Direction);

            ulong hoverEnterAnimationId = mouseData.CurrentHovered.dto.HoverEnterAnimationId;
            if (hoverEnterAnimationId != 0)
            {
                cdk.UMI3DAbstractAnimation anim = cdk.UMI3DAbstractAnimation.Get(hoverEnterAnimationId);

                anim.SetUMI3DProperty(UMI3DEnvironmentLoader.GetEntity(hoverEnterAnimationId), new SetEntityPropertyDto()
                {
                    entityId = hoverEnterAnimationId,
                    property = UMI3DPropertyKeys.AnimationPlaying,
                    value = true
                });

                HoverEnter.Invoke(currentHoverId);
                if (anim != null) anim.Start();
            }
        }

        #endregion
    }
}
