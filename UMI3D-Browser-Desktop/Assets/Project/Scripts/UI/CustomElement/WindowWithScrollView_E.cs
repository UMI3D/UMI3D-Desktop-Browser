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
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class WindowWithScrollView_E
    {
        public event Action OnCloseButtonPressed;

        protected ScrollView_E m_scrollView { get; set; } = null;
        protected Label_E m_label { get; set; } = null;
    }

    public partial class WindowWithScrollView_E
    {
        public void Adds(params Visual_E[] items)
        {
            m_scrollView.Adds(items);
        }

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
            new Button_E(closeButton, styleResourcePath, keys)
            {
                OnClicked = () => { OnCloseButtonPressed(); }
            };
        }

        public void SetVerticalScrollView(string svStyle, StyleKeys svKeys, string dcStyle, StyleKeys dcKeys, string dStyle, StyleKeys dKeys)
        {
            m_scrollView = new ScrollView_E(Root.Q("scrollViewContainer"), svStyle, svKeys)
                .SetVerticalDraggerContainerStyle(dcStyle, dcKeys)
                .SetVerticalDraggerStyle(dStyle, dKeys);
        }

        protected PopUpManipulator PopupManipulator()
                => new PopUpManipulator(Root);
    }

    public partial class WindowWithScrollView_E : Visual_E
    {
        public WindowWithScrollView_E(string visualResourcePath, string styleResourcePath, StyleKeys keys) :
            base(visualResourcePath, styleResourcePath, keys)
        { }
    }
}
