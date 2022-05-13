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
using BrowserDesktop.Menu;
using umi3d.baseBrowser.ui.viewController;
using umi3d.cdk.menu.view;
using UnityEngine.UIElements;

namespace umi3d.DesktopBrowser.menu.Displayer
{
    public abstract partial class AbstractWindowInputDisplayer
    {
        public View_E Displayer { get; protected set; } = null;

        protected static string s_displayerStyle = "UI/Style/Displayers/Displayer";
        protected Label_E m_label { get; set; } = null;

        private void OnDestroy()
        {
            Displayer.RemoveRootFromHierarchy();
        }
    }

    public partial class AbstractWindowInputDisplayer : IDisplayerElement
    {
        public VisualElement GetUXMLContent()
            => Displayer.Root;

        public virtual void InitAndBindUI()
        {
            m_label = new Label_E(Displayer.Root.Q<Label>(), "Corps", StyleKeys.Text("secondaryLight"));
            m_label.value = menu.Name;
            Displayer.Add(m_label);
        }
    }

    public partial class AbstractWindowInputDisplayer : AbstractDisplayer
    {
        public override void Display(bool forceUpdate = false)
        {
            gameObject.SetActive(true);
            Displayer.Display();
        }

        public override void Hide()
        {
            Displayer.Hide();
            gameObject.SetActive(false);
        }

        public override void Clear()
        {
            base.Clear();
            Displayer.Reset();
        }
    }
}