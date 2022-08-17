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
using umi3d.baseBrowser.Menu;
using umi3d.baseBrowser.ui.viewController;
using umi3d.cdk.menu;
using UnityEngine.UIElements;

namespace umi3d.DesktopBrowser.menu.Displayer
{
    public partial class WindowStringInputDisplayer
    {
        private TextField_E m_textField { get; set; } = null;
    }

    public partial class WindowStringInputDisplayer : IDisplayerElement
    {
        public override void InitAndBindUI()
        {
            string UXMLPath = "UI/UXML/Displayers/textFieldInputDisplayer";
            Displayer = new View_E(UXMLPath, s_displayerStyle, null);

            base.InitAndBindUI();
        }
    }

    public partial class WindowStringInputDisplayer : AbstractWindowInputDisplayer
    {
        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            InitAndBindUI();
            if (menu is TextInputMenuItem textMenu)
            {
                string textFieldStylePath = "UI/Style/Displayers/InputTextField";
                m_textField = new TextField_E(Displayer.Root.Q<TextField>(), textFieldStylePath, StyleKeys.DefaultBackground);
                string textInputStyle = "UI/Style/Displayers/InputTextField_Input";
                m_textField.SetTextInputStyle(textInputStyle, StyleKeys.DefaultText);
                if (textMenu.dto.privateParameter) m_textField.TextField.isPasswordField = true;

                m_textField.value = textMenu.GetValue();
                m_textField.ValueChanged += (_, newValue) => textMenu.NotifyValueChange(newValue);
            }
            else throw new System.Exception("MenuItem must be a TextInputMenuItem");
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
            => (menu is TextInputMenuItem) ? 2 : 0;

        public override void Clear()
        {
            base.Clear();
        }
    }
}