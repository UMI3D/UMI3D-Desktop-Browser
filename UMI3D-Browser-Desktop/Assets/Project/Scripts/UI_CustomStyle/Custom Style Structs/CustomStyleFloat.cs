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
using System.Collections.Generic;
using UnityEngine;

namespace Browser.UICustomStyle
{
    [Serializable]
    public struct CustomStyleFloat : ICustomStyleValue<CustomStyleKeyword, float>
    {
        public CustomStyleFloat(float v)
        {
            m_value = v;
            m_keyword = CustomStyleKeyword.Variable;
        }
        public CustomStyleFloat(CustomStyleKeyword keyword)
        {
            m_keyword = keyword;
            m_value = 0f;
        }
        public CustomStyleFloat(CustomStyleKeyword keyword, float v)
        {
            m_keyword = keyword;
            m_value = v;
        }

        [SerializeField]
        private CustomStyleKeyword m_keyword;
        [SerializeField]
        private float m_value;

        public CustomStyleKeyword Keyword { get => m_keyword; set => m_keyword = value; }
        public float Value { get => m_value; set => m_value = value; }

        public override string ToString()
        {
            return $"CustomStyleFloat[Keyword=[{m_keyword}], Value=[{m_value}]";
        }

        //public bool Equals(StyleFloat other);
        //public override bool Equals(object obj);
        //public override int GetHashCode();
        //public override string ToString();

        //public static bool operator ==(StyleFloat lhs, StyleFloat rhs);
        //public static bool operator !=(StyleFloat lhs, StyleFloat rhs);

        //public static implicit operator StyleFloat(StyleKeyword keyword);
        //public static implicit operator StyleFloat(float v);
    }

    [Serializable]
    public struct CustomStylePXAndPercentFloat : ICustomStyleValue<CustomStyleKeyword, float>
    {
        public CustomStylePXAndPercentFloat(float v) : this()
        {
            m_value = v;
            m_keyword = CustomStyleKeyword.Variable;
        }
        public CustomStylePXAndPercentFloat(CustomStyleKeyword keyword) : this()
        {
            m_keyword = keyword;
        }
        public CustomStylePXAndPercentFloat(CustomStyleKeyword keyword, float v) : this()
        {
            m_keyword = keyword;
            m_value = v;
        }

        [SerializeField]
        private CustomStyleKeyword m_keyword;
        [SerializeField]
        private CustomStyleValueMode m_valueMode;
        [SerializeField]
        private float m_value;

        public CustomStyleKeyword Keyword { get => m_keyword; set => m_keyword = value; }
        public CustomStyleValueMode ValueMode => m_valueMode;
        public float Value { get => m_value; set => m_value = value; }

        public override string ToString()
        {
            return $"CustomStyleFloat[Keyword=[{m_keyword}],ValueMode=[{m_valueMode}] Value=[{m_value}]";
        }
    }

    [Serializable]
    public struct CustomStylePercentFloat : ICustomStyleValue<CustomStyleKeyword, float>
    {
        public CustomStylePercentFloat(float v) : this()
        {
            m_value = v;
            m_keyword = CustomStyleKeyword.Variable;
        }
        public CustomStylePercentFloat(CustomStyleKeyword keyword) : this()
        {
            m_keyword = keyword;
        }
        public CustomStylePercentFloat(CustomStyleKeyword keyword, float v) : this()
        {
            m_keyword = keyword;
            m_value = v;
        }

        [SerializeField]
        private CustomStyleKeyword m_keyword;
        [SerializeField]
        private float m_value;

        public CustomStyleKeyword Keyword { get => m_keyword; set => m_keyword = value; }
        public float Value { get => m_value; set => m_value = value; }

        public override string ToString()
        {
            return $"CustomStyleFloat[Keyword=[{m_keyword}], Value=[{m_value}]";
        }
    }
}

