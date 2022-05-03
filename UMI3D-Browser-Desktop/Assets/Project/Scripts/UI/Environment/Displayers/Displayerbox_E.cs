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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public enum DisplayerboxType { ToolboxesPopup, ParametersPopup }
    public partial class Displayerbox_E
    {
        private static VisualElement m_displayerbox
        {
            get
            {
                var displayerbox = new VisualElement();
                displayerbox.name = "displayerbox";
                return displayerbox;
            }
        }
        private static string m_displayerboxToolboxStyle = "UI/Style/Displayers/ToolboxDisplayerbox";
        private static string m_displayerboxParameterStyle = "UI/Style/Displayers/ParameterDisplayerbox";
        private static string GetDisplayerboxType(DisplayerboxType type)
        {
            switch (type)
            {
                case DisplayerboxType.ToolboxesPopup:
                    return m_displayerboxToolboxStyle;
                case DisplayerboxType.ParametersPopup:
                    return m_displayerboxParameterStyle;
                default:
                    throw new System.Exception();
            }
        }
    }

    public partial class Displayerbox_E
    {
        public Displayerbox_E(DisplayerboxType type) :
            base(m_displayerbox, GetDisplayerboxType(type), StyleKeys.DefaultBackgroundAndBorder)
        { }

        public void Add(params Displayer_E[] displayers)
        {
            foreach (Displayer_E displayer in displayers)
                Root.Add(displayer.Root);
        }

        public void Clear()
        {

        }
    }

    public partial class Displayerbox_E : View_E
    { }
}