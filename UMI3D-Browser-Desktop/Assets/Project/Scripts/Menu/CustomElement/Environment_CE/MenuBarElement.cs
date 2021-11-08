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

        private VisualElement horizontalLayout_VE;
        private VisualElement leftLayout_VE;
        private VisualElement centerLayout_VE;
        private VisualElement rightLayout_VE;

        public void Setup(VisualTreeAsset toolboxGE_VTA, VisualTreeAsset toolboxButtonGE_VTA, VisualTreeAsset toolboxSeparatorGE_VTA)
        {
            horizontalLayout_VE = this.Q<VisualElement>("Horizontal-layout");
            leftLayout_VE = this.Q<VisualElement>("Left-layout");
            centerLayout_VE = this.Q<VisualElement>("Center-layout");
            rightLayout_VE = this.Q<VisualElement>("Right-layout");

            #region Left layout
            //DONE add space
            VisualElement spaceOpenButton = new VisualElement();
            spaceOpenButton.style.width = 10;
            leftLayout_VE.Add(spaceOpenButton);

            ToolboxGenericElement openToolbox_TGE = toolboxGE_VTA.CloneTree().Q<ToolboxGenericElement>();
            ToolboxButtonGenericElement openToolboxButton_TBGE = toolboxButtonGE_VTA.CloneTree().Q<ToolboxButtonGenericElement>();
            openToolboxButton_TBGE.Setup("Toolbox", "toolbox", "toolbox", true, () => { Debug.Log("TODO"); });
            openToolbox_TGE.Setup("", openToolboxButton_TBGE);
            leftLayout_VE.Add(openToolbox_TGE);

            //DONE add separator
            ToolboxSeparatorGenericElement separatorOpenToolboxButton = toolboxSeparatorGE_VTA.CloneTree().Q<ToolboxSeparatorGenericElement>();
            separatorOpenToolboxButton.Setup();
            leftLayout_VE.Add(separatorOpenToolboxButton);
            #endregion

            //TODO add toolbox container

            #region Right layout
            //DONE add separator
            ToolboxSeparatorGenericElement separatorSettings = toolboxSeparatorGE_VTA.CloneTree().Q<ToolboxSeparatorGenericElement>();
            separatorSettings.Setup();
            rightLayout_VE.Add(separatorSettings);

            ToolboxGenericElement settings_TGE = toolboxGE_VTA.CloneTree().Q<ToolboxGenericElement>();
            ToolboxButtonGenericElement avatar_TBGE = toolboxButtonGE_VTA.CloneTree().Q<ToolboxButtonGenericElement>();
            avatar_TBGE.Setup("Screenshot", "avatarOn", "avatarOff", true, () => { Debug.Log("TODO"); });
            ToolboxButtonGenericElement sound_TBGE = toolboxButtonGE_VTA.CloneTree().Q<ToolboxButtonGenericElement>();
            sound_TBGE.Setup("label test", "soundOn", "soundOff", true, () => { Debug.Log("TODO"); });
            ToolboxButtonGenericElement mic_TBGE = toolboxButtonGE_VTA.CloneTree().Q<ToolboxButtonGenericElement>();
            mic_TBGE.Setup("labelTestAndTest", "micOn", "micOff", true, () => { Debug.Log("TODO"); });
            settings_TGE.Setup("test", new ToolboxButtonGenericElement[3] { avatar_TBGE, sound_TBGE, mic_TBGE });
            rightLayout_VE.Add(settings_TGE);

            //DONE add separator
            ToolboxSeparatorGenericElement separatorLeaving = toolboxSeparatorGE_VTA.CloneTree().Q<ToolboxSeparatorGenericElement>();
            separatorLeaving.Setup();
            rightLayout_VE.Add(separatorLeaving);

            ToolboxGenericElement leaveEnvironment_TGE = toolboxGE_VTA.CloneTree().Q<ToolboxGenericElement>();
            ToolboxButtonGenericElement leave_TBGE = toolboxButtonGE_VTA.CloneTree().Q<ToolboxButtonGenericElement>();
            leave_TBGE.Setup("", "leave", "leave", true, () => { Debug.Log("TODO"); });
            leaveEnvironment_TGE.Setup("", leave_TBGE);
            rightLayout_VE.Add(leaveEnvironment_TGE);

            //DONE add space
            VisualElement spaceLeaveEnvironment = new VisualElement();
            spaceLeaveEnvironment.style.width = 10;
            rightLayout_VE.Add(spaceLeaveEnvironment);
            #endregion
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
