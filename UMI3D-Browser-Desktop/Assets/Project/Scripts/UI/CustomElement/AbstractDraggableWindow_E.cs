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
    public partial class AbstractDraggableWindow_E
    {
        public event Action CloseButtonPressed;

        protected Icon_E m_windowIcon { get; set; } = null;
        protected Button_E m_closeButton { get; set; } = null;

        public void OnCloseButtonPressed()
            => CloseButtonPressed?.Invoke();

        public virtual void SetWindowIcon(StyleKeys keys)
        {
            if (m_windowIcon != null)
                return;
            
            m_windowIcon = new Icon_E(QR("icon"), "Square2", keys);
            m_windowIcon.UpdateRootManipulator(PopupManipulator());
        }

        public virtual void SetCloseButton()
        {
            if (m_closeButton != null)
                return;
            
            m_closeButton = new Button_E(QR<Button>("closeButton"), "RectangleVertical", StyleKeys.DefaultBackgroundAndBorder);
            var closeIcon = new Icon_E("Square", StyleKeys.DefaultBackground);
            m_closeButton.Add(closeIcon);
            LinkMouseBehaviourChanged(m_closeButton, closeIcon);
            m_closeButton.GetRootManipulator().ProcessDuringBubbleUp = true;

            CloseButtonPressed += Hide;
            m_closeButton.Clicked += OnCloseButtonPressed;
        }

        protected PopUpManipulator PopupManipulator()
                => new PopUpManipulator(Root);
    }

    public partial class AbstractDraggableWindow_E : AbstractWindow_E
    {
        public AbstractDraggableWindow_E(string visualResourcePath) :
            base(visualResourcePath, "Draggable", StyleKeys.DefaultBackground)
        { }

        public override void SetTopBar(string name)
        {
            if (m_topBar == null)
                m_topBar = new Label_E(QR<Label>("windowName"), "Title", StyleKeys.Default);
            m_topBar.UpdateRootManipulator(PopupManipulator());
            base.SetTopBar(name);
        }
    }
}
