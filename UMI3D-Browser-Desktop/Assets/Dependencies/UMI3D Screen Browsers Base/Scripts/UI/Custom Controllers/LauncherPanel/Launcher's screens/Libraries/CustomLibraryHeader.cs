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
using System.Collections.Generic;
using umi3d.baseBrowser.ui.viewController;
using UnityEngine;
using UnityEngine.UIElements;
using static CustomLibraryScreen;

public abstract class CustomLibraryHeader : CustomLibrary
{
    public enum SortTab
    {
        Ascending,
        Descending
    }

    public new class UxmlTraits : CustomLibrary.UxmlTraits
    {
        UxmlEnumAttributeDescription<LibrarySort> m_sort = new UxmlEnumAttributeDescription<LibrarySort>
        {
            name = "header-sort",
            defaultValue = LibrarySort.AscendingName
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as CustomLibraryHeader;

            custom.Set
                (
                    m_sort.GetValueFromBag(bag, cc),
                    m_displayMessage.GetValueFromBag(bag, cc)
                 );
        }
    }

    public virtual string StyleSheetHeaderPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/libraryHeader";
    public override string USSCustomClassDelete_Icon => $"{USSCustomClassName}__header-search";
    public override string USSCustomClassDropDown_Button_Icon_Background => $"{USSCustomClassName}__header-drop__down__button__icon__background";
    public virtual string USSCustomClassHeader_NameSort_Icon => $"{USSCustomClassName}__header-name__sort__icon";
    public virtual string USSCustomClassHeader_SizeSort_Icon => $"{USSCustomClassName}__header-size__sort__icon";
    public virtual string USSCustomClassHeader_Sort_Icon_Asc => $"{USSCustomClassName}__header-sort__icon__ascending";
    public virtual string USSCustomClassHeader_Sort_Icon_Desc => $"{USSCustomClassName}__header-sort__icon__descending";
    public virtual string USSCustomClassHeader_Search_Field => $"{USSCustomClassName}__header-search__field";
    public virtual string USSCustomClassHeader_Filter_Field => $"{USSCustomClassName}__header-filter__field";

    public VisualElement NameSort_Icon = new VisualElement();
    public VisualElement SizeSort_Icon = new VisualElement();
    public CustomTextfield SearchField;
    public CustomDropdown FilterField;

    public override void InitElement()
    {
        base.InitElement();

        try
        {
            this.AddStyleSheetFromPath(StyleSheetHeaderPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }

        NameSort_Icon.AddToClassList(USSCustomClassHeader_NameSort_Icon);
        SizeSort_Icon.AddToClassList(USSCustomClassHeader_SizeSort_Icon);
        SearchField.AddToClassList(USSCustomClassHeader_Search_Field);
        FilterField.AddToClassList(USSCustomClassHeader_Filter_Field);

        TitleLabel.Add(NameSort_Icon);
        SizeLabel.Add(SizeSort_Icon);
        DropDown_Field.Add(SearchField);
        DropDown_Field.Add(FilterField);

        DropDown_Button.ClickedDownWithInfo += (evt, localPosition) =>
        {
            var worldPosition = DropDown_Button.LocalToWorld(localPosition);
            var title = TitleLabel.ContainsPoint(TitleLabel.WorldToLocal(worldPosition));
            var size = SizeLabel.ContainsPoint(SizeLabel.WorldToLocal(worldPosition));

            if (title) HeaderSort = NameSort != SortTab.Ascending ? LibrarySort.AscendingName : LibrarySort.DescendingName;
            if (size) HeaderSort = SizeSort != SortTab.Ascending ? LibrarySort.AscendingSize : LibrarySort.DescendingSize;
        };

        SearchField.label = "Search";
        SearchField.RegisterValueChangedCallback(value => Searched?.Invoke(value));

        FilterField.label = "Filter";
        FilterField.RegisterValueChangedCallback(value => Filtered?.Invoke(value));

        Delete.clicked += () => DisplayMessage = !m_displayMessage;
    }

    public override void Set() => Set(LibrarySort.AscendingName, false);

    public virtual void Set(LibrarySort sort, bool displayMessage)
    {
        Set("Name", "Size", null, null, displayMessage);

        HeaderSort = sort;
    }

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
}
