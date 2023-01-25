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

    /// <summary>
    /// Mode of the Scrollview (Vertical, Horizontal, Both).
    /// </summary>
    public virtual ScrollViewMode Mode
    {
        get => SDC.Mode;
        set
        {
            RemoveFromClassList(USSCustomClassMode(SDC.Mode));
            AddToClassList(USSCustomClassMode(value));
            SDC.Mode = value;
            ModeUpdated();
        }
    }

    public virtual string StyleSheetGamePath => $"USS/game";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/pinnedTools";
    public virtual string USSCustomClassName => "pinned__tools__area";
    public virtual string USSCustomClassMode(ScrollViewMode mode) => $"{USSCustomClassName}-{mode}".ToLower();
    public virtual string USSCustomClassSDC => $"{USSCustomClassName}-sdc";
    public virtual string USSCustomClassSub_SDC => $"{USSCustomClassName}-sub__sdc";

    public CustomScrollableDataCollection<AbstractMenuItem> SDC;
    public CustomScrollableDataCollection<AbstractMenuItem> Sub_SDC;

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
        Sub_SDC.AddToClassList(USSCustomClassSub_SDC);

        SDC.BindItem = (datum, item) =>
        {
            var toolbox = item as CustomToolbox;
            if (datum is MenuItem menuItem)
            {
                UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"menu item = {menuItem.Name}");
            }
            else if (datum is Menu menu)
            {
                toolbox.AddMenu(datum);
                toolbox.Mode = Mode;
                toolbox.ToolboxType = ToolboxType.Main;
                toolbox.ToolClicked = ToolClicked;
                toolbox.ToolboxClicked = (isSelected, toolboxMenu, toolMenu) =>
                {
                    Sub_SDC.ClearSDC();
                    if (isSelected)
                    {
                        SDC.Select(toolboxMenu);
                        Add(Sub_SDC);
                        Sub_SDC.AddDatum(toolMenu);
                    }
                    else
                    {
                        SDC.Unselect(toolboxMenu);
                        Sub_SDC.RemoveFromHierarchy();
                    }
                };
            }
        };
        SDC.UnbindItem = (datum, item) =>
        {
            var toolbox = item as CustomToolbox;
            toolbox.ClearToolbox();
            toolbox.ToolboxType = ToolboxType.Unknown;
            toolbox.ToolClicked = null;
            toolbox.ToolboxClicked = null;
        };
        SDC.SelectionType = SelectionType.Single;
        SDC.SelectItem = (datum, item) =>
        {
            var toolbox = item as CustomToolbox;
            toolbox.IsSelected = true;
        };
        SDC.UnselectItem = (datum, item) =>
        {
            var toolbox = item as CustomToolbox;
            toolbox.SDC.UnselectAll();
            toolbox.IsSelected = false;
        };
        SDC.ReorderableMode = ReorderableMode.Element;
        SDC.IsReorderable = true;

        //Sub tools
        Sub_SDC.BindItem = (datum, item) =>
        {
            var toolbox = item as CustomToolbox;
            if (datum is MenuItem menuItem)
            {
                UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"menu item = {menuItem.Name}");
            }
            else if (datum is Menu menu)
            {
                toolbox.AddMenu(datum);
                toolbox.Mode = ScrollViewMode.Horizontal;
                toolbox.ToolboxType = ToolboxType.Main;
                toolbox.ToolClicked += ToolClicked;
                toolbox.ToolboxClicked = (isSelected, toolboxMenu, toolMenu) =>
                {
                    if (isSelected) Sub_SDC.AddDatum(toolMenu);
                    else Sub_SDC.RemoveDatum(toolMenu);
                };
            }
        };
        Sub_SDC.UnbindItem = (datum, item) =>
        {
            var toolbox = item as CustomToolbox;
            toolbox.ClearToolbox();
            toolbox.ToolboxType = ToolboxType.Unknown;
            toolbox.ToolClicked = null;
            toolbox.ToolboxClicked = null;
        };
        Sub_SDC.Mode = ScrollViewMode.Vertical;
        Sub_SDC.IsReorderable = false;

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

    #region Implementation

    /// <summary>
    /// Add a menu to the pinned tools area.
    /// </summary>
    /// <param name="menu"></param>
    public virtual void AddMenu(AbstractMenuItem menu)
        => SDC.AddDatum(menu);

    /// <summary>
    /// Whether or not the tools inside the toolbox are reorderable.
    /// </summary>
    /// <param name="value"></param>
    public virtual void AreToolboxReorderable(bool value)
    {
        foreach (var item in SDC.DataToItem.Values)
            (item as CustomToolbox).SDC.IsReorderable = value;
    }

    protected virtual void ModeUpdated()
    {
        foreach (var item in SDC.DataToItem.Values)
            (item as CustomToolbox).Mode = Mode;
    }

    protected virtual void ToolClicked(bool isSelected, AbstractMenuItem toolboxMenu, AbstractMenuItem toolMenu)
    {
        Sub_SDC.ClearSDC();
        Sub_SDC.RemoveFromHierarchy();
        if (isSelected)
        {
            SDC.Select(toolboxMenu);
            //TODO
        }
        else
        {
            SDC.Unselect(toolboxMenu);
            //TODO
        }
    }

    #endregion
}
