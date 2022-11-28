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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class CustomLibraryScreen : CustomMenuScreen
{
    public enum LibrarySort
    {
        AscendingName,
        DescendingName,
        AscendingSize,
        DescendingSize
    }

    public new class UxmlTraits : CustomMenuScreen.UxmlTraits
    {
        UxmlEnumAttributeDescription<LibrarySort> m_sort = new UxmlEnumAttributeDescription<LibrarySort>
        {
            name = "sort-by",
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
            var custom = ve as CustomLibraryScreen;

            custom.Set
                (
                    m_sort.GetValueFromBag(bag, cc)
                 );
        }
    }

    public override string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/libraryScreen";
    public override string USSCustomClassName => "library__screen";
    public virtual string USSCustomClassHeader => $"{USSCustomClassName}-header";
    public virtual string USSCustomClassScrollView => $"{USSCustomClassName}-scroll__view";

    public virtual LibrarySort SortBy
    {
        get => m_sort;
        set
        {
            m_sort = value;

            UpdateFilteredLibrariesOrder();
        }
    }

    public override string ShortScreenTitle => "libraries";
    public CustomLibraryHeader Header;
    public CustomScrollView Libraries_SV;

    protected LibrarySort m_sort;

    public override void InitElement()
    {
        base.InitElement();

        Header.AddToClassList(USSCustomClassHeader);
        Libraries_SV.AddToClassList(USSCustomClassScrollView);

        Add(Header);
        Add(Libraries_SV);

        Header.Sorted = (sort) => SortBy = sort;
        Header.Searched = Searched;
        Header.Filtered = Filtered;

        Libraries_SV.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
    }

    public override void Set() => Set(LibrarySort.AscendingName);

    public virtual void Set(LibrarySort sort)
    {
        m_isSet = false;
        Set("Libraries");

        SortBy = sort;
        Header.HeaderSort = sort;
        m_isSet = true;
    }

    public override VisualElement contentContainer => m_isSet ? Libraries_SV.contentContainer : this;

    #region Implementation

    public event System.Action<List<string>> WrongLibraryPathFound;

    protected List<umi3d.cdk.UMI3DResourcesManager.DataFile> m_libraries;
    protected List<string> m_environments = new List<string>();
    protected List<CustomLibrary> libraries = new List<CustomLibrary>();
    protected List<CustomLibrary> m_librariesFiltered = new List<CustomLibrary>();

    protected string m_searchedValue = "";
    protected string m_filteredValue = "";

    public void InitLibraries()
    {
        UpdateFilterdList(null);

        libraries.Clear();
        bool isThereWrongLibrariesPath = false;
        List<string> PathWrong = new List<string>();
        foreach (var lib in m_libraries)
        {
            // 1. Diplay lib name
            var library = CreateLibrary();
            library.Path = lib.path;
            library.Title = lib.key;

            //2. Display environments which use this lib
            library.Date = lib.date.Substring(0, 10);
            library.Message = "Used by:\n\n";
            library.Message += string.Join('\n', lib.applications);
            //UnityEngine.Debug.Log($"{lib.culture}, {lib.date}, {lib.dateformat}");

            //3. Display lib size
            DirectoryInfo dirInfo = new DirectoryInfo(lib.path);
            if (DirSize(dirInfo, out var dirSize))
            {
                dirSize /= Mathf.Pow(10, 6);
                dirSize = System.Math.Round(dirSize, 2);
                library.Size = dirSize.ToString() + " mo";
            }
            else
            {
                library.Size = "NaN";
                isThereWrongLibrariesPath = true;
                PathWrong.Add(lib.path);
            }

            //4.Bind the button to unistall this lib
            library.Delete.clicked += () =>
            {
                var dialogueBox = CreateDialogueBox();
                dialogueBox.Set
                    (
                        ElementCategory.Menu,
                        ElementSize.Medium,
                        DialogueboxType.Confirmation,
                        $"Do you want to uninstall this library ?",
                        $"'{lib.key}' is required by {lib.applications.Count} environments",
                        "Cancel",
                        "Delete"
                    );
                dialogueBox.Callback += (index) =>
                {
                    if (index != 1) return;
                    umi3d.cdk.UMI3DResourcesManager.RemoveLibrary(lib.key);
                    library.RemoveFromHierarchy();
                    libraries.Remove(library);
                    UpdateFilterdList(Header.FilterField.value);
                };
                dialogueBox.ChoiceA.Type = ButtonType.Default;
                dialogueBox.AddToTheRoot(this);
            };

            libraries.Add(library);
        }
        if (isThereWrongLibrariesPath) WrongLibraryPathFound?.Invoke(PathWrong);
        m_librariesFiltered.AddRange(libraries);

        UpdateOrder();
    }

    protected void UpdateFilteredLibrariesOrder()
    {
        switch (m_sort)
        {
            case LibrarySort.AscendingName:
                m_librariesFiltered?.Sort((lib0, lib1) => string.Compare(lib0.Title, lib1.Title));
                break;
            case LibrarySort.DescendingName:
                m_librariesFiltered?.Sort((lib0, lib1) => string.Compare(lib1.Title, lib0.Title));
                break;
            case LibrarySort.AscendingSize:
                m_librariesFiltered?.Sort((lib0, lib1) => string.Compare(lib0.Size, lib1.Size));
                break;
            case LibrarySort.DescendingSize:
                m_librariesFiltered?.Sort((lib0, lib1) => string.Compare(lib1.Size, lib0.Size));
                break;
            default:
                break;
        }

        UpdateOrder();
    }

    protected void UpdateOrder()
    {
        if (m_librariesFiltered == null) return;
        Libraries_SV.Clear();
        foreach (var lib in m_librariesFiltered) Libraries_SV.Add(lib);
    }

    /// <summary>
    /// For textfield search.
    /// </summary>
    /// <param name="ce"></param>
    protected void Searched(ChangeEvent<string> ce)
    {
        m_searchedValue = ce.newValue;
        UpdateSelection();
    }

    /// <summary>
    /// For dropdown filter.
    /// </summary>
    /// <param name="ce"></param>
    protected void Filtered(ChangeEvent<string> ce)
    {
        m_filteredValue = ce.newValue == "All" ? "" : ce.newValue;
        UpdateSelection();
    }

    protected void UpdateSelection()
    {
        m_librariesFiltered.Clear();
        foreach (var lib in libraries) if (lib.Title.Contains(m_searchedValue) && lib.Message.Contains(m_filteredValue)) m_librariesFiltered.Add(lib);
        UpdateFilteredLibrariesOrder();
    }

    protected void UpdateFilterdList(string current)
    {
        m_libraries = umi3d.cdk.UMI3DResourcesManager.Libraries;
        m_environments.Clear();
        Header.FilterField.choices.Clear();
        foreach (var lib in m_libraries)
        {
            if (lib.applications == null) return;

            foreach (var app in lib.applications) if (!m_environments.Contains(app)) m_environments.Add(app);
            Header.FilterField.choices.AddRange(m_environments);
        }
        Header.FilterField.choices.Insert(0, "All");
        if (string.IsNullOrEmpty(current) || !Header.FilterField.choices.Contains(current)) Header.FilterField.index = 0;
        else Header.FilterField.index = Header.FilterField.choices.IndexOf(current);
    }

    /// <summary>
    /// Returns the size of a directory in bytes.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public bool DirSize(DirectoryInfo d, out double size)
    {
        size = 0;
        try
        {
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis) size += fi.Length;

            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                if (!DirSize(di, out var subSize)) return false;
                size += subSize;
            }
        }
        catch (System.Exception)
        {
            UnityEngine.Debug.Log($"Libraries path not valide");
            return false;
        }
        
        return true;
    }

    protected abstract CustomLibrary CreateLibrary();
    protected abstract CustomDialoguebox CreateDialogueBox();

    #endregion
}
