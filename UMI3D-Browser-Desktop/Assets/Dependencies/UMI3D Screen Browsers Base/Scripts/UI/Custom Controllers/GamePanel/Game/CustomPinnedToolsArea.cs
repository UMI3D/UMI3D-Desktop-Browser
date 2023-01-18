/*
Copyright 2019 - 2023 Inetum

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
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.menu;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomPinnedToolsArea : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlEnumAttributeDescription<ScrollViewMode> m_mode = new UxmlEnumAttributeDescription<ScrollViewMode>
        {
            name = "mode",
            defaultValue = ScrollViewMode.Vertical
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as CustomPinnedToolsArea;

            custom.Set
                (
                    m_mode.GetValueFromBag(bag, cc)
                );
        }
    }

    public virtual ScrollViewMode Mode
    {
        get => SDC.Mode;
        set
        {
            RemoveFromClassList(USSCustomClassMode(SDC.Mode));
            AddToClassList(USSCustomClassMode(value));
            SDC.Mode = value;

        }
    }

    public virtual string StyleSheetGamePath => $"USS/game";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/pinnedTools";
    public virtual string USSCustomClassName => "pinned__tools__area";
    public virtual string USSCustomClassMode(ScrollViewMode mode) => $"{USSCustomClassName}-{mode}".ToLower();
    public virtual string USSCustomClassSDC => $"{USSCustomClassName}-sdc";

    public CustomScrollableDataCollection<AbstractMenuItem> SDC;

    protected bool m_hasBeenInitialized;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetGamePath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);
        SDC.AddToClassList(USSCustomClassSDC);

        SDC.BindItem = (datum, item) =>
        {
            if (datum is MenuItem menuItem) UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"menu item = {menuItem.Name}");
            else if (datum is Menu menu) UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"menu {menu.Name}");
        };
        SDC.Size = 200f;

        Add(SDC);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual void Set() => Set(ScrollViewMode.Vertical);

    /// <summary>
    /// Set this UI element.
    /// </summary>
    /// <param name="mode"></param>
    public virtual void Set(ScrollViewMode mode)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Mode = mode;
    }
}
