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
    public class ScrollableDataCollection_C<D> : BaseDataCollection_C<D>
    {
        public new class UxmlFactory : UxmlFactory<ScrollableDataCollection_C<D>, UxmlTraits> { }

        public new class UxmlTraits : BaseDataCollection_C<D>.UxmlTraits
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

                custom.Category = m_category.GetValueFromBag(bag, cc);
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
                Scrollview.Category = m_category;
            }
        }
        
        public override string StyleSheetPath_MainTheme => $"USS/container";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetContainersFolderPath}/scrollableDataCollection";

        public override string UssCustomClass_Emc => "sdc";
        public virtual string USSCustomClassCategory(ElementCategory category) => $"{UssCustomClass_Emc}-{category}".ToLower();
        
        public ScrollView_C Scrollview = new ScrollView_C { name = "scroll-view" };

        public override VisualElement DataContainer => Scrollview;

        protected ElementCategory m_category;

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void InitElement()
        {
            base.InitElement();
            Add(Scrollview);
        }

        protected override void UpdateMode(ChangeEvent<ScrollViewMode> e)
        {
            base.UpdateMode(e);
            Scrollview.mode = e.newValue;
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
                    text.LocalisedText = $"item {datum}";
                };
                previewItem.UnbindItem = (datum, item) =>
                {
                    var text = item as umi3d.commonScreen.Displayer.Text_C;
                    text.LocalisedText = null;
                };

                for (int i = 0; i < 100; i++) previewItem.AddDatum(i);

                return previewItem;
            }
        }
    }
}