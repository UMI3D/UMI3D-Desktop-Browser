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
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class WindowWithScrollView_E
    {
        protected ScrollView_E m_scrollView { get; set; } = null;
    }

    public partial class WindowWithScrollView_E : AbstractWindow_E
    {
        public WindowWithScrollView_E(string visualResourcePath, string styleResourcePath, StyleKeys keys) :
            base(visualResourcePath, styleResourcePath, keys)
        { }

        public void Adds(params Visual_E[] items)
        {
            m_scrollView.Adds(items);
        }

        public void SetVerticalScrollView(string svStyle, StyleKeys svKeys, string dcStyle, StyleKeys dcKeys, string dStyle, StyleKeys dKeys)
        {
            m_scrollView = new ScrollView_E(Root.Q("scrollViewContainer"), svStyle, svKeys);
            m_scrollView.SetVerticalDraggerContainerStyle(dcStyle, dcKeys);
            m_scrollView.SetVerticalDraggerStyle(dStyle, dKeys);
        }
    }
}
