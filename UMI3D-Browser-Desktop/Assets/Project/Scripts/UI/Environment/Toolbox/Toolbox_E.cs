/*
Copyright 2019 Gfi Informatique

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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public enum ToolboxType { Pinned, SubPinned, Popup }
    public partial class Toolbox_E
    {
        protected Label_E m_title { get; set; } = null;
        protected ScrollView_E m_scrollView { get; set; } = null;

        private static string m_toolboxPinnedStyle => "Box1";
        private static string m_toolboxSubPinnedStyle => "BoxV1";
        private static string m_toolboxPopupStyle => "Box2";
        private static string GetToolboxType(ToolboxType type)
        {
            switch (type)
            {
                case ToolboxType.Pinned:
                    return m_toolboxPinnedStyle;
                case ToolboxType.SubPinned:
                    return m_toolboxSubPinnedStyle;
                case ToolboxType.Popup:
                    return m_toolboxPopupStyle;
                default:
                    throw new System.Exception();
            }
        }

        public void SetToolboxName(string text)
        {
            if (text == null) m_title.Hide();
            else m_title.Display();
            m_title.value = text;
        }

        public void AddRange(params View_E[] items)
            => m_scrollView.AddRange(items);
    }

    public partial class Toolbox_E : Box_E
    {
        public Toolbox_E(ToolboxType type = ToolboxType.Pinned) :
            this(null, type)
        { }
        public Toolbox_E(string toolboxName, ToolboxType type = ToolboxType.Pinned, params ToolboxItem_E[] items) :
            base("UI/UXML/Toolbox/Toolbox", GetToolboxType(type), StyleKeys.DefaultBackgroundAndBorder)
        {
            SetToolboxName(toolboxName);


            //if (type == ToolboxType.Pinned)
            //{
            //    backwardLayout.style.display = DisplayStyle.None;
            //    forwardLayout.style.display = DisplayStyle.None;
            //}

            if (items.Length > 0)
                AddRange(items);
        }

        public override V Q<V>(string name = null)
            => m_scrollView.Q<V>(name);
        public override void Add(View_E child)
            => m_scrollView.Add(child);
        public override void Insert(int index, View_E item)
            => m_scrollView.Insert(index, item);
        public override void Remove(View_E item)
            => m_scrollView.Remove(item);

        protected override void Initialize()
        {
            base.Initialize();

            m_title = new Label_E(QR<Label>(), "Corps", StyleKeys.Text("corps0"));

            var scrollViewBox = new View_E("UI/UXML/horizontalScrollView", null, null);
            scrollViewBox.InsertRootTo(QR("mainBox"));
            VisualElement backward = scrollViewBox.QR("backward");
            VisualElement forward = scrollViewBox.QR("forward");

            m_scrollView = new ScrollView_E(scrollViewBox.QR<ScrollView>());
            m_scrollView.HSliderValueChanged += (value, low, high) =>
            {
                backward.visible = (value > low) ? true : false;
                forward.visible = (value < high) ? true : false;
            };

            m_scrollView.SetHBackwardButton("ButtonH", StyleKeys.Bg("backward"));
            m_scrollView.SetHForwarddButton("ButtonH", StyleKeys.Bg("forward"));
            backward.Insert(0, m_scrollView.HBackwardButton);
            forward.Insert(1, m_scrollView.HForwardButton);
        }
    }
}

