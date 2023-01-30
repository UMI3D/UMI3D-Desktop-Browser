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
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class Tool_C : CustomButton
    {
        public new class UxmlFactory : UxmlFactory<Tool_C, UxmlTraits> { }

        public new class UxmlTraits : CustomButton.UxmlTraits
        {
            protected UxmlEnumAttributeDescription<ToolType> m_toolType = new UxmlEnumAttributeDescription<ToolType>
            {
                name = "tool-type",
                defaultValue = ToolType.Unknown
            };

            protected UxmlBoolAttributeDescription m_isSelected = new UxmlBoolAttributeDescription
            {
                name = "is-selected",
                defaultValue = false,
            };

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="ve"></param>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var custom = ve as Tool_C;

                custom.Set
                    (
                        m_category.GetValueFromBag(bag, cc),
                        m_size.GetValueFromBag(bag, cc),
                        m_shape.GetValueFromBag(bag, cc),
                        m_type.GetValueFromBag(bag, cc),
                        m_label.GetValueFromBag(bag, cc),
                        m_labelDirection.GetValueFromBag(bag, cc),
                        m_iconAlignment.GetValueFromBag(bag, cc),
                        m_toolType.GetValueFromBag(bag, cc),
                        m_isSelected.GetValueFromBag(bag, cc)
                    );
            }
        }

        /// <summary>
        /// Type of the tool
        /// </summary>
        public virtual ToolType ToolType
        {
            get => m_toolType;
            set
            {
                RemoveFromClassList(USSCustomClassToolType(m_toolType));
                AddToClassList(USSCustomClassToolType(value));
                m_toolType = value;
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
                Type = m_isSelected ? ButtonType.Primary : ButtonType.Default;
            }
        }

        public virtual string StyleSheetToolPath => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/tool";
        public virtual string USSCustomClassTool => "tool";
        public virtual string USSCustomClassIcon => $"{USSCustomClassTool}-icon";
        public virtual string USSCustomClassToolType(ToolType type) => $"{USSCustomClassTool}-type__{type}".ToLower();

        public VisualElement Icon = new VisualElement { name = "icon" };

        protected ToolType m_toolType;
        protected bool m_isSelected;

        public Tool_C() => Set();

        public override void InitElement()
        {
            if (LabelVisual == null) LabelVisual = new Text_C();
            if (TextVisual == null) TextVisual = new Text_C();

            base.InitElement();

            try
            {
                this.AddStyleSheetFromPath(StyleSheetToolPath);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            AddToClassList(USSCustomClassTool);
            Icon.AddToClassList(USSCustomClassIcon);

            clicked += () => ToolClicked?.Invoke(IsSelected, ToolMenu);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Set() => Set(ElementCategory.Game, ElementSize.Medium, ButtonShape.Square, ButtonType.Default, null, ElemnetDirection.Bottom, ElementAlignment.Center, ToolType.Unknown, false);

        /// <summary>
        /// Set this ui element.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="size"></param>
        /// <param name="shape"></param>
        /// <param name="type"></param>
        /// <param name="label"></param>
        /// <param name="direction"></param>
        /// <param name="alignment"></param>
        /// <param name="isSelected"></param>
        public virtual void Set(ElementCategory category, ElementSize size, ButtonShape shape, ButtonType type, string label, ElemnetDirection direction, ElementAlignment alignment, ToolType toolType, bool isSelected)
        {
            base.Set(category, size, shape, type, label, direction, alignment);

            Add(Icon);

            LabelDirection = ElemnetDirection.Bottom;

            ToolType = toolType;
            IsSelected = isSelected;
        }

        #region Implementation

        public AbstractMenuItem ToolMenu;
        /// <summary>
        /// Action raised when this tool is clicked. The first argument is the current selection status.
        /// </summary>
        public System.Action<bool, AbstractMenuItem> ToolClicked;

        /// <summary>
        /// Add a menu item in the tool. Set <see cref="ToolMenu"/>, <see cref="ToolType"/> and icon.
        /// </summary>
        /// <param name="toolMenu"></param>
        public virtual void AddMenu(AbstractMenuItem toolMenu)
        {
            ToolMenu = toolMenu;

            if (toolMenu is Menu menu)
            {
                ToolType = menu.MenuItems.Count > 0 ? ToolType.Tool : ToolType.Toolbox;

                if (menu.icon2D != null) SetToolIcon(menu.icon2D);
            }
            else if (toolMenu is MenuItem menuItem)
            {
                UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"");
            }
        }

        /// <summary>
        /// Clear this tool. Set <see cref="ToolMenu"/> to null, <see cref="ToolType"/> to <see cref="ToolType.Unknown"/> and icon to default.
        /// </summary>
        public virtual void ClearTool()
        {
            ToolMenu = null;
            ToolType = ToolType.Unknown;
            SetToolIcon(null as Texture2D);
        }

        /// <summary>
        /// Set the icon of this tool.
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetToolIcon(Texture2D value)
        {
            if (value == null) Icon.style.backgroundImage = StyleKeyword.Null;
            else Icon.style.backgroundImage = value;
        }
        /// <summary>
        /// Set the icon of this tool.
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetToolIcon(VectorImage value)
        {
            if (value == null) Icon.style.backgroundImage = StyleKeyword.Null;
            else Icon.style.backgroundImage = new StyleBackground(value);
        }
        /// <summary>
        /// Set the icon of this tool.
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetToolIcon(Sprite value)
        {
            if (value == null) Icon.style.backgroundImage = StyleKeyword.Null;
            else Icon.style.backgroundImage = new StyleBackground(value);
        }

        #endregion
    }
}

namespace umi3d.UiPreview.commonScreen.Displayer
{
    public class Tool_Preview: Tool_C
    {
        public new class UxmlFactory : UxmlFactory<Tool_Preview, UxmlTraits>
        {
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                Tool_C previewItem = base.Create(bag, cc) as Tool_C;

                //Tool1
                Menu tool1 = new Menu { Name = "tool1" };
                //Item1
                MenuItem item1 = new ButtonMenuItem { Name = "Button Item1" };
                tool1.Add(item1);
                //Item2
                DropDownInputMenuItem item2 = new DropDownInputMenuItem { Name = "Enum Item2", options = new List<string>() { "un", "deux", "trois" } };
                item2.NotifyValueChange("un");
                tool1.Add(item2);

                previewItem.AddMenu(tool1);
                previewItem.ToolClicked = (isSelected, menu) =>
                {
                    previewItem.IsSelected = !isSelected;
                };

                return previewItem;
            }
        }
    }
}
