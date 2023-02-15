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
using System;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;
using static umi3d.commonScreen.menu.LibraryScreen_C;

namespace umi3d.commonScreen.menu
{
    public class LibraryHeader_C : Library_C
    {
        public enum SortTab
        {
            Ascending,
            Descending
        }

        public new class UxmlFactory : UxmlFactory<LibraryHeader_C, UxmlTraits> { }

        public new class UxmlTraits : Library_C.UxmlTraits
        {
            UxmlEnumAttributeDescription<LibrarySort> m_sort = new UxmlEnumAttributeDescription<LibrarySort>
            {
                name = "header-sort",
                defaultValue = LibrarySort.AscendingName
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
                var custom = ve as LibraryHeader_C;

                custom.HeaderSort = m_sort.GetValueFromBag(bag, cc);
            }
        }

        public override bool AllowDeletion
        {
            get => m_allowDeletion;
            set
            {
                m_allowDeletion = value;
                if (value) DropDown_Field.Add(DeleteField);
                else DeleteField.RemoveFromHierarchy();
            }
        }

        public virtual string StyleSheetHeaderPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/libraryHeader";

        public override string USSCustomClassDelete_Icon => $"{UssCustomClass_Emc}__header-search";
        public override string USSCustomClassDropDown_Button_Icon_Background => $"{UssCustomClass_Emc}__header-drop__down__button__icon__background";
        public virtual string USSCustomClassHeader_NameSort_Icon => $"{UssCustomClass_Emc}__header-name__sort__icon";
        public virtual string USSCustomClassHeader_SizeSort_Icon => $"{UssCustomClass_Emc}__header-size__sort__icon";
        public virtual string USSCustomClassHeader_Sort_Icon_Asc => $"{UssCustomClass_Emc}__header-sort__icon__ascending";
        public virtual string USSCustomClassHeader_Sort_Icon_Desc => $"{UssCustomClass_Emc}__header-sort__icon__descending";
        public virtual string USSCustomClassHeader_Search_Field => $"{UssCustomClass_Emc}__header-search__field";
        public virtual string USSCustomClassHeader_Filter_Field => $"{UssCustomClass_Emc}__header-filter__field";

        public VisualElement NameSort_Icon = new VisualElement();
        public VisualElement SizeSort_Icon = new VisualElement();
        public Textfield_C SearchField = new Textfield_C { name = "search-field" };
        public Dropdown_C FilterField = new Dropdown_C { name = "filter" };
        public Button_C DeleteField = new Button_C { name = "Delete" };

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetHeaderPath);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            NameSort_Icon.AddToClassList(USSCustomClassHeader_NameSort_Icon);
            SizeSort_Icon.AddToClassList(USSCustomClassHeader_SizeSort_Icon);
            SearchField.AddToClassList(USSCustomClassHeader_Search_Field);
            FilterField.AddToClassList(USSCustomClassHeader_Filter_Field);
        }

        protected override void InitElement()
        {
            base.InitElement();
            DropDown_Button.ClickedDownWithInfo += (evt, localPosition) =>
            {
                var worldPosition = DropDown_Button.LocalToWorld(localPosition);
                var title = TitleLabel.ContainsPoint(TitleLabel.WorldToLocal(worldPosition));
                var size = SizeLabel.ContainsPoint(SizeLabel.WorldToLocal(worldPosition));

                if (title) HeaderSort = NameSort != SortTab.Ascending ? LibrarySort.AscendingName : LibrarySort.DescendingName;
                if (size) HeaderSort = SizeSort != SortTab.Ascending ? LibrarySort.AscendingSize : LibrarySort.DescendingSize;
            };

            SearchField.LocaliseLabel = new LocalisationAttribute("Search", "LibrariesScreen", "Search_Label");
            SearchField.ValueChanged += value => Searched?.Invoke(value);

            FilterField.LocalisedLabel = "Filter";
            FilterField.ValueChanged += (index, value) => Filtered?.Invoke(value);

            DeleteField.LocaliseText = new LocalisationAttribute("Delete All libraries", "LibrariesScreen", "Delete_All_libraries");
            DeleteField.Type = ButtonType.Danger;

            Delete.clicked += () => DisplayMessage = !m_displayMessage;

            TitleLabel.Add(NameSort_Icon);
            SizeLabel.Add(SizeSort_Icon);
            Main.Add(Delete);
            DropDown_Field.Add(SearchField);
            DropDown_Field.Add(FilterField);
            DropDown_Field.Add(DeleteField);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Title = new LocalisationAttribute("Name", "LibrariesScreen", "Sort_Name");
            Size = new LocalisationAttribute("Size", "LibrariesScreen", "Sort_Size");
            HeaderSort = LibrarySort.AscendingName;
        }

        #region Implementation

        public LibrarySort HeaderSort
        {
            get => m_sort;
            set
            {
                m_sort = value;
                switch (m_sort)
                {
                    case LibrarySort.AscendingName:
                        NameSort_Icon.RemoveFromClassList(USSCustomClassHeader_Sort_Icon_Desc);
                        NameSort_Icon.AddToClassList(USSCustomClassHeader_Sort_Icon_Asc);
                        SizeSort_Icon.RemoveFromClassList(USSCustomClassHeader_Sort_Icon_Asc);
                        SizeSort_Icon.RemoveFromClassList(USSCustomClassHeader_Sort_Icon_Desc);
                        NameSort = SortTab.Ascending;
                        break;
                    case LibrarySort.DescendingName:
                        NameSort_Icon.RemoveFromClassList(USSCustomClassHeader_Sort_Icon_Asc);
                        NameSort_Icon.AddToClassList(USSCustomClassHeader_Sort_Icon_Desc);
                        SizeSort_Icon.RemoveFromClassList(USSCustomClassHeader_Sort_Icon_Asc);
                        SizeSort_Icon.RemoveFromClassList(USSCustomClassHeader_Sort_Icon_Desc);
                        NameSort = SortTab.Descending;
                        break;
                    case LibrarySort.AscendingSize:
                        SizeSort_Icon.RemoveFromClassList(USSCustomClassHeader_Sort_Icon_Desc);
                        SizeSort_Icon.AddToClassList(USSCustomClassHeader_Sort_Icon_Asc);
                        NameSort_Icon.RemoveFromClassList(USSCustomClassHeader_Sort_Icon_Asc);
                        NameSort_Icon.RemoveFromClassList(USSCustomClassHeader_Sort_Icon_Desc);
                        SizeSort = SortTab.Ascending;
                        break;
                    case LibrarySort.DescendingSize:
                        SizeSort_Icon.RemoveFromClassList(USSCustomClassHeader_Sort_Icon_Asc);
                        SizeSort_Icon.AddToClassList(USSCustomClassHeader_Sort_Icon_Desc);
                        NameSort_Icon.RemoveFromClassList(USSCustomClassHeader_Sort_Icon_Asc);
                        NameSort_Icon.RemoveFromClassList(USSCustomClassHeader_Sort_Icon_Desc);
                        SizeSort = SortTab.Descending;
                        break;
                    default:
                        break;
                }
                Sorted?.Invoke(m_sort);
            }
        }

        public System.Action<LibrarySort> Sorted;
        /// <summary>
        /// For textField search.
        /// </summary>
        public System.Action<ChangeEvent<string>> Searched;
        /// <summary>
        /// For dropdown filter.
        /// </summary>
        public System.Action<ChangeEvent<string>> Filtered;

        protected LibrarySort m_sort;
        protected SortTab NameSort;
        protected SortTab SizeSort;

        protected override void DropDownClicked()
        { }

        #endregion
    }
}
