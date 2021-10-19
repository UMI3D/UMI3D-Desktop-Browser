/*
Copyright 2019 Gfi Informatique

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
using BrowserDesktop.Selection.Intent;
using inetum.unityUtils;
using System.Collections.Generic;
using System.Linq;
using umi3d.cdk;
using umi3d.cdk.interaction;
using umi3d.common;
using umi3d.common.interaction;
using umi3d.common.userCapture;
using UnityEngine;
using UnityEngine.Events;

namespace BrowserDesktop.Controller
{
    public class MouseAndKeyboardController : AbstractController
    {

        #region properties
        static public bool CanProcess = false;

        /// <summary>
        /// True if an Abstract Input is currently hold by a user.
        /// </summary>
        public static bool isInputHold = false;

        [SerializeField]
        private IntentSelector selector;
        private SelectionData selectionData;

        //public UMI3DBrowserAvatar Avatar;
        public Transform CameraTransform;

        public InteractionMapper InteractionMapper;

        public umi3d.cdk.menu.Menu contextualMenu;

        int navigationDirect = 0;
        [SerializeField]
        List<DofGroupEnum> dofGroups = new List<DofGroupEnum>();
        [SerializeField]
        List<CursorKeyInput> ManipulationActionInput = new List<CursorKeyInput>();

        AutoProjectOnHover reason = new AutoProjectOnHover();

        /// <summary>
        /// Avatar bone linked to this input.
        /// </summary>
        [ConstEnum(typeof(BoneType), typeof(uint))]
        public uint interactionBoneType = BoneType.RightHand;

        [ConstEnum(typeof(BoneType), typeof(uint))]
        public uint hoverBoneType = BoneType.Head;

        Dictionary<int, int> manipulationMap;

        public class HoverEvent : UnityEvent<ulong> { };

        [HideInInspector]
        static public HoverEvent HoverEnter = new HoverEvent();

        [HideInInspector]
        static public HoverEvent HoverUpdate = new HoverEvent();

        [HideInInspector]
        static public HoverEvent HoverExit = new HoverEvent();

        private const float manipulationInputUsageTimeout = 0.1f; //ms
        private float timeSinceLastInput = 0; //ms
        #endregion


        #region Inputs

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

        #region ForceProjectionMenuItem
        void CreateForceProjectionMenuItem()
        {
            if (CircularMenu.Exists && selectionData.ForceProjectionMenuItem != null)
            {
                if (!CircularMenu.Instance.menuDisplayManager.menu.Contains(selectionData.ForceProjectionMenuItem))
                {
                    if (selector.selectionData.ForceProjectionReleasable)
                        CircularMenu.Instance.menuDisplayManager.menu.Add(selectionData.ForceProjectionMenuItem);
                }
                else if (CircularMenu.Instance.menuDisplayManager.menu.Count + EventMenu.NbEventsDIsplayed == 1)
                {
                    DeleteForceProjectionMenuItem();
                }
            }
        }

        void ForceProjectionMenuItem(bool pressed)
        {
            if (selectionData.ForceProjectionReleasable)
            {
                DeleteForceProjectionMenuItem();
                UnequipeForceProjection();
            }
        }

        void UnequipeForceProjection()
        {
            InteractionMapper.ReleaseTool(currentTool.id, new RequestedByUser());
            CircularMenu.Collapse();
        }

        void DeleteForceProjectionMenuItem()
        {
            if (selectionData.ForceProjectionMenuItem != null)
            {
                if (CircularMenu.Exists)
                {
                    CircularMenu.Instance.menuDisplayManager.menu.Remove(selectionData.ForceProjectionMenuItem);
                }
            }
        }

        public void CircularMenuColapsed()
        {
            CursorHandler.State = CursorHandler.CursorState.Hover;
        }
        #endregion

        #region monoBehaviour
        public void Awake()
        {
            foreach (KeyInput input in GetComponentsInChildren<KeyInput>())
            {
                KeyInputs.Add(input);
                input.Init(this);
                input.bone = interactionBoneType;
            }

            selectionData.ForceProjectionMenuItem = new HoldableButtonMenuItem
            {
                Name = "Release",
                Holdable = false
            };
            selectionData.ForceProjectionMenuItem.Subscribe(ForceProjectionMenuItem);

            foreach (var input in inputs)
            {
                input.onInputUp.AddListener(new UnityAction(() => {
                    if (associatedInputs[currentTool.id].Contains(input))
                        timeSinceLastInput = 0;
                }));
            }

        }

        void InputDown(KeyInput input) { }
        void InputUp(KeyInput input) { }

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
            CursorHandler.Instance.ExitIndicator = selectionData.ForceProjection;
            if (CanProcess)
            {
                if (MainMenu.IsDisplaying)
                {
                    //Hover();
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
                }
            }
           if (timeSinceLastInput <= manipulationInputUsageTimeout)
                timeSinceLastInput += Time.deltaTime;
        }
        #endregion

        #region hover
        void MouseHandler()
        {
            if (!(
                        isInteracting()
                        && (CursorHandler.State == CursorHandler.CursorState.Clicked || SideMenu.IsExpanded)
               ))
            {
 
                return; //Everything below is using the hover paradigm and therefore cannot be used with Intent Detection selection
                //Hover();
            }
            else
            {
                //CircularMenu.Instance.Follow(selectionData.centeredWorldPoint);
            }
        }

        //void Hover()
        //{
        //    if (selectionData.ForceProjection)
        //    {
        //        if (CircularMenu.Exists && (!CircularMenu.Instance.IsEmpty() || EventMenu.NbEventsDIsplayed > 0))
        //        {
        //            CreateForceProjectionMenuItem();
        //        }
        //        else if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.LeaveContextualMenu))
        //            ||
        //            Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ContextualMenuNavigationBack)))
        //        {
        //            if(selectionData.ForceProjectionReleasable)
        //                UnequipeForceProjection();
        //        }
        //    }

        //    if (selectionData.CurrentHovered != null)
        //    {
        //        if (selectionData.CurrentHovered != selectionData.OldHovered)
        //        {
        //            if (selectionData.OldHovered != null)
        //            {
        //                if (!selectionData.ForceProjection)
        //                {
        //                    if (selectionData.SelectionState == SelectionState.Selected)
        //                    {
        //                        InteractionMapper.ReleaseTool(currentTool.id, new RequestedByUser());
        //                    }
        //                    CircularMenu.Collapse();
        //                }


        //                selectionData.OldHovered.HoverExit(hoverBoneType, selectionData.LastHoveredId, selectionData.lastPoint, selectionData.lastNormal, selectionData.lastDirection);

        //                if (selectionData.CurrentHovered.dto.HoverExitAnimationId != 0)
        //                {
        //                    UMI3DNodeAnimation anim = UMI3DNodeAnimation.Get(selectionData.CurrentHovered.dto.HoverExitAnimationId);
        //                    HoverExit.Invoke(selectionData.LastHoveredId);
        //                    if (anim != null)
        //                        anim.Start();
        //                }

        //                selectionData.OldHovered = null;
        //            }

        //            selectionData.SelectionState = SelectionState.Hovering;

        //            if (selectionData.CurrentHovered.dto.interactions.Count > 0 && IsCompatibleWith(selectionData.CurrentHovered) && !selectionData.ForceProjection && !isInputHold)
        //            {
        //                InteractionMapper.SelectTool(selectionData.CurrentHovered.dto.id, true, this, selectionData.CurrentHoveredId, reason);
        //                CursorHandler.State = CursorHandler.CursorState.Hover;
        //                selectionData.SelectionState = SelectionState.AutoProjected;
        //                CircularMenu.Instance.MenuColapsed.AddListener(CircularMenuColapsed);
        //                selectionData.OldHovered = selectionData.CurrentHovered;
        //            }

        //            selectionData.CurrentHovered.HoverEnter(hoverBoneType, selectionData.CurrentHoveredId, selectionData.point, selectionData.normal, selectionData.direction);

        //            if (selectionData.CurrentHovered.dto.HoverEnterAnimationId != 0)
        //            {
        //                UMI3DNodeAnimation anim = UMI3DNodeAnimation.Get(selectionData.CurrentHovered.dto.HoverEnterAnimationId);
        //                HoverEnter.Invoke(selectionData.CurrentHoveredId);
        //                if (anim != null)
        //                    anim.Start();
        //            }
        //        }
        //        else
        //        {
        //            if (selectionData.LastHoveredId != 0 && selectionData.CurrentHoveredId != selectionData.LastHoveredId)
        //            {
        //                if (associatedInputs.ContainsKey(selectionData.CurrentHovered.dto.id))
        //                {
        //                    foreach (var input in associatedInputs[selectionData.CurrentHovered.dto.id])
        //                        input.UpdateHoveredObjectId(selectionData.CurrentHoveredId);
        //                }
        //            }
        //        }

        //        selectionData.CurrentHovered.Hovered(hoverBoneType, selectionData.CurrentHoveredId, selectionData.point, selectionData.normal, selectionData.direction);
        //    }
        //    else if (selectionData.OldHovered != null)
        //    {
        //        if (!selectionData.ForceProjection)
        //        {
        //            if (selectionData.SelectionState == SelectionState.AutoProjected)
        //            {
        //                CircularMenu.Instance.MenuColapsed.RemoveListener(CircularMenuColapsed);
        //                InteractionMapper.ReleaseTool(currentTool.id, new RequestedByUser());
        //            }
        //            CircularMenu.Collapse();
        //            CursorHandler.State = CursorHandler.CursorState.Default;
        //        }

        //        selectionData.OldHovered.HoverExit(hoverBoneType, selectionData.LastHoveredId, selectionData.lastPoint, selectionData.lastNormal, selectionData.lastDirection);

        //        if (selectionData.OldHovered.dto.HoverExitAnimationId != 0)
        //        {
        //            UMI3DNodeAnimation anim = UMI3DNodeAnimation.Get(selectionData.OldHovered.dto.HoverExitAnimationId);
        //            HoverExit.Invoke(selectionData.LastHoveredId);
        //            if (anim != null)
        //                anim.Start();
        //        }

        //        selectionData.OldHovered = null;
        //        selectionData.SelectionState = SelectionState.None;
        //    }
        //}
        #endregion

        #region clear
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

        public void ClearParameters()
        {
            KeyMenuInputs.ForEach((a) => { Destroy(a); });
            KeyMenuInputs = new List<KeyMenuInput>();

            floatParameterInputs.ForEach((a) => { Destroy(a); });
            floatParameterInputs = new List<FloatParameterInput>();
            floatRangeParameterInputs.ForEach((a) => { Destroy(a); });
            floatRangeParameterInputs = new List<FloatRangeParameterInput>();
            intParameterInputs.ForEach((a) => { Destroy(a); });
            intParameterInputs = new List<IntParameterInput>();
            boolParameterInputs.ForEach((a) => { Destroy(a); });
            boolParameterInputs = new List<BooleanParameterInput>();
            stringParameterInputs.ForEach((a) => { Destroy(a); });
            stringParameterInputs = new List<StringParameterInput>();
            stringEnumParameterInputs.ForEach((a) => { Destroy(a); });
            stringEnumParameterInputs = new List<StringEnumParameterInput>();
        }
        #endregion

        /// <summary>
        /// Create a menu to access each interactions of a tool separately.
        /// </summary>
        /// <param name="interactions"></param>
        public override void CreateInteractionsMenuFor(AbstractTool tool)
        {
            Debug.Log("oups");
        }

        #region requirements
        /// <summary>
        /// Check if a tool can be projected on this controller.
        /// </summary>
        /// <param name="tool"> The tool to be projected.</param>
        /// <returns></returns>
        public override bool IsCompatibleWith(AbstractTool tool)
        {
            List<AbstractInteractionDto> interactions = tool.interactions;
            List<AbstractInteractionDto> manips = interactions.FindAll(x => x is ManipulationDto);
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
        /// Check if a tool requires the generation of a menu to be projected.
        /// </summary>
        /// <param name="tool"> The tool to be projected.</param>
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

        public bool RequiresParametersMenu(AbstractTool tool)
        {
            List<AbstractInteractionDto> interactions = tool.interactions;
            List<AbstractInteractionDto> parameters = interactions.FindAll(x => x is AbstractParameterDto);
            // return ((events.Count > 7 || manips.Count > 0) && (events.Count > 6 || manips.Count > 1));
            return (parameters.Count > 0);
        }
        #endregion

        protected override bool isInteracting()
        {
           return (currentTool != null) && (timeSinceLastInput <= manipulationInputUsageTimeout);
        }

        protected override bool isNavigating()
        {
            throw new System.NotImplementedException();
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


        #region findInput
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
                KeyMenuInput inputMenu = KeyMenuInputs.Find(i => i.IsAvailable() || !unused);
                if (inputMenu == null)
                {
                    inputMenu = this.gameObject.AddComponent<KeyMenuInput>();
                    inputMenu.bone = interactionBoneType;
                    KeyMenuInputs.Add(inputMenu);
                }
                return inputMenu;
            }
            return input;
        }

        public override AbstractUMI3DInput FindInput(FormDto form, bool unused = true)
        {
            FormInput inputMenu = FormInputs.Find(i => i.IsAvailable() || !unused);
            if (inputMenu == null)
            {
                inputMenu = this.gameObject.AddComponent<FormInput>();
                inputMenu.bone = interactionBoneType;
                FormInputs.Add(inputMenu);
            }
            return inputMenu;
        }

        public override AbstractUMI3DInput FindInput(LinkDto link, bool unused = true)
        {
            LinkInput inputMenu = LinkInputs.Find(i => i.IsAvailable() || !unused);
            if (inputMenu == null)
            {
                inputMenu = this.gameObject.AddComponent<LinkInput>();
                inputMenu.bone = interactionBoneType;
                LinkInputs.Add(inputMenu);
            }
            return inputMenu;
        }

        public override AbstractUMI3DInput FindInput(AbstractParameterDto param, bool unused = true)
        {
            if (param is FloatRangeParameterDto)
            {
                FloatRangeParameterInput floatRangeInput = floatRangeParameterInputs.Find(i => i.IsAvailable());
                if (floatRangeInput == null)
                {
                    floatRangeInput = this.gameObject.AddComponent<FloatRangeParameterInput>();
                    floatRangeParameterInputs.Add(floatRangeInput);
                }
                return floatRangeInput;
            }
            else if (param is FloatParameterDto)
            {
                FloatParameterInput floatInput = floatParameterInputs.Find(i => i.IsAvailable());
                if (floatInput == null)
                {
                    floatInput = this.gameObject.AddComponent<FloatParameterInput>();
                    floatParameterInputs.Add(floatInput);
                }
                return floatInput;
            }
            else if (param is IntegerParameterDto)
            {
                IntParameterInput intInput = intParameterInputs.Find(i => i.IsAvailable());
                if (intInput == null)
                {
                    intInput = new IntParameterInput();
                    intParameterInputs.Add(intInput);
                }
                return intInput;

            }
            else if (param is IntegerRangeParameterDto)
            {
                throw new System.NotImplementedException();
            }
            else if (param is BooleanParameterDto)
            {
                BooleanParameterInput boolInput = boolParameterInputs.Find(i => i.IsAvailable());
                if (boolInput == null)
                {
                    boolInput = this.gameObject.AddComponent<BooleanParameterInput>();
                    boolParameterInputs.Add(boolInput);
                }
                return boolInput;
            }
            else if (param is StringParameterDto)
            {
                StringParameterInput stringInput = stringParameterInputs.Find(i => i.IsAvailable());
                if (stringInput == null)
                {
                    stringInput = this.gameObject.AddComponent<StringParameterInput>();
                    stringParameterInputs.Add(stringInput);
                }
                return stringInput;
            }
            else if (param is EnumParameterDto<string>)
            {
                StringEnumParameterInput stringEnumInput = stringEnumParameterInputs.Find(i => i.IsAvailable());
                if (stringEnumInput == null)
                {
                    stringEnumInput = this.gameObject.AddComponent<StringEnumParameterInput>();
                    stringEnumParameterInputs.Add(stringEnumInput);
                }
                return stringEnumInput;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Tool : Projection & release
        public override void Release(AbstractTool tool, InteractionMappingReason reason)
        {
            base.Release(tool, reason);
            if (reason is ToolNeedToBeUpdated && tool.interactions.Count > 0) return;

            //if (selectionData.CurrentHovered != null && selectionData.CurrentHovered.dto.id == tool.id)
            //{
            //    selectionData.CurrentHovered = null;
            //    selectionData.CurrentHoveredTransform = null;
            //    selectionData.SelectionState = SelectionState.None;
            //    CursorHandler.State = CursorHandler.CursorState.Default;
            //}
            if (selectionData.ForceProjection)
            {
                selectionData.ForceProjection = false;
                DeleteForceProjectionMenuItem();
            }
            tool.onReleased(interactionBoneType);
        }

        public override void Project(AbstractTool tool, bool releasable, InteractionMappingReason reason, ulong hoveredObjectId)
        {
            base.Project(tool, releasable, reason, hoveredObjectId); ;
            if (reason is RequestedByEnvironment)
            {
                selectionData.ForceProjection = true;
                selectionData.ForceProjectionReleasable = releasable;
            }
            tool.onProjected(interactionBoneType);
        }

        protected override ulong GetCurrentHoveredId()
        {
            return selectionData.SelectedId;
        }
        #endregion
    }
}