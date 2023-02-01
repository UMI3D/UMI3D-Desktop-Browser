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
using umi3d.cdk.menu;
using umi3d.commonMobile.game;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.game;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public class PinnedToolsArea_C : VisualElement, ICustomElement
    {
        /// <summary>
        /// Current instance of this ui element. If there is no instance a new one is created.
        /// </summary>
        public static PinnedToolsArea_C Instance
        {
            get
            {
                if (s_instance != null) return s_instance;

                s_instance = new PinnedToolsArea_C();
                return s_instance;
            }
            set
            {
                s_instance = value;
            }
        }
        protected static PinnedToolsArea_C s_instance;

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            protected UxmlEnumAttributeDescription<ScrollViewMode> m_mode = new UxmlEnumAttributeDescription<ScrollViewMode>
            {
                name = "mode",
                defaultValue = ScrollViewMode.Vertical
            };

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="ve"></param>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                if (Application.isPlaying) return;

                base.Init(ve, bag, cc);
                var custom = ve as PinnedToolsArea_C;

                custom.Mode = m_mode.GetValueFromBag(bag, cc);
            }
        }

        /// <summary>
        /// Mode of the Scrollview (Vertical, Horizontal, Both).
        /// </summary>
        public virtual ScrollViewMode Mode
        {
            get => SDC.Mode;
            set
            {
                RemoveFromClassList(USSCustomClassMode(SDC.Mode));
                AddToClassList(USSCustomClassMode(value));
                SDC.Mode = value;
                ModeUpdated();
            }
        }

        public virtual string StyleSheetGamePath => $"USS/game";
        public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/pinnedTools";
        public virtual string USSCustomClassName => "pinned__tools__area";
        public virtual string USSCustomClassMode(ScrollViewMode mode) => $"{USSCustomClassName}-{mode}".ToLower();
        public virtual string USSCustomClassSDC => $"{USSCustomClassName}-sdc";
        public virtual string USSCustomClassSub_SDC => $"{USSCustomClassName}-sub__sdc";

        public ScrollableDataCollection_C<AbstractMenuItem> SDC = new ScrollableDataCollection_C<AbstractMenuItem> { name = "sdc" };
        public ScrollableExpandableDataCollection_C<AbstractMenuItem> Sub_SDC = new ScrollableExpandableDataCollection_C<AbstractMenuItem> { name = "sub-sdc" };

        public PinnedToolsArea_C() => Set();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void InitElement()
        {
            try
            {
                this.AddStyleSheetFromPath(StyleSheetGamePath);
                this.AddStyleSheetFromPath(StyleSheetPath);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            AddToClassList(USSCustomClassName);
            SDC.AddToClassList(USSCustomClassSDC);
            Sub_SDC.AddToClassList(USSCustomClassSub_SDC);

            SDC.MakeItem = datum => new Container.Toolbox_C();
            SDC.BindItem = (datum, item) =>
            {
                var toolbox = item as Toolbox_C;
                if (datum is MenuItem menuItem)
                {
                    UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"menu item = {menuItem.Name}");
                }
                else if (datum is Menu menu)
                {
                    toolbox.AddMenu(datum);
                    toolbox.Mode = Mode;
                    toolbox.ToolboxType = ToolboxType.Main;
                    toolbox.ToolClicked = OnToolClicked;
                    toolbox.ToolboxClicked = (isSelected, toolboxMenu, toolMenu) =>
                    {
                        Sub_SDC.ClearDC();
                        if (isSelected)
                        {
                            SDC.Select(toolboxMenu);
                            Add(Sub_SDC);
                            Sub_SDC.AddDatum(toolMenu);
                        }
                        else
                        {
                            SDC.Unselect(toolboxMenu);
                            Sub_SDC.RemoveFromHierarchy();
                        }
                    };
                }
            };
            SDC.UnbindItem = (datum, item) =>
            {
                var toolbox = item as Toolbox_C;
                toolbox.ClearToolbox();
                toolbox.ToolboxType = ToolboxType.Unknown;
                toolbox.ToolClicked = null;
                toolbox.ToolboxClicked = null;
            };
            SDC.SelectionType = SelectionType.Single;
            SDC.SelectItem = (datum, item) =>
            {
                var toolbox = item as Toolbox_C;
                toolbox.IsSelected = true;
            };
            SDC.UnselectItem = (datum, item) =>
            {
                var toolbox = item as Toolbox_C;
                toolbox.SDC.UnselectAll();
                toolbox.IsSelected = false;
            };
            SDC.ReorderableMode = ReorderableMode.Element;
            SDC.IsReorderable = true;
            SDC.Category = ElementCategory.Game;

            //Sub tools
            Sub_SDC.MakeItem = datum => new Toolbox_C();
            Sub_SDC.BindItem = (datum, item) =>
            {
                var toolbox = item as Toolbox_C;
                if (datum is MenuItem menuItem)
                {
                    UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"menu item = {menuItem.Name}");
                }
                else if (datum is Menu menu)
                {
                    toolbox.AddMenu(datum);
                    toolbox.Mode = ScrollViewMode.Horizontal;
                    toolbox.ToolboxType = ToolboxType.Main;
                    toolbox.ToolClicked = OnToolClicked;
                    toolbox.ToolboxClicked = (isSelected, toolboxMenu, toolMenu) =>
                    {
                        if (isSelected) Sub_SDC.AddDatum(toolMenu);
                        else Sub_SDC.RemoveDatum(toolMenu);
                    };
                }
            };
            Sub_SDC.UnbindItem = (datum, item) =>
            {
                var toolbox = item as Toolbox_C;
                toolbox.ToolboxType = ToolboxType.Unknown;
                toolbox.ToolClicked = null;
                toolbox.ToolboxClicked = null;
                toolbox.ClearToolbox();
            };
            Sub_SDC.Mode = ScrollViewMode.Vertical;
            Sub_SDC.IsReorderable = false;
            Sub_SDC.AnimationTimeIn = 1f;
            Sub_SDC.AnimationTimeOut = .5f;
            Sub_SDC.Category = ElementCategory.Game;

            Add(SDC);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void Set() => InitElement();

        #region Implementation

        /// <summary>
        /// Event raised when a tool is clicked. First argument is whether or not it is selected. Second is the menu of this tool.
        /// </summary>
        public System.Action<bool, AbstractMenuItem> ToolClicked;

        /// <summary>
        /// Add a menu to the pinned tools area.
        /// </summary>
        /// <param name="menu"></param>
        public virtual void AddMenu(AbstractMenuItem menu)
            => SDC.AddDatum(menu);

        /// <summary>
        /// Whether or not the tools inside the toolbox are reorderable.
        /// </summary>
        /// <param name="value"></param>
        public virtual void AreToolboxReorderable(bool value)
        {
            foreach (var item in SDC.DataToItem.Values)
                (item as Toolbox_C).SDC.IsReorderable = value;
        }

        protected virtual void ModeUpdated()
        {
            foreach (var item in SDC.DataToItem.Values)
                (item as Toolbox_C).Mode = Mode;
        }

        protected virtual void OnToolClicked(bool isSelected, AbstractMenuItem toolboxMenu, AbstractMenuItem toolMenu)
        {
            Sub_SDC.ClearDC();
            Sub_SDC.RemoveFromHierarchy();
            if (isSelected) SDC.Select(toolboxMenu);
            else SDC.Unselect(toolboxMenu);
            ToolClicked?.Invoke(isSelected, toolMenu);
        }

        #endregion
    }
}

namespace umi3d.UiPreview.commonScreen.game
{
    public class PinnedToolsArea_Preview: PinnedToolsArea_C
    {
        public new class UxmlFactory : UxmlFactory<PinnedToolsArea_Preview, UxmlTraits> 
        {
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                PinnedToolsArea_C previewItem = base.Create(bag, cc) as PinnedToolsArea_C;

                //Toolbox1
                Menu toolbox1 = new Menu { Name = "toolbox1" };

                //Tool1
                Menu tool1 = new Menu { Name = "tool1" };
                //Item1
                MenuItem item1 = new ButtonMenuItem { Name = "Button Item1" };
                tool1.Add(item1);
                //Item2
                DropDownInputMenuItem item2 = new DropDownInputMenuItem { Name = "Enum Item2", options = new List<string>() { "un", "deux", "trois" } };
                item2.NotifyValueChange("un");
                tool1.Add(item2);
                toolbox1.Add(tool1);

                //Tool2
                Menu tool2 = new Menu { Name = "tool2" };
                //Item3
                MenuItem item3 = new BooleanInputMenuItem { Name = "Toggle Item3" };
                tool2.Add(item3);
                //Item4
                MenuItem item4 = new FloatRangeInputMenuItem { Name = "Slider Item4", min = 0f, max = 50f, value = 0f };
                tool2.Add(item4);
                toolbox1.Add(tool2);

                //Toolbox2
                Menu toolbox2 = new Menu { Name = "toolbox2" };

                //Toolbox3
                Menu toolbox3 = new Menu { Name = "toolbox3" };
                toolbox2.Add(toolbox3);

                //Tool4
                Menu tool4 = new Menu { Name = "tool4" };
                //Item5
                MenuItem item5 = new TextInputMenuItem { Name = "Text Item5" };
                tool4.Add(item5);
                toolbox3.Add(tool4);

                //Toolbox4
                Menu toolbox4 = new Menu { Name = "toolbox4" };
                toolbox3.Add(toolbox4);

                //Tool5
                Menu tool5 = new Menu { Name = "tool5" };
                //Item5
                MenuItem item6 = new TextInputMenuItem { Name = "Text Item6" };
                tool5.Add(item6);
                toolbox4.Add(tool5);

                previewItem.AddMenu(toolbox1);
                previewItem.AddMenu(toolbox2);

                return previewItem;
            }
        }
    }
}
