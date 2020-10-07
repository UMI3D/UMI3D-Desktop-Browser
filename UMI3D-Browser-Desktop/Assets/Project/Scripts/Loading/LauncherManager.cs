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

public class LauncherManager : MonoBehaviour
{
    #region Fields

    #region UI Fields
    [SerializeField]
    private PanelRenderer panelRenderer;

    private VisualElement root;

    //Domain screen
    VisualElement urlScreen;
    TextField urlInput;
    TextField portInput;
    Button urlEnterBtn;

    //Login password screen
    VisualElement loginPasswordScreen;
    TextField loginInput;
    TextField passwordInput;
    Button confirmLoginBtn;
    Button cancelLoginBtn;

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
        public string port;
        public string login;
    }

    private Data currentConnectionData;

    [SerializeField]
    public string currentScene;
    [SerializeField]
    public string sceneToLoad;

    #endregion

    #endregion

    void Start()
    {
        Debug.Assert(panelRenderer != null);
        root = panelRenderer.visualTree;

        InitUI();
   
        ResetLauncher();

        root.Insert(0, new DialogueBoxElement());
    }

    #region UI Binding

    private void InitUI()
    {
        BindURLScreen();
        BindLoginScreen();
        BindLibrariesScreen();
    }

    private void BindURLScreen()
    {
        urlScreen = root.Q<VisualElement>("url-screen");

        urlInput = urlScreen.Q<TextField>("url-input");
        portInput = urlScreen.Q<TextField>("port-input");
        urlEnterBtn = urlScreen.Q<Button>("url-enter-btn");

        urlEnterBtn.clickable.clicked += SetDomain;

        urlScreen.Q<Button>("manage-library-btn").clickable.clicked += () =>
        {
            HideAllScreens();
            librariesScreen.style.display = DisplayStyle.Flex;
        };
    }

    private void BindLoginScreen()
    {
        loginPasswordScreen = root.Q<VisualElement>("login-password-screen");
        loginInput = loginPasswordScreen.Q<TextField>("login-input");
        passwordInput = loginPasswordScreen.Q<TextField>("password-input");
        Debug.Assert(passwordInput != null);

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

    /// <summary>
    /// Gets the url and port written by the users and stores them.
    /// </summary>
    private void SetDomain()
    {
        string url = urlInput.value;
        string port = portInput.value;

        if (string.IsNullOrEmpty(url))
        {
            urlScreen.Q<Label>("url-error").style.display = DisplayStyle.Flex;
            urlScreen.Q<Label>("url-error").text = "The domain is empty.";
        } else
        {
            currentConnectionData.ip = url;
            currentConnectionData.port = port;

            urlScreen.style.display = DisplayStyle.None;
            loginPasswordScreen.style.display = DisplayStyle.Flex;

            Debug.Log("<color=green> Connection info : " + url + " " + port + " + </color>");
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
            //TODO : display error message
        } else
        {
            currentConnectionData.login = login;
            Debug.Log("<color=green> Connection info : " + currentConnectionData.ip + ":" + currentConnectionData.port + " + </color>");
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

        loginInput.value = currentConnectionData.login;
        passwordInput.value = string.Empty;
        portInput.value = currentConnectionData.port;
        urlInput.value = currentConnectionData.ip;
    }

    private void HideAllScreens()
    {
        urlScreen.style.display = DisplayStyle.None;
        urlScreen.Q<Label>("url-error").style.display = DisplayStyle.None;
        loginPasswordScreen.style.display = DisplayStyle.None;
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
    #endregion
}
