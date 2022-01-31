/*
Copyright 2019 Gfi Informatique

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
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.uI.viewController
{
    public partial class Toolbox_E
    {
        
    }

    public partial class Toolbox_E
    {
        private Visual_E m_toolboxName { get;  set; } = null;
    }

    public partial class Toolbox_E
    {
        public Toolbox_E(string toolboxName, bool isScrollable, params ToolboxItem_E[] items) : 
            base("UI/UXML/Toolbox/Toolbox1", 
                "UI/Style/Toolbox/Toolbox", 
                new StyleKeys( "", null))
        {
            AddVisualStyle(Root.Q<Label>(), 
                "UI/Style/Toolbox/ToolboxName", 
                new StyleKeys(toolboxName, "", null, null));
            Adds(items);
            if (isScrollable)
            {

            }
            else
            {

            }
        }

        //public Toolbox_E AddItems(params ToolboxItem_E[] toolItems)
        //{
        //    for (int i = 0; i < toolItems.Length; ++i)
        //    {
        //        if (i > 0 || this.items.Count > 0) AddSpacer();
        //        toolItems[i].AddTo(itemsContainer);
        //        this.items.Add(toolItems[i]);
        //    }
        //    return this;
        //}

    }

    public partial class Toolbox_E : ScrollView_E
    {
        protected override void Initialize()
        {
            base.Initialize();

            //label = this.Q<Label>("toolbox-name");
            //itemsContainer = this.Q<VisualElement>("items-container");
        }
    }
}

