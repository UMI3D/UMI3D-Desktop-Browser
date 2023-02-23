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
using System.Collections.Generic;
using umi3d.cdk.menu;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Container
{
    public class Toolbox_C : BaseVisual_C
    {
        public new class UxmlFactory : UxmlFactory<Toolbox_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            protected UxmlEnumAttributeDescription<ToolboxType> m_toolboxType = new UxmlEnumAttributeDescription<ToolboxType>
            {
                name = "toolbox-type",
                defaultValue = ToolboxType.Unknown
            };

            protected UxmlEnumAttributeDescription<ElementCategory> m_category = new UxmlEnumAttributeDescription<ElementCategory>
            {
                name = "category",
                defaultValue = ElementCategory.Game
            };

            protected UxmlLocaliseAttributeDescription m_toolboxName = new UxmlLocaliseAttributeDescription
            {
                name = "toolbox-name"
            };

            protected UxmlEnumAttributeDescription<ScrollViewMode> m_mode = new UxmlEnumAttributeDescription<ScrollViewMode>
            {
                name = "mode",
                defaultValue = ScrollViewMode.Horizontal
            };

            protected UxmlBoolAttributeDescription m_displayToolsName = new UxmlBoolAttributeDescription
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

                custom.ToolboxType = m_toolboxType.GetValueFromBag(bag, cc);
                custom.Category = m_category.GetValueFromBag(bag, cc);
                custom.ToolboxName = m_toolboxName.GetValueFromBag(bag, cc);
                custom.Mode = m_mode.GetValueFromBag(bag, cc);
                custom.DisplayToolsName = m_displayToolsName.GetValueFromBag(bag, cc);
                custom.IsSelected = m_isSelected.GetValueFromBag(bag, cc);
                custom.ToolboxType = m_toolboxType.GetValueFromBag(bag, cc);
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
        public virtual LocalisationAttribute ToolboxName
        {
            get => ToolboxNameText.LocalisedText;
            set
            {
                if (value.IsEmpty) ToolboxNameText.RemoveIfIsInHierarchy();
                else this.InsertIfNotInHierarchy(0, ToolboxNameText);
                ToolboxNameText.LocalisedText = value;
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
                AddToClassList(USSCustomClassMode(value));
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
        /// Whether or not the toolbox is selected.
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
        /// <summary>
        /// Whether or not the toolbox is pinned.
        /// </summary>
        public virtual bool IsPinned
        {
            get => m_isPinned;
            set => m_isPinned = value;
        }

        public override string StyleSheetPath_MainTheme => $"USS/container";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetContainersFolderPath}/toolbox";

        public override string UssCustomClass_Emc => "toolbox";
        public virtual string USSCustomClassType(ToolboxType type) => $"{UssCustomClass_Emc}-type__{type}".ToLower();
        public virtual string USSCustomClassCategory(ElementCategory category) => $"{UssCustomClass_Emc}-{category}".ToLower();
        public virtual string USSCustomClassMode(ScrollViewMode mode) => $"{UssCustomClass_Emc}-{mode}".ToLower();
        public virtual string USSCustomClassSelected => $"{UssCustomClass_Emc}-selected";
        public virtual string USSCustomClassToolboxName => $"{UssCustomClass_Emc}-name";
        public virtual string USSCustomClassPinned => $"{UssCustomClass_Emc}-pinned";
        public virtual string USSCustomClassEdit => $"{UssCustomClass_Emc}-edit";

        public Text_C ToolboxNameText = new Text_C { name = "toolbox-name" };
        public Button_C PinnedButton = new Button_C { name = "pinned" };
        public Button_C EditButton = new Button_C { name = "edit" };
        public ScrollableDataCollection_C<AbstractMenuItem> SDC = new ScrollableDataCollection_C<AbstractMenuItem> { name = "sdc" };

        protected ToolboxType m_ToolboxType;
        protected bool m_displayToolsName;
        protected bool m_isSelected;
        protected bool m_isPinned;

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            ToolboxNameText.AddToClassList(USSCustomClassToolboxName);
            PinnedButton.AddToClassList(USSCustomClassPinned);
            EditButton.AddToClassList(USSCustomClassEdit);
        }

        protected override void InitElement()
        {
            base.InitElement();
            SDC.MakeItem = datum => new Tool_C();
            SDC.BindItem = (datum, item) =>
            {
                var tool = item as Tool_C;

                tool.ToolClicked = (isSelected, datum) =>
                {
                    if (!isSelected) SDC.Select(datum);
                    else SDC.Unselect(datum);
                };

                tool.AddMenu(datum);

                if (DisplayToolsName) tool.LocalisedLabel = datum.Name;
            };
            SDC.UnbindItem = (datum, item) =>
            {
                var tool = item as Tool_C;
                tool.ToolClicked = null;
                tool.LocalisedLabel = null;
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

            PinnedButton.Category = ElementCategory.Game;
            PinnedButton.Height = ElementSize.Small;
            PinnedButton.Shape = ButtonShape.Round;

            EditButton.Category = ElementCategory.Game;
            EditButton.Height = ElementSize.Small;
            EditButton.Shape = ButtonShape.Round;
            EditButton.LocaliseText = "···";

            EditButton.clicked += () =>
            {
                SDC.IsReorderable = !SDC.IsReorderable;
            };

            ToolboxNameText.Add(PinnedButton);
            ToolboxNameText.Add(EditButton);
            Add(SDC);
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

                menu.onAbstractMenuItemAdded.AddListener(SDC.AddDatum);
                menu.OnAbstractMenuItemRemoved.AddListener(SDC.RemoveDatum);
            }
            else if (item is MenuItem) SDC.AddDatum(item); //TO test
        }

        /// <summary>
        /// Clear this toolbox. Set menu to null, name to null, clear SDC.
        /// </summary>
        public void ClearToolbox()
        {
            if (ToolboxMenu is Menu menu)
            {
                menu.onAbstractMenuItemAdded.RemoveListener(SDC.AddDatum);
                menu.OnAbstractMenuItemRemoved.RemoveListener(SDC.RemoveDatum);
            }

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
