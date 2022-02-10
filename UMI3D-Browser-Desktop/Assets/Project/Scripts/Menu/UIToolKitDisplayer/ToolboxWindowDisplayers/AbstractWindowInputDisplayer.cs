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
using BrowserDesktop.Menu;
using System;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using umi3dDesktopBrowser.uI.viewController;
using UnityEngine.UIElements;

namespace umi3d.DesktopBrowser.menu.Displayer
{
    public abstract partial class AbstractWindowInputDisplayer
    {
        public Displayer_E Displayer { get; protected set; } = null;
    }

    public partial class AbstractWindowInputDisplayer : IDisplayerElement
    {
        public VisualElement GetUXMLContent()
        {
            return Displayer.Root;
        }

        public virtual void InitAndBindUI()
        {
            Displayer = new Displayer_E();
        }
    }

    public partial class AbstractWindowInputDisplayer : AbstractDisplayer
    {
        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);

        }

        public override void Display(bool forceUpdate = false)
        {
            Displayer.Display();
        }

        public override void Hide()
        {
            Displayer.Hide();
        }

        public override void Clear()
        {
            base.Clear();
            Displayer.Reset();
        }
    }
}