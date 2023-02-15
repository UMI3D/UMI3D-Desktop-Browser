/*
Copyright 2019 - 2022 Inetum

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
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public class Cursor_C : Visual_C
    {
        public new class UxmlFactory : UxmlFactory<Cursor_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlEnumAttributeDescription<ElementPseudoState> m_state = new UxmlEnumAttributeDescription<ElementPseudoState>
            {
                name = "state",
                defaultValue = ElementPseudoState.Enabled
            };
            protected UxmlLocaliseAttributeDescription m_action = new UxmlLocaliseAttributeDescription
            {
                name = "action"
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
                var custom = ve as Cursor_C;

                custom.State = m_state.GetValueFromBag(bag, cc);
                custom.Action = m_action.GetValueFromBag(bag, cc);
            }
        }

        public virtual ElementPseudoState State
        {
            get => m_state;
            set
            {
                RemoveFromClassList(USSCustomClassState(m_state));
                AddToClassList(USSCustomClassState(value));
                m_state = value;
                if (value == ElementPseudoState.Disabled) this.SetEnabled(false);
                else this.SetEnabled(true);
            }
        }
        public virtual LocalisationAttribute Action
        {
            get => ActionText.LocaliseText;
            set
            {
                ActionText.LocaliseText = value;
                if (value.IsEmpty) ActionText.RemoveFromHierarchy();
                else
                {
                    Add(ActionText);
                    ActionText.schedule.Execute(() =>
                    {
                        var halfWidth = ActionText.layout.width / 2f;
                        ActionText.style.right = halfWidth + 40f;
                    });
                }
            }
        }

        public override string StyleSheetPath_MainTheme => $"USS/game";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetGamesFolderPath}/cursor";

        public override string UssCustomClass_Emc => "cursor";
        public virtual string USSCustomClassTarget => $"{UssCustomClass_Emc}-target";
        public virtual string USSCustomClassAction => $"{UssCustomClass_Emc}-action";
        public virtual string USSCustomClassState(ElementPseudoState state) => $"{UssCustomClass_Emc}-{state}".ToLower();

        public VisualElement Target = new VisualElement { name = "target" };
        public Text_C ActionText = new Text_C { name = "action" };

        protected ElementPseudoState m_state;

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            Target.AddToClassList(USSCustomClassTarget);
            ActionText.AddToClassList(USSCustomClassAction);
        }

        protected override void InitElement()
        {
            base.InitElement();
            Add(Target);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            State = ElementPseudoState.Enabled;
            Action = null;
        }
    }
}
