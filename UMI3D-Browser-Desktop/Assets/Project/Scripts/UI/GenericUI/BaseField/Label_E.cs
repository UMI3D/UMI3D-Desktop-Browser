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

namespace umi3dDesktopBrowser.uI.viewController
{
    public partial class Label_E : INotifyValueChanged<string>
    {
        public string value 
        { 
            get => m_label?.text; 
            set
            {
                if (value == m_label.text)
                    return;
                var previousValue = m_label.text;
                var (styleSO, _, _) = m_visualStyles[m_label];
                var newValue = m_styleApplicator.GetTextAfterFormatting(styleSO.TextFormat.NumberOfVisibleCharacter, value);
                m_label.text = newValue;
                OnValueChanged?.Invoke(previousValue, newValue);
            }
        }

        public void SetValueWithoutNotify(string newValue)
        {
            throw new NotImplementedException();
        }
    }

    public partial class Label_E
    {
        public event Action<string, string> OnValueChanged;

        protected Label m_label => (Label)Root;
    }

    public partial class Label_E
    {
        public Label_E(string text = "") :
            this(null, null, text)
        { }

        public Label_E(string styleResourcePath, StyleKeys keys, string text = "") :
            this(new Label(), styleResourcePath, keys, text)
        { }

        public Label_E(Label label, string styleResourcePath, StyleKeys keys, string text = "") :
            base(label, styleResourcePath, keys)
        {
            value = text;
        }
    }

    public partial class Label_E : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();
        }

    }
}