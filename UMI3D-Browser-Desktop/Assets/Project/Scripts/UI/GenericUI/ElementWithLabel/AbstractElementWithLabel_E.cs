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
using System.Collections.Generic;
using umi3DBrowser.UICustomStyle;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.uI.viewController
{
    public abstract partial class AbstractElementWithLabel_E<E>
    {
        public abstract E Element { get; protected set; }
    }

    public abstract partial class AbstractElementWithLabel_E<E>
    {
        protected Label m_label { get; set; } = null;
        public Label_E Label { get; protected set; } = null;
    }

    public abstract partial class AbstractElementWithLabel_E<E>
    {
        public AbstractElementWithLabel_E(string visualResourcePath, string styleResourcePath, StyleKeys keys) :
            base(visualResourcePath, styleResourcePath, keys)
        { }

        public virtual void SetLabel(string styleResourcePath, StyleKeys keys)
        {
            if (Label == null)
                Label = new Label_E(m_label, styleResourcePath, keys);
            else
                throw new System.NotImplementedException();
        }
            //=> AddVisualStyle(m_label, styleResourcePath, keys);
    }

    public abstract partial class AbstractElementWithLabel_E<E> : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();
            m_label = Root.Q<Label>();
        }

        public override void Reset()
        {
            base.Reset();
            Label.Reset();
        }
    }
}