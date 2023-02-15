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
using umi3d.commonScreen.game;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public class ToolsItemsWindow_C : FormSDC_C<AbstractMenuItem>
    {
        public virtual string USSCustomClassToolsItemsWindow => "tools__items__window";

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            AddToClassList(USSCustomClassToolsItemsWindow);
        }

        protected override void InitElement()
        {
            base.InitElement();
            SDC.MakeItem = datum => DisplayerManager.MakeDisplayer(datum);
            SDC.BindItem = (datum, item) =>
            {
                DisplayerManager.BindItem(datum, item);
            };
            SDC.UnbindItem = (datum, item) =>
            {
                DisplayerManager.UnbindItem(item);
            };
        }

        #region Implementation

        public AbstractMenuItem ToolMenu;

        /// <summary>
        /// Add a menu to this tools items window.
        /// </summary>
        /// <param name="menu"></param>
        public virtual void AddMenu(AbstractMenuItem menu)
        {
            ToolMenu = menu;

            if (menu is Menu _menu && _menu.MenuItems.Count > 0)
            {
                foreach (var menuItem in _menu.MenuItems) SDC.AddDatum(menuItem);
            }
            else if (menu is MenuItem menuItem) SDC.AddDatum(menuItem);
        }

        #endregion
    }
}

namespace umi3d.UiPreview.commonScreen.game
{
    public class ToolsItemsWindow_Preview : ToolsItemsWindow_C
    {
        public new class UxmlFactory : UxmlFactory<ToolsItemsWindow_Preview, UxmlTraits> 
        {
            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            /// <returns></returns>
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                ToolsItemsWindow_C item = base.Create(bag, cc) as ToolsItemsWindow_C;

                Menu tool = new Menu { Name = "tool" };

                MenuItem item1 = new ButtonMenuItem { Name = "Button Item1" };
                tool.Add(item1);

                DropDownInputMenuItem item2 = new DropDownInputMenuItem { Name = "Enum Item2", options = new List<string>() { "un", "deux", "trois" } };

                item2.NotifyValueChange("un");
                tool.Add(item2);

                MenuItem item3 = new BooleanInputMenuItem { Name = "Toggle Item3" };
                tool.Add(item3);

                MenuItem item4 = new FloatRangeInputMenuItem { Name = "Slider Item4", min = 0f, max = 50f, value = 0f };
                tool.Add(item4);

                MenuItem item5 = new TextInputMenuItem { Name = "Text Item5" };
                tool.Add(item5);

                item.Title = tool.Name;
                item.AddMenu(tool);

                return item;
            }
        }
    }
}
