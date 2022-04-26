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
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public abstract partial class AbstractWindow_E
    {
        public event Action OnCloseButtonPressed;

        protected Label_E m_label { get; set; } = null;
        protected ButtonWithIcon_E m_closeButton { get; set; } = null;

        protected virtual string m_iconStyle => "UI/Style/Windows/Window_Icon";
        protected virtual string m_windowNameStyle => "UI/Style/Windows/Window_Name";
        protected virtual string m_closeButtonBGStyle => "UI/Style/Windows/Window_CloseButtonBackground";
        protected virtual string m_closeButtonIconStyle => "UI/Style/Windows/Window_CloseButtonIcon";
    }

    public abstract partial class AbstractWindow_E : Visual_E
    {
        public AbstractWindow_E(string visualResourcePath, string styleResourcePath, StyleKeys keys) :
            base(visualResourcePath, styleResourcePath, keys)
        { }

        public void SetIcon(string styleResourcePath, StyleKeys keys)
        {
            VisualElement icon = Root.Q("icon");
            AddVisualStyle(icon, styleResourcePath, keys, PopupManipulator());
        }

        public void SetTopBar(string name, string styleResourcePath, StyleKeys keys)
        {
            m_label = new Label_E(Root.Q<Label>("windowName"), styleResourcePath, keys);
            m_label.value = name;
            m_label.UpdateVisualManipulator(PopupManipulator());
        }

        public void SetTopBarName(string name)
        {
            if (m_label != null)
                m_label.value = name;
        }

        public void SetCloseButton(string styleResourcePath, StyleKeys keys)
        {
            Button closeButton = Root.Q<Button>("closeButton");
            OnCloseButtonPressed += Hide;
            if (m_closeButton == null)
                m_closeButton = new ButtonWithIcon_E(closeButton);
            m_closeButton.SetButton(styleResourcePath, keys, () => OnCloseButtonPressed());
        }
        public void SetCloseButton(string butttonStyleResourcePath, StyleKeys buttonKeys, string iconStyleResourcePath, StyleKeys iconKeys)
        {
            SetCloseButton(butttonStyleResourcePath, buttonKeys);
            m_closeButton.SetIcon(iconStyleResourcePath, iconKeys);
            m_closeButton.ApplyButtonStyleWithIcon();
        }

        protected PopUpManipulator PopupManipulator()
                => new PopUpManipulator(Root);
    }
}
