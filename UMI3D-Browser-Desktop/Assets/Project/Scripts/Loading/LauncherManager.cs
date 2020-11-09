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

using System;
using System.Collections;

using UnityEngine;
using Unity.UIElements.Runtime;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

using BrowserDesktop.Cursor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using umi3d.cdk;
using BrowserDesktop.Controller;
using System.Security.Cryptography;
using umi3d.common;

public class LauncherManager : MonoBehaviour
{
    #region Fields

    #region UI Fields
    [SerializeField]
    private PanelRenderer panelRenderer;

    [SerializeField]
    private VisualTreeAsset libraryEntryTreeAsset;
    [SerializeField]
    private VisualTreeAsset dialogueBoxTreeAsset;

    private VisualElement root;

    //Domain screen
    VisualElement urlScreen;
    TextField urlInput;
    Button urlEnterBtn;


    //Libraries
    VisualElement librariesScreen;
    ScrollView librariesList;
    Button backMenuBnt;

    //Favorite env
    public VisualTreeAsset favoriteEnvItemTreeAsset;
    SliderElement favoriteEnvironmentSlider;

    #endregion

    #region Data


    private UserPreferencesManager.Data currentConnectionData;

    private List<UserPreferencesManager.Data> favoriteConnectionData = new List<UserPreferencesManager.Data>();

    [SerializeField]
    public string currentScene;
    [SerializeField]
    public string sceneToLoad;

    Action nextStep = null;
    Action previousStep = null;

    #endregion

    #endregion

    void Start()
    {
        Debug.Assert(panelRenderer != null);
        Debug.Assert(dialogueBoxTreeAsset != null);
        Debug.Assert(libraryEntryTreeAsset != null);
        root = panelRenderer.visualTree;

        InitUI();
   
        ResetLauncher();

        root.Insert(0, new DialogueBoxElement());
    }

    #region UI Binding

    private void InitUI()
    {
        root.Q<Label>("version").text = umi3d.UMI3DVersion.version;
        BindURLScreen();
        BindLibrariesScreen();
    }

    private void BindURLScreen()
    {
        urlScreen = root.Q<VisualElement>("url-screen");

        urlInput = urlScreen.Q<TextField>("url-input");
        urlEnterBtn = urlScreen.Q<Button>("url-enter-btn");

        urlEnterBtn.clickable.clicked += SetDomain;

        var manageLibraryBtn = urlScreen.Q<Button>("manage-library-btn");
        manageLibraryBtn.clickable.clicked += DisplayLibraries;
        manageLibraryBtn.transform.position -= new Vector3(75, 0, 0);

        favoriteEnvironmentSlider = new SliderElement();
        favoriteEnvironmentSlider.SetUp(urlScreen.Q<VisualElement>("slider"));
    }

    private void BindLibrariesScreen()
    {
        librariesScreen = root.Q<VisualElement>("libraries-manager-screen");
        librariesList = librariesScreen.Q<ScrollView>("libraries-list");

        backMenuBnt = root.Q<Button>("back-menu-btn");
        backMenuBnt.clickable.clicked += ResetLauncher;
    }

    #endregion

    #region Action

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (DialogueBoxElement.IsADialogueBoxDislayed)
                DialogueBoxElement.CloseDialogueBox(true);
            else
                nextStep?.Invoke();
        }
        else if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.MainMenuToggle)))
        {
            if (DialogueBoxElement.IsADialogueBoxDislayed)
                DialogueBoxElement.CloseDialogueBox(false);
            else
                previousStep?.Invoke();
        }
    }

    /// <summary>
    /// Gets the url and port written by the users and stores them.
    /// </summary>
    private void SetDomain()
    {
        string url = urlInput.value;

        if (string.IsNullOrEmpty(url))
        {
            urlScreen.Q<Label>("url-error").style.display = DisplayStyle.Flex;
            urlScreen.Q<Label>("url-error").text = "The domain is empty.";
        } else
        {
            currentConnectionData.ip = url;

            urlScreen.style.display = DisplayStyle.None;;
            previousStep = ResetLauncher;
            Connect();
        }
    }

    /// <summary>
    /// Initiates the connection
    /// </summary>
    private void Connect()
    {
        UserPreferencesManager.StoreUserData(currentConnectionData);
      
        StartCoroutine(WaitReady());
    }

    IEnumerator WaitReady()
    {
        CursorHandler.Instance.Clear();
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);

        while (!ConnectionMenu.Exists)
            yield return new WaitForEndOfFrame();
        ConnectionMenu.Instance.Connect(currentConnectionData);
        SceneManager.UnloadSceneAsync(currentScene);
    }

    private void ResetLauncher()
    {
        currentConnectionData = UserPreferencesManager.GetPreviousConnectionData();
        favoriteConnectionData = UserPreferencesManager.GetFavoriteConnectionData();

        DisplayFavoriteEnvironments();

        librariesScreen.style.display = DisplayStyle.None;
        urlScreen.style.display = DisplayStyle.Flex;
        backMenuBnt.style.display = DisplayStyle.None;

        previousStep = null;
        nextStep = SetDomain;
        urlInput.value = currentConnectionData.ip;
    }

    private void DisplayFavoriteEnvironments()
    {
        favoriteEnvironmentSlider.ClearItems();
        foreach (var env in favoriteConnectionData)
        {
            var item = favoriteEnvItemTreeAsset.CloneTree().Q<VisualElement>("favorite-env-item");
            item.Q<Label>().text = env.environmentName;
            item.RegisterCallback<MouseDownEvent>(e =>
            {
                if (e.clickCount == 2)
                {
                    this.currentConnectionData.ip = env.ip;
                    Connect();
                }
            });
            item.Q<Button>("delete-item").clickable.clicked += () =>
            {
                DialogueBoxElement dialogue = dialogueBoxTreeAsset.CloneTree().Q<DialogueBoxElement>();
                dialogue.Setup("env.environmentName", "Delete this environment from favorites ?", "Yes", "No", (b) =>
                {
                    if (b)
                    {
                        favoriteConnectionData.Remove(favoriteConnectionData.Find(d => d.ip == env.ip));
                        UserPreferencesManager.StoreFavoriteConnectionData(favoriteConnectionData);
                        favoriteEnvironmentSlider.RemoveElement(item);
                    }
                },
                true);
                root.Add(dialogue);
            };
            favoriteEnvironmentSlider.AddElement(item);
        }
    }

    private void DisplayLibraries()
    {
        urlScreen.style.display = DisplayStyle.None;
        backMenuBnt.style.display = DisplayStyle.Flex;
        librariesScreen.style.display = DisplayStyle.Flex;
        nextStep = null;
        previousStep = ResetLauncher;

        librariesList.Clear();

        Dictionary<string, List<UMI3DResourcesManager.DataFile>> libs = new Dictionary<string, List<UMI3DResourcesManager.DataFile>>();
        foreach (var lib in UMI3DResourcesManager.Libraries)
        {
            if (lib.applications != null)
                foreach (var app in lib.applications)
                {
                    if (!libs.ContainsKey(app)) libs[app] = new List<UMI3DResourcesManager.DataFile>();
                    libs[app].Add(lib);
                }
        }

        foreach (var app in libs)
        {
            foreach (var lib in app.Value)
            {
                var entry = libraryEntryTreeAsset.CloneTree();
                entry.Q<Label>("library-name").text = lib.key;

                DirectoryInfo dirInfo = new DirectoryInfo(lib.path);
                double dirSize = DirSize(dirInfo) / Mathf.Pow(10, 6);
                dirSize = Math.Round(dirSize, 2);
                entry.Q<Label>("library-size").text = dirSize.ToString() + " mo"; ;

                entry.Q<Button>("library-unistall").clickable.clicked += () =>
                {
                    DialogueBoxElement dialogue = dialogueBoxTreeAsset.CloneTree().Q<DialogueBoxElement>();
                    dialogue.Setup("Are you sure ... ?", "This library is required for " + app.Key + " environment", "YES", "NO", (b) =>
                    {
                        if (b)
                        {
                            lib.applications.Remove(app.Key);
                            UMI3DResourcesManager.RemoveLibrary(lib.key);
                            DisplayLibraries();
                        }
                    });
                    root.Add(dialogue);
                };
                librariesList.Add(entry);
            }
        }

        /*
        foreach (var app in libs)
        {
            var entry = libraryEntryTreeAsset.CloneTree();
            entry.Q<Label>("library-name").text = app.Key;

            entry.Q<Button>("library-unistall").clickable.clicked += () =>
            {
                if (isLibraryCurrentRemoved)
                    return;

                isLibraryCurrentRemoved = true;

                DialogueBoxElement dialogue = dialogueBoxTreeAsset.CloneTree().Q<DialogueBoxElement>();
                dialogue.Setup("Remove library", "Are your sure to unistall all libraries required for " + app.Key, "Unistall", "Cancel", (b) =>
                {
                    if (b)
                    {
                        foreach (var lib in app.Value)
                        {
                            lib.applications.Remove(app.Key);
                            if (lib.applications.Count <= 0)
                                UMI3DResourcesManager.RemoveLibrary(lib.key);
                        }
                        DisplayLibraries();
                    }
                    isLibraryCurrentRemoved = false;
                },
                true);

                root.Add(dialogue);
            };

            librariesList.Add(entry);
        }*/
    }

    /// <summary>
    /// Returns the size of a directory in bytes
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public long DirSize(DirectoryInfo d)
    {
        long size = 0;
        // Add file sizes.
        FileInfo[] fis = d.GetFiles();
        foreach (FileInfo fi in fis)
        {
            size += fi.Length;
        }
        // Add subdirectory sizes.
        DirectoryInfo[] dis = d.GetDirectories();
        foreach (DirectoryInfo di in dis)
        {
            size += DirSize(di);
        }
        return size;
    }

    #endregion
}
