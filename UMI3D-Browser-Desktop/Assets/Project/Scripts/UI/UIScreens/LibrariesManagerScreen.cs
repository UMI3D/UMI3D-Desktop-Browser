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
using UnityEngine;
using UnityEngine.UIElements;

public class LibrariesManagerScreen
{
    VisualElement root;

    Button backMenuBnt, nextMenuBnt;
    System.Action back;

    //Asset libraries screen
    ScrollView librariesList;

    VisualTreeAsset libraryTA;
    UIDocument document;
    System.Action<string, string, string, string, System.Action<bool>> displayDialogueBox;

    public LibrariesManagerScreen
        (
            VisualElement rootDocument, 
            VisualTreeAsset libraryTA, 
            UIDocument document, 
            System.Action displayHome, 
            System.Action<string, string, string, string, System.Action<bool>> displayDialogueBox
        )
    {
        this.libraryTA = libraryTA;
        this.document = document;
        this.displayDialogueBox = displayDialogueBox;

        backMenuBnt = rootDocument.Q<Button>("backMenuBtn");
        nextMenuBnt = rootDocument.Q<Button>("nextMenuBtn"); 
        back =() =>
        {
            Hide();
            displayHome();
        };

        root = rootDocument.Q<VisualElement>("libraries-manager-screen");

        //Library screen
        librariesList = root.Q<ScrollView>("libraries-list");
    }

    /// <summary>
    /// Displays the libraries install on users' computers and allows them to unistall these libs.
    /// </summary>
    public void Display()
    {
        //Display and hide UI
        backMenuBnt.style.display = DisplayStyle.Flex;
        nextMenuBnt.style.display = DisplayStyle.None;
        root.style.display = DisplayStyle.Flex;
        backMenuBnt.clickable.clicked += back;
        DisplayLibraries();
    }

    public void Hide()
    {
        root.style.display = DisplayStyle.None;
        backMenuBnt.clickable.clicked -= back;
    }

    private void DisplayLibraries()
    {
        librariesList.Clear();

        Dictionary<string, List<umi3d.cdk.UMI3DResourcesManager.DataFile>> libs = new Dictionary<string, List<umi3d.cdk.UMI3DResourcesManager.DataFile>>();
        foreach (var lib in umi3d.cdk.UMI3DResourcesManager.Libraries)
        {
            if (lib.applications != null)
                foreach (var app in lib.applications)
                {
                    if (!libs.ContainsKey(app)) libs[app] = new List<umi3d.cdk.UMI3DResourcesManager.DataFile>();
                    libs[app].Add(lib);
                }
        }

        foreach (var app in libs)
        {
            foreach (var lib in app.Value)
            {
                // 1. Diplay lib name
                var entry = libraryTA.CloneTree();
                entry.Q<Label>("library-name").text = lib.key;

                //2. Display environments which use this lib
                var dropdown = entry.Q<DropdownElement>();
                dropdown.SetUp(document, "dropdown-label-medium");
                dropdown.SetOptions(lib.applications);

                //3. Display lib size
                DirectoryInfo dirInfo = new DirectoryInfo(lib.path);
                double dirSize = DirSize(dirInfo) / Mathf.Pow(10, 6);
                dirSize = System.Math.Round(dirSize, 2);
                entry.Q<Label>("library-size").text = dirSize.ToString() + " mo";


                //4.Bind the button to unistall this lib
                entry.Q<Button>("library-unistall").clickable.clicked += () =>
                {
                    displayDialogueBox
                    (
                        "Do you want to uninstall this labrary ?", 
                        $"This library is required for {app.Key} environment", 
                        "YES", 
                        "NO", 
                        (b) =>
                        {
                            if (!b) return;
                            lib.applications.Remove(app.Key);
                            umi3d.cdk.UMI3DResourcesManager.RemoveLibrary(lib.key);
                            DisplayLibraries();
                        }
                    );
                };
                librariesList.Add(entry);
            }
        }
    }

    /// <summary>
    /// Returns the size of a directory in bytes.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public long DirSize(DirectoryInfo d)
    {
        long size = 0;
        // Add file sizes.
        FileInfo[] fis = d.GetFiles();
        foreach (FileInfo fi in fis) size += fi.Length;

        // Add subdirectory sizes.
        DirectoryInfo[] dis = d.GetDirectories();
        foreach (DirectoryInfo di in dis) size += DirSize(di);
        return size;
    }
}
