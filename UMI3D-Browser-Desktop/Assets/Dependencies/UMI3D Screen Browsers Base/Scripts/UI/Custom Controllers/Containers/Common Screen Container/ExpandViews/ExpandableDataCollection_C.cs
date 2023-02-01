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
using System.Drawing;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.game;
using umi3d.UiPreview.commonScreen.game;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Container
{
    public class ExpandableDataCollection_C<D> : AbstractDataCollection_C<D>
    {
        public new class UxmlTraits : AbstractDataCollection_C<D>.UxmlTraits
        {
            protected UxmlFloatAttributeDescription m_animationTimeIn = new UxmlFloatAttributeDescription
            {
                name = "animation-time-in",
                defaultValue = 0f
            };
            protected UxmlFloatAttributeDescription m_animationTimeOut = new UxmlFloatAttributeDescription
            {
                name = "animation-time-out",
                defaultValue = 0f
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
                var custom = ve as ExpandableDataCollection_C<D>;

                custom.Mode = m_ScrollViewMode.GetValueFromBag(bag, cc);
                custom.SelectionType = m_selectionType.GetValueFromBag(bag, cc);
                custom.Size = m_size.GetValueFromBag(bag, cc);
                custom.IsReorderable = m_isReorderable.GetValueFromBag(bag, cc);
                custom.ReorderableMode = m_reorderableMode.GetValueFromBag(bag, cc);
                custom.AnimationTimeIn = m_animationTimeIn.GetValueFromBag(bag, cc);
                custom.AnimationTimeOut = m_animationTimeOut.GetValueFromBag(bag, cc);
            }
        }

        /// <summary>
        /// Animation time when add an item
        /// </summary>
        public virtual float AnimationTimeIn { get; set; }
        /// <summary>
        /// Animation time when remove an item
        /// </summary>
        public virtual float AnimationTimeOut { get; set; }

        public virtual string StyleSheetContainerPath => $"USS/container";
        public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetContainersFolderPath}/expandableDataCollection";
        public override string USSCustomClassName => "edc";
        public virtual string USSCustomClassContentViewport => $"{USSCustomClassName}-content__viewport";
        public virtual string USSCustomClassContentContainer => $"{USSCustomClassName}-content__container";

        public VisualElement ContentVieport = new VisualElement { name = "content-viewport" };
        public VisualElement ContentContainer = new VisualElement { name = "content-container" };

        public override VisualElement DataContainer => ContentContainer;

        protected float m_animationTime;

        public ExpandableDataCollection_C() => InitElement();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
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
            ContentVieport.AddToClassList(USSCustomClassContentViewport);
            ContentContainer.AddToClassList(USSCustomClassContentContainer);

            ContentContainer.RegisterCallback<GeometryChangedEvent>(ce =>
            {
                bool isAnimationIn = ce.newRect.height > ce.oldRect.height;
                ContentVieport.AddAnimation
                (
                    this,
                    () => ContentVieport.style.height = ContentVieport.resolvedStyle.height,
                    () => ContentVieport.style.height = ContentContainer.resolvedStyle.height,
                    "height",
                    isAnimationIn ? AnimationTimeIn : AnimationTimeOut
                );
            });

            Add(ContentVieport);
            ContentVieport.Add(ContentContainer);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void Set() => InitElement();

        #region Implementation

        #endregion
    }
}

namespace umi3d.UiPreview.commonScreen.Container
{
    public class ExpandableDataCollection_Preview_Int: ExpandableDataCollection_C<int>
    {
        public new class UxmlFactory : UxmlFactory<ExpandableDataCollection_Preview_Int, UxmlTraits>
        {
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                ExpandableDataCollection_C<int> previewItem = base.Create(bag, cc) as ExpandableDataCollection_C<int>;

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

                for (int i = 0; i < 3; i++) previewItem.AddDatum(i);

                return previewItem;
            }
        }
    }
}
