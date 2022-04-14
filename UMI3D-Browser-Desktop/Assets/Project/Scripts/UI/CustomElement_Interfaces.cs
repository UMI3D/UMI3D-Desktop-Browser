/*
Copyright 2019 - 2021 Inetum

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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public interface IClickableElement
    {
        /// <summary>
        /// Action to perform when the element is clicked.
        /// </summary>
        public Action OnClicked { get; set; }
        /// <summary>
        /// State of the button.
        /// </summary>
        public bool IsOn { get; }
        /// <summary>
        /// Change the staate of the clickable element.
        /// </summary>
        /// <param name="value"></param>
        public void Toggle(bool value);
    }
}
