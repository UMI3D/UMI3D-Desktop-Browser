/*
Copyright 2019 - 2022 Inetum

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
using umi3d.baseBrowser.Menu;
using umi3d.cdk.menu.view;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Container
{
    public partial class ObjectMenuFormContainer : IDisplayerElement
    {
        public System.Func<VisualElement> GetContainer;

        public VisualElement GetUXMLContent() => GetContainer != null ? GetContainer() : null;

        public void InitAndBindUI()
        {
        }

        private void Start()
        {
            InitAndBindUI();
        }
    }

    public partial class ObjectMenuFormContainer : AbstractSimpleContainer
    {
        public System.Action<bool> DisplayObjectMenu;
        public System.Action<int, VisualElement> InsertDisplayer;
        public System.Action<VisualElement> RemoveDisplayer;

        public override void Display(bool forceUpdate = false) => DisplayObjectMenu?.Invoke(true);

        public override void Hide() => DisplayObjectMenu?.Invoke(false);

        public override void Insert(AbstractDisplayer element, int index, bool updateDisplay = true)
        {
            if (!(element is IDisplayerElement displayer)) return;

            InsertDisplayer?.Invoke(index, displayer.GetUXMLContent());
            m_displayers.Insert(index, element);
        }

        public override bool Remove(AbstractDisplayer element, bool updateDisplay = true)
        {
            if (!(element is IDisplayerElement displayer)) return false;
            if (!Contains(element)) return false;

            RemoveDisplayer?.Invoke(displayer.GetUXMLContent());
            m_displayers.Remove(element);
            return true;
        }
    }
}
