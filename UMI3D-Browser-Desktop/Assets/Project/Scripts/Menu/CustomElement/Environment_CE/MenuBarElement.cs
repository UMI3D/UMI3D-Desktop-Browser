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

using BrowserDesktop.Menu.Environment.Settings;
using BrowserDesktop.UI.GenericElement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


namespace BrowserDesktop.UI.CustomElement
{
    public class MenuBarElement : AbstractGenericAndCustomElement
    {
        /// <summary>
        /// To be recognized by UI Builder
        /// </summary>
        public new class UxmlFactory : UxmlFactory<MenuBarElement, UxmlTraits> { }

        private VisualElement leftLayout_VE;
        private ToolboxMenuBarSV_E centerLayout_VE;
        public ToolboxMenuBarSV_E ToolboxLayout => centerLayout_VE;
        private VisualElement rightLayout_VE;

        private ToolboxButtonGenericElement avatar_TBGE;
        public ToolboxButtonGenericElement Avatar_TBGE => avatar_TBGE;
        private ToolboxButtonGenericElement sound_TBGE;
        public ToolboxButtonGenericElement Sound_TBGE => sound_TBGE;
        private ToolboxButtonGenericElement mic_TBGE;
        public ToolboxButtonGenericElement Mic_TBGE => mic_TBGE;

        public VisualElement SubMenuLayout { get; private set; }

        protected override void Initialize()
        {
            base.Initialize();

            leftLayout_VE = this.Q<VisualElement>("Left-layout");
            centerLayout_VE = this.Q<VisualElement>("Center-layout").Q<ToolboxMenuBarSV_E>();
            rightLayout_VE = this.Q<VisualElement>("Right-layout");
            SubMenuLayout = this.parent.Q<VisualElement>("sub-menu-layout");
        }

        public void Setup(VisualTreeAsset toolboxGE_VTA, VisualTreeAsset toolboxButtonGE_VTA, VisualTreeAsset toolboxSeparatorGE_VTA, UIDocument uiDocument)
        {
            Initialize();

            #region Test

            var screenshot_TBGE = toolboxButtonGE_VTA.
                CloneTree().
                Q<ToolboxButtonGenericElement>().
                Setup("Screenshot", "avatarOn", "avatarOff", true, () =>
                {
                    ActivateDeactivateAvatarTracking.Instance.ToggleTrackingStatus();
                });
            var import = toolboxButtonGE_VTA.
                CloneTree().
                Q<ToolboxButtonGenericElement>().
                Setup("import", "soundOn", "soundOff", true, () =>
                {

                });
            ToolboxGenericElement image = toolboxGE_VTA.
                CloneTree().
                Q<ToolboxGenericElement>().
                Setup("Image", new ToolboxButtonGenericElement[2] { screenshot_TBGE, import });
            //centerLayout_VE.AddElement(image);

            var gallery = toolboxButtonGE_VTA.
                CloneTree().
                Q<ToolboxButtonGenericElement>().
                Setup("gallery", "micOn", "micOff", false, () =>
                {

                });
            ToolboxGenericElement test = toolboxGE_VTA.
                CloneTree().
                Q<ToolboxGenericElement>().
                Setup("Test", gallery);
            test.AddTo(SubMenuLayout);


            #endregion

            #region Left layout

            AddSpacer(leftLayout_VE);
            
            ToolboxButtonGenericElement openToolboxButton_TBGE = toolboxButtonGE_VTA.
                CloneTree().
                Q<ToolboxButtonGenericElement>().
                Setup("Toolbox", "toolbox", "toolbox", true, () => 
                {
                    Menu.Environment.MenuBar_UIController.Instance.StartCoroutine(LogWorldPosition(test, image));
                    Menu.DialogueBox_UIController.
                        Setup("TODO", "Not implemented yed", "Close", () => { }).
                        DisplayFrom(uiDocument);
                });
            toolboxGE_VTA.
                CloneTree().
                Q<ToolboxGenericElement>().
                Setup("", openToolboxButton_TBGE).
                AddTo(leftLayout_VE);

            AddSeparator(leftLayout_VE, toolboxSeparatorGE_VTA);

            #endregion

            centerLayout_VE.
                Setup("menuBar", "scrollView-btn", (vE) => 
                { 
                    AddSeparator(vE, toolboxSeparatorGE_VTA); 
                });

            //Test
            centerLayout_VE.AddElement(image);

            #region Right layout

            AddSeparator(rightLayout_VE, toolboxSeparatorGE_VTA);

            avatar_TBGE = toolboxButtonGE_VTA.
                CloneTree().
                Q<ToolboxButtonGenericElement>().
                Setup("Screenshot", "avatarOn", "avatarOff", true, () => 
                {
                    ActivateDeactivateAvatarTracking.Instance.ToggleTrackingStatus();
                });
            sound_TBGE = toolboxButtonGE_VTA.
                CloneTree().
                Q<ToolboxButtonGenericElement>().
                Setup("label test", "soundOn", "soundOff", true, () => 
                {
                    ActivateDeactivateAudio.Instance.ToggleAudioStatus();
                });
            mic_TBGE = toolboxButtonGE_VTA.
                CloneTree().
                Q<ToolboxButtonGenericElement>().
                Setup("labelTestAndTest", "micOn", "micOff", false, () => 
                {
                    ActivateDeactivateMicrophone.Instance.ToggleMicrophoneStatus();
                });
            toolboxGE_VTA.
                CloneTree().
                Q<ToolboxGenericElement>().
                Setup("test", new ToolboxButtonGenericElement[3] { avatar_TBGE, sound_TBGE, mic_TBGE }).
                AddTo(rightLayout_VE);

            AddSeparator(rightLayout_VE, toolboxSeparatorGE_VTA);

            
            ToolboxButtonGenericElement leave_TBGE = toolboxButtonGE_VTA.
                CloneTree().
                Q<ToolboxButtonGenericElement>().
                Setup("", "leave", "leave", true, () => 
                {
                    Menu.DialogueBox_UIController.
                        Setup("Leave environment", "Are you sure ...?", "YES", "NO", (b) =>
                        {
                            if (b)
                                ConnectionMenu.Instance.Leave();
                        }).
                        DisplayFrom(uiDocument);
                });
            toolboxGE_VTA.
                CloneTree().
                Q<ToolboxGenericElement>().
                Setup("", leave_TBGE).
                AddTo(rightLayout_VE);

            AddSpacer(rightLayout_VE);

            #endregion

            ReadyToDisplay();
        }

        private IEnumerator LogWorldPosition(VisualElement test, VisualElement image)
        {
            yield return null;

            Debug.Log($"test x = {test.worldBound.x}");
            Debug.Log($"image x = {image.worldBound.x}");
            //test.style.left = image.ChangeCoordinatesTo(test, new Vector2(image.layout.x, test.layout.y)).x;

            test.style.left = image.WorldToLocal(new Vector2(image.worldBound.x, 0f)).x;
        }

        private void AddSpacer(VisualElement layoutContainer_VE)
        {
            VisualElement space = new VisualElement();
            space.style.width = 10;
            layoutContainer_VE.Add(space);
        }

        private void AddSeparator(VisualElement layoutContainer_VE, VisualTreeAsset toolboxSeparatorGE_VTA)
        {
            toolboxSeparatorGE_VTA.
                CloneTree().
                Q<ToolboxSeparatorGenericElement>().
                Setup().
                AddTo(layoutContainer_VE);
        }

        public override void OnApplyUserPreferences()
        {
            if (!displayed)
                return;
        }
    }
}
