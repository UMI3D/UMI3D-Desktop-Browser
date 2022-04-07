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
    public class MouseAndKeyboardController : AbstractController
    {
        static public bool CanProcess = false;

        //public UMI3DBrowserAvatar Avatar;
        public Transform CameraTransform;

        public InteractionMapper InteractionMapper;

        public umi3d.cdk.menu.Menu contextualMenu;

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

        Dictionary<int, int> manipulationMap;

        public class HoverEvent : UnityEvent<ulong> { };

        [HideInInspector]
        public static HoverEvent HoverEnter = new HoverEvent();
        [HideInInspector]
        public static HoverEvent HoverUpdate = new HoverEvent();
        [HideInInspector]
        public static HoverEvent HoverExit = new HoverEvent();

        #region Hover

        public struct MouseData
        {
            public bool ForceProjection;
            public bool ForceProjectionReleasable;
            public HoldableButtonMenuItem ForceProjectionMenuItem;

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

            public void save()
            {
                if (saveDelay > 0)
                {
                    saveDelay--;
                }
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
            {
                return saveDelay > 0;
            }
        }
        public enum HoverState { None, Hovering, AutoProjected }

        [SerializeField]
        public MouseData mouseData;
        public const float timeToHold = 0.1f;

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
            if (mouseData.ForceProjectionReleasable)
            {
                DeleteForceProjectionMenuItem();
                UnequipeForceProjection();
            }
        }

        void DeleteForceProjectionMenuItem()
        {
            if (mouseData.ForceProjectionMenuItem != null)
            {
                Debug.Log("<color=green>TODO: </color>" + $"CircularMenu");
                //if (CircularMenu.Exists)
                //{
                //    CircularMenu.Instance.menuDisplayManager.menu.Remove(mouseData.ForceProjectionMenuItem);
                //}
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
                    mouseData.save();
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

        public void CircularMenuColapsed()
        {
            if (mouseData.CurrentHovered == null) return;
            // CircularMenu.Instance.MenuColapsed.RemoveListener(CircularMenuColapsed);
            CursorHandler.State = CursorHandler.CursorState.Hover;
            mouseData.saveDelay = 3;
        }

        void MouseHandler()
        {
            if (mouseData.HoverState != HoverState.AutoProjected)
            {
                mouseData.save();
                Vector3 screenPosition = Input.mousePosition;
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
                    {
                        interactables.Add((hit, interactable));
                    }
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
                if(CursorHandler.State != CursorHandler.CursorState.Clicked)
                    UpdateTool();
                Hover();
            }
            else
            {
                //CircularMenu.Instance.Follow(mouseData.centeredWorldPoint);
            }
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

        void Hover()
        {
            if (mouseData.CurrentHovered != null)
            {
                if (mouseData.CurrentHovered != mouseData.OldHovered)
                {
                    if (mouseData.OldHovered != null)
                    {
                        mouseData.OldHovered.HoverExit(hoverBoneType, mouseData.LastHoveredId, mouseData.lastPoint, mouseData.lastNormal, mouseData.lastDirection);

                        if (mouseData.CurrentHovered.dto.HoverExitAnimationId != 0)
                        {
                            UMI3DNodeAnimation anim = UMI3DNodeAnimation.Get(mouseData.CurrentHovered.dto.HoverExitAnimationId);
                            HoverExit.Invoke(mouseData.LastHoveredId);
                            if (anim != null)
                                anim.Start();
                        }
                        mouseData.OldHovered = null;
                    }

                    mouseData.CurrentHovered.HoverEnter(hoverBoneType, mouseData.CurrentHoveredId, mouseData.point, mouseData.normal, mouseData.direction);

                    if (mouseData.CurrentHovered.dto.HoverEnterAnimationId != 0)
                    {
                        UMI3DNodeAnimation anim = UMI3DNodeAnimation.Get(mouseData.CurrentHovered.dto.HoverEnterAnimationId);
                        HoverEnter.Invoke(mouseData.CurrentHoveredId);
                        if (anim != null)
                            anim.Start();
                    }
                }
                else
                {
                    if (mouseData.LastHoveredId != 0 && mouseData.CurrentHoveredId != mouseData.LastHoveredId)
                    {
                        if (associatedInputs.ContainsKey(mouseData.CurrentHovered.dto.id))
                        {
                            foreach (var input in associatedInputs[mouseData.CurrentHovered.dto.id])
                                input.UpdateHoveredObjectId(mouseData.CurrentHoveredId);
                        }
                    }
                }
                mouseData.CurrentHovered.Hovered(hoverBoneType, mouseData.CurrentHoveredId, mouseData.point, mouseData.normal, mouseData.direction);
            }
            else if (mouseData.OldHovered != null)
            {

                mouseData.OldHovered.HoverExit(hoverBoneType, mouseData.LastHoveredId, mouseData.lastPoint, mouseData.lastNormal, mouseData.lastDirection);

                if (mouseData.OldHovered.dto.HoverExitAnimationId != 0)
                {
                    UMI3DNodeAnimation anim = UMI3DNodeAnimation.Get(mouseData.OldHovered.dto.HoverExitAnimationId);
                    HoverExit.Invoke(mouseData.LastHoveredId);
                    if (anim != null)
                        anim.Start();
                }
                mouseData.OldHovered = null;
            }
        }

        bool ShouldAutoProject(InteractableDto tool)
        {
            List<AbstractInteractionDto> manips = tool.interactions.FindAll(x => x is ManipulationDto);
            List<AbstractInteractionDto> events = tool.interactions.FindAll(x => x is EventDto);
            List<AbstractInteractionDto> parameters = tool.interactions.FindAll(x => x is AbstractParameterDto);
            return (((parameters.Count == 0) && (events.Count <= 7) && (manips.Count == 0)));
        }

        

        /// <summary>
        /// Create a menu to access each interactions of a tool separately.
        /// </summary>
        /// <param name="interactions"></param>
        public override void CreateInteractionsMenuFor(AbstractTool tool)
        {
            Debug.Log("oups");
        }

        /// <summary>
        /// Check if a tool can be projected on this controller.
        /// </summary>
        /// <param name="tool"> The tool to be projected.</param>
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