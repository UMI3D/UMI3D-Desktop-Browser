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

using umi3d.cdk.menu;
using umi3dDesktopBrowser.uI.viewController;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu.Displayer
{
    public class ToolDisplayer : umi3d.cdk.menu.view.AbstractDisplayer, IDisplayerElement
    {
        [SerializeField]
        [Tooltip("Visual Tree Asset of a toolbox button.")]
        private VisualTreeAsset toolboxButton_ge_VTA;

        private ToolboxButtonGenericElement toolButton;

        #region Init and Clear

        private Container.ToolsContainer parent;
        public Container.ToolsContainer Parent
        {
            get
            {
                if (parent == null)
                    throw new System.Exception("Parent null in ToolDisplayer");
                else
                    return parent;
            }
            set => parent = value;
        }

        public bool Initialized => toolButton != null;

        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            InitAndBindUI();
        }

        public void InitAndBindUI()
        {
            if (Initialized) return;
            toolButton = toolboxButton_ge_VTA.
                CloneTree().
                Q<ToolboxButtonGenericElement>().
                Setup(menu.Name, menu.icon2D, Select);
            (Parent.
                GetUXMLContent() as ToolboxGenericElement).
                AddTool(toolButton);
        }

        public override void Clear()
        {
            base.Clear();
            toolButton.Remove();
            toolButton = null;
        }

        #endregion

        #region Display and Hide

        /// <summary>
        /// ToolDisplayer don't need to be displayed.
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Display(bool forceUpdate = false)
        {
        }

        /// <summary>
        /// ToolDisplayer don't need to be hiden.
        /// </summary>
        public override void Hide()
        {
        }

        #endregion

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return 2; //(menu is EventMenuItem) ? 2 : (menu is MenuItem) ? 1 : 0;
        }

        public VisualElement GetUXMLContent()
        {
            return toolButton;
        }

        


        /*/// <summary>
        /// Notify that the button has been pressed.
        /// </summary>
        public void NotifyPress()
        {
            menuItem?.NotifyValueChange(!menuItem.GetValue());
        }*/

    }
}
