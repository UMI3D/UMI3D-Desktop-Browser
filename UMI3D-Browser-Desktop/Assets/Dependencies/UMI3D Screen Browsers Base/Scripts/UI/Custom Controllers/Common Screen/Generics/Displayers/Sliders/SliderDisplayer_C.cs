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
using umi3d.cdk.menu;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class SliderDisplayer_C : Slider_C, IDisplayer<FloatRangeInputMenuItem>
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public FloatRangeInputMenuItem DisplayerMenu { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void BindDisplayer()
        {
            LocaliseLabel = DisplayerMenu.Name;
            lowValue = DisplayerMenu.min;
            highValue = DisplayerMenu.max;
            value = DisplayerMenu.value;
            this.RegisterValueChangedCallback(SliderOnValueChanged);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void UnbindDisplayer()
        {
            LocaliseLabel = null;
            lowValue = 0;
            highValue = 0;
            value = 0;
            this.UnregisterValueChangedCallback(SliderOnValueChanged);
        }

        /// <summary>
        /// Notify that the value of this slider changed.
        /// </summary>
        /// <param name="e"></param>
        public void SliderOnValueChanged(ChangeEvent<float> e)
        {
            var newValue = DisplayerMenu.continuousRange ? e.newValue : (int)e.newValue;
            DisplayerMenu.NotifyValueChange(newValue);
        }
    }
}
