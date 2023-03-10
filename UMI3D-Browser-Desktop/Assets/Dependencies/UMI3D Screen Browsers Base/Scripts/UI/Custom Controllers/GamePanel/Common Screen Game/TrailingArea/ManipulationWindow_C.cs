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
using umi3d.baseBrowser.utils;
using umi3d.cdk.menu;
using umi3d.cdk.menu.interaction;
using umi3d.common.interaction;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public class ManipulationWindow_C : FormSEDC_C<ManipulationMenuItem>
    {
        protected override void InitElement()
        {
            base.InitElement();
            ManipulationMenu = Resources.Load<MenuAsset>("Scriptables/GamePanel/ManipulationMenu");

            this.SetHeight(285f);

            SEDC.Mode = ScrollViewMode.Horizontal;
            SEDC.MakeItem = datum =>
            {
                var manipulation = new Manipulation_C();

                manipulation.BindSourceToggleValue(out Derive<bool> toggleValue);
                toggleValue.ValueChanged += e => ManipulationSelected(e, manipulation);

                return manipulation;
            };
            SEDC.BindItem = (datum, element) =>
            {
                var manipulation = element as Manipulation_C;

                manipulation.Dof = datum.dof.dofs;
                manipulation.MenuItem = datum;
                datum.Subscribe(manipulation.Select);

                Manipulations.Add(manipulation);

                if (SEDC.Data.Count == 1) manipulation.ToggleValue = true;
            };
            SEDC.UnbindItem = (datum, element) =>
            {
                var manipulation = element as Manipulation_C;

                manipulation.ToggleValue = false;
                manipulation.IsToggle = true;
                manipulation.MenuItem = null;
                datum.UnSubscribe(manipulation.Select);

                Manipulations.Remove(manipulation);
            };
            SEDC.AnimationTimeIn = 1f;
            SEDC.AnimationTimeOut = .5f;

            ManipulationMenu.menu.onAbstractMenuItemAdded.AddListener(menu =>
            {
                if (menu is not ManipulationMenuItem manip) return;

                SEDC.AddDatum(manip);
            });
            ManipulationMenu.menu.OnAbstractMenuItemRemoved.AddListener(menu =>
            {
                if (menu is not ManipulationMenuItem manip) return;

                SEDC.RemoveDatum(manip);
            });
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Title = "Manipulations";
            Category = ElementCategory.Game;
        }

        #region Implementation

        public MenuAsset ManipulationMenu;
        public List<Manipulation_C> Manipulations = new List<Manipulation_C>();

        protected virtual void ManipulationSelected(ChangeEvent<bool> evt, Manipulation_C manip)
        {
            if (!evt.newValue) return;

            manip.IsToggle = false;
            manip.MenuItem.UnSubscribe(manip.Select);
            manip.MenuItem.Select();
            manip.MenuItem.Subscribe(manip.Select);
            Manipulations.ForEach(manipulation =>
            {
                if (manipulation.Dof == manip.Dof) return;

                manipulation.IsToggle = true;
                manipulation.ToggleValue = false;
            });
        }

        #endregion
    }
}
