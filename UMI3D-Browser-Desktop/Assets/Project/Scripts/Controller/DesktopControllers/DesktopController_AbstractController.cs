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
using BrowserDesktop.Menu;
using BrowserDesktop.Parameters;
using inetum.unityUtils;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.cdk.interaction;
using umi3d.common;
using umi3d.common.interaction;
using umi3d.common.userCapture;
using UnityEngine;
using UnityEngine.Events;

namespace BrowserDesktop.Controller
{
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
            if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ContextualMenuNavigationDirect)) || Input.mouseScrollDelta.y < 0)
            {
                navigationDirect++;
            }
            else if (Input.mouseScrollDelta.y > 0)
            {
                navigationDirect--;
            }
        }

        private void LateUpdate()
        {
            CursorHandler.Instance.ExitIndicator = mouseData.ForceProjection;
            if (CanProcess)
            {
                if (MainMenu.IsDisplaying)
                {
                    mouseData.Save();
                    UpdateTool();
                    Hover();
                }
                else
                {
                    if (navigationDirect != 0)
                    {
                        if (navigationDirect > 0)
                            ManipulationInput.NextManipulation();
                        else
                            ManipulationInput.PreviousManipulation();
                        navigationDirect = 0;
                    }
                    MouseHandler();
                }
            }
        }

        #endregion

        #region Projections

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

        public override void Project(AbstractTool tool, bool releasable, InteractionMappingReason reason, ulong hoveredObjectId)
        {
            base.Project(tool, releasable, reason, hoveredObjectId); ;
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