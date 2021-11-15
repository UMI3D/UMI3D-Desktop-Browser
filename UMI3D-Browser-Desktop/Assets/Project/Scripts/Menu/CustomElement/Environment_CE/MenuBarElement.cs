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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


namespace BrowserDesktop.UI.CustomElement
{
    public class MenuBarElement : VisualElement
    {
        /// <summary>
        /// To be recognized by UI Builder
        /// </summary>
        public new class UxmlFactory : UxmlFactory<MenuBarElement, UxmlTraits> { }
        /// <summary>
        /// To be recognized by UI Builder
        /// </summary>
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private VisualElement leftLayout_VE;
        private VisualElement centerLayout_VE;
        private VisualElement rightLayout_VE;

        private ToolboxButtonGenericElement avatar_TBGE;
        public ToolboxButtonGenericElement Avatar_TBGE => avatar_TBGE;
        private ToolboxButtonGenericElement sound_TBGE;
        public ToolboxButtonGenericElement Sound_TBGE => sound_TBGE;
        private ToolboxButtonGenericElement mic_TBGE;
        public ToolboxButtonGenericElement Mic_TBGE => mic_TBGE;

        public void Setup(VisualTreeAsset toolboxGE_VTA, VisualTreeAsset toolboxButtonGE_VTA, VisualTreeAsset toolboxSeparatorGE_VTA, UIDocument uiDocument)
        {
            leftLayout_VE = this.Q<VisualElement>("Left-layout");
            centerLayout_VE = this.Q<VisualElement>("Center-layout");
            rightLayout_VE = this.Q<VisualElement>("Right-layout");

            #region Left layout
            //DONE add space
            AddSpacer(leftLayout_VE);

            ToolboxGenericElement openToolbox_TGE = toolboxGE_VTA.CloneTree().Q<ToolboxGenericElement>();
            ToolboxButtonGenericElement openToolboxButton_TBGE = toolboxButtonGE_VTA.CloneTree().Q<ToolboxButtonGenericElement>();
            openToolboxButton_TBGE.Setup("Toolbox", "toolbox", "toolbox", true, () => { Debug.Log("TODO"); });
            openToolbox_TGE.Setup("", openToolboxButton_TBGE);
            leftLayout_VE.Add(openToolbox_TGE);

            //DONE add separator
            AddSeparator(leftLayout_VE, toolboxSeparatorGE_VTA);
            #endregion

            //TODO add toolbox container

            #region Right layout
            //DONE add separator
            AddSeparator(rightLayout_VE, toolboxSeparatorGE_VTA);

            ToolboxGenericElement settings_TGE = toolboxGE_VTA.CloneTree().Q<ToolboxGenericElement>();
            avatar_TBGE = toolboxButtonGE_VTA.CloneTree().Q<ToolboxButtonGenericElement>();
            avatar_TBGE.Setup("Screenshot", "avatarOn", "avatarOff", true, () => {
                ActivateDeactivateAvatarTracking.Instance.ToggleTrackingStatus();
            });
            sound_TBGE = toolboxButtonGE_VTA.CloneTree().Q<ToolboxButtonGenericElement>();
            sound_TBGE.Setup("label test", "soundOn", "soundOff", true, () => {
                ActivateDeactivateAudio.Instance.ToggleAudioStatus();
            });
            mic_TBGE = toolboxButtonGE_VTA.CloneTree().Q<ToolboxButtonGenericElement>();
            mic_TBGE.Setup("labelTestAndTest", "micOn", "micOff", false, () => {
                ActivateDeactivateMicrophone.Instance.ToggleMicrophoneStatus();
            });
            settings_TGE.Setup("test", new ToolboxButtonGenericElement[3] { avatar_TBGE, sound_TBGE, mic_TBGE });
            rightLayout_VE.Add(settings_TGE);

            //DONE add separator
            AddSeparator(rightLayout_VE, toolboxSeparatorGE_VTA);

            ToolboxGenericElement leaveEnvironment_TGE = toolboxGE_VTA.CloneTree().Q<ToolboxGenericElement>();
            ToolboxButtonGenericElement leave_TBGE = toolboxButtonGE_VTA.CloneTree().Q<ToolboxButtonGenericElement>();
            leave_TBGE.Setup("", "leave", "leave", true, () => {
                Menu.DialogueBox_UIController.Instance.Setup("Leave environment", "Are you sure ...?", "YES", "NO", (b) =>
                {
                    if (b)
                        ConnectionMenu.Instance.Leave();
                });
                Menu.DialogueBox_UIController.Instance.DisplayFrom(uiDocument);
            });
            leaveEnvironment_TGE.Setup("", leave_TBGE);
            rightLayout_VE.Add(leaveEnvironment_TGE);

            //DONE add space
            AddSpacer(rightLayout_VE);
            #endregion
        }

        private void AddSpacer(VisualElement layoutContainer_VE)
        {
            VisualElement space = new VisualElement();
            space.style.width = 10;
            layoutContainer_VE.Add(space);
        }

        private void AddSeparator(VisualElement layoutContainer_VE, VisualTreeAsset toolboxSeparatorGE_VTA)
        {
            ToolboxSeparatorGenericElement separator = toolboxSeparatorGE_VTA.CloneTree().Q<ToolboxSeparatorGenericElement>();
            separator.Setup();
            layoutContainer_VE.Add(separator);
        }

        /// <summary>
        /// Apply user preferences when needed.
        /// </summary>
        public void OnApplyUserPreferences()
        {
            //TODO change theme
            
        }


    }
}
