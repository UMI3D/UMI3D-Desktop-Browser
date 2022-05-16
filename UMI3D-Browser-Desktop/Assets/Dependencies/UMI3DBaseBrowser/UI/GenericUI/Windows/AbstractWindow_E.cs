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
using umi3DBrowser.UICustomStyle;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.ui.viewController
{
    public abstract partial class AbstractWindow_E
    {
        protected Label_E m_topBar { get; set; } = null;

        public virtual void SetTopBar(string name)
            => UpdateTopBarName(name);

        public void UpdateTopBarName(string name)
        {
            if (m_topBar != null)
                m_topBar.value = name;
        }
    }

    public abstract partial class AbstractWindow_E : View_E
    {
        public AbstractWindow_E(string partialVisualPath, string partialStylePath, StyleKeys keys) :
            base(partialVisualPath, partialStylePath, keys)
        { }

        protected override VisualElement GetVisualRoot(string resourcePath)
        {
            var path = (resourcePath == null) ? null : $"UI/UXML/Windows/{resourcePath}";
            return base.GetVisualRoot(path);
        }

        protected override CustomStyle_SO GetStyleSO(string resourcePath)
        {
            var path = (resourcePath == null) ? null : $"UI/Style/Windows/{resourcePath}";
            return base.GetStyleSO(path);
        }
    }
}
