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
using BrowserDesktop.Interaction;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.cdk.interaction;
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.baseBrowser.Controller
{
    public abstract class BaseController : AbstractController
    {
        #region Types
        public class HoverEvent : UnityEngine.Events.UnityEvent<ulong> { };
        public struct MouseData
        {
            public bool ForceProjection, ForceProjectionReleasable;
            public HoldableButtonMenuItem ForceProjectionReleasableButton;

            public Interactable LastProjected, OldHovered, CurrentHovered;
            public ulong LastHoveredId, CurrentHoveredId;
            public Transform CurrentHoveredTransform;

            public Vector3 LastPosition, LastNormal, LastDirection;
            public Vector3 Position, Normal, Direction;
            public Vector3 CenteredWorldPosition, WorldPosition, WorldNormal, WorlDirection;

            public HoverState HoverState;

            public int saveDelay;

            public void Save()
            {
                if (saveDelay > 0) saveDelay--;
                else
                {
                    if (saveDelay < 0) saveDelay = 0;
                    OldHovered = CurrentHovered;
                    LastHoveredId = CurrentHoveredId;
                    CurrentHovered = null;
                    CurrentHoveredTransform = null;
                    CurrentHoveredId = 0;
                    LastPosition = Position;
                    LastNormal = Normal;
                    LastDirection = Direction;
                }
            }

            public bool IsDelaying() => saveDelay > 0;
        }
        public enum HoverState
        {
            None, //No hovering 
            Hovering, //Mouse is hovering an object
            AutoProjected //The projection is auto.
        }
        #endregion

        #region Fields
        public MouseData mouseData;

        [SerializeField]
        protected cdk.menu.view.MenuDisplayManager m_objectMenu;
        [SerializeField]
        protected Transform CameraTransform;
        [SerializeField]
        protected InteractionMapper InteractionMapper;
        [Header("Degrees Of Freedom")]
        [SerializeField]
        protected List<DofGroupEnum> dofGroups = new List<DofGroupEnum>();
        [Header("Bone Type")]
        /// <summary>
        /// Avatar bone linked to this input.
        /// </summary>
        [SerializeField]
        [inetum.unityUtils.ConstEnum(typeof(common.userCapture.BoneType), typeof(uint))]
        protected uint interactionBoneType = common.userCapture.BoneType.RightHand;
        [SerializeField]
        [inetum.unityUtils.ConstEnum(typeof(common.userCapture.BoneType), typeof(uint))]
        protected uint hoverBoneType = common.userCapture.BoneType.Head;

        protected List<ManipulationGroup> ManipulationInputs = new List<ManipulationGroup>();
        protected List<inputs.interactions.KeyMenuInput> KeyMenuInputs = new List<inputs.interactions.KeyMenuInput>();
        protected List<inputs.interactions.FormInput> FormInputs = new List<inputs.interactions.FormInput>();
        protected List<inputs.interactions.LinkInput> LinkInputs = new List<inputs.interactions.LinkInput>();
        /// <summary>
        /// Instantiated float parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<parameters.FloatParameterInput> floatParameterInputs = new List<parameters.FloatParameterInput>();
        /// <summary>
        /// Instantiated float range parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<parameters.FloatRangeParameterInput> floatRangeParameterInputs = new List<parameters.FloatRangeParameterInput>();
        /// <summary>
        /// Instantiated int parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<parameters.IntParameterInput> intParameterInputs = new List<parameters.IntParameterInput>();
        /// <summary>
        /// Instantiated bool parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<parameters.BooleanParameterInput> boolParameterInputs = new List<parameters.BooleanParameterInput>();
        /// <summary>
        /// Instantiated string parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<parameters.StringParameterInput> stringParameterInputs = new List<parameters.StringParameterInput>();
        /// <summary>
        /// Instantiated string enum parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<parameters.StringEnumParameterInput> stringEnumParameterInputs = new List<parameters.StringEnumParameterInput>();

        protected int m_navigationDirect = 0;
        protected AutoProjectOnHover reason = new AutoProjectOnHover();

        public static HoverEvent HoverEnter = new HoverEvent();
        public static HoverEvent HoverUpdate = new HoverEvent();
        public static HoverEvent HoverExit = new HoverEvent();
        public static bool CanProcess = false;
        /// <summary>
        /// True if an Abstract Input is currently hold by a user.
        /// </summary>
        public static bool IsInputHold = false;
        #endregion

        #region Monobehaviour Life Cycle
        protected virtual void Awake()
        {
            mouseData.ForceProjectionReleasableButton = new HoldableButtonMenuItem
            {
                Name = "Release",
                Holdable = false
            };
            mouseData.ForceProjectionReleasableButton.Subscribe(ReleaseForceProjection);

            mouseData.saveDelay = 0;
            m_objectMenu?.menu.onContentChange.AddListener(OnMenuObjectContentChange);
        }

        protected virtual void LateUpdate()
        {
            if (!CanProcess) return;

            if (m_navigationDirect > 0) ManipulationInput.NextManipulation();
            else if (m_navigationDirect < 0) ManipulationInput.PreviousManipulation();
            m_navigationDirect = 0;
            MouseHandler();
        }
        protected virtual void Update() { }
        #endregion

        protected abstract void OnMenuObjectContentChange();

        #region Inputs
        public override void Clear()
        {
            foreach (ManipulationGroup input in ManipulationInputs) if (!input.IsAvailable()) input.Dissociate();
            //foreach (KeyInput input in KeyInputs) if (!input.IsAvailable()) input.Dissociate();
            ClearParameters();
        }
        protected void ClearParameters()
        {
            System.Action<AbstractUMI3DInput> action = (input) =>
            {
                input.Dissociate();
                Destroy(input);
            };

            ClearInputs(ref KeyMenuInputs, action);
            ClearInputs(ref floatParameterInputs, action);
            ClearInputs(ref floatRangeParameterInputs, action);
            ClearInputs(ref intParameterInputs, action);
            ClearInputs(ref boolParameterInputs, action);
            ClearInputs(ref stringParameterInputs, action);
            ClearInputs(ref stringEnumParameterInputs, action);
        }
        protected void ClearInputs<T>(ref List<T> inputs, System.Action<T> action)
            where T : AbstractUMI3DInput
        {
            inputs.ForEach(action);
            inputs = new List<T>();
        }

        public override DofGroupOptionDto FindBest(DofGroupOptionDto[] options)
        {
            foreach (var GroupOption in options)
            {
                bool ok = true;
                foreach (DofGroupDto dof in GroupOption.separations)
                {
                    if (!dofGroups.Contains(dof.dofs))
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok) return GroupOption;
            }

            throw new System.NotImplementedException();
        }
        public override AbstractUMI3DInput FindInput(FormDto form, bool unused = true)
            => FindInput(FormInputs, i => i.IsAvailable() || !unused, this.gameObject);
        public override AbstractUMI3DInput FindInput(LinkDto link, bool unused = true)
            => FindInput(LinkInputs, i => i.IsAvailable() || !unused, this.gameObject);
        public override AbstractUMI3DInput FindInput(AbstractParameterDto param, bool unused = true)
        {
            if (param is FloatRangeParameterDto) return FindInput(floatRangeParameterInputs, i => i.IsAvailable(), this.gameObject);
            else if (param is FloatParameterDto) return FindInput(floatParameterInputs, i => i.IsAvailable(), this.gameObject);
            else if (param is IntegerParameterDto) return FindInput(intParameterInputs, i => i.IsAvailable());
            else if (param is IntegerRangeParameterDto) throw new System.NotImplementedException();
            else if (param is BooleanParameterDto) return FindInput(boolParameterInputs, i => i.IsAvailable(), this.gameObject);
            else if (param is StringParameterDto) return FindInput(stringParameterInputs, i => i.IsAvailable(), this.gameObject);
            else if (param is EnumParameterDto<string>) return FindInput(stringEnumParameterInputs, i => i.IsAvailable(), this.gameObject);
            else return null;
        }
        protected AbstractUMI3DInput FindInput<T>(List<T> inputs, System.Predicate<T> predicate, GameObject gO = null) where T : AbstractUMI3DInput, new()
        {
            T input = inputs.Find(predicate);
            if (input == null) AddInput(inputs, out input, gO);
            return input;
        }
        protected void AddInput<T>(List<T> inputs, out T input, GameObject gO) where T : AbstractUMI3DInput, new()
        {
            if (gO != null) input = gO.AddComponent<T>();
            else input = new T();

            if (input is inputs.interactions.KeyMenuInput keyMenuInput) keyMenuInput.bone = interactionBoneType;
            else if (input is inputs.interactions.FormInput formInput) formInput.bone = interactionBoneType;
            else if (input is inputs.interactions.LinkInput linkInput) linkInput.bone = interactionBoneType;

            input.Menu = m_objectMenu?.menu;
            inputs.Add(input);
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
            if (!m_objectMenu.menu.Contains(mouseData.ForceProjectionReleasableButton))
                m_objectMenu?.menu.Add(mouseData.ForceProjectionReleasableButton);
        }
        protected void RemoveForceProjectionReleaseButton()
        {
            if (mouseData.ForceProjectionReleasableButton == null) return;
            m_objectMenu?.menu.Remove(mouseData.ForceProjectionReleasableButton);
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
                        if (!dofGroups.Contains(dof.dofs)) return false;
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
                if (!isInteractionsEmpty && isCompatible && !IsInputHold) SetAutoProjection();
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
