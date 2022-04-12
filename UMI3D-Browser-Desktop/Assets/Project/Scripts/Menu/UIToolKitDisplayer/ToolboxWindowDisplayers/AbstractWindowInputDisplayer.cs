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
using umi3d.cdk.menu.view;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine.UIElements;

namespace umi3d.DesktopBrowser.menu.Displayer
{
    public abstract partial class AbstractWindowInputDisplayer
    {
        public Displayer_E Displayer { get; protected set; } = null;

        private void OnDestroy()
        {
            Displayer.Remove();
        }
    }

    public partial class AbstractWindowInputDisplayer : IDisplayerElement
    {
        public VisualElement GetUXMLContent()
            => Displayer.Root;

        public virtual void InitAndBindUI()
            => Displayer = new Displayer_E();
    }

    public partial class AbstractWindowInputDisplayer : AbstractDisplayer
    {
        public override void Display(bool forceUpdate = false)
        {
            gameObject.SetActive(true);
            Displayer.Display();
        }

        public override void Hide()
        {
            Displayer.Hide();
            gameObject.SetActive(false);
        }

        public override void Clear()
        {
            base.Clear();
            Displayer.Reset();
        }
    }
}