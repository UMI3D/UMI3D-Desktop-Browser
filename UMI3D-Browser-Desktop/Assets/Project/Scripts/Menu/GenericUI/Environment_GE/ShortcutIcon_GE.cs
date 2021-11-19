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

namespace BrowserDesktop.UI.GenericElement
{
    public class ShortcutIcon_GE : GenericAndCustomElement
    {
        public new class UxmlFactory : UxmlFactory<ShortcutIcon_GE, UxmlTraits> { }

        private enum IconType
        {
            //H: 512px
            SQUARE, //W: 512px
            MOUSE, //W: 548px
            FN, //W: 721px
            CTRL, //W: 845px
            SHIFT, //W: 952px
        }

        private IconType iconType;

        /// <summary>
        /// The icon height in px.
        /// </summary>
        private float iconHeightPX = 25f;
        private float iconWidthPX;

        /// <summary>
        /// Set the size and the sprite of the icon.
        /// </summary>
        /// <param name="sprite">The sprite of the icon.</param>
        public ShortcutIcon_GE Setup(Sprite sprite)
        {
            Initialize();

            if (sprite == null)
            {
                this.style.backgroundImage = StyleKeyword.Auto;

                iconType = IconType.SQUARE;

                iconWidthPX = iconHeightPX;

                Debug.LogError($"Sprite missing");
            }
            else
            {
                this.style.backgroundImage = new StyleBackground(sprite);

                float width = sprite.rect.width;
                float height = sprite.rect.height;

                if (width <= 512f) iconType = IconType.SQUARE;
                else if (width <= 548f) iconType = IconType.MOUSE;
                else if (width <= 721f) iconType = IconType.FN;
                else if (width <= 845f) iconType = IconType.CTRL;
                else if (width <= 952f) iconType = IconType.SHIFT;

                iconWidthPX = (width / height) * iconHeightPX;
            }


            /*if (sprite.name.Length == 3)
            {
                this.style.borderBottomWidth = 0;
                this.style.borderRightWidth = 0;
                this.style.borderLeftWidth = 0;
                this.style.borderTopWidth = 0;
            }*/

            //OnApplyUserPreferences();

            return this;
        }

        public override void OnApplyUserPreferences()
        {
            if (!displayed) return;

            UserPreferences.UserPreferences.TextAndIconPref.ApplyIconPref(this, (iconType == IconType.MOUSE) ? $"shortcut-{IconType.MOUSE}" : $"shortcut-KEY", iconWidthPX, iconHeightPX);
        }


    }
}

