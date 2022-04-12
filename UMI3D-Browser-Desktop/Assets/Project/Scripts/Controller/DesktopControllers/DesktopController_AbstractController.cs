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
using BrowserDesktop.Interaction;
using System.Collections.Generic;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine;

namespace BrowserDesktop.Controller
{
    public partial class MouseAndKeyboardController : AbstractController
    {
        public override List<AbstractUMI3DInput> inputs
        {
            get
            {
                List<AbstractUMI3DInput> list = new List<AbstractUMI3DInput>();
                list.AddRange(ManipulationInputs);
                list.AddRange(KeyInputs);
                list.AddRange(KeyMenuInputs);
                list.AddRange(floatParameterInputs);
                list.AddRange(floatRangeParameterInputs);
                list.AddRange(intParameterInputs);
                list.AddRange(boolParameterInputs);
                list.AddRange(stringParameterInputs);
                list.AddRange(stringEnumParameterInputs);
                return list;
            }
        }
    }

    public partial class MouseAndKeyboardController : AbstractController
    {
        #region Monobehaviour Life Cycle

        public void Awake()
        {
            foreach (KeyInput input in GetComponentsInChildren<KeyInput>())
            {
                KeyInputs.Add(input);
                input.Init(this);
                input.bone = interactionBoneType;
            }

            mouseData.ForceProjectionMenuItem = new HoldableButtonMenuItem
            {
                Name = "Release",
                Holdable = false
            };
            mouseData.ForceProjectionMenuItem.Subscribe(ForceProjectionMenuItem);

            mouseData.saveDelay = 0;
        }

        private void Update()
        {
            if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ContextualMenuNavigationBack)))
            {
                if (m_isCursorMovementFree)
                {
                    CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Center);
                    IsFreeAndHovering = false;
                }
                else
                {
                    CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);
                    if (mouseData.HoverState != HoverState.None && m_objectMenu.menu.Count > 0)
                    {
                        m_objectMenu.Expand(true);
                        IsFreeAndHovering = true;
                    }
                }
            }

            if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ContextualMenuNavigationDirect)) || Input.mouseScrollDelta.y < 0)
            {
                m_navigationDirect++;
            }
            else if (Input.mouseScrollDelta.y > 0)
            {
                m_navigationDirect--;
            }
        }

        private void LateUpdate()
        {
            CursorHandler.Instance.ExitIndicator = mouseData.ForceProjection;
            if (!CanProcess)
                return;

            if (m_navigationDirect > 0)
                ManipulationInput.NextManipulation();
            else if (m_navigationDirect < 0)
                ManipulationInput.PreviousManipulation();
            m_navigationDirect = 0;
            MouseHandler();
        }

        #endregion

        #region Input

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

        public override AbstractUMI3DInput FindInput(ManipulationDto manip, DofGroupDto dof, bool unused = true)
        {
            ManipulationGroup group = ManipulationInputs.Find(i => i.IsAvailableFor(manip));
            if (group == null)
            {
                group = ManipulationGroup.Instanciate(this, ManipulationActionInput, dofGroups, transform);
                if (group == null)
                {
                    Debug.LogWarning("find manip input FAILED");
                    return null;
                }
                group.bone = interactionBoneType;
                ManipulationInputs.Add(group);
            }
            return group;
        }

        public override AbstractUMI3DInput FindInput(EventDto evt, bool unused = true, bool tryToFindInputForHoldableEvent = false)
        {
            KeyInput input = KeyInputs.Find(i => i.IsAvailable() || !unused);
            if (input == null)
                return FindInput(KeyMenuInputs, i => i.IsAvailable() || !unused, this.gameObject);
            return input;
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

        public override void Clear()
        {
            foreach (ManipulationGroup input in ManipulationInputs)
            {
                if (!input.IsAvailable())
                    input.Dissociate();
            }
            foreach (KeyInput input in KeyInputs)
            {
                if (!input.IsAvailable())
                    input.Dissociate();
            }
            ClearParameters();
        }

        #endregion

        #region Projections

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="reason"></param>
        public override void Release(AbstractTool tool, InteractionMappingReason reason)
        {
            //try
            //{
            base.Release(tool, reason);
            if (reason is ToolNeedToBeUpdated && tool.interactions.Count > 0) return;

            if (mouseData.CurrentHovered != null && mouseData.CurrentHovered.dto.id == tool.id)
            {
                mouseData.CurrentHovered = null;
                mouseData.CurrentHoveredTransform = null;
                mouseData.HoverState = HoverState.None;
                CursorHandler.State = CursorHandler.CursorState.Default;
            }
            if (mouseData.ForceProjection)
            {
                mouseData.ForceProjection = false;
                DeleteForceProjectionMenuItem();
            }
            tool.onReleased(interactionBoneType);
            //}
            //catch { }
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
            }
            tool.onProjected(interactionBoneType);
        }

        #endregion

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public override bool IsCompatibleWith(AbstractTool tool)
        {
            List<AbstractInteractionDto> manips = tool.interactions.FindAll(x => x is ManipulationDto);
            foreach (var man in manips)
            {
                var man2 = man as ManipulationDto;
                if (
                    !man2.dofSeparationOptions.Exists((sep) =>
                    {
                        foreach (DofGroupDto dof in sep.separations)
                        {
                            if (!dofGroups.Contains(dof.dofs))
                                return false;
                        }
                        return true;
                    }))
                    return false;
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tool"></param>
        public override void CreateInteractionsMenuFor(AbstractTool tool)
        {
            Debug.Log("oups");
        }

        protected override bool isInteracting()
        {
            throw new System.NotImplementedException();
        }

        protected override bool isNavigating()
        {
            throw new System.NotImplementedException();
        }

        protected override ulong GetCurrentHoveredId()
        {
            return mouseData.CurrentHoveredId;
        }
    }
}