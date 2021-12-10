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

using BrowserDesktop.Menu.Environment;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine;

namespace BrowserDesktop.Menu.Container
{
    public class RootToolsContainer : ToolsContainer
    {
        #region Display, Hide, Collapse, Extand

        public override void Display(bool forceUpdate = false)
        {
            throw new System.NotImplementedException();
        }

        public override void Hide()
        {
            throw new System.NotImplementedException();
        }

        #region Collapse And Expand

        public override void Collapse(bool forceUpdate = false)
        {
        }

        public override void Expand(bool forceUpdate = false)
        {
            if (pined)
                MenuBar_UIController.AddInMenu(toolbox);
            else
                Debug.Log("<color=green>TODO: </color>" + $"Expand RootToolsContainer in ToolboxesWindo");
        }

        #endregion

        #endregion

        public override AbstractMenuDisplayContainer CurrentMenuDisplayContainer()
        {
            throw new System.NotImplementedException();
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            throw new System.NotImplementedException();
        }

    }
}

