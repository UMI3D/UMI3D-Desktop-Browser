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
using System;
using umi3d.cdk.menu;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine.UIElements;

namespace umi3d.DesktopBrowser.menu.Displayer
{
    public partial class WindowButtonInputDisplayer
    {
        private Button_E m_button { get; set; } = null;
    }

    public partial class WindowButtonInputDisplayer : IDisplayerElement
    {
        public override void InitAndBindUI()
        {
            base.InitAndBindUI();
            string UXMLPath = "UI/UXML/Displayers/buttonInputDisplayer";
            m_input = new Visual_E(UXMLPath, null, null);

            string labelStylePath = "UI/Style/Displayers/DisplayerLabel";
            StyleKeys labelKeys = new StyleKeys("", "", null);
            m_label = new Label_E(m_input.Root.Q<Label>(), labelStylePath, labelKeys);
            m_label.value = menu.Name;
            m_input.Add(m_label);
            
            Displayer.AddDisplayer(m_input.Root);
        }
    }

    public partial class WindowButtonInputDisplayer : AbstractWindowInputDisplayer
    {
        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            InitAndBindUI();
            if (menu is ButtonMenuItem buttonMenu)
            {
                string buttonStylePath = "UI/Style/Displayers/InputButton";
                StyleKeys buttonKeys = new StyleKeys("", "", null);
                m_button = new Button_E(m_input.Root.Q<Button>(), buttonStylePath, buttonKeys);
                m_button.Text = menu.Name;
                m_input.Add(m_button);

                m_button.Clicked += () => { buttonMenu.NotifyValueChange(!buttonMenu.GetValue()); }; 
            }
            else if (menu is HoldableButtonMenuItem holdableButtonMenu)
            {
                string buttonStylePath = "UI/Style/Displayers/InputButton";
                StyleKeys buttonKeys = new StyleKeys("", "", null);
                m_button = new Button_E(m_input.Root.Q<Button>(), buttonStylePath, buttonKeys);
                m_button.Text = menu.Name;
                m_input.Add(m_button);

                m_button.ClickedDown += () => holdableButtonMenu.NotifyValueChange(true);
                m_button.ClickedUp += () => holdableButtonMenu.NotifyValueChange(false);
            }
            else if (menu is BooleanInputMenuItem toggleMenu)
            {
                string toggleStylePath = "UI/Style/Displayers/InputToggle";
                StyleKeys toggleOnKeys = new StyleKeys(null, "toggleOn", null);
                StyleKeys toggleOffKeys = new StyleKeys(null, "toggleOff", null);
                m_button = new Button_E(m_input.Root.Q<Button>(), toggleStylePath, toggleOnKeys, toggleOffKeys, toggleMenu.GetValue());
                m_button.Text = null;

                m_button.Clicked += () =>
                {
                    bool newValue = !m_button.IsOn;
                    m_button.Toggle(newValue);
                    toggleMenu.NotifyValueChange(newValue);
                };
            }
            else
                throw new System.Exception("MenuItem must be a ButtonMenuItem or a HoldableButtonMenuItem");
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
            => (menu is ButtonMenuItem || menu is HoldableButtonMenuItem || menu is BooleanInputMenuItem) ? 2 : 0;

        public override void Clear()
        {
            base.Clear();
        }
    }
}