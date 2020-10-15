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
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.Events;

namespace BrowserDesktop.Menu
{
    public class CircularMenu : umi3d.common.Singleton<CircularMenu>
    {
        public CircularMenuContainer circularMenuContainer;

        public bool IsExpanded { get { return circularMenuContainer.isExpanded; } }
        public MenuDisplayManager menuDisplayManager;

        public Camera camera;

        public UnityEvent MenuColapsed = new UnityEvent();
        public UnityEvent MenuExpand = new UnityEvent();

        Vector2 PositionToFollow;

        public bool IsEmpty()
        {
            return circularMenuContainer.Count() == 0;
        }

        private void Start()
        {
            Debug.Assert(circularMenuContainer != null);
            Debug.Assert(circularMenuContainer != menuDisplayManager);
            Debug.Assert(camera != menuDisplayManager);

            menuDisplayManager.firstButtonBackButtonPressed.AddListener(Collapse);
            menuDisplayManager.Display(true);
        }

        public void Update()
        {
            if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.LeaveContextualMenu)))
            {
                Collapse();
            }
            else if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ContextualMenuNavigationBack)))
            {
                if (circularMenuContainer.isExpanded)
                {
                    menuDisplayManager.Back();
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

        void _Display()
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
                    CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);
                }
            }
        }

        public void HideMenu()
        {
            circularMenuContainer.HideMenu();
        }

        public void ShowMenu()
        {
            circularMenuContainer.ShowMenu();
        }

        public void Display(Vector3 WorldPosition)
        {
            //TODO
            //Follow(WorldPosition);
            _Display();
        }

        public void Display(Vector2 WorldPosition)
        {
            //TODO
            //Follow(WorldPosition);
            _Display();
        }

        public void Collapse()
        {
            //circleMenuContainer?.Back();
            CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Center);
            circularMenuContainer.Collapse();
            MenuColapsed.Invoke();
        }

        /*public void Follow(Vector3 WorldPosition)
        {
            //TODO
            //SetPosition(WorldPosition);
        }*/

        //public void Hide(bool clear)
        //{
        //    MenuDisplayManager.Hide(true);
        //    CursorHandler.Movement = CursorHandler.CursorMovement.Center;
        //    displaying = false;
        //}

        /*public void SetPosition(Vector3 WorldPosition)
        {
            //TODO
            //setPosition((Vector2)Camera.WorldToScreenPoint(WorldPosition));
        }

        public void setPosition(Vector2 ScreenPosition)
        {
            PositionToFollow = ScreenPosition;
            //RectTransform.position = ScreenPosition;
        }*/
    }
}