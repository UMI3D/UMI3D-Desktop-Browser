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

public class ShortcutIconElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ShortcutIconElement, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits { }

    /// <summary>
    /// The icon height in px.
    /// </summary>
    private int iconHeightPX = 25;

    /// <summary>
    /// Set the size and the sprite of the icon.
    /// </summary>
    /// <param name="sprite">The sprite of the icon.</param>
    public void Setup(Sprite sprite)
    {
        if (sprite == null) return;
        float height = sprite.rect.height;
        float width = sprite.rect.width;

        int iconWidthPX = (int) ((width / height) * (float) iconHeightPX);

        this.style.height = iconHeightPX;
        this.style.width = iconWidthPX;

        this.style.backgroundImage = new StyleBackground(sprite);
        if (sprite.name.Length == 3)
        {
            this.style.borderBottomWidth = 0;
            this.style.borderRightWidth = 0;
            this.style.borderLeftWidth = 0;
            this.style.borderTopWidth = 0;
        }
    }
}
