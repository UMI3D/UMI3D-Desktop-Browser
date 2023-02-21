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
using System;
using System.Collections.Generic;
using umi3d.cdk.menu;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.game;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public class ToolsWindow_C : FormSDC_C<AbstractMenuItem>
    {
        public virtual string USSCustomClassToolsWindow => "tools__window";
        public virtual string USSCustomClassInputsBox => $"{USSCustomClassToolsWindow}-inputs__box";

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            AddToClassList(USSCustomClassToolsWindow);
        }

        protected override void InitElement()
        {
            base.InitElement();
            
            /// Scrollable data collection.
            /// Items: Expandable data collection.
            /// 
            /// Each item caontains Toolboxes and/or EDC (Inputs box).
            SDC.MakeItem = datum => new ExpandableDataCollection_C<AbstractMenuItem>();
            SDC.BindItem = BindSDC;
            SDC.UnbindItem = (datum, item) =>
            {
                ExpandableDataCollection_C<AbstractMenuItem> edc = item as ExpandableDataCollection_C<AbstractMenuItem>;

                edc.AnimationTimeIn = 0f;
                edc.AnimationTimeOut = 0f;

                edc.ClearDC();

                edc.MakeItem = null;
                edc.BindItem = null;
                edc.UnbindItem = null;
                edc.FindItem = null;
            };
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Title = "Toolbox";
        }

        #region Implementation

        public AbstractMenuItem RootMenu;

        /// <summary>
        /// Action raised when the pin status of a menu change.
        /// </summary>
        public Action<bool, AbstractMenuItem> Pinned;

        /// <summary>
        /// Add the root menu to this tools items window.
        /// </summary>
        /// <param name="menu"></param>
        public virtual void AddRoot(AbstractMenuItem menu)
        {
            RootMenu = menu;

            if (menu is Menu _menu)
            {
                if (_menu.SubMenu.Count > 0) foreach (var subMenu in _menu.SubMenu) SDC.AddDatum(subMenu);

                _menu.onAbstractMenuItemAdded.AddListener(SDC.AddDatum);
                _menu.OnAbstractMenuItemRemoved.AddListener(SDC.RemoveDatum);
            }
            else if (menu is MenuItem menuItem) throw new System.Exception("Should not happen");
        }

        protected virtual void BindSDC(AbstractMenuItem datum, VisualElement item)
        {
            /// Expandable data collection.
            /// Items: Toolboxes and/or EDC
            /// 
            /// The first item is alwayse a Toolbox. The last item is always an EDC.
            ExpandableDataCollection_C<AbstractMenuItem> edc = item as ExpandableDataCollection_C<AbstractMenuItem>;

            edc.MakeItem = edcDatum =>
            {
                if (edcDatum is Menu menu)
                {
                    if (menu.SubMenu.Count > 0) return new Toolbox_C();
                    else if (menu.MenuItems.Count > 0) return new ExpandableDataCollection_C<AbstractMenuItem>();
                    else throw new System.Exception("menu empty");
                }
                else
                {
                    UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"");
                    return null;
                }
            };
            Action pinned = null;
            edc.BindItem = (edcDatum, edcItem) => BindEDC(edc, edcDatum, edcItem, ref pinned);
            edc.UnbindItem = (edcDatum, edcItem) => UnbindEDC(edcDatum, edcItem, pinned);
            edc.FindItem = param =>
            {
                if (param.Item1 is Menu menu)
                {
                    if (menu.SubMenu.Count > 0 && param.Item2 is Toolbox_C) return true;
                    else if (menu.MenuItems.Count > 0 && param.Item2 is ExpandableDataCollection_C<AbstractMenuItem>) return true;
                }
                return false;
            };

            edc.AnimationTimeIn = 1f;
            edc.AnimationTimeOut = 0.5f;

            if (datum is Menu menu)
            {
                menu.onContentChange.AddListener(() =>
                {
                    if (menu.Count == 1) edc.AddDatum(datum);
                    else if (menu.Count == 0) edc.RemoveDatum(datum);
                });
            }
        }

        protected virtual void BindEDC(ExpandableDataCollection_C<AbstractMenuItem> edc, AbstractMenuItem datum, VisualElement item, ref Action pinned)
        {
            if (item is Toolbox_C toolbox)
            {
                toolbox.AddMenu(datum);
                toolbox.Mode = ScrollViewMode.Horizontal;
                if (edc.Data.IndexOf(datum) == 0) toolbox.ToolboxType = ToolboxType.Main;
                else toolbox.ToolboxType = ToolboxType.Sub;
                toolbox.ToolClicked = (isSelected, toolboxMenu, toolMenu) =>
                {
                    if (isSelected)
                    {
                        edc.AddDatum(toolMenu);
                    }
                    else
                    {
                        edc.RemoveDatum(toolMenu);
                    }
                };
                toolbox.ToolboxClicked = (isSelected, toolboxMenu, toolMenu) =>
                {
                    if (isSelected)
                    {
                        edc.AddDatum(toolMenu);
                    }
                    else
                    {
                        int index = edc.Data.IndexOf(toolMenu);
                        var range = edc.Data.GetRange(index, edc.Data.Count - index);
                        range.ForEach(e => edc.RemoveDatum(e));
                    }
                };
                pinned = () =>
                {
                    toolbox.IsPinned = !toolbox.IsPinned;
                    toolbox.IsSelected = toolbox.IsPinned;
                    Pinned?.Invoke(toolbox.IsPinned, datum);
                };
                toolbox.PinnedButton.clicked += pinned;
            }
            else if (item is ExpandableDataCollection_C<AbstractMenuItem> inputs)
            {
                var menu = datum as Menu;

                inputs.AddToClassList(USSCustomClassInputsBox);
                inputs.MakeItem = inputsDatum => DisplayerManager.MakeDisplayer(inputsDatum);
                inputs.BindItem = (inputsDatum, inputsItem) =>
                {
                    DisplayerManager.BindItem(inputsDatum, inputsItem);
                };
                inputs.UnbindItem = (inputsDatum, inputsItem) =>
                {
                    DisplayerManager.UnbindItem(inputsItem);
                };
                inputs.FindItem = param => DisplayerManager.IsCompatible(param.Item1, param.Item2);

                foreach (var menuItem in menu.MenuItems) inputs.AddDatum(menuItem);
            }
        }

        protected virtual void UnbindEDC(AbstractMenuItem datum, VisualElement item, Action pinned)
        {
            if (item is Toolbox_C toolbox)
            {
                toolbox.ToolboxType = ToolboxType.Unknown;
                toolbox.ToolClicked = null;
                toolbox.ToolboxClicked = null;
                toolbox.PinnedButton.clicked -= pinned;
                toolbox.ClearToolbox();
            }
            else if (item is ExpandableDataCollection_C<AbstractMenuItem> inputs)
            {
                inputs.ClearDC();
                inputs.MakeItem = null;
                inputs.BindItem = null;
                inputs.UnbindItem = null;
            }
        }

        #endregion
    }
}

namespace umi3d.UiPreview.commonScreen.game
{
    public class ToolsWindow_Preview: ToolsWindow_C
    {
        public new class UxmlFactory : UxmlFactory<ToolsWindow_Preview, UxmlTraits>
        {
            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            /// <returns></returns>
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                ToolsWindow_C previewItem = base.Create(bag, cc) as ToolsWindow_C;

                //Root
                Menu root = new Menu { Name = "root" };

                //Toolbox1
                Menu toolbox1 = new Menu { Name = "toolbox1" };
                root.Add(toolbox1);

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
                root.Add(toolbox2);

                //Toolbox3
                Menu toolbox3 = new Menu { Name = "toolbox3" };
                toolbox2.Add(toolbox3);

                //Tool4
                Menu tool4 = new Menu { Name = "tool4" };
                //Item5
                MenuItem item5 = new TextInputMenuItem { Name = "Text Item5" };
                tool4.Add(item5);
                toolbox3.Add(tool4);

                previewItem.Category = ElementCategory.Game;
                previewItem.Title = "Toolbox";
                previewItem.AddRoot(root);

                return previewItem;
            }
        }
    }
}
