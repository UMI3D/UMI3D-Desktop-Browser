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
using System.Collections.Generic;
using umi3d.cdk;
using BrowserDesktop.Controller;

public class LauncherManager : MonoBehaviour
{
    #region Fields

    #region UI Fields

    [SerializeField]
    private PanelRenderer panelRenderer = null;

    [SerializeField]
    private VisualTreeAsset libraryEntryTreeAsset = null;
    [SerializeField]
    private VisualTreeAsset dialogueBoxTreeAsset = null;

    private VisualElement root;

    //SetDomain screen
    VisualElement umiLogo;
    VisualElement urlScreen;
    TextField urlInput;
    Button urlEnterBtn;


    //Asset libraries screen
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

        root.RegisterCallback<GeometryChangedEvent>(ResizeElements);
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

        umiLogo = urlScreen.Q<VisualElement>("logo");
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
        CheckShortcuts();
    }

    /// <summary>
    /// Allows users to use escape and return keys to navigate through the launcher.
    /// </summary>
    private void CheckShortcuts()
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
    /// Gets the url and port written by users and stores them.
    /// </summary>
    private void SetDomain()
    {
        string url = urlInput.value;

        if (string.IsNullOrEmpty(url))
        {
            //urlScreen.Q<Label>("url-error").style.display = DisplayStyle.Flex;
            //urlScreen.Q<Label>("url-error").text = "The domain is empty.";
        } else
        {
            currentConnectionData.ip = url;

            urlScreen.style.display = DisplayStyle.None;;
            previousStep = ResetLauncher;
            Connect();
        }
    }

    /// <summary>
    /// Initiates the connection to the server.
    /// </summary>
    private void Connect()
    {
        UserPreferencesManager.StoreUserData(currentConnectionData);
      
        StartCoroutine(WaitReady());
    }

    /// <summary>
    /// Load the environment scene when it is ready.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitReady()
    {
        CursorHandler.Instance.Clear();
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);

        while (!ConnectionMenu.Exists)
            yield return new WaitForEndOfFrame();
        ConnectionMenu.Instance.Connect(currentConnectionData);
        SceneManager.UnloadSceneAsync(currentScene);
    }

    /// <summary>
    /// Reset the display of the launcher.
    /// </summary>
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

    /// <summary>
    /// Displays the favorites environments stored on  users' computers.
    /// </summary>
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
                dialogue.Setup(env.environmentName, "Delete this environment from favorites ?", "YES", "NO", (b) =>
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

    /// <summary>
    /// Displays the libraries install on users' computers and allows tehm to unistall these libs.
    /// </summary>
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
                // 1. Diplay lib name
                var entry = libraryEntryTreeAsset.CloneTree();
                entry.Q<Label>("library-name").text = lib.key;

                //2. Display environments which use this lib
                var dropdown = entry.Q<DropdownElement>();
                dropdown.SetUp(panelRenderer, "dropdown-label-medium");
                dropdown.SetOptions(lib.applications);

                //3. Display lib size
                DirectoryInfo dirInfo = new DirectoryInfo(lib.path);
                double dirSize = DirSize(dirInfo) / Mathf.Pow(10, 6);
                dirSize = Math.Round(dirSize, 2);
                entry.Q<Label>("library-size").text = dirSize.ToString() + " mo"; ;


                //4.Bind the button to unistall this lib
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

    /// <summary>
    /// Resize some elements when the window is resized, to make the UI more responsive.
    /// </summary>
    /// <param name="e"></param>
    private void ResizeElements(GeometryChangedEvent e)
    {
        float height = e.newRect.height * 0.16f;
        umiLogo.style.height = height;
        umiLogo.style.minHeight = height;

        umiLogo.style.marginBottom = e.newRect.height * 0.08f;
    }

    #endregion
}
