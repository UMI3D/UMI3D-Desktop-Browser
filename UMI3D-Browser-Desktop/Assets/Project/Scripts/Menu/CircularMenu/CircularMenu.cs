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
using umi3d.cdk.menu.view;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace BrowserDesktop.Menu
{
    public class CircularMenu : umi3d.common.Singleton<CircularMenu>
    {
        public SimpleUIContainer2D circularMenuContainer;

        public bool IsExpanded { get { return circularMenuContainer.isExpanded; } }
        public MenuDisplayManager menuDisplayManager;

        public UnityEvent MenuColapsed = new UnityEvent();
        public UnityEvent MenuExpand = new UnityEvent();

        int nbOfDisplayersLastFrame = 0;
        bool isInformationPopUpDisplayed = false;

        public bool IsEmpty()
        {
            return circularMenuContainer.Count() == 0;
        }

        private void Start()
        {
            Debug.Assert(circularMenuContainer != null);
            Debug.Assert(circularMenuContainer != menuDisplayManager);

            menuDisplayManager.firstButtonBackButtonPressed.AddListener(_Collapse);
            menuDisplayManager.Display(true);

            nbOfDisplayersLastFrame = circularMenuContainer.Count();
        }

        public void Update()
        {
            if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.LeaveContextualMenu)))
            {
                _Collapse();
            }
            else if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ContextualMenuNavigationBack)))
            {
                if (circularMenuContainer.isExpanded)
                {
                    CloseMenu();
                }
                else if (!IsEmpty())
                {
                    _Display();
                }
            }

            if (circularMenuContainer.isDisplayed && circularMenuContainer.isExpanded && IsEmpty())
            {
                menuDisplayManager.Back();
            }

            ManageOptionCursor();
        }

        /*void LateUpdate()
        {
            if (nbOfDisplayersLastFrame != circularMenuContainer.Count())
            {
                nbOfDisplayersLastFrame = circularMenuContainer.Count();

                if (isInformationPopUpDisplayed && nbOfDisplayersLastFrame == 0)
                {
                    Debug.Log("Cacher");
                    CloseMenu();
                    ConnectionMenu.Instance.panelRenderer.visualTree.Q<VisualElement>("information-pop-up-parameters").Clear();
                    isInformationPopUpDisplayed = false;
                }
                    
                else if (!isInformationPopUpDisplayed && nbOfDisplayersLastFrame > 0)
                {
                    _Display();
                    Debug.Log("Affcher"); isInformationPopUpDisplayed = true;
                }
                    
            }

            nbOfDisplayersLastFrame = circularMenuContainer.Count();
        }*/
        
        public void CloseMenu(bool updateSideMenu = true)
        {
            menuDisplayManager.Back();
            if(updateSideMenu)
                SideMenu.Display(false, false);
        }

        void ManageOptionCursor()
        {
            if (CursorHandler.Exists)
            {
                bool display = false;
                if (!circularMenuContainer.isDisplayed || !circularMenuContainer.isExpanded)
                {
                    display = !IsEmpty();
                }

                CursorHandler.Instance.MenuIndicator = display;
            }
        }

        public static void Setup(CircularMenuContainer circularMenuContainer)
        {
            if (Exists)
            {
                Instance.circularMenuContainer = circularMenuContainer;
            }
        }

        public static void Display(bool updateSideMenu = true)
        {
            if (Exists)
                Instance._Display(updateSideMenu);
        }


        void _Display(bool updateSideMenu = true)
        {
            if (true)
            {
                if (updateSideMenu)
                    SideMenu.Display(true, true);
                if (!circularMenuContainer.isDisplayed)
                {
                    circularMenuContainer.Display(true);
                }
                if (!circularMenuContainer.isExpanded)
                {
                    circularMenuContainer.Expand();
                    MenuExpand.Invoke();
                    CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);
                }
            }
        }

        public static void Collapse()
        {
            if (Exists)
                Instance._Collapse();
        }

        public void _Collapse()
        {
            CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Center);
            circularMenuContainer.Collapse();
            MenuColapsed.Invoke();
        }
    }
}