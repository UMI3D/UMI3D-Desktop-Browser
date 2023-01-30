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
using UnityEngine;

namespace umi3d.commonScreen.Displayer
{
    public class ButtonDisplayer_C : Button_C, IDisplayer<ButtonMenuItem>
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ButtonMenuItem DisplayerMenu { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void BindDisplayer()
        {
            text = DisplayerMenu.Name;
            ClickedDown += OnClickedDown;
            ClickedUp += OnClickedUp;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void UnbindDisplayer()
        {
            text = null;
            ClickedDown -= OnClickedDown;
            ClickedUp -= OnClickedUp;
        }

        /// <summary>
        /// Notify that this item is down.
        /// </summary>
        public void OnClickedDown() => DisplayerMenu.NotifyValueChange(true);
        /// <summary>
        /// Notify that this item is up.
        /// </summary>
        public void OnClickedUp() => DisplayerMenu.NotifyValueChange(false);
    }
}
