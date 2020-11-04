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
using System.Threading;
using umi3d.cdk.menu.view;
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

        public bool IsEmpty()
        {
            return circularMenuContainer.Count() == 0;
        }

        public int Count()
        {
            return circularMenuContainer.Count();
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
                //_Collapse();
            }
            else if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ContextualMenuNavigationBack)))
            {
                SideMenu.Display(!SideMenu.IsExpanded, !SideMenu.IsExpanded);
            }

            if (circularMenuContainer.isDisplayed && circularMenuContainer.isExpanded && IsEmpty())
            {
                menuDisplayManager.Back();
            }

            ManageOptionCursor();
        }
        
        public void CloseMenu()
        {
            menuDisplayManager.Back();
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

        public void Display()
        {
            if (true)
            {
                if (!circularMenuContainer.isDisplayed)
                {
                    circularMenuContainer.Display(true);
                }
                if (!circularMenuContainer.isExpanded)
                {
                    circularMenuContainer.Expand();
                    MenuExpand.Invoke();
                    //CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);
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
            //CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Center);//
            circularMenuContainer.Collapse();
            MenuColapsed.Invoke();
        }
    }
}