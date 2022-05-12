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
    public partial class Toolbox_E
    {
        protected Label_E m_title { get; set; } = null;
        protected ScrollView_E m_scrollView { get; set; } = null;
        protected VisualElement m_backward;
        protected VisualElement m_forward;

        public void SetName(string text)
        {
            if (text == null) m_title.Hide();
            else m_title.Display();
            m_title.value = text;
        }

        public void AddRange(params View_E[] items)
            => m_scrollView.AddRange(items);

        public static Toolbox_E NewMenuToolbox(string toolboxName, params ToolboxItem_E[] items)
        {
            var toolbox = new Toolbox_E("Box_p", items);
            toolbox.SetName(toolboxName);
            toolbox.m_backward.style.display = DisplayStyle.None;
            toolbox.m_forward.style.display = DisplayStyle.None;

            return toolbox;
        }

        public static Toolbox_E NewSubMenuToolbox(string toolboxName, params ToolboxItem_E[] items)
        {
            var toolbox = new Toolbox_E("Box_w1", items);
            toolbox.SetName(toolboxName);
            toolbox.m_scrollView.HSliderValueChanged += (value, low, high) =>
            {
                toolbox.m_backward.visible = (value > low) ? true : false;
                toolbox.m_forward.visible = (value < high) ? true : false;
            };

            return toolbox;
        }

        public static Toolbox_E NewWindowToolbox(string toolboxName, params ToolboxItem_E[] items)
        {
            var toolbox = new Toolbox_E("Box_mp", items);
            toolbox.SetName(toolboxName);
            toolbox.m_scrollView.HSliderValueChanged += (value, low, high) =>
            {
                toolbox.m_backward.visible = (value > low) ? true : false;
                toolbox.m_forward.visible = (value < high) ? true : false;
            };

            return toolbox;
        }
    }

    public partial class Toolbox_E : Box_E
    {
        protected Toolbox_E(string partialStylePath, params ToolboxItem_E[] items) :
            base("UI/UXML/Toolbox/Toolbox", partialStylePath, StyleKeys.Default_Bg_Border)
        {
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

            m_title = new Label_E(QR<Label>(), "TitleToolbox", StyleKeys.Text("primaryLight"));

            var scrollViewBox = new View_E("UI/UXML/horizontalScrollView", null, null);
            //scrollViewBox.InsertRootTo(QR("mainBox"));
            scrollViewBox.InsertRootTo(Root);
            m_backward = scrollViewBox.QR("backward");
            m_forward = scrollViewBox.QR("forward");

            m_scrollView = new ScrollView_E(scrollViewBox.QR<ScrollView>());
            m_scrollView.SetHBackwardButton("ButtonH", StyleKeys.Bg("backward"));
            m_scrollView.SetHForwarddButton("ButtonH", StyleKeys.Bg("forward"));
            m_backward.Insert(0, m_scrollView.HBackwardButton);
            m_forward.Insert(1, m_scrollView.HForwardButton);
        }
    }
}

