using BrowserDesktop.Controller;
using BrowserDesktop.Cursor;
using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.common;
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ConnectionMenu : Singleton<ConnectionMenu>
{
    #region Fields

    private LauncherManager.Data connectionData;

    public ClientPCIdentifier identifier;

    [SerializeField]
    private string launcherScene;

    [SerializeField]
    private VisualTreeAsset dialogueBoxTreeAsset;

    private LoadingBar loader;

    #region UI Fields

    public PanelRenderer renderer;

    private VisualElement connectionScreen;

    private VisualElement passwordScreen;
    private TextField passwordInput;
    private Button connectBtn;
    private Button goBackButton;

    private VisualElement assetsLibrariesScreen;
    private Label assetsRequiredWarning;
    private ScrollView assetsRequiredList;
    [SerializeField]
    private VisualTreeAsset libraryEntryTreeAsset;
    private Button confirmDLLibrariesBtn;
    private Button denyDLLibrariesBtn;

    #endregion

    #endregion

    #region Methods

    #region Monobeavior's callback

    protected override void Awake()
    {
        base.Awake();
        Debug.Assert(renderer != null);
        Debug.Assert(!string.IsNullOrEmpty(launcherScene));
        Debug.Assert(identifier != null);

        loader = new LoadingBar(renderer.visualTree);

        InitUI();

        identifier.GetPasswordAction = GetPassword;
        identifier.ShouldDownloadLib = ShouldDownloadLibraries;
        // TODO : identifier.GetParameters = GetParameterDtos;

        ShouldDownloadLibraries(new List<string> { "Lib 1", "Lib 2" }, (b) => Debug.Log("Pomme"));
    }

    private void Start()
    {
        UMI3DCollaborationClientServer.Instance.OnConnectionLost.AddListener(OnConnectionLost);
    }

    #endregion

    #region UI Binding

    private void InitUI()
    {
        connectionScreen = renderer.visualTree.Q<VisualElement>("connection-menu");

        BindPasswordScreen();
        BindAssetsLibrariesRequiredScreen();

        HideAllScreens();
    }

    private void BindPasswordScreen()
    {
        passwordScreen = connectionScreen.Q<VisualElement>("password-screen");
        passwordInput = passwordScreen.Q<TextField>("password-input");
        connectBtn = passwordScreen.Q<Button>("connect-btn");
        goBackButton = passwordScreen.Q<Button>("go-back-btn");
        goBackButton.clickable.clicked += Leave;
    }

    private void BindAssetsLibrariesRequiredScreen()
    {
        assetsLibrariesScreen = connectionScreen.Q<VisualElement>("libraries-screen");
        assetsRequiredWarning = assetsLibrariesScreen.Q<Label>("assets-required-warning");
        assetsRequiredList = assetsLibrariesScreen.Q<ScrollView>("assets-required-list");
        confirmDLLibrariesBtn = assetsLibrariesScreen.Q<Button>("confirm-libraries-btn");
        denyDLLibrariesBtn = assetsLibrariesScreen.Q<Button>("cancel-libraries-btn");
    }

    #endregion

    #region Actions

    /// <summary>
    /// Uses the connection data to connect to te server.
    /// </summary>
    /// <param name="connectionData"></param>
    public void Connect(LauncherManager.Data connectionData)
    {
        this.connectionData = connectionData;

        connectionScreen.style.display = DisplayStyle.Flex;

        loader.OnProgressChange(0);
        //DisplayLoginPassword(false);
        string url = "http://" + connectionData.ip + ":" + connectionData.port + UMI3DNetworkingKeys.media;
        UMI3DCollaborationClientServer.GetMedia(url, GetMediaSucces, GetMediaFailed);
    }
    
    /// <summary>
    /// Clears the environement and goes back to the launcher.
    /// </summary>
    public void Leave()
    {
        UMI3DEnvironmentLoader.Clear();
        UMI3DResourcesManager.Instance.ClearCache();
        UMI3DCollaborationClientServer.Logout(() => { GameObject.Destroy(UMI3DClientServer.Instance.gameObject); }, null);

        SceneManager.LoadScene(launcherScene, LoadSceneMode.Single);
    }

    private void HideAllScreens()
    {
        passwordScreen.style.display = DisplayStyle.None;
        assetsLibrariesScreen.style.display = DisplayStyle.None;
    }

    #endregion

    #region Events/Callbacks

    private void GetMediaFailed(string error)
    {
        CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);
        var dialogueBox = dialogueBoxTreeAsset.CloneTree().Q<DialogueBoxElement>();
        renderer.visualTree.Add(dialogueBox);

        dialogueBox.Setup("Server error",
            error,
            "Leave",
            "Also leave ?",
            (b) => Leave());   
    }

    private void GetMediaSucces(MediaDto media)
    {
        UMI3DCollaborationClientServer.Connect();
    }

    private void OnConnectionLost()
    {
        MouseAndKeyboardController.CanProcess = false;
        Action<bool> callback = (b) => {
            if (b) Connect(connectionData);
            else Leave();
            MouseAndKeyboardController.CanProcess = true;
        };
        OnConnectionLost(callback);
    }

    private void OnConnectionLost(Action<bool> callback)
    {
        CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);
        var dialogueBox = dialogueBoxTreeAsset.CloneTree().Q<DialogueBoxElement>();
        renderer.visualTree.Add(dialogueBox);

        dialogueBox.Setup("Connection to the server lost",
            "Leave to the connection menu or try again ?",
            "Try again ?",
            "Leave",
            callback);
    }

    /// <summary>
    /// Asks users a password to join the environement.
    /// </summary>
    private void GetPassword(Action<string> callback)
    {
        HideAllScreens();
        CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);
        passwordScreen.style.display = DisplayStyle.Flex;

        connectBtn.clickable.clicked += () => callback.Invoke(passwordInput.value);
    }

    /// <summary>
    /// Checks if users need to download libraries to join the environement.
    /// If yes, a screen is displayed to explain that.
    /// </summary>
    private void ShouldDownloadLibraries(List<string> ids, Action<bool> callback)
    {
        if (ids.Count == 0) callback.Invoke(true);
        else
        {
            HideAllScreens();
            assetsLibrariesScreen.style.display = DisplayStyle.Flex;
            if (ids.Count > 1)
                assetsRequiredWarning.text = ids.Count + " libraries are required to join the environement :";
            else
                assetsRequiredWarning.text = "1 library is required to join the environement :";

            assetsRequiredList.Clear();
            foreach(string id in ids)
            {
                var library = libraryEntryTreeAsset.CloneTree();
                library.Q<Label>("library-name").text = id;
                library.Q<Label>("library-status").text = "Required";
                assetsRequiredList.Add(library);
            }

            CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);

            confirmDLLibrariesBtn.clickable.clicked += () =>
            {
                Debug.Log("Ananas");
                CloseAssetsLibrariesRequiredScreen(true, callback);
            };
            denyDLLibrariesBtn.clickable.clicked += () =>
            {
                Debug.Log("Fraise");
                CloseAssetsLibrariesRequiredScreen(false, callback);
            };
        }
    }

    private void CloseAssetsLibrariesRequiredScreen(bool b, Action<bool> callback)
    {
        callback.Invoke(b);
        HideAllScreens();
        CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Center);

        Debug.Log("ROOO");
    }

    #endregion

    #endregion
}
