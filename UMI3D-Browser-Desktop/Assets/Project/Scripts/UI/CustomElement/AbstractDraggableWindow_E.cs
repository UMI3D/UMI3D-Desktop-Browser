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
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class AbstractDraggableWindow_E
    {
        public event Action CloseButtonPressed;

        protected VisualElement m_topBox { get; set; } = null;
        protected VisualElement m_mainBox { get; set; } = null;
        protected VisualElement m_bottomBox { get; set; } = null;

        protected Icon_E m_windowIcon { get; set; } = null;
        protected Button_E m_closeButton { get; set; } = null;

        protected float m_rootWidth => Root.resolvedStyle.width;
        protected float m_rootHeight => Root.resolvedStyle.height;

        protected float m_pinnableLimit = 10f;
        protected bool m_isPinned
            => Root.resolvedStyle.left == 0f || Root.resolvedStyle.left == Screen.width - m_rootWidth / 2f;
        protected bool m_canBePinned
            => Root.resolvedStyle.left <= m_pinnableLimit || Root.resolvedStyle.left >= Screen.width - m_pinnableLimit - m_rootWidth;

        public void OnCloseButtonPressed()
            => CloseButtonPressed?.Invoke();

        public virtual void SetWindowIcon(StyleKeys keys)
        {
            if (m_windowIcon != null)
                return;
            
            m_windowIcon = new Icon_E(QR("icon"), "Square2", keys);
            m_windowIcon.Root.AddManipulator(GetNewWindowManipulator());
        }

        public virtual void SetCloseButton()
        {
            if (m_closeButton != null)
                return;
            
            m_closeButton = new Button_E(QR<Button>("closeButton"), "Close", StyleKeys.Default_Bg_Border);
            var closeIcon = new Icon_E("Square", StyleKeys.DefaultBackground);
            m_closeButton.Add(closeIcon);
            LinkMouseBehaviourChanged(m_closeButton, closeIcon);
            m_closeButton.GetRootManipulator().ProcessDuringBubbleUp = true;

            CloseButtonPressed += Hide;
            m_closeButton.Clicked += OnCloseButtonPressed;
        }

        protected WindowManipulator GetNewWindowManipulator()
        {
            var manipulator = new WindowManipulator(Root);
            manipulator.MouseUp += () =>
            {
                bool shouldPin()
                => !m_isPinned && m_canBePinned;

                if (shouldPin()) UIManager.StartCoroutine(Pin());
            };
            return manipulator;
        }

        protected IEnumerator Pin()
        {
            yield return new WaitUntil(() => m_rootWidth > 0f && m_rootHeight > 0f);
            if (Root.resolvedStyle.left <= m_pinnableLimit)
                Root.style.left = 0f;
            else
            {
                Root.style.left = StyleKeyword.Auto;
                Root.style.right = 0f;
            }
        }
    }

    public partial class AbstractDraggableWindow_E : AbstractWindow_E
    {
        public AbstractDraggableWindow_E(string partialVisualPath) :
            base(partialVisualPath, "Draggable", StyleKeys.DefaultBackground)
        { }

        public override void SetTopBar(string name)
        {
            if (m_topBar == null)
                m_topBar = new Label_E(QR<Label>("windowName"), "TitleDraggableWindow", StyleKeys.Default);
            m_topBar.Root.AddManipulator(GetNewWindowManipulator());
            base.SetTopBar(name);
        }

        protected override void Initialize()
        {
            base.Initialize();

            m_topBox = QR("topBox");
            m_mainBox = QR("mainBox");
            m_bottomBox = QR("bottomBox");
        }
    }
}
