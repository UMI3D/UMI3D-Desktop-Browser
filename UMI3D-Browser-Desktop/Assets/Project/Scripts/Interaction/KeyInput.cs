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
using inetum.unityUtils;
using System.Collections;
using umi3d.cdk;
using umi3d.cdk.interaction;
using umi3d.common;
using umi3d.common.interaction;
using umi3d.common.userCapture;
using UnityEngine;

namespace BrowserDesktop.Interaction
{
    [System.Serializable]
    public class KeyInput : AbstractUMI3DInput
    {
        #region Fields

        /// <summary>
        /// Button to activate this input.
        /// </summary>
        public InputLayoutManager.Input activationButton;
        private KeyCode LastFrameButton;

        /// <summary>
        /// Avatar bone linked to this input.
        /// </summary>
        [ConstEnum(typeof(BoneType), typeof(uint))]
        public uint bone = BoneType.None;

        /// <summary>
        /// Use lock if the Input is Used elsewhere;
        /// </summary>
        private int locked = 0;
        public bool Locked { get { return locked > 0; } set { if (value) locked++; else { locked--; if (locked < 0) locked = 0; } } }

        public bool Down { get; protected set; }

        /// <summary>
        /// Associtated interaction (if any).
        /// </summary>
        public EventDto associatedInteraction { get; protected set; }

        /// <summary>
        /// True if the rising edge event has been sent through network (to avoid sending falling edge only).
        /// </summary>
        protected bool risingEdgeEventSent = false;

        ulong toolId;

        ulong hoveredObjectId;
        private bool swichOnDown = false;
        public bool SwichOnDown { get => swichOnDown; protected set => swichOnDown = value; }

        #endregion

        #region Monobehaviour Life Cycle

        protected virtual void Start()
        {
            StartCoroutine(InitEventDisplayer());

            onInputDown.AddListener(() =>
            {
                SwichOnDown = (CursorHandler.State == CursorHandler.CursorState.Hover);
                if (SwichOnDown)
                {
                    CursorHandler.State = CursorHandler.CursorState.Clicked;
                }
            });
            onInputUp.AddListener(() =>
            {
                if (SwichOnDown && CursorHandler.State == CursorHandler.CursorState.Clicked)
                {
                    CursorHandler.State = CursorHandler.CursorState.Hover;
                }
            });
        }

        protected virtual void Update()
        {
            if (LastFrameButton != InputLayoutManager.GetInputCode(activationButton))
            {
                ResetButton();
                LastFrameButton = InputLayoutManager.GetInputCode(activationButton);
            }

            if (associatedInteraction != null && (!SideMenu.Exists || !SideMenu.IsExpanded))
            {
                if (Input.GetKeyDown(InputLayoutManager.GetInputCode(activationButton)))
                {

                    if (associatedInteraction.TriggerAnimationId != 0)
                        StartAnim(associatedInteraction.TriggerAnimationId);

                    onInputDown.Invoke();
                    Down = true;

                    if ((associatedInteraction).hold)
                        SetEventDTO<EventStateChangedDto>(true);
                    else
                        SetEventDTO<EventTriggeredDto>();
                }

                if (Input.GetKeyUp(InputLayoutManager.GetInputCode(activationButton)) || Down && !Input.GetKey(InputLayoutManager.GetInputCode(activationButton)))
                {
                    onInputUp.Invoke();
                    Down = false;

                    if (associatedInteraction.ReleaseAnimationId != 0)
                        StartAnim(associatedInteraction.ReleaseAnimationId);

                    if ((associatedInteraction).hold && risingEdgeEventSent)
                        SetEventDTO<EventStateChangedDto>();
                }
            }
        }

        #endregion

        private void SetEventDTO<T>(bool val = false) where T : InteractionRequestDto, new()
        {
            if (typeof(T).IsAssignableFrom(typeof(EventStateChangedDto)))
            {
                EventStateChangedDto eventdto = new EventStateChangedDto
                {
                    active = val,
                    boneType = bone,
                    id = associatedInteraction.id,
                    toolId = this.toolId,
                    hoveredObjectId = hoveredObjectId
                };
                UMI3DClientServer.SendData(eventdto, true);
                risingEdgeEventSent = val;
                MouseAndKeyboardController.isInputHold = val;
            }
            else if (typeof(T).IsAssignableFrom(typeof(EventTriggeredDto)))
            {
                EventTriggeredDto eventdto = new EventTriggeredDto
                {
                    boneType = bone,
                    id = associatedInteraction.id,
                    toolId = this.toolId,
                    hoveredObjectId = hoveredObjectId
                };
                UMI3DClientServer.SendData(eventdto, true);
            }
        }

        private void StartAnim(ulong id)
        {
            UMI3DNodeAnimation.Get(id)?.Start();
        }

        #region Displayer

        EventDisplayer eventDisplayer;

        IEnumerator InitEventDisplayer()
        {
            yield return null;
            eventDisplayer = EventMenu.CreateDisplayer();
            eventDisplayer?.Display(false);
        }

        private void DiplayDisplayer(string label, string inputName, Texture2D icon = null)
        {
            //Debug.Log("displayer = " + label);
            if (eventDisplayer != null)
            {
                eventDisplayer.Display(true);
                eventDisplayer.SetUp(label, inputName, icon);
            }

            //...

            Shortcuts.Instance.AddShortcut(label, new string[1] { inputName });
        }

        private void DisplayDisplayer(string label, KeyCode key_code)
        {

        }

        #endregion



        #region Associate and Dissociate

        public override void Associate(AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
        {
            if (associatedInteraction != null)
            {
                throw new System.Exception("This input is already binded to a interaction ! (" + associatedInteraction + ")");
            }

            if (IsCompatibleWith(interaction))
            {
                associatedInteraction = interaction as EventDto;
                this.hoveredObjectId = hoveredObjectId;
                this.toolId = toolId;

                FileDto fileToLoad = UMI3DEnvironmentLoader.Parameters.ChooseVariante(associatedInteraction.icon2D?.variants);

                if (fileToLoad != null)
                {
                    string url = fileToLoad.url;
                    string ext = fileToLoad.extension;
                    string authorization = fileToLoad.authorization;
                    IResourcesLoader loader = UMI3DEnvironmentLoader.Parameters.SelectLoader(ext);

                    if (loader != null)
                    {
                        UMI3DResourcesManager.LoadFile(
                            interaction.id,
                            fileToLoad,
                            loader.UrlToObject,
                            loader.ObjectFromCache,
                            (o) =>
                            {
                                var obj = o as Texture2D;
                                if (obj == null)
                                    DiplayDisplayer(associatedInteraction.name, InputLayoutManager.GetInputCode(activationButton).ToString());
                                else
                                    DiplayDisplayer(associatedInteraction.name, InputLayoutManager.GetInputCode(activationButton).ToString(), obj);
                            },
                            (Umi3dException str) =>
                            {
                                DiplayDisplayer(associatedInteraction.name, InputLayoutManager.GetInputCode(activationButton).ToString());
                            },
                            loader.DeleteObject
                            );
                    }
                    else
                        DiplayDisplayer(associatedInteraction.name, InputLayoutManager.GetInputCode(activationButton).ToString());
                }
                else
                {
                    //Debug.Log("display test = " + associatedInteraction.name);
                    DiplayDisplayer(associatedInteraction.name, InputLayoutManager.GetInputCode(activationButton).ToString());
                }
            }
            else
            {
                throw new System.Exception("Trying to associate an uncompatible interaction !");
            }
        }

        public override void Associate(ManipulationDto manipulation, DofGroupEnum dofs, ulong toolId, ulong hoveredObjectId)
        {
            throw new System.NotImplementedException();
        }

        public override void Dissociate()
        {
            //Debug.Log($"Dissociate KeyInput EventDisplayer");
            if (Down) onInputUp.Invoke();
            ResetButton();
            eventDisplayer?.Display(false);
            associatedInteraction = null;
            Shortcuts.Instance.ClearShortcut();
        }

        void ResetButton()
        {
            if (associatedInteraction != null && (associatedInteraction).hold && risingEdgeEventSent)
            {
                SetEventDTO<EventStateChangedDto>();
            }
            risingEdgeEventSent = false;
        }

        #endregion

        public override AbstractInteractionDto CurrentInteraction()
        {
            return associatedInteraction;
        }

        public override bool IsCompatibleWith(AbstractInteractionDto interaction)
        {
            return (interaction is EventDto);
        }

        public override bool IsAvailable()
        {
            return associatedInteraction == null && !Locked;
        }

        public override void UpdateHoveredObjectId(ulong hoveredObjectId)
        {
            this.hoveredObjectId = hoveredObjectId;
        }
    }
}