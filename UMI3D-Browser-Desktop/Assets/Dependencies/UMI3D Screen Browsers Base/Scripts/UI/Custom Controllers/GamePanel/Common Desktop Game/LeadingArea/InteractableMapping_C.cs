/*
Copyright 2019 - 2023 Inetum

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
using System.Collections.Generic;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.commonScreen;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace umi3d.commonDesktop.game
{
    public class InteractableMapping_C : BaseVisual_C
    {
        public new class UxmlFactory : UxmlFactory<InteractableMapping_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            protected UxmlEnumAttributeDescription<ControllerEnum> m_controller = new UxmlEnumAttributeDescription<ControllerEnum>
            {
                name = "controller",
                defaultValue = ControllerEnum.MouseAndKeyboard
            };

            protected UxmlLocaliseAttributeDescription m_interactableName = new UxmlLocaliseAttributeDescription
            {
                name = "interactable-name"
            };

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="ve"></param>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                if (Application.isPlaying) return;

                base.Init(ve, bag, cc);
                var custom = ve as InteractableMapping_C;

                custom.Controller = m_controller.GetValueFromBag(bag, cc);
                custom.InteractableName = m_interactableName.GetValueFromBag(bag, cc);
            }
        }

        /// <summary>
        /// The current controller use with this browser.
        /// </summary>
        public virtual ControllerEnum Controller
        {
            get => m_controller;
            set => m_controller = value;
        }
        /// <summary>
        /// Name of the interactable currently hovered (by default : "Interactable mapping"). This text will be displayed at the top.
        /// </summary>
        public virtual LocalisationAttribute InteractableName
        {
            get => InteractableNameText.LocalisedText;
            set => InteractableNameText.LocalisedText = value.IsEmpty ? "Interactable mapping" : value;
        }

        public override string StyleSheetPath_MainTheme => $"USS/game";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetGamesFolderPath}/interactableMapping";

        public override string UssCustomClass_Emc => "interactable__mapping";
        public virtual string USSCustomClassMain => $"{UssCustomClass_Emc}-main";
        public virtual string USSCustomClassInteractableName => $"{UssCustomClass_Emc}-interactable__name";

        public VisualElement Main = new VisualElement { name = "main" };
        public Text_C InteractableNameText = new Text_C { name = "interactable-name" };
        public ScrollView_C ScrollView = new ScrollView_C { name = "scroll-view" };

        protected ControllerEnum m_controller;

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            Main.AddToClassList(USSCustomClassMain);
            InteractableNameText.AddToClassList(USSCustomClassInteractableName);
        }

        protected override void InitElement()
        {
            KeyboardInteraction.Mapped = AddMapping;
            KeyboardInteraction.Unmapped = RemoveMapping;

            Add(Main);
            Main.Add(InteractableNameText);
            Main.Add(ScrollView);

            pickingMode = PickingMode.Ignore;
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Controller = ControllerEnum.MouseAndKeyboard;
            InteractableName = null;
        }

        #region Implementation

        /// <summary>
        /// Event raised when an interaction has been mapped.
        /// </summary>
        public event System.Action MappingAdded;
        /// <summary>
        /// Event raised when all interactions have been unmapped.
        /// </summary>
        public event System.Action MappingRemoved;
        /// <summary>
        /// Key: Keyboard interaction. Value: the customInteractableMappingRow corresponding to this interaction.
        /// </summary>
        public static Dictionary<KeyboardInteraction, InteractableMappingRow_C> S_interactionMapping = new Dictionary<KeyboardInteraction, InteractableMappingRow_C>();

        protected List<InteractableMappingRow_C> WaitingRows = new List<InteractableMappingRow_C>();
        protected List<InteractableMappingRow_C> ActiveRows = new List<InteractableMappingRow_C>();

        /// <summary>
        /// Add a mapping for this <paramref name="interaction"/> with this <paramref name="action"/>
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public void AddMapping(KeyboardInteraction interaction, string name, InputAction action)
        {
            var row = CreateMappingRow();
            row.AddMapping(name, Controller, action);

            if (interaction is null)
                UnityEngine.Debug.LogWarning($"interaction is null");
            else if (S_interactionMapping.ContainsKey(interaction))
                UnityEngine.Debug.LogWarning($"S_interactionMapping already contain {interaction}");
            
                S_interactionMapping.Add(interaction, row);

            if (S_interactionMapping.Count == 1)
            {
                row.AddLeftClick();
            }
            MappingAdded?.Invoke();
        }

        /// <summary>
        /// Remove the mapping corresponding to the <paramref name="interaction"/>.
        /// </summary>
        /// <param name="interaction"></param>
        public void RemoveMapping(KeyboardInteraction interaction)
        {
            if (!S_interactionMapping.ContainsKey(interaction)) return;
            RemoveMappingRow(S_interactionMapping[interaction]);
            S_interactionMapping.Remove(interaction);
            if (S_interactionMapping.Count == 0) MappingRemoved?.Invoke();
        }

        protected virtual InteractableMappingRow_C CreateMappingRow()
        {
            InteractableMappingRow_C row = null;
            if (WaitingRows.Count == 0) row = new InteractableMappingRow_C();
            else
            {
                row = WaitingRows[WaitingRows.Count - 1];
                WaitingRows.RemoveAt(WaitingRows.Count - 1);
            }
            ActiveRows.Add(row);
            ScrollView.Add(row);

            return row;
        }
        protected virtual void RemoveMappingRow(InteractableMappingRow_C row)
        {
            if (row == null) return;
            ActiveRows.Remove(row);
            WaitingRows.Add(row);
            row.RemoveFromHierarchy();
            row.RemoveAllMapping();
        }

        #endregion
    }
}
