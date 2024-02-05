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
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    /// <summary>
    /// 2D Displayer for range input.
    /// </summary>
    public class SliderDisplayer : AbstractDisplayer, baseBrowser.Menu.IDisplayerElement
    {
        public string LabelText;
        public ElementCategory Category;
        public ElementSize Size = ElementSize.Medium;
        public ElementDirection Direction = ElementDirection.Leading;

        protected FloatRangeInputMenuItem menuItem;
        protected Slider_C slider;

        private void OnValidate()
        {
            if (slider == null) return;
            slider.Category = Category;
            slider.Size = Size;
            slider.DirectionDisplayer = Direction;
            slider.LocaliseLabel = LabelText;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="System.Exception"></exception>
        public override void SetMenuItem(AbstractMenuItem item)
        {
            if (item is FloatRangeInputMenuItem)
            {
                menuItem = item as FloatRangeInputMenuItem;
                LabelText = menuItem.ToString();
                InitAndBindUI();
            }
            else throw new System.Exception("MenuItem must be a Range Input");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void InitAndBindUI()
        {
            if (slider != null) return;

            slider = new Slider_C
            {
                Category = Category,
                Size = Size,
                DirectionDisplayer = Direction
            };
            slider.name = gameObject.name;
            slider.LocaliseLabel = LabelText;
            slider.lowValue = menuItem.min;
            slider.highValue = menuItem.max;
            slider.value = menuItem.value;
            slider.RegisterValueChangedCallback(SliderOnValueChanged);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            slider.UnregisterValueChangedCallback(SliderOnValueChanged);
            slider.RemoveFromHierarchy();
            StopAllCoroutines();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Display(bool forceUpdate = false) => slider.Display();
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Hide() => slider.Hide();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public VisualElement GetUXMLContent() => slider;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public override int IsSuitableFor(umi3d.cdk.menu.AbstractMenuItem menu)
        {
            return (menu is FloatRangeInputMenuItem) ? 2 : 0;
        }

        void SliderOnValueChanged(ChangeEvent<float> e)
        {
            var newValue = menuItem.continuousRange ? e.newValue : (int)e.newValue;
            menuItem.NotifyValueChange(newValue);
        }

        private void OnDestroy()
        {
            slider?.RemoveFromHierarchy();
        }
    }
}