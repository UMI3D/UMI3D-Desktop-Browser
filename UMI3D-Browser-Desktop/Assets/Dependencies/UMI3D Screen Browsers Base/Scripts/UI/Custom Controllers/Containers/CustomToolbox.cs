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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

public class CustomToolbox : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlEnumAttributeDescription<ToolboxType> m_toolboxType = new UxmlEnumAttributeDescription<ToolboxType>
        {
            name = "toolbox-type",
            defaultValue = ToolboxType.Unknown
        };

        UxmlEnumAttributeDescription<ElementCategory> m_category = new UxmlEnumAttributeDescription<ElementCategory>
        {
            name = "category",
            defaultValue = ElementCategory.Game
        };

        UxmlStringAttributeDescription m_toolboxName = new UxmlStringAttributeDescription
        {
            name = "toolbox-name",
            defaultValue = null
        };

        UxmlEnumAttributeDescription<ScrollViewMode> m_mode = new UxmlEnumAttributeDescription<ScrollViewMode>
        {
            name = "mode",
            defaultValue = ScrollViewMode.Horizontal
        };

        UxmlBoolAttributeDescription m_displayToolsName = new UxmlBoolAttributeDescription
        {
            name = "display-tools-name",
            defaultValue = false
        };

        protected UxmlBoolAttributeDescription m_isSelected = new UxmlBoolAttributeDescription
        {
            name = "is-selected",
            defaultValue = false,
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomToolbox;

            custom.Set
                (
                    m_toolboxType.GetValueFromBag(bag, cc),
                    m_category.GetValueFromBag(bag, cc),
                    m_toolboxName.GetValueFromBag(bag, cc),
                    m_mode.GetValueFromBag(bag, cc),
                    m_displayToolsName.GetValueFromBag(bag, cc)
                 );
        }
    }

    /// <summary>
    /// Set the category of this toolbox.
    /// </summary>
    public virtual ToolboxType ToolboxType
    {
        get => m_ToolboxType;
        set
        {
            RemoveFromClassList(USSCustomClassType(m_ToolboxType));
            AddToClassList(USSCustomClassType(value));
            m_ToolboxType = value;
        }
    }
    /// <summary>
    /// Set the category of this toolbox.
    /// </summary>
    public virtual ElementCategory Category
    {
        get => SDC.Category;
        set
        {
            RemoveFromClassList(USSCustomClassCategory(SDC.Category));
            AddToClassList(USSCustomClassCategory(value));
            SDC.Category = value;
        }
    }
    /// <summary>
    /// Set the name of this toolbox.
    /// </summary>
    public virtual string ToolboxName
    {
        get => ToolboxNameText.text;
        set
        {
            if (string.IsNullOrEmpty(value)) ToolboxNameText.RemoveFromHierarchy();
            else Insert(0, ToolboxNameText);
            ToolboxNameText.text = value;
        }
    }
    /// <summary>
    /// Set the mode of this toolbox (Verticcal, Horizontal, Both).
    /// </summary>
    public virtual ScrollViewMode Mode
    {
        get => SDC.Mode;
        set
        {
            RemoveFromClassList(USSCustomClassMode(SDC.Mode));
            AddToClassList(USSCustomClassMode(SDC.Mode));
            SDC.Mode = value;
        }
    }
    /// <summary>
    /// Whether or not the name of the tools should be display.
    /// </summary>
    public virtual bool DisplayToolsName
    {
        get => m_displayToolsName;
        set
        {
            m_displayToolsName = value;
            //TODO update tools name when modify.
        }
    }
    /// <summary>
    /// Whether or not the tool is selected.
    /// </summary>
    public virtual bool IsSelected
    {
        get => m_isSelected;
        set
        {
            m_isSelected = value;
            if (value) AddToClassList(USSCustomClassSelected);
            else RemoveFromClassList(USSCustomClassSelected);
        }
    }

    public virtual string StyleSheetContainerPath => $"USS/container";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetContainersFolderPath}/toolbox";
    public virtual string USSCustomClassName => "toolbox";
    public virtual string USSCustomClassType(ToolboxType type) => $"{USSCustomClassName}-type__{type}".ToLower();
    public virtual string USSCustomClassCategory(ElementCategory category) => $"{USSCustomClassName}-{category}".ToLower();
    public virtual string USSCustomClassMode(ScrollViewMode mode) => $"{USSCustomClassName}-{mode}".ToLower();
    public virtual string USSCustomClassSelected => $"{USSCustomClassName}-selected";
    public virtual string USSCustomClassToolboxName => $"{USSCustomClassName}-name";

    public CustomText ToolboxNameText;
    public CustomScrollableDataCollection<AbstractMenuItem> SDC;

    protected bool m_hasBeenInitialized;
    protected ToolboxType m_ToolboxType;
    protected bool m_displayToolsName;
    protected bool m_isSelected;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetContainerPath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);
        ToolboxNameText.AddToClassList(USSCustomClassToolboxName);

        SDC.BindItem = (datum, item) =>
        {
            var tool = item as CustomTool;
            tool.Menu = datum;
            tool.ToolClicked = (isSelected, datum) =>
            {
                if (isSelected) SDC.Select(datum);
                else SDC.Unselect(datum);
            };

            if (datum is Menu menu)
            {
                if (menu.MenuItems.Count > 0) tool.ToolType = ToolType.Tool;
                else tool.ToolType = ToolType.Toolbox;
                if (DisplayToolsName) tool.Label = menu.Name;
                if (menu.icon2D != null) tool.SetToolIcon(menu.icon2D);
            }
            else if (datum is MenuItem menuItem)
            {
                UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"");
            }
        };
        SDC.UnbindItem = (datum, item) =>
        {
            var tool = item as CustomTool;
            tool.Menu = null;
            tool.ToolClicked = null;
            tool.Label = null;
            tool.SetToolIcon(null as Texture2D);
        };
        SDC.Size = 104f;
        SDC.SelectionType = SelectionType.Single;
        SDC.SelectItem = (datum, item) =>
        {
            var tool = item as CustomTool;

            tool.IsSelected = true;
            if (tool.ToolType == ToolType.Tool) ToolClicked?.Invoke(true, datum);
            else if (tool.ToolType == ToolType.Toolbox) ToolboxClicked?.Invoke(true, datum);
        };
        SDC.UnselectItem = (datum, item) =>
        {
            var tool = item as CustomTool;

            tool.IsSelected = false;
            if (tool.ToolType == ToolType.Tool) ToolClicked?.Invoke(false, datum);
            else if (tool.ToolType == ToolType.Toolbox) ToolboxClicked?.Invoke(false, datum);
        };

        Add(SDC);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual void Set() => Set(ToolboxType.Unknown, ElementCategory.Game, null, ScrollViewMode.Horizontal, false);

    /// <summary>
    /// set this UI element.
    /// </summary>
    /// <param name="category"></param>
    /// <param name="name"></param>
    /// <param name="mode"></param>
    public virtual void Set(ToolboxType type, ElementCategory category, string name, ScrollViewMode mode, bool displayToolsName)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        ToolboxType = type;
        Category = category;
        ToolboxName = name;
        Mode = mode;
        DisplayToolsName = displayToolsName;
    }

    #region Implementation

    public AbstractMenuItem ToolboxMenu;

    /// <summary>
    /// Action raised when a tool is clicked (param is whether or not the tool is selected).
    /// </summary>
    public System.Action<bool, AbstractMenuItem> ToolClicked;
    /// <summary>
    /// Action raised when a tool as a toolbox is clicked (param is whether or not the tool is selected).
    /// </summary>
    public System.Action<bool, AbstractMenuItem> ToolboxClicked;

    /// <summary>
    /// Add a menu item in the Toolbox.
    /// </summary>
    /// <param name="item"></param>
    public void AddMenu(AbstractMenuItem item)
    {
        ToolboxMenu = item;
        ToolboxName = item.Name;

        if (item is Menu menu && menu.SubMenu.Count > 0)
            foreach (var subMenu in menu.SubMenu) SDC.AddDatum(subMenu);
        else if (item is Menu || item is MenuItem) SDC.AddDatum(item);
    }

    /// <summary>
    /// Clear this toolbox. Set menu to null, name to null, clear SDC.
    /// </summary>
    public void ClearToolbox()
    {
        ToolboxMenu = null;
        ToolboxName = null;

        SDC.ClearSDC();
    }

    #endregion
}
