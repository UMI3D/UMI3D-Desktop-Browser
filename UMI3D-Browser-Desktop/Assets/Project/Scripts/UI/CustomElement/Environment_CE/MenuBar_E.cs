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

//using BrowserDesktop.Menu.Environment.Settings;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


namespace umi3dDesktopBrowser.uI.viewController
{
    /// <summary>
    /// A menuBar has 3 layout (Tools area, Application settings area and Leave button area).
    /// </summary>
    public partial class MenuBar_E
    {
        public float Space { get; set; } = 10f;
        //public Action<VisualElement> AddSeparator { get; set; } = (ve) => { Debug.Log("<color=green>TODO: </color>" + $"AddSeparator in MenuBarElement."); };
        public VisualElement SubMenuLayout { get; private set; }
        public static MenuBar_E Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new MenuBar_E();
                }
                return m_instance;
            }
        }
    }

    public partial class MenuBar_E
    {
        protected ToolboxItem_E m_toolboxButton;
        protected ToolboxItem_E m_avatar;
        protected ToolboxItem_E m_sound;
        protected ToolboxItem_E m_mic;
        protected ToolboxItem_E m_leave;
        protected ScrollView_E m_scrollView { get; set; }

        private static MenuBar_E m_instance;
        private VisualElement leftLayout_VE;
        private VisualElement centerLayout_VE;
        private VisualElement rightLayout_VE;
    }

    public partial class MenuBar_E
    {
        private MenuBar_E() : 
            base("UI/UXML/MenuBar/menuBar1", 
                "UI/Style/MenuBar/MenuBar", 
                new StyleKeys("", null)) { }


        public void AddToolbox(params Toolbox_E[] toolboxes)
        {
            m_scrollView.Adds(toolboxes);
        }

        public MenuBar_E AddCenter(params Toolbox_E[] toolboxes)
        {
            //centerLayout_VE.AddToolboxes(toolboxes);
            return this;
        }


        public void AddInSubMenu(ToolboxGenericElement tools, ToolboxGenericElement parent)
        {
            tools.AddTo(SubMenuLayout);
            //tools.style.left = parent.ChangeCoordinatesTo(tools, new Vector2(parent.layout.x, parent.layout.y)).x;
            logWorldPosition = () =>
            {
                Debug.Log($"tool x = {tools.worldBound.x}");
                Debug.Log($"parent x = {parent.worldBound.x}");
                tools.style.left = parent.ChangeCoordinatesTo(tools, new Vector2(parent.layout.x, parent.layout.y)).x;

                //test.style.left = image.WorldToLocal(new Vector2(image.worldBound.x, 0f)).x;
            };
            //Menu.Environment.MenuBar_UIController.Instance.StartCoroutine(LogWorldPositionCoroutine());
        }
    }

    public partial class MenuBar_E
    {
        private IEnumerator LogWorldPositionCoroutine()
        {
            yield return null;

            logWorldPosition();
        }

        private Action logWorldPosition;
    }

    public partial class MenuBar_E : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();

            leftLayout_VE = Root.Q<VisualElement>("Left-layout");
            centerLayout_VE = Root.Q<VisualElement>("Center-layout");
            rightLayout_VE = Root.Q<VisualElement>("Right-layout");
            //SubMenuLayout = this.parent.Q<VisualElement>("sub-menu-layout");

            m_toolboxButton = new ToolboxItem_E("Toolbox", "Toolbox");
            new Toolbox_E("", false, m_toolboxButton)    
                .AddTo(leftLayout_VE);

            AddSeparator(leftLayout_VE);

            //Scroll view
            m_scrollView = new ScrollView_E(centerLayout_VE, "UI/UXML/MenuBar/toolboxesScrollView", null, null)
            {
                AddSeparator = AddSeparator
            };
            m_scrollView.SetHorizontalBackwardButtonStyle(m_scrollView.Root.Q("backwardButton"), 
                "UI/Style/MenuBar/ScrollView_Button", 
                new StyleKeys("backward", null));
            m_scrollView.SetHorizontalForwarddButtonStyle(m_scrollView.Root.Q("forwardButton"), 
                "UI/Style/MenuBar/ScrollView_Button", 
                new StyleKeys("forward", null));
            AddVisualStyle(m_scrollView.Root.Q("backward").Q("separator"), 
                "UI/Style/MenuBar/Separator", 
                new StyleKeys("", 
                null));
            AddVisualStyle(m_scrollView.Root.Q("forward").Q("separator"), 
                "UI/Style/MenuBar/Separator", 
                new StyleKeys("", 
                null));

            AddSeparator(rightLayout_VE);

            m_avatar = new ToolboxItem_E("AvatarOn", "AvatarOff", "");
            m_sound = new ToolboxItem_E("SoundOn", "SoundOff", "");
            m_mic = new ToolboxItem_E("MicOn", "MicOff", "");
            new Toolbox_E("", false, m_avatar, m_sound, m_mic)    
                .AddTo(rightLayout_VE);

            AddSeparator(rightLayout_VE);

            m_leave = new ToolboxItem_E("Leave", "");
            new Toolbox_E("", false, m_leave)
                .AddTo(rightLayout_VE);
        }

        protected Visual_E AddSeparator(VisualElement layout)
        {
            Visual_E separator = new Visual_E(new VisualElement(),
                "UI/Style/MenuBar/Separator",
                new StyleKeys("", null));
            separator.AddTo(layout);
            return separator;
        }
    }
}
