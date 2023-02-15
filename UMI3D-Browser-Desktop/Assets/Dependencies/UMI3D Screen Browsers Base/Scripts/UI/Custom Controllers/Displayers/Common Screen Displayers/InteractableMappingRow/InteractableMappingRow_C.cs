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
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.inputs.interactions.BaseKeyInteraction;

namespace umi3d.commonScreen.Displayer
{
    public class InteractableMappingRow_C : Visual_C
    {
        public new class UxmlFactory : UxmlFactory<InteractableMappingRow_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            protected UxmlLocaliseAttributeDescription m_actionName = new UxmlLocaliseAttributeDescription
            {
                name = "action-name"
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
                var custom = ve as InteractableMappingRow_C;

                custom.ActionName = m_actionName.GetValueFromBag(bag, cc);
            }
        }

        /// <summary>
        /// Name of the action.
        /// </summary>
        public virtual LocalisationAttribute ActionName
        {
            get => ActionNameText.LocaliseText;
            set => ActionNameText.LocaliseText = value;
        }

        public override string StyleSheetPath_MainTheme => "USS/displayer";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/interactableMappingRow";

        public override string UssCustomClass_Emc => "interactable__mapping__row";
        public virtual string USSCustomClassActionName => $"{UssCustomClass_Emc}-action__name";
        public virtual string USSCustomClassMain => $"{UssCustomClass_Emc}-main";

        public Text_C ActionNameText = new Text_C { name = "action-name" };
        public VisualElement Main = new VisualElement { name = "main" };

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            ActionNameText.AddToClassList(USSCustomClassActionName);
            Main.AddToClassList(USSCustomClassMain);
        }

        protected override void InitElement()
        {
            base.InitElement();
            Add(ActionNameText);
            Add(Main);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            ActionName = null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override VisualElement contentContainer => IsSet ? Main : this;

        #region Implementation

        protected List<Mapping_C> WaitingMappings = new List<Mapping_C>();
        protected List<Mapping_C> ActiveMappings = new List<Mapping_C>();
        /// <summary>
        /// List of the children corresponding of the mapping (CustomMapping and CustomText).
        /// </summary>
        public List<VisualElement> MappingChildren = new List<VisualElement>();

        /// <summary>
        /// Add the mapping for this interaction.
        /// </summary>
        /// <param name="name">Name of the action</param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        public void AddMapping(string name, ControllerEnum controller, InputAction action)
        {
            ActionName = name;

            List<(ControllerInputEnum, string)> mappings = null;
            switch (controller)
            {
                case ControllerEnum.MouseAndKeyboard:
                    mappings = action.GetFirstMappingFromController(ControllerInputEnum.Keyboard, ControllerInputEnum.Mouse);
                    foreach (var mapping in mappings)
                    {
                        var map = CreateMapping();
                        if (mapping.Item1 == ControllerInputEnum.Keyboard)
                        {
                            map.MappingName = mapping.Item2;
                            map.Type = MappingType.Keyboard;
                        }
                        else
                        {
                            //TODO for manipulation
                        }

                        Add(map);
                        MappingChildren.Add(map);
                    }
                    break;
                case ControllerEnum.Touch:
                    //TODO
                    break;
                case ControllerEnum.GameController:
                    mappings = action.GetFirstMappingFromController(ControllerInputEnum.Gamepad);
                    //TODO
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Remove all mapping.
        /// </summary>
        public void RemoveAllMapping()
        {
            for (int i = 0; i < MappingChildren.Count; i++)
            {
                var child = MappingChildren[i];
                child.RemoveFromHierarchy();
                if (child is not Mapping_C mapping) return;
                RemoveMapping(mapping);
            }
        }

        protected virtual Mapping_C CreateMapping()
        {
            Mapping_C mapping = null;
            if (WaitingMappings.Count == 0) mapping = new Mapping_C();
            else
            {
                mapping = WaitingMappings[WaitingMappings.Count - 1];
                WaitingMappings.RemoveAt(WaitingMappings.Count - 1);
            }
            ActiveMappings.Add(mapping);

            return mapping;
        }
        protected virtual void RemoveMapping(Mapping_C mapping)
        {
            if (mapping == null) return;
            ActiveMappings.Remove(mapping);
            WaitingMappings.Add(mapping);
        }

        #endregion
    }
}
