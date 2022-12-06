///*
//Copyright 2019 Gfi Informatique

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//*/
//using BrowserDesktop.Controller;
//using BrowserDesktop.Cursor;
//using BrowserDesktop.Menu;
//using System.Collections;
//using System.Collections.Generic;
//using umi3d.cdk;
//using umi3d.cdk.interaction;
//using umi3d.common.interaction;
//using umi3d.common.userCapture;
//using UnityEngine;
//using UnityEngine.Events;

//namespace BrowserDesktop.Interaction
//{
//    [System.Serializable]
//    public class ManipulationInput : AbstractUMI3DInput
//    {
//        #region Instances List

//        static public int currentInstance { get; private set; }

//        #endregion

//        /// <summary>
//        /// Button to activate this input.
//        /// </summary>
//        public CursorKeyInput activationButton;

//        /// <summary>
//        /// Input multiplicative strength.
//        /// </summary>
//        public float strength = 1;
//        public umi3d.baseBrowser.Cursor.FrameIndicator frameIndicator;
//        public Transform manipulationCursor;

//        /// <summary>
//        /// Frame of reference of the <see cref="associatedInteraction"/> (if any).
//        /// </summary>
//        protected Transform frameOfReference;

//        /// <summary>
//        /// Launched coroutine for network message sending (if any).
//        /// </summary>
//        /// <see cref="networkMessageSender"/>
//        protected Coroutine messageSenderCoroutine;

//        Vector3 StartPosition;
//        Transform cursor;
//        bool manipulated = false;

//        //ManipulationDisplayer ManipulationDisplayer;
//        ManipulationElement manipulationDisplayer;

//        protected void Start()
//        {
//            /*if (ManipulationDisplayer == null)
//            {
//                ManipulationDisplayer = ManipulationMenu.CreateDisplayer();
//                ManipulationDisplayer.gameObject.SetActive(false);
//            }*/
//            if (manipulationDisplayer == null)
//            {
//                manipulationDisplayer = ManipulationDisplayerManager.CreateDisplayer();
//                manipulationDisplayer.Display(false);
//            }

//            Debug.LogWarning($"There were references to CircularMenu and MainMenu here [Manipulation Input]");
//        }

//        private void OnDestroy()
//        {
//            if (manipulationDisplayer != null) manipulationDisplayer.Remove();
//            manipulationDisplayer = null;
//        }

//        public override void Init(AbstractController controller)
//        {
//            base.Init(controller);
//            if (messageSenderCoroutine != null)
//            {
//                StopCoroutine(messageSenderCoroutine);
//                messageSenderCoroutine = null;
//            }
//        }

//        protected IEnumerator networkMessageSender()
//        {
//            while (true)
//            {
//                //if (/*(!CircularMenu.Exists || !CircularMenu.Instance.IsExpanded) &&*/ !MainMenu.IsDisplaying)
//                if (true)
//                {
//                    if (Active && associatedInteraction != null && InputLayoutManager.GetInputCode(activationButton.activationButton) != KeyCode.None)
//                    {
//                        if (Input.GetKey(InputLayoutManager.GetInputCode(activationButton.activationButton)))
//                        {
//                            if (manipulated)
//                            {
//                                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

//                                manipulationCursor.position = frameIndicator.Project(ray, DofGroup);

//                                Vector3 distanceInWorld = manipulationCursor.position - StartPosition;
//                                Vector3 distanceInFrame = frameOfReference.InverseTransformDirection(distanceInWorld);

//                                var pararmeterDto = new ManipulationRequestDto()
//                                {
//                                    boneType = bone,
//                                    id =  associatedInteraction.id,
//                                    toolId = this.toolId,
//                                    hoveredObjectId = GetCurrentHoveredObjectId()
//                                };
//                                MapDistanceWithDof(distanceInFrame, ref pararmeterDto);
//                                UMI3DClientServer.SendData(pararmeterDto, true);
//                            }
//                            else
//                            {
//                                if (DoesPerformRotation())
//                                {
//                                    umi3d.baseBrowser.Controller.BaseCursor.SetMovement(this, umi3d.baseBrowser.Controller.BaseCursor.CursorMovement.Free);
//                                }

//                                manipulated = true;
                                
//                                StartPosition = frameOfReference.position;
//                                manipulationCursor.position = StartPosition;
//                                frameIndicator.gameObject.SetActive(true);
//                                frameIndicator.DofGroup = DofGroup;
//                                frameIndicator.Frame = frameOfReference;
//                                umi3d.baseBrowser.Controller.BaseCursor.State = umi3d.baseBrowser.Controller.BaseCursor.CursorState.Clicked;
//                            }
//                        }
//                        else if (manipulated)
//                        {
//                            manipulated = false;
//                            umi3d.baseBrowser.Controller.BaseCursor.SetMovement(this, umi3d.baseBrowser.Controller.BaseCursor.CursorMovement.Center);
//                            frameIndicator.gameObject.SetActive(false);
//                            umi3d.baseBrowser.Controller.BaseCursor.State = umi3d.baseBrowser.Controller.BaseCursor.CursorState.Hover;
//                        }

//                    }
//                }
//                yield return new WaitForSeconds(1f / networkFrameRate);
//            }
//        }

//        bool DoesPerformRotation()
//        {
//            if (DofGroup == DofGroupEnum.RX || DofGroup == DofGroupEnum.RY || DofGroup == DofGroupEnum.RZ || DofGroup == DofGroupEnum.RX_RZ || DofGroup == DofGroupEnum.RX_RY || DofGroup == DofGroupEnum.RY_RZ || DofGroup == DofGroupEnum.RX_RY_RZ)
//                return true;
//            else
//                return false;
//        }
//    }
//}