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

using BrowserDesktop.Controller;
using BrowserDesktop.Cursor;
using BrowserDesktop.Menu;
using BrowserDesktop.preferences;
using inetum.unityUtils;
using System;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.cdk.interaction;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using umi3d.common;
using umi3d.common.interaction;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// This class is reponsible for connecting users to environments. It implies asking for login/password or parameters if 
/// necessary.
/// </summary>
public class ConnectionMenu : SingleBehaviour<ConnectionMenu>
{
    #region Fields

    private ServerPreferences.Data connectionData;

    public ClientPCIdentifier identifier;

    public MenuAsset Menu;
    public MenuDisplayManager MenuDisplayManager;

    public Camera cam;

    [SerializeField]
    private string launcherScene = null;

    private LoadingBar loader;

    public bool isDisplayed = true;
    bool isPasswordVisible = false;

    Action nextStep = null;

    #region UI Fields

    public UIDocument uiDocument;

    private VisualElement connectionScreen;

    private VisualElement logo;
    private VisualElement passwordScreen;
    private TextField loginInput;
    private TextField passwordInput;
    private Button connectBtn;
    private Button goBackButton;

    private VisualElement parametersScreen;

    VisualElement loadingScreen;

    #endregion

    #endregion

    #region Methods

    #region Monobeavior's callback

    protected override void Awake()
    {
        base.Awake();
        Debug.Assert(uiDocument != null);
        Debug.Assert(!string.IsNullOrEmpty(launcherScene));
        Debug.Assert(identifier != null);
        Debug.Assert(Menu != null);
        Debug.Assert(MenuDisplayManager != null);
        Debug.Assert(cam != null);

        identifier.GetPinAction = GetPassword;
        identifier.GetIdentityAction = GetIdentity;
        identifier.ShouldDownloadLib = ShouldDownloadLibraries;
        identifier.GetParameters = GetParameterDtos;
    }

    private void Start()
    {
        InitUI();


        UMI3DCollaborationClientServer.Instance.OnConnectionLost.AddListener(OnConnectionLost);
        UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(OnEnvironmentLoaded);
    }

    private void Update()
    {
        ManageInputs();
    }

    /// <summary>
    /// Manages the Return input to navigate through the menu.
    /// </summary>
    private void ManageInputs()
    {
        if (!isDisplayed || DialogueBox_E.Instance.Displayed) return;
        else if (Input.GetKeyDown(KeyCode.Return)) nextStep?.Invoke();
    }

    #endregion

    #region UI Binding

    private void InitUI()
    {
        VisualElement root = uiDocument.rootVisualElement;

        loader = new LoadingBar(root);
        loader.SetText("Connection");

        loadingScreen = root.Q<VisualElement>("loading-screen");

        connectionScreen = root.Q<VisualElement>("connection-menu");

        root.Q<Label>("version").text = BrowserDesktop.BrowserVersion.Version;

        BindPasswordScreen();
        passwordScreen.style.display = DisplayStyle.None;

        parametersScreen = root.Q<VisualElement>("parameters-screen");
        connectionScreen.style.display = DisplayStyle.Flex;

        root.RegisterCallback<GeometryChangedEvent>(ResizeElements);
    }

    private void BindPasswordScreen()
    {
        passwordScreen = connectionScreen.Q<VisualElement>("password-screen");
        logo = connectionScreen.Q<VisualElement>("logo");
        passwordInput = passwordScreen.Q<TextField>("password-input");
        loginInput = passwordScreen.Q<TextField>("login-input");
        connectBtn = passwordScreen.Q<Button>("connect-btn");
        goBackButton = uiDocument.rootVisualElement.Q<Button>("back-menu-btn");
        goBackButton.clickable.clicked += Leave;

        var passwordVisibleBtn = passwordScreen.Q<VisualElement>("password-visibility");
        passwordVisibleBtn.RegisterCallback<MouseDownEvent>(e =>
        {

            passwordVisibleBtn.ClearClassList();
            isPasswordVisible = !isPasswordVisible;
            if (isPasswordVisible)
                passwordVisibleBtn.AddToClassList("input-eye-button-on");
            else
                passwordVisibleBtn.AddToClassList("input-eye-button-off");
            passwordInput.isPasswordField = !isPasswordVisible;

            passwordInput.Focus();
        });
    }

    /// <summary>
    /// Resize some elements when the window is resized, to make the UI more responsive.
    /// </summary>
    private void ResizeElements(GeometryChangedEvent e)
    {
        logo.style.height = e.newRect.height * 0.16f;
        logo.style.marginBottom = e.newRect.height * 0.08f;
    }

    #endregion

    #region Actions

    string url = null;

    /// <summary>
    /// Uses the connection data to connect to te server.
    /// </summary>
    /// <param name="connectionData"></param>
    public void Connect(ServerPreferences.Data connectionData)
    {
        this.connectionData = connectionData;

        loader.OnProgressChange(0);
        var curentUrl = "http://" + connectionData.ip + UMI3DNetworkingKeys.media;
        url = curentUrl;
        UMI3DCollaborationClientServer.GetMedia(url, GetMediaSucces, GetMediaFailed, (e) => url == curentUrl && e.count < 3);
    }

    /// <summary>
    /// Clears the environment and goes back to the launcher.
    /// </summary>
    public void Leave()
    {
        url = null;
        cam.backgroundColor = new Color(0.196f, 0.196f, 0.196f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        UMI3DEnvironmentLoader.Clear();
        UMI3DResourcesManager.Instance.ClearCache();
        UMI3DCollaborationClientServer.Logout(() => { GameObject.Destroy(UMI3DClientServer.Instance.gameObject); }, null);

        SceneManager.LoadScene(launcherScene, LoadSceneMode.Single);
    }

    /// <summary>
    /// Inits the UI the environment is loaded.
    /// </summary>
    private void OnEnvironmentLoaded()
    {
        isDisplayed = false;

        connectionScreen.style.display = DisplayStyle.None;

        CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Center);
    }

    #endregion

    #region Events/Callbacks

    private void GetMediaFailed(string error)
    {
        DialogueBox_E.
            Setup(
                "Server error",
                error,
                "Leave",
                Leave,
                uiDocument
            );
    }

    private void GetMediaSucces(MediaDto media)
    {
        this.connectionData.environmentName = media.name;
        this.uiDocument.rootVisualElement.Q<Label>("environment-name").text = media.name;

        SessionInformationMenu.Instance.SetEnvironmentName(media);

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
        DialogueBox_E.
            Setup(
                "Connection to the server lost",
                "Leave to the connection menu or try again ?",
                "Try again ?",
                "Leave",
                callback,
                uiDocument
            );
    }

    /// <summary>
    /// Asks users a login to join the environement.
    /// </summary>
    private void GetPassword(Action<string> callback)
    {
        DisplayScreenToLogin();

        AskLogin(false);
        connectBtn.clickable.clicked += () => SendIdentity((login, password) => callback(password));

        nextStep = () => SendIdentity((login, password) => callback(password));
    }

    /// <summary>
    /// Asks users a login and password to join the environement.
    /// </summary>
    private void GetIdentity(Action<string, string> callback)
    {

        DisplayScreenToLogin();

        AskLogin(true);
        connectBtn.clickable.clicked += () => SendIdentity(callback);
        nextStep = () => SendIdentity(callback);
    }

    private void DisplayScreenToLogin()
    {
        loadingScreen = uiDocument.rootVisualElement.Q<VisualElement>("loading-screen");
        loadingScreen.style.display = DisplayStyle.None;
        CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);
        passwordScreen.style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// Show or hide the form to set the login.
    /// </summary>
    /// <param name="val"></param>
    private void AskLogin(bool val)
    {
        passwordScreen.Q<TextField>("login-input").style.display = val ? DisplayStyle.Flex : DisplayStyle.None;
        passwordScreen.Q<Label>("login-label").style.display = val ? DisplayStyle.Flex : DisplayStyle.None;
    }

    /// <summary>
    /// Show or hide the form to set the password.
    /// </summary>
    /// <param name="val"></param>
    private void AskPassword(bool val)
    {
        passwordScreen.Q<VisualElement>("password-container").style.display = val ? DisplayStyle.Flex : DisplayStyle.None;
        passwordScreen.Q<Label>("password-label").style.display = val ? DisplayStyle.Flex : DisplayStyle.None;
    }

    /// <summary>
    /// Sends the login/password to the server.
    /// </summary>
    /// <param name="callback"></param>
    private void SendIdentity(Action<string, string> callback)
    {
        passwordScreen.style.display = DisplayStyle.None;
        callback.Invoke(loginInput.value, passwordInput.value);
        UMI3DCollaborationClientServer.Identity.login = loginInput.value;
        loadingScreen.style.display = DisplayStyle.Flex;
        loader.SetText("Loading");
        nextStep = null;
    }

    /// <summary>
    /// Checks if users need to download libraries to join the environement.
    /// If yes, a screen is displayed to explain that.
    /// </summary>
    private void ShouldDownloadLibraries(List<string> ids, Action<bool> callback)
    {
        loader.SetText("Loading environment"); // TODO change
        if (ids.Count == 0)
        {
            callback.Invoke(true);
        }
        else
        {
            string title = (ids.Count == 1) ? "One assets library is required" : ids.Count + " assets libraries are required";

            DialogueBox_E.
            Setup(
                title, 
                "Download libraries and connect to the server ?", 
                "Accept", 
                "Deny", 
                (b) =>
                {
                    callback.Invoke(b);
                    CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Center);
                },
                uiDocument
                );
        }
    }

    /// <summary>
    /// Asks users some parameters when they join the environement.
    /// </summary>
    /// <param name="form"></param>
    /// <param name="callback"></param>
    void GetParameterDtos(FormDto form, Action<FormAnswerDto> callback)
    {
        loadingScreen.style.display = DisplayStyle.None;
        parametersScreen.style.display = DisplayStyle.Flex;

        CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);
        if (form == null)
            callback.Invoke(null);
        else
        {

            FormAnswerDto answer = new FormAnswerDto()
            {
                boneType = 0,
                hoveredObjectId = 0,
                id = form.id,
                toolId = 0,
                answers = new List<ParameterSettingRequestDto>()
            };

            Menu.menu.RemoveAll();
            Menu.menu.Name = form.name;

            foreach (var param in form.fields)
            {
                var c = GlobalToolMenuManager.GetInteractionItem(param);
                Menu.menu.Add(c.Item1);
                answer.answers.Add(c.Item2);
            }

            ButtonMenuItem send = new ButtonMenuItem() { Name = "Join", toggle = false };
            UnityAction<bool> action = (bool b) => {
                parametersScreen.style.display = DisplayStyle.None;
                MenuDisplayManager.Hide(true);
                Menu.menu.RemoveAll();
                callback.Invoke(answer);
                CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Center);
                nextStep = null;
                LocalInfoSender.CheckFormToUpdateAuthorizations(form);
            };
            send.Subscribe(action);
            Menu.menu.Add(send);
            nextStep = () => action(true);
            MenuDisplayManager.Display(true);
        }
    }

    

    #endregion

    #endregion
}