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
using BrowserDesktop.UI.GenericElement;
using DesktopBrowser.UI.GenericElement;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


namespace BrowserDesktop.UI.CustomElement
{
    public class MenuBarElement : AbstractGenericAndCustomElement
    {
        public enum Layout
        {
            LEFT, CENTER, RIGHT
        }

        #region Fields

        /// <summary>
        /// To be recognized by UI Builder
        /// </summary>
        public new class UxmlFactory : UxmlFactory<MenuBarElement, UxmlTraits> { }

        public float Space { get; set; } = 10f;

        private VisualElement leftLayout_VE;
        private ToolboxMenuBarSV_E centerLayout_VE;
        public ToolboxMenuBarSV_E ToolboxLayout => centerLayout_VE;
        private VisualElement rightLayout_VE;


        public VisualElement SubMenuLayout { get; private set; }


        #endregion

        #region Initialization

        public MenuBarElement() : base()
        {
            //Flex
            this.style.flexShrink = 0f;
            //Size
            this.style.width = Length.Percent(100f);
        }

        public MenuBarElement(VisualTreeAsset visualTA): base(visualTA) { }

        protected override void Initialize()
        {
            base.Initialize();

            leftLayout_VE = this.Q<VisualElement>("Left-layout");
            centerLayout_VE = this.Q<VisualElement>("Center-layout").Q<ToolboxMenuBarSV_E>();
            rightLayout_VE = this.Q<VisualElement>("Right-layout");
            SubMenuLayout = this.parent.Q<VisualElement>("sub-menu-layout");
        }

        #endregion

        public MenuBarElement Add(ToolboxGenericElement toolbox, Layout layout)
        {
            switch (layout)
            {
                case Layout.LEFT:
                    toolbox.
                        AddTo(leftLayout_VE);
                    break;
                case Layout.CENTER:
                    throw new NotImplementedException();
                case Layout.RIGHT:
                    toolbox.
                        AddTo(rightLayout_VE);
                    break;
            }
            return this;
        }
        public MenuBarElement AddLeft(ToolboxGenericElement toolbox)
        {
            return Add(toolbox, Layout.LEFT);
        }
        public MenuBarElement AddLeft(Toolbox_E toolbox)
        {
            //return Add(toolbox, Layout.LEFT);
            return this;
        }
        public MenuBarElement AddCenter(ToolboxGenericElement toolbox)
        {
            return Add(toolbox, Layout.CENTER);
        }
        public MenuBarElement AddRight(ToolboxGenericElement toolbox)
        {
            return Add(toolbox, Layout.RIGHT);
        }

        public MenuBarElement Setup()
        {
            Initialize();


            centerLayout_VE.
                Setup("menuBar", "scrollView-btn", (vE) => 
                { 
                    //AddSeparator(vE, toolboxSeparatorGE_VTA); 
                });


            #region Test

            //var screenshot_TBGE = toolboxButtonGE_VTA.
            //    CloneTree().
            //    Q<ToolboxButtonGenericElement>().
            //    Setup("Screenshot", "avatarOn", "avatarOff", true, () =>
            //    {
            //        ActivateDeactivateAvatarTracking.Instance.ToggleTrackingStatus();
            //    });
            //var import = toolboxButtonGE_VTA.
            //    CloneTree().
            //    Q<ToolboxButtonGenericElement>().
            //    Setup("import", "soundOn", "soundOff", true, () =>
            //    {

            //    });
            //ToolboxGenericElement image = toolboxGE_VTA.
            //    CloneTree().
            //    Q<ToolboxGenericElement>().
            //    Setup("Image", new ToolboxButtonGenericElement[2] { screenshot_TBGE, import });
            //centerLayout_VE.AddElement(image);

            //var gallery = toolboxButtonGE_VTA.
            //    CloneTree().
            //    Q<ToolboxButtonGenericElement>().
            //    Setup("gallery", "micOn", "micOff", false, () =>
            //    {

            //    });
            //ToolboxGenericElement test = toolboxGE_VTA.
            //    CloneTree().
            //    Q<ToolboxGenericElement>().
            //    Setup("Test", gallery);
            //test.AddTo(SubMenuLayout);

            //logWorldPosition = () =>
            //{
            //    Debug.Log($"test x = {test.worldBound.x}");
            //    Debug.Log($"image x = {image.worldBound.x}");
            //    test.style.left = image.ChangeCoordinatesTo(test, new Vector2(image.layout.x, test.layout.y)).x;

            //    //test.style.left = image.WorldToLocal(new Vector2(image.worldBound.x, 0f)).x;
            //};

            #endregion

            ReadyToDisplay();

            return this;
        }

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

        #region Private Functions

        private IEnumerator LogWorldPositionCoroutine()
        {
            yield return null;

            logWorldPosition();
        }

        private Action logWorldPosition;

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

        #region Separator

        public void AddLeft(ToolboxSeparatorGenericElement separator)
        {
            separator.
                AddTo(leftLayout_VE);
        }
        public void AddRight(ToolboxSeparatorGenericElement separator)
        {
            separator.
                AddTo(rightLayout_VE);
        }

        #endregion

        #endregion

        public override void OnApplyUserPreferences()
        {
            if (!Displayed)
                return;
        }
    }
}
