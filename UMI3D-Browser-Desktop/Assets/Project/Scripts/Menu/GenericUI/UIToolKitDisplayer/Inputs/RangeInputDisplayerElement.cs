/*
Copyright 2019 Gfi Informatique

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
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    /// <summary>
    /// 2D Displayer for range input.
    /// </summary>
    public class RangeInputDisplayerElement : AbstractRangeInputDisplayer, IDisplayerElement
    {
        public VisualTreeAsset rangeInputVisualTreeAsset;

        /// <summary>
        /// Slider.
        /// </summary>
        VisualElement sliderContainer;
        Slider slider;
        TextField sliderValue;

        /// <summary>
        /// Frame rate applied to message emission through network (high values can cause network flood).
        /// </summary>
        public float networkFrameRate = 30;

        /// <summary>
        /// Launched coroutine for network message sending (if any).
        /// </summary>
        /// <see cref="networkMessageSender"/>
        protected Coroutine messageSenderCoroutine;

        protected bool valueChanged = false;

        protected IEnumerator networkMessageSender()
        {
            while (true)
            {
                if (valueChanged)
                {
                    var newValue = menuItem.continuousRange ? slider.value : (int)slider.value;
                    NotifyValueChange(slider.value);
                    valueChanged = false;
                }
                yield return new WaitForSeconds(1f / networkFrameRate);
            }
        }

        public override void Clear()
        {
            base.Clear();
            slider.UnregisterValueChangedCallback(SliderOnValueChanged);
            sliderValue.UnregisterCallback<FocusOutEvent>(SliderTextFieldOnValueChanged);

            sliderContainer.RemoveFromHierarchy();
            StopAllCoroutines();
        }

        /// <summary>
        /// Display the range input.
        /// </summary>
        public override void Display(bool forceUpdate = false)
        {
            InitAndBindUI();

            sliderContainer.style.display = DisplayStyle.Flex;

            if (menuItem.continuousRange)
            {
                slider.lowValue = menuItem.min;
                slider.highValue = menuItem.max;

            }
            else
            {
                slider.lowValue = 0;
                slider.highValue = (menuItem.max - menuItem.min) / menuItem.increment;

            }
            slider.label = menuItem.ToString();
            slider.value = menuItem.GetValue();
            sliderValue.value = FormatValue(slider.value);

            slider.RegisterValueChangedCallback(SliderOnValueChanged);
            sliderValue.RegisterCallback<FocusOutEvent>(SliderTextFieldOnValueChanged);

            if (sliderContainer.resolvedStyle.display == DisplayStyle.Flex)
                messageSenderCoroutine = StartCoroutine(networkMessageSender());
        }

        string FormatValue(float f)
        {
            return string.Format("{0:###0.##}", f);
        }

        void SliderOnValueChanged(ChangeEvent<float> e)
        {
            valueChanged = true;
            var newValue = menuItem.continuousRange ? e.newValue : (int)e.newValue;
            sliderValue.value = FormatValue(newValue);
        }

        void SliderTextFieldOnValueChanged(FocusOutEvent e)
        {
            float f = 0;
            if (float.TryParse(sliderValue.value, out f) && slider.value != f)
            {
                valueChanged = true;
                var newValue = menuItem.continuousRange ? f : (int)f;
                slider.value = newValue;
                sliderValue.value = FormatValue(slider.value);
            }
        }

        /// <summary>
        /// Hide the range input.
        /// </summary>
        public override void Hide()
        {
            sliderContainer.style.display = DisplayStyle.None;

            slider.UnregisterValueChangedCallback(SliderOnValueChanged);
            sliderValue.UnregisterCallback<FocusOutEvent>(SliderTextFieldOnValueChanged);

            StopCoroutine(messageSenderCoroutine);
        }

        public VisualElement GetUXMLContent()
        {
            InitAndBindUI();
            return sliderContainer;
        }

        public void InitAndBindUI()
        {
            if (sliderContainer == null)
            {
                sliderContainer = rangeInputVisualTreeAsset.CloneTree();
                sliderValue = sliderContainer.Q<TextField>();
                slider = sliderContainer.Q<Slider>();
            }
        }

        private void OnDestroy()
        {
            sliderContainer?.RemoveFromHierarchy();
        }
    }
}