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
using BrowserDesktop.Controller;
using BrowserDesktop.Cursor;
using BrowserDesktop.Menu;
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using umi3d.common.userCapture;
using UnityEngine;
using UnityEngine.Events;

namespace BrowserDesktop.Interaction
{
    [System.Serializable]
    public class ManipulationInput : AbstractUMI3DInput
    {
        #region Instances List

        static public int currentInstance { get; private set; }


        static public void NextManipulation()
        {
            SwicthManipulation(currentInstance + 1);
        }

        static public void PreviousManipulation()
        {
            SwicthManipulation(currentInstance - 1);
        }

        static public void SelectFirst()
        {
            SwicthManipulation(0);
        }

        static public void SwicthManipulation(int i)
        {
            if (ManipulationGroup.CurrentManipulationGroup == null || ManipulationGroup.InputInstances == null || !ManipulationGroup.InputInstances.ContainsKey(ManipulationGroup.CurrentManipulationGroup))
                return;
            List<ManipulationInput> instances = ManipulationGroup.InputInstances[ManipulationGroup.CurrentManipulationGroup];
            if (instances.Count > 0)
            {
                if (currentInstance < instances.Count)
                    instances[currentInstance].Deactivate();
                currentInstance = i;
                if (currentInstance < 0)
                    currentInstance = instances.Count - 1;
                else if (currentInstance >= instances.Count)
                    currentInstance = 0;
                if (currentInstance < instances.Count)
                    instances[currentInstance].Activate();
            }
        }

        static public ManipulationInput CurrentManipulation { get { List<ManipulationInput> instances = ManipulationGroup.InputInstances[ManipulationGroup.CurrentManipulationGroup]; if (instances.Count > 0) return instances[currentInstance]; else return null; } }

        //bool Active { get => active; set { active = value; ManipulationDisplayer?.State(active); } }
        bool Active { get => active; set { active = value; manipulationDisplayer?.SetState(active); } }

        internal void Activate()
        {
            Active = true;
        }
        internal void Deactivate()
        {
            Active = false;
            manipulated = false;
            frameIndicator.gameObject.SetActive(false);
        }

        #endregion

        /// <summary>
        /// DofGroup of the Manipulation.
        /// </summary>
        public DofGroupEnum DofGroup;

        /// <summary>
        /// Button to activate this input.
        /// </summary>
        public CursorKeyInput activationButton;

        public Sprite Icon;

        /// <summary>
        /// Avatar bone linked to this input.
        /// </summary>
        public uint bone;

        /// <summary>
        /// Frame rate applied to message emission through network (high values can cause network flood).
        /// </summary>
        public float networkFrameRate = 30;

        /// <summary>
        /// Input multiplicative strength.
        /// </summary>
        public float strength = 1;
        public FrameIndicator frameIndicator;
        public Transform manipulationCursor;

        [System.Serializable]
        public class ManipulationInfo : UnityEvent<Transform, DofGroupEnum> { }
        public ManipulationInfo onActivation = new ManipulationInfo();
        public UnityEvent onDesactivation = new UnityEvent();

        /// <summary>
        /// Associtated interaction (if any).
        /// </summary>
        protected ManipulationDto associatedInteraction;

        /// <summary>
        /// Frame of reference of the <see cref="associatedInteraction"/> (if any).
        /// </summary>
        protected Transform frameOfReference;

        /// <summary>
        /// Launched coroutine for network message sending (if any).
        /// </summary>
        /// <see cref="networkMessageSender"/>
        protected Coroutine messageSenderCoroutine;

        Vector3 StartPosition;
        Transform cursor;
        bool active = false;
        bool manipulated = false;

        //ManipulationDisplayer ManipulationDisplayer;
        ManipulationElement manipulationDisplayer;

        ulong toolId;

        protected void Start()
        {
            /*if (ManipulationDisplayer == null)
            {
                ManipulationDisplayer = ManipulationMenu.CreateDisplayer();
                ManipulationDisplayer.gameObject.SetActive(false);
            }*/
            if (manipulationDisplayer == null)
            {
                manipulationDisplayer = ManipulationDisplayerManager.CreateDisplayer();
                manipulationDisplayer.Display(false);
            }
        }

        private void OnDestroy()
        {
            if (manipulationDisplayer != null) manipulationDisplayer.Remove();
            manipulationDisplayer = null;
        }

        public override void Init(AbstractController controller)
        {
            base.Init(controller);
            if (messageSenderCoroutine != null)
            {
                StopCoroutine(messageSenderCoroutine);
                messageSenderCoroutine = null;
            }
        }

        public override void Associate(AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
        {
            if (associatedInteraction != null)
            {
                throw new System.Exception("This input is already binded to a interaction ! (" + associatedInteraction + ")");
            }

            if (IsCompatibleWith(interaction))
            {
                this.toolId = toolId;
                foreach (DofGroupOptionDto group in (interaction as ManipulationDto).dofSeparationOptions)
                {
                    foreach (DofGroupDto sep in group.separations)
                    {
                        if (sep.dofs == DofGroup)
                        {
                            Associate(interaction as ManipulationDto, sep.dofs, toolId, hoveredObjectId);
                            return;
                        }
                    }
                }
            }
            else
            {
                throw new System.Exception("Trying to associate an uncompatible interaction !");
            }
        }

        public void DisplayDisplayer(bool display)
        {
            //ManipulationDisplayer?.gameObject.SetActive(display);
            manipulationDisplayer?.Display(display);
        }


        public override void Associate(ManipulationDto manipulation, DofGroupEnum dofs, ulong toolId, ulong hoveredObjectId)
        {
            if (associatedInteraction != null)
            {
                throw new System.Exception("This input is already binded to a interaction ! (" + associatedInteraction + ")");
            }
            if (dofs == DofGroup)
            {
                this.hoveredObjectId = hoveredObjectId;
                associatedInteraction = manipulation;

                /*if (ManipulationDisplayer == null)
                {
                    ManipulationDisplayer = ManipulationMenu.CreateDisplayer();
                }
                if (ManipulationDisplayer != null)
                {
                    ManipulationDisplayer.Set(associatedInteraction.name, Icon);
                }*/
                if(manipulationDisplayer == null)
                {
                    manipulationDisplayer = ManipulationDisplayerManager.CreateDisplayer();
                } 
                if(manipulationDisplayer != null)
                {
                    manipulationDisplayer.SetUp(associatedInteraction.name, Icon);
                }

                StartCoroutine(SetFrameOFReference());
                onActivation.Invoke(frameOfReference, dofs);
                messageSenderCoroutine = StartCoroutine(networkMessageSender());
                activationButton.Locked = true;

            }
            else
            {
                throw new System.Exception("Trying to associate an uncompatible interaction !");
            }
        }

        protected IEnumerator SetFrameOFReference()
        {
            var wait = new WaitForFixedUpdate();
            while (true)
            {
                if (associatedInteraction?.frameOfReference != null)
                {
                    GameObject frame = UMI3DEnvironmentLoader.GetNode(associatedInteraction.frameOfReference).gameObject;
                    if (frame == null)
                        yield return wait;
                    else
                    {
                        frameOfReference = frame.transform;
                        yield break;
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

        ulong hoveredObjectId;
        ulong GetCurrentHoveredObjectId() { return hoveredObjectId; }

        protected IEnumerator networkMessageSender()
        {
            while (true)
            {
                Debug.Log("<color=green>TODO: </color>" + $"CircularMenu");
                if (/*(!CircularMenu.Exists || !CircularMenu.Instance.IsExpanded) &&*/ !MainMenu.IsDisplaying)
                {
                    if (Active && associatedInteraction != null && InputLayoutManager.GetInputCode(activationButton.activationButton) != KeyCode.None)
                    {
                        if (Input.GetKey(InputLayoutManager.GetInputCode(activationButton.activationButton)))
                        {
                            if (manipulated)
                            {
                                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                                manipulationCursor.position = frameIndicator.Project(ray, DofGroup);

                                Vector3 distanceInWorld = manipulationCursor.position - StartPosition;
                                Vector3 distanceInFrame = frameOfReference.InverseTransformDirection(distanceInWorld);

                                var pararmeterDto = new ManipulationRequestDto()
                                {
                                    boneType = bone,
                                    id =  associatedInteraction.id,
                                    toolId = this.toolId,
                                    hoveredObjectId = GetCurrentHoveredObjectId()
                                };
                                MapDistanceWithDof(distanceInFrame, ref pararmeterDto);
                                UMI3DClientServer.SendData(pararmeterDto, true);
                            }
                            else
                            {
                                if (DoesPerformRotation())
                                {
                                    CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);
                                }

                                manipulated = true;
                                
                                StartPosition = frameOfReference.position;
                                manipulationCursor.position = StartPosition;
                                frameIndicator.gameObject.SetActive(true);
                                frameIndicator.DofGroup = DofGroup;
                                frameIndicator.Frame = frameOfReference;
                                CursorHandler.State = CursorHandler.CursorState.Clicked;
                            }
                        }
                        else if (manipulated)
                        {
                            manipulated = false;
                            CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Center);
                            frameIndicator.gameObject.SetActive(false);
                            CursorHandler.State = CursorHandler.CursorState.Hover;
                        }

                    }
                }
                yield return new WaitForSeconds(1f / networkFrameRate);
            }
        }

        void MapDistanceWithDof(Vector3 distance, ref ManipulationRequestDto Manipulation)
        {
            switch (DofGroup)
            {
                //dof 2
                case DofGroupEnum.XY:
                    Manipulation.translation = new Vector3(distance.x, distance.y, 0) * strength;
                    break;
                case DofGroupEnum.XZ:
                    Manipulation.translation = new Vector3(distance.x, 0, distance.z) * strength;
                    break;
                case DofGroupEnum.YZ:
                    Manipulation.translation = new Vector3(0, distance.y, distance.z) * strength;
                    break;
                //dof 1
                case DofGroupEnum.X:
                    Manipulation.translation = new Vector3(distance.x, 0, 0) * strength;
                    break;
                case DofGroupEnum.Y:
                    Manipulation.translation = new Vector3(0, distance.y, 0) * strength;
                    break;
                case DofGroupEnum.Z:
                    Manipulation.translation = new Vector3(0, 0, distance.z) * strength;
                    break;
                case DofGroupEnum.RX:
                    if (Radius(distance.y, distance.z) > 0.1f)
                        Manipulation.rotation = Quaternion.Euler(AngleMod(Mathf.Rad2Deg * Mathf.Atan2(distance.z, distance.y)) * strength, 0, 0);
                    break;
                case DofGroupEnum.RY:
                    if (Radius(distance.z, distance.x) > 0.1f)
                        Manipulation.rotation = Quaternion.Euler(0, AngleMod(Mathf.Rad2Deg * Mathf.Atan2(distance.x, distance.z)) * strength, 0);
                    break;
                case DofGroupEnum.RZ:
                    if (Radius(distance.x, distance.y) > 0.1f)
                        Manipulation.rotation = Quaternion.Euler(0, 0, AngleMod(Mathf.Rad2Deg * Mathf.Atan2(distance.y, distance.x) - 90) * strength);
                    break;
            }
        }


        float Radius(float x, float y)
        {
            return Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
        }

        float AngleMod(float angle)
        {
            while (true)
            {
                if (angle <= 180)
                {
                    if (angle > -180) return angle;
                    angle += 360;
                }
                else
                {
                    angle -= 360;
                }
            }
        }

        bool DoesPerformRotation()
        {
            if (DofGroup == DofGroupEnum.RX || DofGroup == DofGroupEnum.RY || DofGroup == DofGroupEnum.RZ || DofGroup == DofGroupEnum.RX_RZ || DofGroup == DofGroupEnum.RX_RY || DofGroup == DofGroupEnum.RY_RZ || DofGroup == DofGroupEnum.RX_RY_RZ)
                return true;
            else
                return false;
        }

        public override AbstractInteractionDto CurrentInteraction()
        {
            return associatedInteraction;
        }

        public override void Dissociate()
        {
            if (messageSenderCoroutine != null)
            {
                StopCoroutine(messageSenderCoroutine);
                messageSenderCoroutine = null;
            }
            /*ManipulationDisplayer?.gameObject.SetActive(false);
            if (ManipulationDisplayer != null) Destroy(ManipulationDisplayer.gameObject);
            ManipulationDisplayer = null;*/
            manipulationDisplayer?.Remove();
            manipulationDisplayer = null;

            activationButton.Locked = false;
            associatedInteraction = null;
            onDesactivation.Invoke();
        }


        public override bool IsCompatibleWith(AbstractInteractionDto interaction)
        {
            return (interaction is ManipulationDto) &&
                (interaction as ManipulationDto).dofSeparationOptions.Exists(sep => sep.separations.Exists(dof => dof.dofs == DofGroup));
        }

        public bool IsCompatibleWith(DofGroupEnum dofGroup)
        {
            return (dofGroup == DofGroup);
        }

        public override bool IsAvailable()
        {
            return associatedInteraction == null && (activationButton.IsAvailable() || activationButton.Locked);
        }

        public override void UpdateHoveredObjectId(ulong hoveredObjectId)
        {
            this.hoveredObjectId = hoveredObjectId;
        }
    }
}