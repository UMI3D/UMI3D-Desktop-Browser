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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public interface ISingleUI
    { }

    public interface IClickableElement
    {
        /// <summary>
        /// event performed when the element is clicked.
        /// </summary>
        public event Action Clicked;

        public void ResetClickedEvent();
        public void OnClicked();
    }

    public interface IHoldableElement
    {
        public event Action ClickedDown;
        public event Action ClickedUp;
        public bool IsPressed { get; }

        public void OnClickedDown();
        public void OnClickedUp();
        public void SetHoldableButton();
        public void UnSetHoldableButton();
    }

    public interface IStateCustomisableElement
    {
        /// <summary>
        /// State of the element.
        /// </summary>
        public bool IsOn { get; }
        public Dictionary<View_E, (StyleKeys, StyleKeys)> StateKeys { get; }

        /// <summary>
        /// Change the state of the element.
        /// </summary>
        /// <param name="value"></param>
        public void Toggle(bool value);
        public void AddStateKeys(View_E visual, string styleResourcePath, StyleKeys on, StyleKeys off);
        public void UpdateStateKeys(View_E view, StyleKeys on, StyleKeys off);
    }
}
