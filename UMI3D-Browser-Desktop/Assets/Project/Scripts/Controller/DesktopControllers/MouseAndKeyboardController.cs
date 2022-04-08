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
    public partial class MouseAndKeyboardController
    {
        static public bool CanProcess = false;

        public Transform CameraTransform;

        public InteractionMapper InteractionMapper;

        int navigationDirect = 0;
        [SerializeField]
        List<DofGroupEnum> dofGroups = new List<DofGroupEnum>();

        AutoProjectOnHover reason = new AutoProjectOnHover();

        /// <summary>
        /// Avatar bone linked to this input.
        /// </summary>
        [ConstEnum(typeof(BoneType), typeof(uint))]
        public uint interactionBoneType = BoneType.RightHand;

        [ConstEnum(typeof(BoneType), typeof(uint))]
        public uint hoverBoneType = BoneType.Head;

        #region Hover

        /// <summary>
        /// True if an Abstract Input is currently hold by a user.
        /// </summary>
        public static bool isInputHold = false;

        #endregion

        #region Inputs

        #region Inputs Fields

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

        public override List<AbstractUMI3DInput> inputs
        {
            get {
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

        #endregion

        #region Clear Menu and Inputs

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

        #endregion

        #region Find Input

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
            {
                return FindInput(KeyMenuInputs, i => i.IsAvailable() || !unused, this.gameObject);
            }
            return input;
        }

        public override AbstractUMI3DInput FindInput(FormDto form, bool unused = true)
        {
            return FindInput(FormInputs, i => i.IsAvailable() || !unused, this.gameObject);
        }

        public override AbstractUMI3DInput FindInput(LinkDto link, bool unused = true)
        {
            return FindInput(LinkInputs, i => i.IsAvailable() || !unused, this.gameObject);
        }

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

        private AbstractUMI3DInput FindInput<T>(List<T> inputs, System.Predicate<T> predicate, GameObject gO = null) where T : AbstractUMI3DInput, new()
        {
            T input = inputs.Find(predicate);
            if (input == null)
            {
                AddInput(inputs, out input, gO);
            }
            return input;
        }

        private void AddInput<T>(List<T> inputs, out T input, GameObject gO) where T : AbstractUMI3DInput, new()
        {
            if (gO != null) input = gO.AddComponent<T>();
            else input = new T();

            if (input is KeyMenuInput) (input as KeyMenuInput).bone = interactionBoneType;
            else if (input is FormInput) (input as FormInput).bone = interactionBoneType;
            else if (input is LinkInput) (input as LinkInput).bone = interactionBoneType;
            inputs.Add(input);
        }

        #endregion

        void InputDown(KeyInput input) { }
        void InputUp(KeyInput input) { }

        #endregion

        #region Force Projection (projection without hovering)

        void CreateForceProjectionMenuItem()
        {
            Debug.Log("<color=green>TODO: </color>" + $"CircularMenu");
            //if (CircularMenu.Exists && mouseData.ForceProjectionMenuItem != null)
            //{
            //    if (!CircularMenu.Instance.menuDisplayManager.menu.Contains(mouseData.ForceProjectionMenuItem))
            //    {
            //        if (mouseData.ForceProjectionReleasable)
            //            CircularMenu.Instance.menuDisplayManager.menu.Add(mouseData.ForceProjectionMenuItem);
            //    }
            //    else if (CircularMenu.Instance.menuDisplayManager.menu.Count + EventMenu.NbEventsDIsplayed == 1)
            //    {
            //        DeleteForceProjectionMenuItem();
            //    }
            //}
        }

        void UnequipeForceProjection()
        {
            InteractionMapper.ReleaseTool(currentTool.id, new RequestedByUser());
            mouseData.ForceProjection = false;
            mouseData.CurrentHovered = null;
            mouseData.CurrentHoveredTransform = null;
            mouseData.OldHovered = null;
            Debug.Log("<color=green>TODO: </color>" + $"CircularMenu");
            //CircularMenu.Collapse();
            mouseData.HoverState = HoverState.None;
        }

        void ForceProjectionMenuItem(bool pressed)
        {
            if (!mouseData.ForceProjectionReleasable)
                return;

            DeleteForceProjectionMenuItem();
            UnequipeForceProjection();
        }

        void DeleteForceProjectionMenuItem()
        {
            if (mouseData.ForceProjectionMenuItem == null)
                return;

            Debug.Log("<color=green>TODO: </color>" + $"CircularMenu");
            //if (CircularMenu.Exists)
            //    CircularMenu.Instance.menuDisplayManager.menu.Remove(mouseData.ForceProjectionMenuItem);

        }

        #endregion

        

        public void CircularMenuColapsed()
        {
            if (mouseData.CurrentHovered == null) return;
            // CircularMenu.Instance.MenuColapsed.RemoveListener(CircularMenuColapsed);
            CursorHandler.State = CursorHandler.CursorState.Hover;
            mouseData.saveDelay = 3;
        }

        private void UpdateTool()
        {
            if (mouseData.ForceProjection)
            {
                Debug.Log("<color=green>TODO: </color>" + $"CircularMenu");
                //if (CircularMenu.Exists && (!CircularMenu.Instance.IsEmpty() || EventMenu.NbEventsDIsplayed > 0))
                //{
                //    CreateForceProjectionMenuItem();
                //}
                if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.LeaveContextualMenu))
                    ||
                    Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ContextualMenuNavigationBack)))
                {
                    if (mouseData.ForceProjectionReleasable)
                        UnequipeForceProjection();
                }
            }

            if (mouseData.CurrentHovered != null)
            {
                if (mouseData.CurrentHovered != mouseData.LastProjected)
                {
                    if (mouseData.LastProjected != null)
                    {
                        if (!mouseData.ForceProjection)
                        {
                            if (mouseData.HoverState == HoverState.AutoProjected)
                            {
                                InteractionMapper.ReleaseTool(currentTool.id, new RequestedByUser());
                            }
                            Debug.Log("<color=green>TODO: </color>" + $"CircularMenu");
                            //CircularMenu.Collapse();
                        }
                        mouseData.LastProjected = null;
                    }

                    mouseData.HoverState = HoverState.Hovering;

                    if (mouseData.CurrentHovered.dto.interactions.Count > 0 && IsCompatibleWith(mouseData.CurrentHovered) && !mouseData.ForceProjection && !isInputHold)
                    {
                        InteractionMapper.SelectTool(mouseData.CurrentHovered.dto.id, true, this, mouseData.CurrentHoveredId, reason);
                        CursorHandler.State = CursorHandler.CursorState.Hover;
                        mouseData.HoverState = HoverState.AutoProjected;
                        Debug.Log("<color=green>TODO: </color>" + $"CircularMenu");
                        //CircularMenu.Instance.MenuColapsed.AddListener(CircularMenuColapsed);
                        mouseData.LastProjected = mouseData.CurrentHovered;
                    }
                }
            }
            else if (mouseData.LastProjected != null)
            {
                if (!mouseData.ForceProjection)
                {
                    if (mouseData.HoverState == HoverState.AutoProjected)
                    {
                        Debug.Log("<color=green>TODO: </color>" + $"CircularMenu");
                        //CircularMenu.Instance.MenuColapsed.RemoveListener(CircularMenuColapsed);
                        if (currentTool != null)
                            InteractionMapper.ReleaseTool(currentTool.id, new RequestedByUser());
                    }
                    //CircularMenu.Collapse();
                    CursorHandler.State = CursorHandler.CursorState.Default;
                }

                mouseData.LastProjected = null;
                mouseData.HoverState = HoverState.None;
            }
        }

        bool ShouldAutoProject(InteractableDto tool)
        {
            List<AbstractInteractionDto> manips = tool.interactions.FindAll(x => x is ManipulationDto);
            List<AbstractInteractionDto> events = tool.interactions.FindAll(x => x is EventDto);
            List<AbstractInteractionDto> parameters = tool.interactions.FindAll(x => x is AbstractParameterDto);
            return (((parameters.Count == 0) && (events.Count <= 7) && (manips.Count == 0)));
        }

        public bool RequiresParametersMenu(AbstractTool tool)
        {
            List<AbstractInteractionDto> interactions = tool.interactions;
            List<AbstractInteractionDto> parameters = interactions.FindAll(x => x is AbstractParameterDto);
            // return ((events.Count > 7 || manips.Count > 0) && (events.Count > 6 || manips.Count > 1));
            return (parameters.Count > 0);
        }
    }
}