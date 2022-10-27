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

namespace umi3d.mobileBrowser.Container
{
    public partial class ObjectMenuFormContainer : IDisplayerElement
    {
        public VisualElement GetUXMLContent() => baseBrowser.connection.BaseGamePanelController.Instance.Game.TrailingArea.ObjectMenu;

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
        public override void Display(bool forceUpdate = false) 
        {
            if (!baseBrowser.connection.BaseGamePanelController.Exists || baseBrowser.connection.BaseGamePanelController.Instance.GamePanel.CurrentView != CustomGamePanel.GameViews.Game) return;
            baseBrowser.connection.BaseGamePanelController.Instance.Game.TrailingArea.DisplayObjectMenu = true;
        }

        public override void Hide() 
        {
            if (!baseBrowser.connection.BaseGamePanelController.Exists) return;
            baseBrowser.connection.BaseGamePanelController.Instance.Game.TrailingArea.DisplayObjectMenu = false;
        }

        public override void Insert(AbstractDisplayer element, int index, bool updateDisplay = true)
        {
            if (!(element is IDisplayerElement displayer)) return;

            baseBrowser.connection.BaseGamePanelController.Instance.Game.TrailingArea.ObjectMenu.Insert(index, displayer.GetUXMLContent());
            m_displayers.Insert(index, element);
        }

        public override bool Remove(AbstractDisplayer element, bool updateDisplay = true)
        {
            if (!(element is IDisplayerElement displayer)) return false;
            if (!Contains(element)) return false;

            baseBrowser.connection.BaseGamePanelController.Instance.Game.TrailingArea.ObjectMenu.Remove(displayer.GetUXMLContent());
            m_displayers.Remove(element);
            return true;
        }
    }
}
