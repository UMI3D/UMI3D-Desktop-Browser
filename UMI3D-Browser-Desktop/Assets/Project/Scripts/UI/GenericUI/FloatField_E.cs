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
    public partial class FloatField_E
    {
        protected FloatField m_floatField => (FloatField)Root;
    }

    public partial class FloatField_E
    {
        public FloatField_E() :
            this(null, null)
        { }
        public FloatField_E(string styleResourcePath, StyleKeys keys) :
            this(new FloatField(), styleResourcePath, keys)
        { }
        public FloatField_E(FloatField floatField, string styleResourcePath, StyleKeys keys) :
            base(floatField, styleResourcePath, keys)
        { }
    }

    public partial class FloatField_E : AbstractBaseField_E<float>
    {
        protected override void Initialize()
        {
            base.Initialize();
        }
    }
}