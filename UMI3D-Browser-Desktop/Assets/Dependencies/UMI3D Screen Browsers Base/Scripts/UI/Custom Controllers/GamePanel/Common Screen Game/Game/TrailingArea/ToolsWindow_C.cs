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
using umi3d.commonScreen.game;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public class ToolsWindow_C : Container.FormSDC_C<AbstractMenuItem>
    {
        public ToolsWindow_C() => Set();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void InitElement()
        {
            if (TitleLabel == null) TitleLabel = new Displayer.Text_C();
            if (SDC == null) SDC = new Container.ScrollableDataCollection_C<AbstractMenuItem>();

            base.InitElement();

            SDC.MakeItem = datum =>
            {
                ExpandableDataCollection_C<AbstractMenuItem> edc = new ExpandableDataCollection_C<AbstractMenuItem>();
                edc.MakeItem = datum =>
                {
                    return null;
                };
                edc.BindItem = (datum, item) =>
                {

                };
                edc.UnbindItem = (datum, item) =>
                {

                };
                return null;
            };
            SDC.BindItem = (datum, item) =>
            {

            };
            SDC.UnbindItem = (datum, item) =>
            {

            };
        }

        #region Implementation

        public AbstractMenuItem RootMenu;

        /// <summary>
        /// Add a menu to this tools items window.
        /// </summary>
        /// <param name="menu"></param>
        public virtual void AddMenu(AbstractMenuItem menu)
        {
            RootMenu = menu;

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
    public class ToolsWindow_Preview: ToolsWindow_C
    {
        public new class UxmlFactory : UxmlFactory<ToolsWindow_Preview, UxmlTraits>
        {
            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            /// <returns></returns>
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                ToolsWindow_C previewItem = base.Create(bag, cc) as ToolsWindow_C;

                //Root
                Menu root = new Menu { Name = "root" };

                //Toolbox1
                Menu toolbox1 = new Menu { Name = "toolbox1" };
                root.Add(toolbox1);

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
                root.Add(toolbox2);

                //Toolbox3
                Menu toolbox3 = new Menu { Name = "toolbox3" };
                toolbox2.Add(toolbox3);

                //Tool4
                Menu tool4 = new Menu { Name = "tool4" };
                //Item5
                MenuItem item5 = new TextInputMenuItem { Name = "Text Item5" };
                tool4.Add(item5);
                toolbox3.Add(tool4);

                previewItem.Title = "Toolbox";
                previewItem.RootMenu = root;

                return previewItem;
            }
        }
    }
}
