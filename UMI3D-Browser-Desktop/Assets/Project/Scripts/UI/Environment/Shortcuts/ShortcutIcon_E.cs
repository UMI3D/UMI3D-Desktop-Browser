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
    public partial class ShortcutIcon_E
    {
        private static string m_squareStyle = "UI/Style/Shortcuts/Shortcut_Square";
        private static string m_mouseStyle = "UI/Style/Shortcuts/Shortcut_Mouse";
        private static string m_fnStyle = "UI/Style/Shortcuts/Shortcut_FN";
        private static string m_ctrlStyle = "UI/Style/Shortcuts/Shortcut_CTRL";
        private static string m_shiftStyle = "UI/Style/Shortcuts/Shortcut_SHIFT";
        private static StyleKeys m_keys = new StyleKeys();
    }

    public partial class ShortcutIcon_E
    {
        public ShortcutIcon_E() :
            base(new VisualElement(), m_squareStyle, m_keys)
        { }

        public void Setup(Sprite sprite)
        {
            Root.style.backgroundImage = new StyleBackground(sprite);
            float width = sprite.rect.width;
            if (width <= 512f) AddVisualStyle(Root, m_squareStyle, m_keys);
            else if (width <= 548f) AddVisualStyle(Root, m_mouseStyle, m_keys);
            else if (width <= 721f) AddVisualStyle(Root, m_fnStyle, m_keys);
            else if (width <= 845f) AddVisualStyle(Root, m_ctrlStyle, m_keys);
            else if (width <= 952f) AddVisualStyle(Root, m_shiftStyle, m_keys);
        }
    }

    public partial class ShortcutIcon_E : Visual_E
    {
        
    }
}
