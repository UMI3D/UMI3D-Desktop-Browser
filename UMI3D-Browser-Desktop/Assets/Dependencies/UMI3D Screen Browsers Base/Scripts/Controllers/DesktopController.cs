/*
Copyright 2019 - 2023 Inetum

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
using System.Collections.Generic;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.desktopBrowser.Controller
{
    public class DesktopController : umi3d.baseBrowser.Controller.IConcreteController
    {
        //[Header("Input Action")]
        //[SerializeField]
        //protected List<CursorKeyInput> ManipulationActionInput = new List<CursorKeyInput>();
        //protected List<KeyInput> KeyInputs = new List<KeyInput>();

        public List<AbstractUMI3DInput> Inputs
        {
            get
            {
                List<AbstractUMI3DInput> list = new List<AbstractUMI3DInput>();
                //list.AddRange(KeyInputs);
                //list.AddRange(ManipulationInputs);
                return list;
            }
        }
        private bool m_isCursorMovementFree => umi3d.baseBrowser.Controller.BaseCursor.Movement == umi3d.baseBrowser.Controller.BaseCursor.CursorMovement.Free;

        public static bool IsFreeAndHovering = false;
        private static bool s_isRightClickAdded = false;

        #region Monobehaviour Life Cycle
        public void Awake()
        {
            //foreach (KeyInput input in GetComponentsInChildren<KeyInput>())
            //{
            //    KeyInputs.Add(input);
            //    input.Init(this);
            //    input.bone = interactionBoneType;
            //}
        }
        public void Update()
        {
            //if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.MainMenuToggle)))
            //{
            //    if (m_isCursorMovementFree) IsFreeAndHovering = false;
            //    else if (ObjectMenu.menu.Count > 0) IsFreeAndHovering = true;
            //    OnEscClicked();
            //}
            
            //if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ContextualMenuNavigationBack)))
            //{
            //    if (m_isCursorMovementFree) IsFreeAndHovering = false;
            //    else if (ObjectMenu.menu.Count > 0) IsFreeAndHovering = true;
            //    OnSecondActionClicked();
            //}

            //if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.MainActionKey)))
            //    OnMainActionClicked();

            //if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Emote1)))
            //    OnEmoteKeyPressed(0);
            //if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Emote2)))
            //    OnEmoteKeyPressed(1);
            //if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Emote3)))
            //    OnEmoteKeyPressed(2);

            //if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ContextualMenuNavigationDirect)) || Input.mouseScrollDelta.y < 0)
            //    m_navigationDirect++;
            //else if (Input.mouseScrollDelta.y > 0) m_navigationDirect--;
        }
        #endregion

        public AbstractUMI3DInput FindInput(EventDto evt, bool unused = true, bool tryToFindInputForHoldableEvent = false)
            => /*KeyInputs.Find(i => i.IsAvailable() || !unused);*/ null;
    }
}