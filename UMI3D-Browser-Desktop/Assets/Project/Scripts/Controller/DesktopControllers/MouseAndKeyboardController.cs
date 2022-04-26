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
using BrowserDesktop.Parameters;
using inetum.unityUtils;
using System.Collections.Generic;
using umi3d.cdk.interaction;
using umi3d.cdk.menu.view;
using umi3d.common.interaction;
using umi3d.common.userCapture;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;

namespace BrowserDesktop.Controller
{
    public partial class MouseAndKeyboardController
    {
        public Transform CameraTransform;
        public InteractionMapper InteractionMapper;
        [SerializeField]
        private MenuDisplayManager m_objectMenu;

        [Header("Degrees Of Freedom")]
        [SerializeField]
        private List<DofGroupEnum> dofGroups = new List<DofGroupEnum>();

        [Header("Bone Type")]
        /// <summary>
        /// Avatar bone linked to this input.
        /// </summary>
        [ConstEnum(typeof(BoneType), typeof(uint))]
        public uint interactionBoneType = BoneType.RightHand;
        [ConstEnum(typeof(BoneType), typeof(uint))]
        public uint hoverBoneType = BoneType.Head;

        #region Inputs Fields

        [Header("Input Action")]
        [SerializeField]
        List<CursorKeyInput> ManipulationActionInput = new List<CursorKeyInput>();

        List<ManipulationGroup> ManipulationInputs = new List<ManipulationGroup>();
        List<KeyInput> KeyInputs = new List<KeyInput>();

        List<KeyMenuInput> KeyMenuInputs = new List<KeyMenuInput>();
        List<FormInput> FormInputs = new List<FormInput>();
        List<LinkInput> LinkInputs = new List<LinkInput>();

        /// <summary>
        /// Instantiated float parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<FloatParameterInput> floatParameterInputs = new List<FloatParameterInput>();

        /// <summary>
        /// Instantiated float range parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<FloatRangeParameterInput> floatRangeParameterInputs = new List<FloatRangeParameterInput>();

        /// <summary>
        /// Instantiated int parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<IntParameterInput> intParameterInputs = new List<IntParameterInput>();

        /// <summary>
        /// Instantiated bool parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<BooleanParameterInput> boolParameterInputs = new List<BooleanParameterInput>();

        /// <summary>
        /// Instantiated string parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<StringParameterInput> stringParameterInputs = new List<StringParameterInput>();

        /// <summary>
        /// Instantiated string enum parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<StringEnumParameterInput> stringEnumParameterInputs = new List<StringEnumParameterInput>();

        #endregion

        public static bool CanProcess = false;
        /// <summary>
        /// True if an Abstract Input is currently hold by a user.
        /// </summary>
        public static bool IsInputHold = false;

        private int m_navigationDirect = 0;
        private AutoProjectOnHover reason = new AutoProjectOnHover();
    }


    public partial class MouseAndKeyboardController
    {
        #region Inputs

        private void ClearParameters()
        {
            ClearInputs(ref KeyMenuInputs, (a) => { Destroy(a); });
            ClearInputs(ref floatParameterInputs, (a) => { Destroy(a); });
            ClearInputs(ref floatRangeParameterInputs, (a) => { Destroy(a); });
            ClearInputs(ref intParameterInputs, (a) => { Destroy(a); });
            ClearInputs(ref boolParameterInputs, (a) => { Destroy(a); });
            ClearInputs(ref stringParameterInputs, (a) => { Destroy(a); });
            ClearInputs(ref stringEnumParameterInputs, (a) => { Destroy(a); });
        }

        private void ClearInputs<T>(ref List<T> inputs, System.Action<T> action) where T : AbstractUMI3DInput
        {
            inputs.ForEach(action);
            inputs = new List<T>();
        }

        private AbstractUMI3DInput FindInput<T>(List<T> inputs, System.Predicate<T> predicate, GameObject gO = null) where T : AbstractUMI3DInput, new()
        {
            T input = inputs.Find(predicate);
            if (input == null)
                AddInput(inputs, out input, gO);
            return input;
        }

        private void AddInput<T>(List<T> inputs, out T input, GameObject gO) where T : AbstractUMI3DInput, new()
        {
            if (gO != null) input = gO.AddComponent<T>();
            else input = new T();

            if (input is KeyMenuInput keyMenuInput) keyMenuInput.bone = interactionBoneType;
            else if (input is FormInput formInput) formInput.bone = interactionBoneType;
            else if (input is LinkInput linkInput) linkInput.bone = interactionBoneType;

            input.Menu = m_objectMenu?.menu;
            inputs.Add(input);
        }

        #endregion

        #region Force Projection (projection without hovering)

        private void AddForceProjectionReleaseButton()
        {
            if (mouseData.ForceProjectionReleasableButton == null || !mouseData.ForceProjectionReleasable)
                return;
            if (!m_objectMenu.menu.Contains(mouseData.ForceProjectionReleasableButton))
                m_objectMenu?.menu.Add(mouseData.ForceProjectionReleasableButton);
        }

        private void ReleaseForceProjection(bool _)
        {
            if (!mouseData.ForceProjectionReleasable)
                return;

            RemoveForceProjectionReleaseButton();
            UnequipeForceProjection();
        }

        private void RemoveForceProjectionReleaseButton()
        {
            if (mouseData.ForceProjectionReleasableButton == null)
                return;

            m_objectMenu?.menu.Remove(mouseData.ForceProjectionReleasableButton);
        }

        private void UnequipeForceProjection()
        {
            InteractionMapper.ReleaseTool(currentTool.id, new RequestedByUser());
            mouseData.ForceProjection = false;
            mouseData.CurrentHovered = null;
            mouseData.CurrentHoveredTransform = null;
            mouseData.OldHovered = null;
            mouseData.HoverState = HoverState.None;
        }

        #endregion

        #region Update Tool

        private void UpdateTool()
        {
            if (mouseData.ForceProjection && mouseData.ForceProjectionReleasable)
            {
                AddForceProjectionReleaseButton();
                if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.LeaveContextualMenu)))
                    UnequipeForceProjection();
            }

            UpdateToolForForceProjection();
            UpdateToolForNonForceProjection();
        }

        private void UpdateToolForForceProjection ()
        {
            if (!mouseData.ForceProjection)
                return;

            if (mouseData.CurrentHovered != null)
            {
                if (mouseData.CurrentHovered == mouseData.LastProjected)
                    return;

                mouseData.HoverState = HoverState.Hovering;
            }
            else if (mouseData.LastProjected != null)
                mouseData.HoverState = HoverState.None;

            mouseData.LastProjected = null;
        }

        private void UpdateToolForNonForceProjection()
        {
            if (mouseData.ForceProjection)
                return;

            if (mouseData.CurrentHovered != null)
            {
                if (mouseData.CurrentHovered == mouseData.LastProjected)
                    return;

                if (mouseData.LastProjected != null)
                    ReleaseAutoProjection();

                bool isInteractionsEmpty = mouseData.CurrentHovered.dto.interactions.Count == 0;
                bool isCompatible = IsCompatibleWith(mouseData.CurrentHovered);
                if (!isInteractionsEmpty && isCompatible && !IsInputHold)
                    SetAutoProjection();
            }
            else if (mouseData.LastProjected != null)
                ReleaseAutoProjection();
        }

        #endregion

        #region Auto Projection

        private void SetAutoProjection()
        {
            InteractionMapper.SelectTool(mouseData.CurrentHovered.dto.id, true, this, mouseData.CurrentHoveredId, reason);
            mouseData.HoverState = HoverState.AutoProjected;
            CursorHandler.State = CursorHandler.CursorState.Hover;
            mouseData.LastProjected = mouseData.CurrentHovered;
        }

        private void ReleaseAutoProjection()
        {
            if (mouseData.HoverState == HoverState.AutoProjected && currentTool != null)
                InteractionMapper.ReleaseTool(currentTool.id, new RequestedByUser());
            mouseData.HoverState = HoverState.None;
            CursorHandler.State = CursorHandler.CursorState.Default;
            mouseData.LastProjected = null;
        }

        #endregion

        private void OnMenuObjectContentChange()
        {
            if (m_objectMenu?.menu.Count > 0)
                CursorDisplayer.DisplaySettingsCursor(true);
            else
                CursorDisplayer.DisplaySettingsCursor(false);
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
    }
}