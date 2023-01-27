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

namespace umi3d.commonScreen.game
{
    public class ToolsItemsWindow_C : CustomFormSDC<AbstractMenuItem>
    {
        public virtual string USSCustomClassToolsItemsWindow => "tools__items__window";

        public new class UxmlFactory : UxmlFactory<ToolsItemsWindow_C, UxmlTraits> { }

        public ToolsItemsWindow_C() => Set();

        public override void InitElement()
        {
            if (TitleLabel == null) TitleLabel = new Displayer.Text_C();
            if (SDC == null) SDC = new Container.ScrollableDataCollection_C<AbstractMenuItem>();

            base.InitElement();
            AddToClassList(USSCustomClassToolsItemsWindow);

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
