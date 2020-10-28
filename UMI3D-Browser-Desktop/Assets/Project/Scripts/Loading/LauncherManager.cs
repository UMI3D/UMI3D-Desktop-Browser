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
using System.Resources;

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

    #endregion

    #region Data
    public const string dataFile = "userData";
    public const string favoriteDataFile = "favoriteUserData";

    [Serializable]
    public class Data
    {
        public string environmentName;
        public string ip;
    }

    private Data currentConnectionData;

    private List<Data> favoriteConnectionData = new List<Data>();

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

        urlScreen.Q<Button>("manage-library-btn").clickable.clicked += DisplayLibraries;
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
        StoreUserData(currentConnectionData);

        var isAlreadyAFavoriteData = (favoriteConnectionData.Find(d => d.ip == currentConnectionData.ip) != null);

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
        currentConnectionData = GetPreviousConnectionData();
        favoriteConnectionData = GetFavoriteConnectionData();


        librariesScreen.style.display = DisplayStyle.None;
        urlScreen.style.display = DisplayStyle.Flex;
        backMenuBnt.style.display = DisplayStyle.None;

        previousStep = null;
        nextStep = SetDomain;
        urlInput.value = currentConnectionData.ip;
    }

    /// <summary>
    /// Write a previous userInfo data.
    /// </summary>
    /// <param name="data">DataFile to write.</param>
    /// <param name="directory">Directory to write the file into.</param>
    void StoreUserData(Data data)
    {
        string path = umi3d.common.Path.Combine(Application.persistentDataPath, dataFile);
        FileStream file;
        if (File.Exists(path)) file = File.OpenWrite(path);
        else file = File.Create(path);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    /// <summary>
    /// Read a userInfo data in a directory.
    /// </summary>
    /// <returns>A DataFile if the directory containe one, null otherwhise.</returns>
    Data GetPreviousConnectionData()
    {
        string path = umi3d.common.Path.Combine(Application.persistentDataPath, dataFile);
        if (File.Exists(path))
        {
            FileStream file;
            file = File.OpenRead(path);
            BinaryFormatter bf = new BinaryFormatter();
            Data data = (Data)bf.Deserialize(file);
            file.Close();
            return data;
        }
        return new Data();
    }


    /// <summary>
    /// get the connection data about the favorite environments.
    /// </summary>
    /// <returns></returns>
    private List<Data> GetFavoriteConnectionData()
    {
        string path = umi3d.common.Path.Combine(Application.persistentDataPath, favoriteDataFile);
        if (File.Exists(path))
        {
            FileStream file;
            file = File.OpenRead(path);
            BinaryFormatter bf = new BinaryFormatter();
            List<Data> data = (List<Data>)bf.Deserialize(file);
            file.Close();
            return data;
        }
        return new List<Data>();
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

        if (libs.Count == 0)
            librariesScreen.Q<Label>("libraries-title").text = "There is currently no libraries installed";
        else if (libs.Count == 1)
            librariesScreen.Q<Label>("libraries-title").text = "There is one library currently installed";
        else
            librariesScreen.Q<Label>("libraries-title").text = "There are " + libs.Count + " libraries currently installed";

        foreach (var app in libs)
        {
            foreach (var lib in app.Value)
            {
                var entry = libraryEntryTreeAsset.CloneTree();
                entry.Q<Label>("library-name").text = lib.key;

                entry.Q<Button>("library-unistall").clickable.clicked += () =>
                {
                    DialogueBoxElement dialogue = dialogueBoxTreeAsset.CloneTree().Q<DialogueBoxElement>();
                    dialogue.Setup("Remove library", "Are your sure to unistal " + lib.key, "Unistall", "Cancel", (b) =>
                    {
                        if (b)
                        {
                            lib.applications.Remove(app.Key);
                            UMI3DResourcesManager.RemoveLibrary(lib.key);
                            DisplayLibraries();
                        }
                    },
                    true);
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

    #endregion
}
