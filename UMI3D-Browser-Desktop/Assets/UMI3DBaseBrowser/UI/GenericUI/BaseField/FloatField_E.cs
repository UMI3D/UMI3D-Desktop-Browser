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
//using UnityEditor.UIElements;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.ui.viewController
{
    public partial class FloatField_E
    {
        //protected FloatField m_floatField => (FloatField)Root;
        protected TextField m_floatField => (TextField)Root;
    }

    public partial class FloatField_E
    {
        public FloatField_E() :
            this(null, null)
        { }
        public FloatField_E(string styleResourcePath, StyleKeys keys) :
            this(new TextField(), styleResourcePath, keys)
        { }
        public FloatField_E(TextField floatField, string styleResourcePath, StyleKeys keys) :
            base(floatField, styleResourcePath, keys)
        { }
    }

    public partial class FloatField_E : AbstractBaseField_E<string>
    {
        #region To be changed when floatField will be use in runtime
        public override string value 
        {
            get => m_field.value;
            set
            {
                if (float.TryParse(value, out float f))
                {
                    m_field.value = value;
                }
            }
        }

        public override void SetValueWithoutNotify(string newValue)
        {
            if (float.TryParse(newValue, out float f))
                base.SetValueWithoutNotify(newValue);
        }

        public static bool TryConvertToFloat(string stringValue, out float floatValue)
        {
            CultureInfo provider = new CultureInfo("en-US");
            return float.TryParse(stringValue, System.Globalization.NumberStyles.Float, provider, out floatValue);
        }
        #endregion
    }
}