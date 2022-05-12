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
using System.Collections;
using umi3d.cdk.menu;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    /// <summary>
    /// A menuBar has 3 layout (Tools area, Application settings area and Leave button area).
    /// </summary>
    public partial class MenuBar_E
    {
        public ToolboxItem_E ToolboxButton { get; private set; } = null;
        
        public VisualElement SubMenuLayout { get; private set; } = null;


        protected ScrollView_E m_scrollView { get; set; }

        private VisualElement leftLayout_VE;
        private VisualElement centerLayout_VE;

        public static bool AreThereToolboxes { get; set; } = false;

        #region Pin Unpin
        public event Action<bool, Menu> PinnedUnpinned;

        public void OnPinUnpin(bool value, Menu menu)
            => PinnedUnpinned?.Invoke(value, menu);
        #endregion

        public void DisplaySubMenu(bool value)
            => SubMenuLayout.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;

        /// <summary>
        /// Add [toolboxes] in the menu bar's scroll view.
        /// </summary>
        /// <param name="toolboxes"></param>
        public void AddToolboxDeep0(params Toolbox_E[] toolboxes)
            => m_scrollView.AddRange(toolboxes);

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

        private IEnumerator AnimeWindowVisibility(bool state)
        {
            yield return new WaitUntil(() => Root.resolvedStyle.height > 0f);
            Anime(Root, -Root.resolvedStyle.height, 0, 100, state, (elt, val) =>
            {
                elt.style.top = val;
            });
        }
    }

    public partial class MenuBar_E : ISingleUI
    {
        public static MenuBar_E Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new MenuBar_E();
                return m_instance;
            }
        }

        private static MenuBar_E m_instance;
    }

    public partial class MenuBar_E : Box_E
    {
        public override void Reset()
        {
            base.Reset();
            m_scrollView.Reset();
            ToolboxButton.ResetClickedEvent();
        }

        public override void InsertRootTo(VisualElement parent)
            => InsertRootAtTo(0, parent);

        public override void Display()
        {
            if (!AreThereToolboxes)
                return;
            UIManager.StartCoroutine(AnimeWindowVisibility(true));
            IsDisplaying = true;
            OnDisplayedOrHiddenTrigger(true);
        }

        public override void Hide()
        {
            UIManager.StartCoroutine(AnimeWindowVisibility(false));
            IsDisplaying = false;
            OnDisplayedOrHiddenTrigger(false);
        }

        protected override void Initialize()
        {
            base.Initialize();

            leftLayout_VE = Root.Q<VisualElement>("Left-layout");
            centerLayout_VE = Root.Q<VisualElement>("Center-layout");

            #region Sub menu layout

            SubMenuLayout = new VisualElement();
            SubMenuLayout.name = "subMenuLayout";
            SubMenuLayout.style.position = Position.Absolute;
            SubMenuLayout.style.width = Length.Percent(100f);
            SubMenuLayout.style.height = Length.Percent(100f);
            SubMenuLayout.RegisterCallback<MouseDownEvent>(OnSubMenuMouseDown);

            #endregion

            ToolboxButton = ToolboxItem_E.NewMenuItem("Toolbox");
            
            Toolbox_E
                .NewMenuToolbox("", ToolboxButton)
                .InsertRootTo(leftLayout_VE);

            AddSeparator(leftLayout_VE);

            #region ScrollView

            var scrollViewBox = new View_E("UI/UXML/horizontalScrollView", null, null);
            scrollViewBox.InsertRootTo(centerLayout_VE);
            VisualElement backward = scrollViewBox.QR("backward");
            VisualElement forward = scrollViewBox.QR("forward");

            m_scrollView = new ScrollView_E(scrollViewBox.QR<ScrollView>());
            m_scrollView.CreateSeparator = CreateSeparator;
            m_scrollView.HSliderValueChanged += (value, low, high) =>
            {
                backward.visible = (value > low) ? true : false;
                forward.visible = (value < high) ? true : false;
            };

            m_scrollView.SetHBackwardButton("ButtonH", StyleKeys.Bg("backward"));
            m_scrollView.SetHForwarddButton("ButtonH", StyleKeys.Bg("forward"));
            backward.Insert(0, m_scrollView.HBackwardButton);
            forward.Insert(1, m_scrollView.HForwardButton);

            new Icon_E(backward.Q("separator"), "SeparatorVertical", StyleKeys.DefaultBackground);
            new Icon_E(forward.Q("separator"), "SeparatorVertical", StyleKeys.DefaultBackground);

            #endregion
        }

        protected void AddSeparator(VisualElement layout)
            => CreateSeparator().InsertRootTo(layout);
        protected Icon_E CreateSeparator()
        {
            var separator = new Icon_E("SeparatorVertical", StyleKeys.DefaultBackground);
            separator.Root.name = "separator";
            return separator;
        }

        private MenuBar_E() :
            base("UI/UXML/Menus/menuBar", "MenuBar", StyleKeys.Bg("light"))
        { }
    }
}
