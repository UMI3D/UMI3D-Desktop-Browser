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
    public abstract partial class AbstractElementWithIcon_E<E>
    {
        public E Element { get; protected set; } = default;
    }

    public abstract partial class AbstractElementWithIcon_E<E>
    {
        public Visual_E Icon { get; protected set; } = null;
        protected VisualElement m_icon { get; set; } = null;
    }

    public abstract partial class AbstractElementWithIcon_E<E>
    {
        public AbstractElementWithIcon_E(VisualElement visual, string styleResourcePath, StyleKeys keys) :
            base(visual, styleResourcePath, keys)
        { }
        public AbstractElementWithIcon_E(string visualResourcePath, string styleResourcePath, StyleKeys keys) :
            base(visualResourcePath, styleResourcePath, keys)
        { }

        public virtual void SetIcon(string styleResourcePath, StyleKeys keys)
        {
            if (Icon == null)
                Icon = new Visual_E(m_icon, styleResourcePath, keys);
            else
                throw new System.NotImplementedException();
        }
    }

    public abstract partial class AbstractElementWithIcon_E<E> : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();
            m_icon = Root.Q("icon");
        }

        public override void Reset()
        {
            base.Reset();
            Icon.Reset();
        }
    }
}