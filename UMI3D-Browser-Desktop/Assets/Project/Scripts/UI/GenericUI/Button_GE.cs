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
using umi3DBrowser.UICustomStyle;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.uI.viewController
{
    /// <summary>
    /// A button is a clickable area. It can have an icon or/and a label.
    /// </summary>
    public partial class Button_GE
    {
        public Action OnClicked { get; set; } = () => { Debug.Log("<color=green>TODO: </color>" + $"Button_E clicked not implemented"); };
        /// <summary>
        /// State of the button.
        /// </summary>
        public bool IsOn { get; private set; } = false;
    }

    public partial class Button_GE
    {
        private Button m_button = null;
        private VisualElement m_icon = null;
        private VisualElement m_label = null;
        private StyleKeys m_iconOnKey = null;
        private StyleKeys m_iconOffKey = null;
        private StyleKeys m_currentIconKey = null;
        private StyleKeys m_labelOnKey = null;
        private StyleKeys m_labelOffKey = null;
        private StyleKeys m_currentLabelKey = null;
    }

    public partial class Button_GE
    {
        public Button_GE(VisualElement visual, bool isOn = false) :
            base(visual, null, null) 
        {
            IsOn = isOn;
        }
        public Button_GE(VisualElement visual, string styleResourcePath, StyleKeys formatAndStyleKeys, bool isOn = false) : 
            base(visual, styleResourcePath, formatAndStyleKeys)
        {
            IsOn = isOn;
        }
        public Button_GE(string visualResourcePath, string styleResourcePath, StyleKeys formatAndStyleKeys, bool isOn = false) :
            base(visualResourcePath, styleResourcePath, formatAndStyleKeys)
        {
            IsOn = isOn;
        }

        public Button_GE SetIcon(VisualElement icon, string styleResourcePath, StyleKeys iconOnKey, StyleKeys iconOffKey)
            => SetIcon(icon, styleResourcePath, iconOnKey, iconOffKey, IsOn);
        public Button_GE SetIcon(VisualElement icon, string styleResourcePath, StyleKeys iconKey)
            => SetIcon(icon, styleResourcePath, iconKey, null, true);

        private Button_GE SetIcon(VisualElement icon, string styleResourcePath, StyleKeys iconOnKey, StyleKeys iconOffKey, bool isOn)
        {
            CustomStyle_SO style_SO = GetStyleSO(styleResourcePath);
            if (style_SO == null || iconOnKey == null) throw new NullReferenceException($"CustomStyle_SO or iconStyleKeys is null.");
            IsOn = isOn;
            m_icon = icon;
            m_iconOnKey = iconOnKey;
            m_iconOffKey = iconOffKey;
            m_currentIconKey = IsOn ? m_iconOnKey : m_iconOffKey;
            AddVisualStyle(m_icon, style_SO, m_currentIconKey, null, false);
            return this;
        }

        public Button_GE SetLabel(TextElement label, string styleResourcePath, StyleKeys labelOnKey, StyleKeys labelOffKey)
            => SetLabel(label, styleResourcePath, labelOnKey, labelOffKey, IsOn);
        public Button_GE SetLabel(TextElement label, string styleResourcePath, StyleKeys labelKey)
            => SetLabel(label, styleResourcePath, labelKey, null, true);

        private Button_GE SetLabel(TextElement label, string styleResourcePath, StyleKeys labelOnKey, StyleKeys labelOffKey, bool isOn)
        {
            CustomStyle_SO style_SO = GetStyleSO(styleResourcePath);
            if (style_SO == null || labelOnKey == null) throw new NullReferenceException($"CustomStyle_SO or iconStyleKeys is null.");
            IsOn = isOn;
            m_label = label;
            m_labelOnKey = labelOnKey;
            m_labelOffKey = labelOffKey;
            m_currentLabelKey = IsOn ? m_labelOnKey : m_labelOffKey;
            AddVisualStyle(m_label, style_SO, m_currentLabelKey, null, false);
            return this;
        }

        public void Toggle(bool value)
        {
            IsOn = value;
            if (m_icon != null)
                UpdateVisualKeys(m_icon, (IsOn) ? m_iconOnKey : m_iconOffKey);
            if (m_label != null)
                UpdateVisualKeys(m_label, (IsOn) ? m_labelOnKey : m_labelOffKey);
        }
    }

    public partial class Button_GE : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();
            m_button = Root.Q<Button>();
            this.m_button.clicked += () => { this.OnClicked(); };
        }

        public override void Reset()
        {
            base.Reset();
            OnClicked = () => { Debug.Log("<color=green>TODO: </color>" + $"ToolboxItem clicked not implemented"); }; 
            IsOn = false;
            m_button = null;
            m_icon = null;
            m_label = null;
    }

        #region Set and Unset Icon

        //public Button_GE SetIcon(Texture2D icon)
        //{
        //    if (icon != null)
        //        m_button.style.backgroundImage = Background.FromTexture2D(icon);
        //    else
        //        m_button.style.backgroundImage = StyleKeyword.Auto;
        //    return this;
        //}

        //public Button_GE SetIcon(Sprite icon)
        //{
        //    if (icon != null)
        //        m_button.style.backgroundImage = Background.FromSprite(icon);
        //    else
        //        m_button.style.backgroundImage = StyleKeyword.Auto;
        //    return this;
        //}

        //public Button_GE UnSetIcon()
        //{
        //    m_button.style.backgroundImage = StyleKeyword.Auto;
        //    return this;
        //}

        #endregion


        public override void Remove()
        {
            base.Remove();
            m_button.clicked -= OnClicked;
        }
    }
}