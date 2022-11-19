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
using umi3d.cdk.userCapture;
using umi3d.common.interaction;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;

namespace BrowserDesktop.Controller
{
    public class DesktopController : umi3d.baseBrowser.Controller.BaseController
    {
        [Header("Input Action")]
        [SerializeField]
        protected List<CursorKeyInput> ManipulationActionInput = new List<CursorKeyInput>();
        protected List<KeyInput> KeyInputs = new List<KeyInput>();

        public override List<AbstractUMI3DInput> inputs
        {
            get
            {
                List<AbstractUMI3DInput> list = new List<AbstractUMI3DInput>();
                //list.AddRange(ManipulationInputs);
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
        private bool m_isCursorMovementFree => umi3d.baseBrowser.Controller.BaseCursor.Movement == umi3d.baseBrowser.Controller.BaseCursor.CursorMovement.Free;

        public static bool IsFreeAndHovering = false;
        private static bool s_isRightClickAdded = false;

        #region Monobehaviour Life Cycle
        protected override void Awake()
        {
            base.Awake();
            foreach (KeyInput input in GetComponentsInChildren<KeyInput>())
            {
                KeyInputs.Add(input);
                input.Init(this);
                input.bone = interactionBoneType;
            }

            UMI3DClientUserTracking.Instance.EmotePlayedSelfEvent.AddListener(CloseMainMenu);
        }
        protected override void Update()
        {
            base.Update();
            if 
            (
                Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ContextualMenuNavigationBack)) 
                || Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.MainMenuToggle))
            )
            {
                if (m_isCursorMovementFree) CloseMainMenu();
                else OpenMainMenu();
                OnSecondActionClicked();
            }

            if
            (
                Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.MainActionKey))
            )
            {
                OnMainActionClicked();
            }

            if
            (
                Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)
            )
            {
                OnEnterKeyPressed();
            }

            if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ContextualMenuNavigationDirect)) || Input.mouseScrollDelta.y < 0)
                m_navigationDirect++;
            else if (Input.mouseScrollDelta.y > 0) m_navigationDirect--;
        }
        #endregion

        //protected override void OnMenuObjectContentChange()
        //{
        //    if (m_objectMenu?.menu.Count > 0)
        //    {
        //        if (!s_isRightClickAdded)
        //        {
        //            Shortcutbox_E.Instance.AddRightClickShortcut("Object menu");
        //            s_isRightClickAdded = true;
        //        }
        //    }
        //    else
        //    {
        //        if (s_isRightClickAdded)
        //        {
        //            Shortcutbox_E.Instance.RemoveRightClickShortcut();
        //            s_isRightClickAdded = false;
        //        }
        //        m_objectMenu.Collapse(true);
        //    }
        //}

        #region Menu handler
        /// <summary>
        /// Open the main menu and free the mouse cursor
        /// </summary>
        public void OpenMainMenu()
        {
            umi3d.baseBrowser.Controller.BaseCursor.SetMovement(this, umi3d.baseBrowser.Controller.BaseCursor.CursorMovement.Free);
            if (MenuBar_E.AreThereToolboxes) MenuBar_E.Instance.Display();
            Settingbox_E.Instance.Display();
            //if (m_objectMenu.menu.Count > 0)
            //{
            //    m_objectMenu.Expand(true);
            //    IsFreeAndHovering = true;
            //}
        }

        /// <summary>
        /// Close the main menu and lock the mouse cursor
        /// </summary>
        public void CloseMainMenu()
        {
            umi3d.baseBrowser.Controller.BaseCursor.UnSetMovement(this);
            if (MenuBar_E.Instance.IsDisplaying) MenuBar_E.Instance.Hide();
            if (Settingbox_E.Instance.IsDisplaying) Settingbox_E.Instance.Hide();
            if (EmoteWindow_E.Instance.IsDisplaying) EmoteWindow_E.Instance.Hide();
            IsFreeAndHovering = false;
        }
        #endregion

        #region Input
        public override AbstractUMI3DInput FindInput(ManipulationDto manip, DofGroupDto dof, bool unused = true)
        {
            Debug.Log("TODO : Find input for manipulation dto");
            //ManipulationGroup group = ManipulationInputs.Find(i => i.IsAvailableFor(manip));
            //if (group == null)
            //{
            //    group = ManipulationGroup.Instanciate(this, ManipulationActionInput, dofGroups, transform);
            //    if (group == null)
            //    {
            //        Debug.LogWarning("find manip input FAILED");
            //        return null;
            //    }
            //    group.bone = interactionBoneType;
            //    ManipulationInputs.Add(group);
            //}
            //group.Menu = m_objectMenu?.menu;
            //return group;
            return null;
        }

        public override AbstractUMI3DInput FindInput(EventDto evt, bool unused = true, bool tryToFindInputForHoldableEvent = false)
        {
            KeyInput input = KeyInputs.Find(i => i.IsAvailable() || !unused);
            if (input == null)
                return FindInput(KeyMenuInputs, i => i.IsAvailable() || !unused, this.gameObject);
            return input;
        }
        #endregion
    }
}