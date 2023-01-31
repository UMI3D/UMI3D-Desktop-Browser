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
using umi3d.cdk.interaction;
using umi3d.cdk.menu;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using umi3d.commonScreen.game;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Container
{
    public class Toolbox_C : VisualElement, ICustomElement
    {
        public new class UxmlFactory : UxmlFactory<Toolbox_C, UxmlTraits> { }

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

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var custom = ve as Toolbox_C;

                custom.Set
                    (
                        m_toolboxType.GetValueFromBag(bag, cc),
                        m_category.GetValueFromBag(bag, cc),
                        m_toolboxName.GetValueFromBag(bag, cc),
                        m_mode.GetValueFromBag(bag, cc),
                        m_displayToolsName.GetValueFromBag(bag, cc),
                        m_isSelected.GetValueFromBag(bag, cc)
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
                if (string.IsNullOrEmpty(value)) ToolboxNameText.RemoveIfIsInHierarchy();
                else this.InsertIfNotInHierarchy(0, ToolboxNameText);
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

        public CustomText ToolboxNameText = new Displayer.Text_C { name = "toolbox-name" };
        public ScrollableDataCollection_C<AbstractMenuItem> SDC = new ScrollableDataCollection_C<AbstractMenuItem> { name = "sdc" };

        protected ToolboxType m_ToolboxType;
        protected bool m_displayToolsName;
        protected bool m_isSelected;

        public Toolbox_C() => Set();

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

            SDC.MakeItem = datum => new Displayer.Tool_C();
            SDC.BindItem = (datum, item) =>
            {
                var tool = item as Tool_C;

                tool.ToolClicked = (isSelected, datum) =>
                {
                    if (!isSelected) SDC.Select(datum);
                    else SDC.Unselect(datum);
                };

                tool.AddMenu(datum);

                if (DisplayToolsName) tool.Label = datum.Name;
            };
            SDC.UnbindItem = (datum, item) =>
            {
                var tool = item as Tool_C;
                tool.ToolClicked = null;
                tool.Label = null;
                tool.ClearTool();
            };
            SDC.Size = 104f;
            SDC.SelectionType = SelectionType.Single;
            SDC.SelectItem = (datum, item) =>
            {
                var tool = item as Tool_C;

                tool.IsSelected = true;
                if (tool.ToolType == ToolType.Tool) ToolClicked?.Invoke(true, ToolboxMenu, datum);
                else if (tool.ToolType == ToolType.Toolbox) ToolboxClicked?.Invoke(true, ToolboxMenu, datum);
            };
            SDC.UnselectItem = (datum, item) =>
            {
                var tool = item as Tool_C;

                tool.IsSelected = false;
                if (tool.ToolType == ToolType.Tool) ToolClicked?.Invoke(false, ToolboxMenu, datum);
                else if (tool.ToolType == ToolType.Toolbox) ToolboxClicked?.Invoke(false, ToolboxMenu, datum);
            };

            Add(SDC);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void Set()
        {
            InitElement();
        }

        /// <summary>
        /// set this UI element.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        public virtual void Set(ToolboxType type, ElementCategory category, string name, ScrollViewMode mode, bool displayToolsName, bool isSelected)
        {
            ToolboxType = type;
            Category = category;
            ToolboxName = name;
            Mode = mode;
            DisplayToolsName = displayToolsName;
            IsSelected = isSelected;
        }

        #region Implementation

        public AbstractMenuItem ToolboxMenu;

        /// <summary>
        /// Action raised when a tool is clicked (first param is whether or not the tool is selected, second is <see cref="ToolboxMenu"/>, third is toolMenu).
        /// </summary>
        public System.Action<bool, AbstractMenuItem, AbstractMenuItem> ToolClicked;
        /// <summary>
        /// Action raised when a tool as a toolbox is clicked (first param is whether or not the tool is selected, second is <see cref="ToolboxMenu"/>, third is toolMenu).
        /// </summary>
        public System.Action<bool, AbstractMenuItem, AbstractMenuItem> ToolboxClicked;

        /// <summary>
        /// Add a menu item in the Toolbox.
        /// </summary>
        /// <param name="item"></param>
        public void AddMenu(AbstractMenuItem item)
        {
            ToolboxMenu = item;
            ToolboxName = item.Name;

            if (item is Menu menu)
            {
                if (menu.SubMenu.Count > 0) foreach (var subMenu in menu.SubMenu) SDC.AddDatum(subMenu);
            }
            else if (item is MenuItem) SDC.AddDatum(item); //TO test
        }

        /// <summary>
        /// Clear this toolbox. Set menu to null, name to null, clear SDC.
        /// </summary>
        public void ClearToolbox()
        {
            ToolboxMenu = null;
            ToolboxName = null;

            SDC.ClearDC();
        }

        #endregion
    }
}

namespace umi3d.UiPreview.commonScreen.Container
{
    public class Toolbox_Preview: Toolbox_C
    {
        public new class UxmlFactory : UxmlFactory<Toolbox_Preview, UxmlTraits>
        {
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                Toolbox_C previewItem = base.Create(bag, cc) as Toolbox_C;

                //Toolbox1
                Menu toolbox1 = new Menu { Name = "toolbox1" };

                //Tool1
                Menu tool1 = new Menu { Name = "tool1" };
                //Item1
                MenuItem item1 = new ButtonMenuItem { Name = "Button Item1" };
                tool1.Add(item1);
                //Item2
                DropDownInputMenuItem item2 = new DropDownInputMenuItem { Name = "Enum Item2", options = new List<string>() { "un", "deux", "trois" } };
                item2.NotifyValueChange("un");
                tool1.Add(item2);
                toolbox1.Add(tool1);

                //Tool2
                Menu tool2 = new Menu { Name = "tool2" };
                //Item3
                MenuItem item3 = new BooleanInputMenuItem { Name = "Toggle Item3" };
                tool2.Add(item3);
                //Item4
                MenuItem item4 = new FloatRangeInputMenuItem { Name = "Slider Item4", min = 0f, max = 50f, value = 0f };
                tool2.Add(item4);
                toolbox1.Add(tool2);

                //Toolbox2
                Menu toolbox2 = new Menu { Name = "toolbox2" };
                toolbox1.Add(toolbox2);

                //Tool4
                Menu tool4 = new Menu { Name = "tool4" };
                //Item5
                MenuItem item5 = new TextInputMenuItem { Name = "Text Item5" };
                tool4.Add(item5);
                toolbox2.Add(tool4);

                previewItem.ToolboxType = ToolboxType.Main;
                previewItem.AddMenu(toolbox1);

                return previewItem;
            }
        }
    }
}
