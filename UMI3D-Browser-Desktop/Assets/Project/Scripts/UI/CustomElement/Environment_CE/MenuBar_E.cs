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

//using BrowserDesktop.Menu.Environment.Settings;
using BrowserDesktop.UI;
using BrowserDesktop.UI.GenericElement;
using DesktopBrowser.UI.GenericElement;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


namespace DesktopBrowser.UI.CustomElement
{
    /// <summary>
    /// A menuBar has 3 layout (left, center and right). The center layout is a scrollView.
    /// </summary>
    public class MenuBar_E : AbstractGenericAndCustomElement
    {
        #region Fields

        public float Space { get; set; } = 10f;
        public Action<VisualElement> AddSeparator { get; set; } = (ve) => { Debug.Log("<color=green>TODO: </color>" + $"AddSeparator in MenuBarElement."); };

        private VisualElement leftLayout_VE;
        private ToolboxScrollView_E centerLayout_VE;
        private VisualElement rightLayout_VE;
        public VisualElement SubMenuLayout { get; private set; }

        #endregion

        #region Initialization

        public MenuBar_E(VisualTreeAsset visualTA): base(visualTA) { }
        public MenuBar_E(VisualElement root): base(root) { }

        protected override void Initialize()
        {
            base.Initialize();

            leftLayout_VE = Root.Q<VisualElement>("Left-layout");
            centerLayout_VE = new ToolboxScrollView_E(Root.Q("toolboxScrollView"))
            {
                AddSeparator = (ve) => { AddSeparator(ve); }
            };
            rightLayout_VE = Root.Q<VisualElement>("Right-layout");
            //SubMenuLayout = this.parent.Q<VisualElement>("sub-menu-layout");
        }

        #endregion

        #region Add elements

        public MenuBar_E AddLeft(params AbstractGenericAndCustomElement[] ces)
        {
            foreach (AbstractGenericAndCustomElement ce in ces)
            {
                ce.AddTo(leftLayout_VE);
            }
            return this;
        }
        public MenuBar_E AddCenter(params Toolbox_E[] toolboxes)
        {
            centerLayout_VE.AddToolboxes(toolboxes);
            return this;
        }
        public MenuBar_E AddRight(params AbstractGenericAndCustomElement[] ces)
        {
            foreach(AbstractGenericAndCustomElement ce in ces)
            {
                ce.AddTo(rightLayout_VE);
            }
            return this;
        }

        public void AddSpacerToLeftLayout()
        {
            AddSpacer(leftLayout_VE);
        }
        public void AddSpacerToRightLayout()
        {
            AddSpacer(rightLayout_VE);
        }
        private void AddSpacer(VisualElement layoutContainer_VE)
        {
            VisualElement space = new VisualElement();
            space.style.width = Space;
            layoutContainer_VE.Add(space);
        }

        #endregion

        #region Sub Menu

        public void AddInSubMenu(ToolboxGenericElement tools, ToolboxGenericElement parent)
        {
            tools.AddTo(SubMenuLayout);
            //tools.style.left = parent.ChangeCoordinatesTo(tools, new Vector2(parent.layout.x, parent.layout.y)).x;
            logWorldPosition = () =>
            {
                Debug.Log($"tool x = {tools.worldBound.x}");
                Debug.Log($"parent x = {parent.worldBound.x}");
                tools.style.left = parent.ChangeCoordinatesTo(tools, new Vector2(parent.layout.x, parent.layout.y)).x;

                //test.style.left = image.WorldToLocal(new Vector2(image.worldBound.x, 0f)).x;
            };
            //Menu.Environment.MenuBar_UIController.Instance.StartCoroutine(LogWorldPositionCoroutine());
        }

        private IEnumerator LogWorldPositionCoroutine()
        {
            yield return null;

            logWorldPosition();
        }

        private Action logWorldPosition;

        #endregion

        public override void OnApplyUserPreferences()
        {
            if (!Displayed)
                return;
        }
    }
}
