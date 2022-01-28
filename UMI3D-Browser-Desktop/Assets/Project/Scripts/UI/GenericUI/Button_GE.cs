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
        public Action OnClicked { get; set; } = () => { Debug.Log("<color=green>TODO: </color>" + $"ToolboxItem clicked not implemented"); };
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
        private FormatAndStyleKeys m_iconOnKey = null;
        private FormatAndStyleKeys m_iconOffKey = null;
        private FormatAndStyleKeys m_currentIconKey = null;
        private FormatAndStyleKeys m_labelOnKey = null;
        private FormatAndStyleKeys m_labelOffKey = null;
        private FormatAndStyleKeys m_currentLabelKey = null;
    }

    public partial class Button_GE
    {
        public Button_GE(VisualElement visual) :
            base(visual, null, null) { }
        public Button_GE(VisualElement visual, string styleResourcePath, FormatAndStyleKeys formatAndStyleKeys) : 
            base(visual, styleResourcePath, formatAndStyleKeys) { }
        public Button_GE(string visualResourcePath, string styleResourcePath, FormatAndStyleKeys formatAndStyleKeys) :
            base(visualResourcePath, styleResourcePath, formatAndStyleKeys) { }

        public Button_GE SetIcon(VisualElement icon, string styleResourcePath, FormatAndStyleKeys iconOnKey, FormatAndStyleKeys iconOffKey, bool isOn = false)
        {
            CustomStyle_SO style_SO = GetStyleSO(styleResourcePath);
            if (style_SO == null || iconOnKey == null || iconOffKey == null) throw new NullReferenceException($"CustomStyle_SO or iconStyleKeys is null.");
            m_iconOnKey = iconOnKey;
            m_iconOffKey = iconOffKey;
            m_currentIconKey = isOn ? m_iconOnKey : m_iconOffKey;
            AddVisualStyle(icon, style_SO, m_currentIconKey, false);
            m_icon = icon;
            return this;
        }

        public Button_GE SetLabel(VisualElement label, string styleResourcePath, FormatAndStyleKeys labelOnKey, FormatAndStyleKeys labelOffKey, bool isOn = false)
        {
            CustomStyle_SO style_SO = GetStyleSO(styleResourcePath);
            if (style_SO == null || labelOnKey == null || labelOffKey == null) throw new NullReferenceException($"CustomStyle_SO or iconStyleKeys is null.");
            m_labelOnKey = labelOnKey;
            m_labelOffKey = labelOffKey;
            m_currentLabelKey = isOn ? m_iconOnKey : m_iconOffKey;
            AddVisualStyle(label, style_SO, m_currentLabelKey, false);
            m_label = label;
            return this;
        }

        public void Toggle(bool value)
        {
            IsOn = value;
            if (m_icon != null)
                UpdateVisualStyle(m_icon, (IsOn) ? m_iconOnKey : m_iconOffKey);
            if (m_label != null)
                UpdateVisualStyle(m_label, (IsOn) ? m_labelOnKey : m_labelOffKey);
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