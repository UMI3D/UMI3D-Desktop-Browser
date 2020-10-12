using System;
using System.Collections;

using UnityEngine;
using Unity.UIElements.Runtime;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

using umi3d.cdk.collaboration;
using BrowserDesktop.Cursor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using umi3d.cdk;
using BrowserDesktop.Controller;

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

    //Login password screen
    VisualElement loginPasswordScreen;
    TextField loginInput;
    TextField passwordInput;
    Button confirmLoginBtn;
    Button cancelLoginBtn;
    Label loginInputError;

    //Libraries
    VisualElement librariesScreen;
    ScrollView librariesList;

    #endregion

    #region Data
    public const string dataFile = "userData";

    [Serializable]
    public class Data
    {
        public string ip;
        public string login;
    }

    private Data currentConnectionData;

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
        BindLoginScreen();
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

    private void BindLoginScreen()
    {
        loginPasswordScreen = root.Q<VisualElement>("login-password-screen");
        loginInput = loginPasswordScreen.Q<TextField>("login-input");
        passwordInput = loginPasswordScreen.Q<TextField>("password-input");
        loginInputError = loginPasswordScreen.Q<Label>("login-input-error");

        confirmLoginBtn = loginPasswordScreen.Q<Button>("confirm-login-btn");
        confirmLoginBtn.clickable.clicked += Login;

        cancelLoginBtn = loginPasswordScreen.Q<Button>("cancel-login-btn");
        cancelLoginBtn.clickable.clicked += ResetLauncher;
    }

    private void BindLibrariesScreen()
    {
        librariesScreen = root.Q<VisualElement>("libraries-manager-screen");
        librariesList = librariesScreen.Q<ScrollView>("libraries-list");

        var backMenuBnt = librariesScreen.Q<Button>("back-menu-btn");
        backMenuBnt.clickable.clicked += ResetLauncher;
    }

    #endregion

    #region Action

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            nextStep?.Invoke();
        }
        else if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.MainMenuToggle)))
        {
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

            urlScreen.style.display = DisplayStyle.None;
            loginPasswordScreen.style.display = DisplayStyle.Flex;
            nextStep = Login;
            previousStep = ResetLauncher;
        }
    }

    /// <summary>
    /// Gets the login entered by te users and initiates the connection if there is no problem.
    /// </summary>
    private void Login()
    {
        string login = loginInput.value;
        string password = passwordInput.value;

        if (string.IsNullOrEmpty(login))
        {
            Debug.Log("<color=red> Connection error : The login can not be empty </color>");
            loginInputError.text = "Login is empty.";
        } else
        {
            currentConnectionData.login = login;
            Debug.Log("<color=green> Connection info : " + currentConnectionData.ip + "</color>");
            Connect();
        }
    }

    /// <summary>
    /// Initiates the connection
    /// </summary>
    private void Connect()
    {
        loginPasswordScreen.style.display = DisplayStyle.None;

        UMI3DCollaborationClientServer.Identity.login = currentConnectionData.login;

        StoreUserData(currentConnectionData);

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
        currentConnectionData = GetUserData();

        HideAllScreens();
        urlScreen.style.display = DisplayStyle.Flex;
        previousStep = null;
        nextStep = SetDomain;

        loginInput.value = currentConnectionData.login;
        passwordInput.value = string.Empty;
        urlInput.value = currentConnectionData.ip;
    }

    private void HideAllScreens()
    {
        urlScreen.style.display = DisplayStyle.None;
        urlScreen.Q<Label>("url-error").style.display = DisplayStyle.None;
        loginPasswordScreen.style.display = DisplayStyle.None;
        loginInputError.text = string.Empty;
        librariesScreen.style.display = DisplayStyle.None;
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
    Data GetUserData()
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

    private bool isLibraryCurrentRemoved = false;

    private void DisplayLibraries()
    {
        HideAllScreens();
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
            var entry = libraryEntryTreeAsset.CloneTree();
            entry.Q<Label>("library-name").text = app.Key;
            var librariesInstalled = entry.Q<ScrollView>();
            app.Value.ForEach(l => librariesInstalled.Add(new Label { text = l.key }));

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
                });

                root.Add(dialogue);
            };

            librariesList.Add(entry);
        }
    }

    #endregion
}
