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
using BrowserDesktop.UI.GenericElement;
using DesktopBrowser.UI.GenericElement;
using System;
using umi3d.common;
using UnityEngine;
using UnityEngine.UIElements;

namespace DesktopBrowser.UIControllers.Toolbox
{
    public sealed class Tools_UIController : UIController
    {
        [SerializeField]
        private Sprite icon;

        public static ToolboxItem_E CloneAndSetup(string name, string buttonClassOn, string buttonClassOff, bool isOn, Action buttonClicked)
        {
            throw new System.NotImplementedException();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                ToolboxItem_E tool = new ToolboxItem_E(VisualTA)
                {
                    ItemName = "Toolbox",
                    ItemClicked = () =>
                    {
                        Debug.Log("<color=green>TODO: </color>" + $"");
                    }
                }.SetIcon("menuBar-toolbox", "menuBar-toolbox");

                ToolboxItem_E tool1 = new ToolboxItem_E(VisualTA)
                {
                    ItemName = "Avatar",
                    ItemClicked = () =>
                    {
                        Debug.Log("<color=green>TODO: </color>" + $"");
                    }
                }.SetIcon("menuBar-avatarOn", "menuBar-avatarOff");

                ToolboxItem_E tool2 = new ToolboxItem_E(VisualTA)
                {
                    ItemName = "",
                    ItemClicked = () =>
                    {
                        Debug.Log("<color=green>TODO: </color>" + $"");
                    }
                }.SetIcon("toolbox", "toolbox");

                ToolboxItem_E tool3 = new ToolboxItem_E(VisualTA)
                {
                    ItemName = "",
                    ItemClicked = () =>
                    {
                        Debug.Log("<color=green>TODO: </color>" + $"");
                    }
                }.SetIcon("toolbox", "toolbox");

                Toolbox_E toolbox = new Toolbox_E(GetUIController("toolboxes").VisualTA, tool, tool1, tool2, tool3)
                {
                    toolboxName = "test"
                };

                toolbox.AddTo(UIDoc.rootVisualElement);

                //ToolboxItem_E tool = new ToolboxItem_E(Visual)
                //{
                //    ItemName = "Toolbox",
                //    ItemClicked = () =>
                //    {
                //        Debug.Log("<color=green>TODO: </color>" + $"");
                //    }
                //}.SetIcon(icon);

                //tool.AddTo(controller.UIDoc.rootVisualElement);
            }
        }
    }

}