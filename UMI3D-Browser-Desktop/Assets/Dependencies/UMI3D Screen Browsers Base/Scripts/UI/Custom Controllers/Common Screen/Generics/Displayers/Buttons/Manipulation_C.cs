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
using umi3d.baseBrowser.utils;
using umi3d.common.interaction;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class Manipulation_C : Button_C
    {
        public new class UxmlFactory : UxmlFactory<Manipulation_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            protected UxmlBoolAttributeDescription m_toogleValue = new UxmlBoolAttributeDescription
            {
                name = "toggle-value",
                defaultValue = false
            };

            protected UxmlEnumAttributeDescription<DofGroupEnum> m_dof = new UxmlEnumAttributeDescription<DofGroupEnum>
            {
                name = "dof",
                defaultValue = DofGroupEnum.X
            };

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="ve"></param>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var custom = ve as Manipulation_C;

                custom.Dof = m_dof.GetValueFromBag(bag, cc);
            }
        }

        public DofGroupEnum Dof
        {
            get => m_dof;
            set => m_dof.Value = value;
        }
        protected Source<DofGroupEnum> m_dof = DofGroupEnum.X;

        public static string StyleSheetPath_ManipulationStyle => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/manipulation";

        public virtual string USSCustomClassIcon => $"{UssCustomClass_Emc}-icon";
        public virtual string USSCustomClassDof(DofGroupEnum dof) => $"{UssCustomClass_Emc}-dof-{dof}".ToLower();

        public Visual_C Icon = new Visual_C { name = "icon" };

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_ManipulationStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            Icon.AddToClassList(USSCustomClassIcon);
        }

        protected override void InitElement()
        {
            base.InitElement();
            m_dof.ValueChanged += e =>
            {
                Icon.SwitchStyleclasses
                (
                    USSCustomClassDof(e.previousValue),
                    USSCustomClassDof(e.newValue)
                );
                LocalisedLabel = e.newValue.ToString();
            };
            m_toggleValue.ValueChanged += e =>
            {
                Body
                    .SetWidth(e.newValue ? 150f : 90f)
                    .WithAnimation();
                Body
                    .SetHeight(e.newValue ? 150f : 90f)
                    .WithAnimation();
            };
            LabelVisual.style.minWidth = 105f;
        }

        protected override void StartElement()
        {
            base.StartElement();
            Add(Icon);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            IsToggle = true;
            Height = ElementSize.Custom;
            Width = ElementSize.Custom;
            LabelAndInputDirection = ElementAlignment.Bottom;
            LabelAlignment = ElementAlignment.Center;
            Type = ButtonType.Invisible;
        }

        #region Implementation

        public DofGroupDto dofGroup;

        #endregion
    }
}
