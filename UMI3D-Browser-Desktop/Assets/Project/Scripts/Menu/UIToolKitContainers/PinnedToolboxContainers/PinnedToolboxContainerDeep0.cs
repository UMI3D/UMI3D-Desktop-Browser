/*
Copyright 2019 - 2021 Inetum

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
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;

namespace umi3d.desktopBrowser.menu.Container
{
    public partial class PinnedToolboxContainerDeep0
    {
        public Toolbox_E Toolbox { get; private set; } = null;
    }

    public partial class PinnedToolboxContainerDeep0 : AbstractToolboxesContainer
    {
        protected override void Awake()
        {
            base.Awake();
            Toolbox = new Toolbox_E();
        }

        private void OnDestroy()
        {
            Toolbox.Remove();
        }

        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            Toolbox.SetToolboxName(menu.Name);
        }

        public override void Display(bool forceUpdate = false)
        {
            base.Display(forceUpdate);

        }
    }
}
