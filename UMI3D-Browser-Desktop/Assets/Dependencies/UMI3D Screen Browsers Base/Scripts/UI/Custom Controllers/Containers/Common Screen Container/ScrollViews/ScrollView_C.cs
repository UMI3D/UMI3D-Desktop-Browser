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
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Container
{
    public class ScrollView_C : ScrollView
    {
        public new class UxmlFactory : UxmlFactory<ScrollView_C, UxmlTraits> { }

        public new class UxmlTraits : ScrollView.UxmlTraits
        {
            UxmlEnumAttributeDescription<ElementCategory> m_category = new UxmlEnumAttributeDescription<ElementCategory>
            {
                name = "category",
                defaultValue = ElementCategory.Menu
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
                var custom = ve as ScrollView_C;

                
                custom.mode = custom.mode;
                custom.Category = m_category.GetValueFromBag(bag, cc);
            }
        }

        public virtual string StyleSheetContainerPath => $"USS/container";
        public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetContainersFolderPath}/scrollview";
        public virtual string USSCustomClassName => "scrollview";
        public virtual string USSCustomClassCategory(ElementCategory category) => $"{USSCustomClassName}-{category}".ToLower();
        public virtual string USSCustomClassMode(ScrollViewMode mode) => $"{USSCustomClassName}-{mode}".ToLower();

        public new virtual ScrollViewMode mode
        {
            get => m_mode;
            set
            {
                RemoveFromClassList(USSCustomClassMode(m_mode));
                AddToClassList(USSCustomClassMode(value));
                base.mode = value;
                m_mode = value;
            }
        }
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
        

        protected ScrollViewMode m_mode;
        protected ElementCategory m_category;
        protected Length m_containerPadding = float.NaN;

        public ScrollView_C() => InitElement();

        /// <summary>
        /// Initialize this element.
        /// </summary>
        public virtual void InitElement()
        {
            try
            {
                this.AddStyleSheetFromPath(StyleSheetContainerPath);
                this.AddStyleSheetFromPath(StyleSheetPath);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            AddToClassList(USSCustomClassName);

            this.RegisterCallback<CustomStyleResolvedEvent>((evt) =>
            {
                this.TryGetCustomStyle("--padding-container-scrollview", out m_containerPadding);
            });

            horizontalScroller.RegisterCallback<GeometryChangedEvent>((evt) =>
            {
                if (evt.newRect.height > 0) horizontalScroller.style.marginTop = m_containerPadding;
                else horizontalScroller.style.marginTop = StyleKeyword.Null;
            });

            verticalScroller.RegisterCallback<GeometryChangedEvent>((evt) =>
            {
                if (evt.newRect.width > 0) verticalScroller.style.marginLeft = m_containerPadding;
                else verticalScroller.style.marginLeft = StyleKeyword.Null;
            });

            mode = ScrollViewMode.Vertical;
            Category = ElementCategory.Menu;
        }
    }
}
