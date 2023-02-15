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
using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.baseBrowser.ui.viewController;
using umi3d.commonScreen.Container;
using umi3d.UiPreview.commonScreen.game;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Container
{
    public class ScrollableDataCollection_C<D> : AbstractDataCollection_C<D>
    {
        public new class UxmlFactory : UxmlFactory<ScrollableDataCollection_C<D>, UxmlTraits> { }

        public new class UxmlTraits : AbstractDataCollection_C<D>.UxmlTraits
        {
            protected UxmlEnumAttributeDescription<ElementCategory> m_category = new UxmlEnumAttributeDescription<ElementCategory>
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
                var custom = ve as ScrollableDataCollection_C<D>;

                custom.Mode = m_ScrollViewMode.GetValueFromBag(bag, cc);
                custom.SelectionType = m_selectionType.GetValueFromBag(bag, cc);
                custom.Size = m_size.GetValueFromBag(bag, cc);
                custom.IsReorderable = m_isReorderable.GetValueFromBag(bag, cc);
                custom.ReorderableMode = m_reorderableMode.GetValueFromBag(bag, cc);
                custom.Category = m_category.GetValueFromBag(bag, cc);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override ScrollViewMode Mode 
        { 
            get => base.Mode; 
            set
            {
                base.Mode = value;
                ScrollView.mode = value;
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
                ScrollView.Category = m_category;
            }
        }
        
        public virtual string StyleSheetContainerPath => $"USS/container";
        public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetContainersFolderPath}/scrollableDataCollection";
        public override string USSCustomClassName => "sdc";
        public virtual string USSCustomClassCategory(ElementCategory category) => $"{USSCustomClassName}-{category}".ToLower();
        
        public ScrollView_C ScrollView = new ScrollView_C { name = "scroll-view" };

        public override VisualElement DataContainer => ScrollView;

        protected ElementCategory m_category;
        
        public ScrollableDataCollection_C() => InitElement();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void InitElement()
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

            Add(ScrollView);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Set()
        {
            InitElement();
            Set(ElementCategory.Menu, ScrollViewMode.Vertical, SelectionType.None, 0, false, ReorderableMode.Dragger);
        }

        /// <summary>
        /// Set the properties of this Element.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="mode"></param>
        /// <param name="selectionType"></param>
        public virtual void Set(ElementCategory category, ScrollViewMode mode, SelectionType selectionType, float size, bool isReorderable, ReorderableMode reorderableMode)
        {
            Category = category;
            Mode = mode;
            SelectionType = selectionType;
            Size = size;
            IsReorderable = isReorderable;
            ReorderableMode = reorderableMode;
        }
    }
}

namespace umi3d.UiPreview.commonScreen.Container
{
    public class ScrollableDataCollection_Preview_int: ScrollableDataCollection_C<int>
    {
        public new class UxmlFactory : UxmlFactory<ScrollableDataCollection_Preview_int, UxmlTraits>
        {
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                ScrollableDataCollection_C<int> previewItem = base.Create(bag, cc) as ScrollableDataCollection_C<int>;

                previewItem.MakeItem = datum => new umi3d.commonScreen.Displayer.Text_C();
                previewItem.BindItem = (datum, item) =>
                {
                    var text = item as umi3d.commonScreen.Displayer.Text_C;
                    text.LocaliseText = $"item {datum}";
                };
                previewItem.UnbindItem = (datum, item) =>
                {
                    var text = item as umi3d.commonScreen.Displayer.Text_C;
                    text.LocaliseText = null;
                };

                for (int i = 0; i < 100; i++) previewItem.AddDatum(i);

                return previewItem;
            }
        }
    }
}