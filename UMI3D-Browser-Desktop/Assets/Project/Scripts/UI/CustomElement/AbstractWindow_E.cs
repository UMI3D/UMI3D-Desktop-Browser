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
    public abstract partial class AbstractWindow_E
    {
        public event Action CloseButtonPressed;

        protected Visual_E m_windowIcon { get; set; } = null;
        protected Label_E m_windowTopBar { get; set; } = null;
        protected Button_E m_closeButton { get; set; } = null;

        protected virtual string m_iconStyle => "UI/Style/Windows/Window_Icon";
        protected virtual string m_topBarStyle => "UI/Style/Windows/Window_Name";
        protected virtual string m_closeButtonBGStyle => "UI/Style/Windows/Window_CloseButtonBackground";
        protected virtual string m_closeButtonIconStyle => "UI/Style/Windows/Window_CloseButtonIcon";

        public void OnCloseButtonPressed()
            => CloseButtonPressed?.Invoke();

        public void SetWindowIcon(string styleResourcePath, StyleKeys keys, bool isDraggable)
        {
            if (m_windowIcon == null)
                m_windowIcon = new Visual_E(Root.Q("icon"));
            m_windowIcon.UpdateRootStyleAndKeysAndManipulator(styleResourcePath, keys, (isDraggable) ? PopupManipulator() : null);
        }

        public void SetTopBar(string name, string styleResourcePath, StyleKeys keys, bool isDraggable)
        {
            if (m_windowTopBar == null)
                m_windowTopBar = new Label_E(Root.Q<Label>("windowName"));
            m_windowTopBar.UpdateRootStyleAndKeysAndManipulator(styleResourcePath, keys, (isDraggable) ? PopupManipulator() : null);
            m_windowTopBar.value = name;
        }

        public void UpdateTopBarName(string name)
        {
            if (m_windowTopBar != null)
                m_windowTopBar.value = name;
        }

        public void SetCloseButton()
        {
            if (m_closeButton == null)
                m_closeButton = new Button_E(Root.Q<Button>("closeButton"));

            CloseButtonPressed += Hide;
            m_closeButton.Clicked += OnCloseButtonPressed;
        }

        protected PopUpManipulator PopupManipulator()
                => new PopUpManipulator(Root);

        /// <summary>
        /// Anime the VisualElement.
        /// </summary>
        /// <param name="vE"></param>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="durationMs"></param>
        /// <param name="fromStartToEnd"></param>
        /// <param name="animation"></param>
        protected virtual void Anime(VisualElement vE, float startValue, float endValue, int durationMs, bool fromStartToEnd, Action<VisualElement, float> animation)
        {
            Debug.LogWarning("Use of Unity experimental API. May not work in the future. (2021)");
            if (fromStartToEnd)
                vE.experimental.animation.Start(startValue, endValue, durationMs, animation);
            else
                vE.experimental.animation.Start(endValue, startValue, durationMs, animation);
        }
    }

    public abstract partial class AbstractWindow_E : Visual_E
    {
        public AbstractWindow_E(string visualResourcePath, string styleResourcePath, StyleKeys keys) :
            base(visualResourcePath, styleResourcePath, keys)
        { }
    }
}
