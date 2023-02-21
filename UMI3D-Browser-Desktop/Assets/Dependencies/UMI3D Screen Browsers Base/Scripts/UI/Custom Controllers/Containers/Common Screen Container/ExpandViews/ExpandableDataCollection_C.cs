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
using umi3d.commonScreen.Container;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Container
{
    public class ExpandableDataCollection_C<D> : BaseDataCollection_C<D>
    {
        public new class UxmlTraits : BaseDataCollection_C<D>.UxmlTraits
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

        public override string StyleSheetPath_MainTheme => $"USS/container";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetContainersFolderPath}/expandableDataCollection";

        public override string UssCustomClass_Emc => "edc";
        public virtual string USSCustomClassContentViewport => $"{UssCustomClass_Emc}-content__viewport";
        public virtual string USSCustomClassContentContainer => $"{UssCustomClass_Emc}-content__container";

        public Visual_C ContentVieport = new Visual_C { name = "content-viewport" };
        public Visual_C ContentContainer = new Visual_C { name = "content-container" };

        public override VisualElement DataContainer => ContentContainer;

        protected float m_animationTime;

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            ContentVieport.AddToClassList(USSCustomClassContentViewport);
            ContentContainer.AddToClassList(USSCustomClassContentContainer);
        }

        protected override void InitElement()
        {
            base.InitElement();
            ContentContainer.RegisterCallback<GeometryChangedEvent>(ce =>
            {
                if (ce.newRect.height.EqualsEpsilone(ce.oldRect.height)) return;

                if (!canRaiseAnimation) return;
                canRaiseAnimation = false;

                bool isAnimationIn = ce.newRect.height > ce.oldRect.height;
                float newHeight = ce.newRect.height;
                ContentVieport.AddAnimation
                (
                    this,
                    () => ContentVieport.style.height = m_lastHeight,
                    () => ContentVieport.style.height = newHeight,
                    "height",
                    isAnimationIn ? AnimationTimeIn : AnimationTimeOut,
                    callin: () => canRaiseAnimation = false,
                    callback: () =>
                    {
                        m_lastHeight = newHeight;
                        ContentVieport.style.height = StyleKeyword.Null;
                    },
                    callcancel: () => ContentVieport.style.height = StyleKeyword.Null
                );
            });

            ItemAdded += datum =>
            {
                canRaiseAnimation = true;
            };

            ItemRemoved += datum =>
            {
                canRaiseAnimation = true;
            };

            Add(ContentVieport);
            ContentVieport.Add(ContentContainer);
        }

        #region Implementation

        /// <summary>
        /// Last height of the <see cref="ContentVieport"/> after the animation
        /// </summary>
        protected float m_lastHeight;
        /// <summary>
        /// Whether or not the animation can be raised.
        /// </summary>
        protected bool canRaiseAnimation;

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
                    button.LocaliseLabel = $"item {datum}";

                    if (previewItem.Data.IndexOf(datum) == 0)
                    {
                        button.LocaliseText = $"delete last item";
                        button.Type = ButtonType.Danger;
                        button.clicked += removeClick;
                    }
                    else
                    {
                        button.LocaliseText = $"add new item";
                        button.Type = ButtonType.Primary;
                        button.clicked += addClick;
                    }
                };
                previewItem.UnbindItem = (datum, item) =>
                {
                    var button = item as umi3d.commonScreen.Displayer.Button_C;
                    button.LocaliseText = null;
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
