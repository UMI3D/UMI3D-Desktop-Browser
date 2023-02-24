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
using System.IO;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.menu
{
    public class LibraryScreen_C : BaseMenuScreen_C
    {
        public enum LibrarySort
        {
            AscendingName,
            DescendingName,
            AscendingSize,
            DescendingSize
        }

        public new class UxmlFactory : UxmlFactory<LibraryScreen_C, UxmlTraits> { }

        public new class UxmlTraits : BaseMenuScreen_C.UxmlTraits
        {
            protected UxmlEnumAttributeDescription<LibrarySort> m_sort = new UxmlEnumAttributeDescription<LibrarySort>
            {
                name = "sort-by",
                defaultValue = LibrarySort.AscendingName
            };
            protected UxmlBoolAttributeDescription m_allowDeletion = new UxmlBoolAttributeDescription
            {
                name = "allow-deletion",
                defaultValue = false
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
                var custom = ve as LibraryScreen_C;

                custom.SortBy = m_sort.GetValueFromBag(bag, cc);
                custom.AllowDeletion = m_allowDeletion.GetValueFromBag(bag, cc);
            }
        }

        public override string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/libraryScreen";
        public override string UssCustomClass_Emc => "library__screen";
        public virtual string USSCustomClassHeader => $"{UssCustomClass_Emc}-header";
        public virtual string USSCustomClassScrollView => $"{UssCustomClass_Emc}-scroll__view";

        public virtual LibrarySort SortBy
        {
            get => m_sort;
            set
            {
                m_sort = value;

                UpdateFilteredLibrariesOrder();
            }
        }

        public virtual bool AllowDeletion
        {
            get => m_allowDeletion;
            set
            {
                m_allowDeletion = value;
                Header.AllowDeletion = value;
            }
        }

        public override LocalisationAttribute ShortScreenTitle => new LocalisationAttribute("libraries", "LauncherScreen", "Libraries");
        public LibraryHeader_C Header = new LibraryHeader_C();
        public ScrollView_C Libraries_SV = new ScrollView_C { name = "scroll-view" };

        protected LibrarySort m_sort;
        protected bool m_allowDeletion;

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            Header.AddToClassList(USSCustomClassHeader);
            Libraries_SV.AddToClassList(USSCustomClassScrollView);
        }

        protected override void InitElement()
        {
            base.InitElement();
            Add(Header);
            Add(Libraries_SV);

            Header.Sorted = (sort) => SortBy = sort;
            Header.Searched = Searched;
            Header.Filtered = Filtered;
            Header.DeleteField.clicked += DeleteFilteredLibraries;

            Libraries_SV.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Title = new LocalisationAttribute("libraries", "LauncherScreen", "Libraries");
            SortBy = LibrarySort.AscendingName;
            AllowDeletion = false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override VisualElement contentContainer => IsSet ? Libraries_SV.contentContainer : this;

        #region Implementation

        public event System.Action<List<string>> WrongLibraryPathFound;

        protected List<umi3d.cdk.UMI3DResourcesManager.DataFile> m_libraries;
        protected List<string> m_environments = new List<string>();
        protected List<Library_C> libraries = new List<Library_C>();
        protected List<Library_C> m_librariesFiltered = new List<Library_C>();

        protected string m_searchedValue = "";
        protected string m_filteredValue = "";

        /// <summary>
        /// Initialize the libraries.
        /// </summary>
        public void InitLibraries()
        {
            UpdateFilterdList(null);

            libraries.Clear();
            m_librariesFiltered.Clear();
            bool isThereWrongLibrariesPath = false;
            List<string> PathWrong = new List<string>();
            foreach (var lib in m_libraries)
            {
                // 1. Diplay lib name
                var library = new Library_C();
                library.AllowDeletion = m_allowDeletion;
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
                    var dialogueBox = new Dialoguebox_C
                    {
                        Category = ElementCategory.Menu,
                        Size = ElementSize.Medium,
                        Type = DialogueboxType.Confirmation,
                        Title = new LocalisationAttribute($"Do you want to uninstall this library ?", "LibrariesScreen", "Delete_library_Title"),
                        Message = new LocalisationAttribute
                        (
                            $"'{lib.key}' is required by {lib.applications.Count} environments",
                            "LibrariesScreen",
                            "Delete_Library_Description",
                            new string[2] { lib.key, lib.applications.Count.ToString() }
                        ),
                        ChoiceAText = new LocalisationAttribute("Cancel", "GenericStrings", "Cancel"),
                        ChoiceBText = new LocalisationAttribute("Delete", "GenericStrings", "Delete")
                    };
                    dialogueBox.Callback += (index) =>
                    {
                        if (index != 1) return;
                        umi3d.cdk.UMI3DResourcesManager.RemoveLibrary(lib.key);
                        library.RemoveFromHierarchy();
                        libraries.Remove(library);
                        UpdateFilterdList(Header.FilterField.LocalisedValue);
                    };
                    dialogueBox.ChoiceA.Type = ButtonType.Default;
                    dialogueBox.Enqueue(this);
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
            // Is the new value is equal to all or not.
            m_filteredValue = ce.newValue == Header.FilterField.choices[0] ? "" : ce.newValue;
            UpdateSelection();
            if (ce.newValue == Header.FilterField.choices[0])
                Header.DeleteField.LocaliseText = new LocalisationAttribute("Delete all libraries", "LibrariesScreen", "Delete_All_libraries");
            else
                Header.DeleteField.LocaliseText = new LocalisationAttribute($"Delete {ce.newValue} libraries", "LibrariesScreen", "Delete_Filtered_libraries", new string[1] { ce.newValue });
        }

        protected void DeleteFilteredLibraries()
        {
            var dialogueBox = new Dialoguebox_C
            {
                Category = ElementCategory.Menu,
                Size = ElementSize.Medium,
                Type = DialogueboxType.Confirmation,
                Title = new LocalisationAttribute($"Uninstall filtered libraries", "LibrariesScreen", "Delete_Filteredlibraries_Title"),
                Message = new LocalisationAttribute
                (
                    $"Do you want to uninstall all {m_librariesFiltered.Count} filtered libraries ?",
                    "LibrariesScreen", 
                    "Delete_Filteredlibraries_Description", 
                    new string[1] { m_librariesFiltered.Count.ToString() }
                ),
                ChoiceAText = new LocalisationAttribute("Cancel", "GenericStrings", "Cancel"),
                ChoiceBText = new LocalisationAttribute("Delete", "GenericStrings", "Delete")
            };
            dialogueBox.Callback += (index) =>
            {
                if (index != 1) return;
                foreach (var lib in m_librariesFiltered)
                {
                    umi3d.cdk.UMI3DResourcesManager.RemoveLibrary(lib.Title);
                    lib.RemoveFromHierarchy();
                    libraries.Remove(lib);
                    UpdateFilterdList(Header.FilterField.LocalisedValue);
                }
            };
            dialogueBox.ChoiceA.Type = ButtonType.Default;
            dialogueBox.Enqueue(this);
        }

        protected void UpdateSelection()
        {
            m_librariesFiltered.Clear();
            foreach (var lib in libraries) if (lib.Title.Value.Contains(m_searchedValue) && lib.Message.Value.Contains(m_filteredValue)) m_librariesFiltered.Add(lib);
            UpdateFilteredLibrariesOrder();
        }

        protected void UpdateFilterdList(string current)
        {
            m_libraries = umi3d.cdk.UMI3DResourcesManager.Libraries;
            m_environments.Clear();
            Header.FilterField.LocalisedOptions = (string)null;
            foreach (var lib in m_libraries)
            {
                if (lib.applications == null) return;

                foreach (var app in lib.applications) if (!m_environments.Contains(app)) m_environments.Add(app);
                Header.FilterField.LocalisedOptions = m_environments;
            }
            Header.FilterField.LocalisedOptions.Options.Insert(0, new LocalisationAttribute("All", "LibrariesScreen", "Filter_All"));
            _ = Header.FilterField.UpdateOptionsTranslation();
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

        #endregion
    }
}
