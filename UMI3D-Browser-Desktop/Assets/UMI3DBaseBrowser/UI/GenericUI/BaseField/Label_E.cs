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
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class Label_E
    {
        public event Action<string, string> ValueChanged;

        protected Label m_label => (Label)Root;
        protected string m_rawValue { get; set; } = null;

        public void UpdateLabelKeys(StyleKeys keys)
            => UpdateKeys(m_label, keys);
    }

    public partial class Label_E : INotifyValueChanged<string>
    {
        public string value
        {
            get => m_label?.text;
            set
            {
                m_rawValue = value;
                var previousValue = m_label.text;
                var styleSO = GetVisualStyle(m_label);
                if (styleSO == null) m_label.text = value;
                else m_label.text = GetTextAfterFormatting(styleSO.TextFormat.NumberOfVisibleCharacter, value);
                ValueChanged?.Invoke(previousValue, m_label.text);
            }
        }

        public void SetValueWithoutNotify(string newValue)
        {
            throw new NotImplementedException();
        }
    }

    public partial class Label_E : View_E
    {
        public Label_E() :
            this("")
        { }
        public Label_E(string text) :
            this(null, null, text)
        { }
        public Label_E(string partialStylePath, StyleKeys keys, string text = null) :
            this(new Label(), partialStylePath, keys, text)
        { }
        public Label_E(Label label) :
            this(label, null, null, null)
        { }
        public Label_E(Label label, string partialStylePath, StyleKeys keys, string text = null) :
            base(label, partialStylePath, keys)
        {
            if (text != null) value = text;
            else value = m_label.text;
        }

        protected override void ApplyStyle(CustomStyle_SO styleSO, StyleKeys keys, IStyle style, MouseBehaviour mouseBehaviour)
        {
            base.ApplyStyle(styleSO, keys, style, mouseBehaviour);
            if (m_rawValue != null) value = m_rawValue;
        }

        protected override CustomStyle_SO GetStyleSO(string resourcePath)
        {
            var path = (resourcePath == null) ? null : $"UI/Style/Labels/{resourcePath}";
            return base.GetStyleSO(path);
        }
    }
}