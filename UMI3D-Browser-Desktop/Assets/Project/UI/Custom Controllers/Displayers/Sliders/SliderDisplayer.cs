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

namespace umi3d.mobileBrowser.Displayer
{
    /// <summary>
    /// 2D Displayer for range input.
    /// </summary>
    public class SliderDisplayer : AbstractDisplayer, baseBrowser.Menu.IDisplayerElement
    {
        public string LabelText;
        public ElementCategory Category;
        public ElementSize Size;
        public ElemnetDirection Direction;

        protected FloatRangeInputMenuItem menuItem;
        protected Slider_C slider;


        ///// <summary>
        ///// Frame rate applied to message emission through network (high values can cause network flood).
        ///// </summary>
        //public float networkFrameRate = 30;

        ///// <summary>
        ///// Launched coroutine for network message sending (if any).
        ///// </summary>
        ///// <see cref="networkMessageSender"/>
        //protected Coroutine messageSenderCoroutine;

        //protected bool valueChanged = false;

        //protected IEnumerator networkMessageSender()
        //{
        //    while (true)
        //    {
        //        if (valueChanged)
        //        {
        //            var newValue = menuItem.continuousRange ? slider.value : (int)slider.value;
        //            NotifyValueChange(slider.value);
        //            valueChanged = false;
        //        }
        //        yield return new WaitForSeconds(1f / networkFrameRate);
        //    }
        //}

        //string FormatValue(float f)
        //{
        //    return string.Format("{0:###0.##}", f);
        //}

        private void OnValidate()
        {
            if (slider == null) return;
            slider.Set(Category, Size, Direction);
            slider.label = LabelText;
        }

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

        public void InitAndBindUI()
        {
            if (slider != null) return;

            slider = new Slider_C(Category, Size, Direction);
            slider.name = gameObject.name;
            slider.label = LabelText;
            slider.lowValue = menuItem.min;
            slider.highValue = menuItem.max;
            slider.value = menuItem.value;
            slider.RegisterValueChangedCallback(SliderOnValueChanged);
        }

        public override void Clear()
        {
            base.Clear();
            slider.UnregisterValueChangedCallback(SliderOnValueChanged);
            slider.RemoveFromHierarchy();
            StopAllCoroutines();
        }

        public override void Display(bool forceUpdate = false) => slider.Display();
        public override void Hide() => slider.Hide();

        public VisualElement GetUXMLContent() => slider;

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