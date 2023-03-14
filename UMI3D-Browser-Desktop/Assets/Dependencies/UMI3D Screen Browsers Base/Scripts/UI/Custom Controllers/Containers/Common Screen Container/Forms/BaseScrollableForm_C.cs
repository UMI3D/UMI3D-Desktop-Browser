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
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Container
{
    public abstract class BaseScrollableForm_C : BaseVisual_C
    {
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            protected UxmlEnumAttributeDescription<ElementCategory> m_category = new UxmlEnumAttributeDescription<ElementCategory>
            {
                name = "category",
                defaultValue = ElementCategory.Menu
            };
            protected UxmlLocaliseAttributeDescription m_title = new UxmlLocaliseAttributeDescription
            {
                name = "title"
            };
            protected UxmlStringAttributeDescription m_iconPath = new UxmlStringAttributeDescription
            {
                name = "icon-path",
                defaultValue = null
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
                var custom = ve as BaseScrollableForm_C;

                custom.Category = m_category.GetValueFromBag(bag, cc);
                custom.Title = m_title.GetValueFromBag(bag, cc);
                custom.IconPath = m_iconPath.GetValueFromBag(bag, cc);
            }
        }

        /// <summary>
        /// Category of this element (<see cref="ElementCategory"/>)
        /// </summary>
        public virtual ElementCategory Category
        {
            get => m_category;
            set
            {
                RemoveFromClassList(USSCustomClassCategory(m_category));
                AddToClassList(USSCustomClassCategory(value));
                m_category = value;
            }
        }

        /// <summary>
        /// Display a title for this container.
        /// </summary>
        public virtual LocalisationAttribute Title
        {
            get => TitleLabel.LocaliseText;
            set
            {
                IsSet = false;
                if (value.IsEmpty) TitleLabel.RemoveFromHierarchy();
                else Insert(0, TitleLabel);
                TitleLabel.LocaliseText = value;
                IsSet = true;
            }
        }

        /// <summary>
        /// Display an icon for this container.
        /// </summary>
        public virtual string IconPath
        {
            get => m_iconPath;
            set
            {
                IsSet = false;
                if (string.IsNullOrEmpty(value))
                {
                    m_iconPath = null;
                    IconVisual.style.backgroundImage = StyleKeyword.Null;
                    IconVisual.RemoveFromHierarchy();
                }
                else
                {
                    m_iconPath = value;
                    Insert(1, IconVisual);
                    IconVisual.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(value));
                }
                IsSet = true;
            }
        }

        public override string StyleSheetPath_MainTheme => $"USS/container";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetContainersFolderPath}/scrollableForm";

        public override string UssCustomClass_Emc => "scrollable__form";
        public virtual string USSCustomClassCategory(ElementCategory category) => $"{UssCustomClass_Emc}-{category}".ToLower();
        public virtual string USSCustomClassTitle => $"{UssCustomClass_Emc}-title";
        public virtual string USSCustomClassIcon => $"{UssCustomClass_Emc}-icon";

        public Text_C TitleLabel = new Text_C { name = "title" };
        public VisualElement IconVisual = new VisualElement { name = "icon" };

        protected ElementCategory m_category;
        protected string m_iconPath;

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            TitleLabel.AddToClassList(USSCustomClassTitle);
            IconVisual.AddToClassList(USSCustomClassIcon);
        }

        protected override void InitElement()
        {
            base.InitElement();
            TitleLabel.TextStyle = TextStyle.LowTitle;
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Category = ElementCategory.Menu;
            Title = null;
            IconPath = null;
        }
    }
}
