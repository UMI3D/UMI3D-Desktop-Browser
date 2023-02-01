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
using System.Collections;
using System.Collections.Generic;
using umi3d.commonScreen.Container;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Container
{
    public class ScrollableExpandableDataCollection_C<D> : ExpandableDataCollection_C<D>
    {
        public new class UxmlFactory : UxmlFactory<ScrollableExpandableDataCollection_C<D>, UxmlTraits> { }

        public new class UxmlTraits : ExpandableDataCollection_C<D>.UxmlTraits
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
                var custom = ve as ScrollableExpandableDataCollection_C<D>;

                custom.Mode = m_ScrollViewMode.GetValueFromBag(bag, cc);
                custom.SelectionType = m_selectionType.GetValueFromBag(bag, cc);
                custom.Size = m_size.GetValueFromBag(bag, cc);
                custom.IsReorderable = m_isReorderable.GetValueFromBag(bag, cc);
                custom.ReorderableMode = m_reorderableMode.GetValueFromBag(bag, cc);
                custom.AnimationTimeIn = m_animationTimeIn.GetValueFromBag(bag, cc);
                custom.AnimationTimeOut = m_animationTimeOut.GetValueFromBag(bag, cc);
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

        public virtual string USSCustomClassCategory(ElementCategory category) => $"{USSCustomClassName}-{category}".ToLower();

        public ScrollView_C Scrollview = new ScrollView_C { name = "scrollview" };

        protected ElementCategory m_category;

        public override void InitElement()
        {
            base.InitElement();

            Add(Scrollview);
            Scrollview.Add(ContentVieport);
        }
    }
}

namespace umi3d.UiPreview.commonScreen.Container
{
    public class ScrollableExpandableDataCollection_Preview_Int : ScrollableExpandableDataCollection_C<int>
    {
        public new class UxmlFactory : UxmlFactory<ScrollableExpandableDataCollection_Preview_Int, UxmlTraits>
        {
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                ScrollableExpandableDataCollection_C<int> previewItem = base.Create(bag, cc) as ScrollableExpandableDataCollection_C<int>;

                void removeClick()
                {
                    previewItem.RemoveDatum(previewItem.Data[previewItem.Data.Count - 1]);
                }

                void addClick()
                {
                    previewItem.AddDatum(previewItem.Data.Count);
                }

                previewItem.MakeItem = datum => new umi3d.commonScreen.Displayer.Button_C();
                previewItem.BindItem = (datum, item) =>
                {
                    var button = item as umi3d.commonScreen.Displayer.Button_C;
                    button.style.width = Length.Percent(100);
                    button.Label = $"item {datum}";

                    if (previewItem.Data.IndexOf(datum) == 0)
                    {
                        button.text = $"delete last item";
                        button.Type = ButtonType.Danger;
                        button.clicked += removeClick;
                    }
                    else
                    {
                        button.text = $"add new item";
                        button.Type = ButtonType.Primary;
                        button.clicked += addClick;
                    }
                };
                previewItem.UnbindItem = (datum, item) =>
                {
                    var button = item as umi3d.commonScreen.Displayer.Button_C;
                    button.text = null;
                    button.Type = ButtonType.Default;

                    if (previewItem.Data.IndexOf(datum) == 0) button.clicked -= removeClick;
                    else button.clicked -= addClick;
                };
                previewItem.AnimationTimeIn = 1f;
                previewItem.AnimationTimeOut = 1f;

                for (int i = 0; i < 10; i++) previewItem.AddDatum(i);

                return previewItem;
            }
        }
    }
}
