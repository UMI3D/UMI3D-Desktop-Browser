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
using System;
using umi3d.cdk.menu;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    /// <summary>
    /// A menuBar has 3 layout (Tools area, Application settings area and Leave button area).
    /// </summary>
    public partial class MenuBar_E
    {
        public ToolboxItem_E ToolboxButton { get; private set; } = null;
        public ToolboxItem_E Avatar { get; private set; } = null;
        public ToolboxItem_E Sound { get; private set; } = null;
        public ToolboxItem_E Mic { get; private set; } = null;
        public ToolboxItem_E LeaveButton { get; private set; } = null;
        public VisualElement SubMenuLayout { get; private set; } = null;

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

        protected ScrollView_E m_scrollView { get; set; }

        private VisualElement leftLayout_VE;
        private VisualElement centerLayout_VE;
        private VisualElement rightLayout_VE;

        private static MenuBar_E m_instance;
        private static string m_menuUXML => "UI/UXML/Menus/menuBar";
        private static string m_menuStyle => "UI/Style/Menus/Menus";
        private static StyleKeys m_menuKeys => new StyleKeys(null, "", null);
        private static string m_separatorStyle => "UI/Style/MenuBar/Separator";
        private static StyleKeys m_separatorKeys => new StyleKeys(null, "", null);
    }

    public partial class MenuBar_E
    {
        #region Pin Unpin
        public event Action<bool, Menu> OnPinnedUnpinned;

        public void PinUnpin(bool value, Menu menu)
            => OnPinnedUnpinned?.Invoke(value, menu);
        #endregion

        public void DisplayToolboxButton(bool value)
        {
            if (value) ToolboxButton.Display();
            else ToolboxButton.Hide();
        }

        public void DisplaySubMenu(bool value)
            => SubMenuLayout.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;

        /// <summary>
        /// Add [toolboxes] in the menu bar's scroll view.
        /// </summary>
        /// <param name="toolboxes"></param>
        public void AddToolboxDeep0(params Toolbox_E[] toolboxes)
            => m_scrollView.Add(toolboxes);

        public void RemoveToolboxDeep0(Toolbox_E toolbox)
            => m_scrollView.Remove(toolbox);

        public void AddToolboxDeep1Plus(Toolbox_E toolbox)
        {
            toolbox.InsertRootTo(SubMenuLayout);

            //tools.style.left = parent.ChangeCoordinatesTo(tools, new Vector2(parent.layout.x, parent.layout.y)).x;
            //logWorldPosition = () =>
            //{
            //    Debug.Log($"tool x = {tools.worldBound.x}");
            //    Debug.Log($"parent x = {parent.worldBound.x}");
            //    tools.style.left = parent.ChangeCoordinatesTo(tools, new Vector2(parent.layout.x, parent.layout.y)).x;

            //    //test.style.left = image.WorldToLocal(new Vector2(image.worldBound.x, 0f)).x;
            //};
            //Menu.Environment.MenuBar_UIController.Instance.StartCoroutine(LogWorldPositionCoroutine());
        }

        //private IEnumerator LogWorldPositionCoroutine()
        //{
        //    yield return null;

        //    logWorldPosition();
        //}

        //private Action logWorldPosition;

        public event Action OnSubMenuMouseDownEvent;

        private void OnSubMenuMouseDown(MouseDownEvent e)
            => OnSubMenuMouseDownEvent?.Invoke();
    }

    public partial class MenuBar_E : Visual_E
    {
        public override void Reset()
        {
            base.Reset();
            m_scrollView.Reset();
        }

        public override void InsertRootTo(VisualElement parent)
            => InsertRootAtTo(0, parent);

        protected override void Initialize()
        {
            base.Initialize();

            leftLayout_VE = Root.Q<VisualElement>("Left-layout");
            centerLayout_VE = Root.Q<VisualElement>("Center-layout");
            rightLayout_VE = Root.Q<VisualElement>("Right-layout");

            #region Sub menu layout

            SubMenuLayout = new VisualElement();
            SubMenuLayout.name = "subMenuLayout";
            SubMenuLayout.style.position = Position.Absolute;
            SubMenuLayout.style.width = Length.Percent(100f);
            SubMenuLayout.style.height = Length.Percent(100f);
            SubMenuLayout.RegisterCallback<MouseDownEvent>(OnSubMenuMouseDown);

            #endregion

            ToolboxButton = new ToolboxItem_E("Toolbox", "Toolbox");
            new Toolbox_E("", ToolboxType.Pinned, ToolboxButton)
                .InsertRootTo(leftLayout_VE);

            AddSeparator(leftLayout_VE);

            #region ScrollView

            string scrollViewUXML = "UI/UXML/horizontalScrollView";
            m_scrollView = new ScrollView_E(scrollViewUXML)
            {
                CreateSeparator = CreateSeparator
            };
            m_scrollView.InsertRootTo(centerLayout_VE);

            VisualElement backwardContainer = m_scrollView.Root.Q("backwardButton");
            VisualElement backwardLayout = m_scrollView.Root.Q("backward");
            string ButtonStyle = "UI/Style/MenuBar/ScrollView_Button";
            StyleKeys backwardButtonKeys = new StyleKeys(null, "backward", null);
            m_scrollView.SetHorizontalBackwardButtonStyle(backwardContainer, backwardLayout, ButtonStyle, backwardButtonKeys);

            VisualElement forwardContainer = m_scrollView.Root.Q("forwardButton");
            VisualElement forwardLayout = m_scrollView.Root.Q("forward");
            StyleKeys forwardButtonKeys = new StyleKeys(null, "forward", null);
            m_scrollView.SetHorizontalForwarddButtonStyle(forwardContainer, forwardLayout, ButtonStyle, forwardButtonKeys);

            VisualElement backwardSeparator = m_scrollView.Root.Q("backward").Q("separator");
            AddVisualStyle(backwardSeparator, m_separatorStyle, m_separatorKeys);
            VisualElement forwardSeparator = m_scrollView.Root.Q("forward").Q("separator");
            AddVisualStyle(forwardSeparator, m_separatorStyle, m_separatorKeys);

            #endregion

            AddSeparator(rightLayout_VE);

            Avatar = new ToolboxItem_E("AvatarOn", "AvatarOff", "");
            Sound = new ToolboxItem_E("SoundOn", "SoundOff", "");
            Mic = new ToolboxItem_E("MicOn", "MicOff", "");
            new Toolbox_E("", ToolboxType.Pinned, Avatar, Sound, Mic)
                .InsertRootTo(rightLayout_VE);

            AddSeparator(rightLayout_VE);

            LeaveButton = new ToolboxItem_E("Leave", "");
            new Toolbox_E("", ToolboxType.Pinned, LeaveButton)
                .InsertRootTo(rightLayout_VE);
        }

        protected void AddSeparator(VisualElement layout)
            => CreateSeparator().InsertRootTo(layout);
        protected Visual_E CreateSeparator()
            => new Visual_E(new VisualElement(), m_separatorStyle, m_separatorKeys);

        private MenuBar_E() :
            base(m_menuUXML, m_menuStyle, m_menuKeys)
        { }
    }
}