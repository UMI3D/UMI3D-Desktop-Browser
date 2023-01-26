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
    public class ToggleDisplayer_C : Toggle_C, IDisplayer<BooleanInputMenuItem>
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public BooleanInputMenuItem DisplayerMenu { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void BindDisplayer()
        {
            label = DisplayerMenu.Name;
            this.RegisterValueChangedCallback(OnValueChanged);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void UnbindDisplayer()
        {
            label = null;
            this.UnregisterValueChangedCallback(OnValueChanged);
        }

        /// <summary>
        /// Notify that the value of this toggle changed.
        /// </summary>
        /// <param name="e"></param>
        public void OnValueChanged(ChangeEvent<bool> e)
        {
            if (e.previousValue == e.newValue) return;
            DisplayerMenu.NotifyValueChange(e.newValue);
        }
    }
}
