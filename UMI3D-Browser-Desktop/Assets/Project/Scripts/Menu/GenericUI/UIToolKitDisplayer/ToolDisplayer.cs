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

using BrowserDesktop.UI.GenericElement;
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.menu;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu.Displayer
{
    public class ToolDisplayer : umi3d.cdk.menu.view.AbstractDisplayer, IDisplayerElement
    {
        [SerializeField]
        [Tooltip("Visual Tree Asset of a toolbox button.")]
        private VisualTreeAsset toolboxButton_ge_VTA;

        //private MenuItem menuItem;

        private ToolboxButtonGenericElement toolboxButton;

        private bool initialized = false;

        public System.Action OnButtonPressed = null;

        #region Abstract Displayer

        /// <summary>
        /// ToolDisplayer don't need to be displayed.
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Display(bool forceUpdate = false)
        {
            return;
            //Debug.Log($"Tool [{menu.Name}] displays");
            //toolboxButton.style.display = DisplayStyle.Flex;
        }

        /// <summary>
        /// ToolDisplayer don't need to be hiden.
        /// </summary>
        public override void Hide()
        {
            return;
            //Debug.Log($"Tool [{menu.Name}] hides");
            //toolboxButton.style.display = DisplayStyle.None;
        }

        public override void Clear()
        {
            base.Clear();
            toolboxButton.Remove();
            toolboxButton = null;

            initialized = false;
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return 2; //(menu is EventMenuItem) ? 2 : (menu is MenuItem) ? 1 : 0;
        }

        #endregion

        #region IDisplayerElement

        public VisualElement GetUXMLContent()
        {
            return toolboxButton;
        }

        public void InitAndBindUI()
        {
            if (initialized) return;
            else initialized = true;

            toolboxButton = toolboxButton_ge_VTA.CloneTree().Q<ToolboxButtonGenericElement>();
        }

        #endregion

        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);

            //if (menu is MenuItem)
            //{
            //    menuItem = menu as MenuItem;
            //}

            InitAndBindUI();

            toolboxButton.Setup(menu.Name, menu.icon2D, () => 
            { 
                Debug.Log("<color=green>TODO: </color>" + $"Toolbox button pressed.");
                if (OnButtonPressed == null)
                    menu.Select();
                else
                    OnButtonPressed();
            });
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
