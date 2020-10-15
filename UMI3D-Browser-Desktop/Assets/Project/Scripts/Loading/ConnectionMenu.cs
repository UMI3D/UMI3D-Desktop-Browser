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
using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using umi3d.common;
using umi3d.common.interaction;
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ConnectionMenu : Singleton<ConnectionMenu>
{
    #region Fields

    private LauncherManager.Data connectionData;

    public ClientPCIdentifier identifier;

    public MenuAsset Menu;
    public MenuDisplayManager MenuDisplayManager;

    [SerializeField]
    private string launcherScene;

    [SerializeField]
    private VisualTreeAsset dialogueBoxTreeAsset;

    private LoadingBar loader;

    #region UI Fields

    public PanelRenderer panelRenderer;

    private VisualElement connectionScreen;

    private VisualElement passwordScreen;
    private TextField loginInput;
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

    private VisualElement parametersScreen;

    #endregion

    #endregion

    #region Methods

    #region Monobeavior's callback

    protected override void Awake()
    {
        base.Awake();
        Debug.Assert(panelRenderer != null);
        Debug.Assert(!string.IsNullOrEmpty(launcherScene));
        Debug.Assert(identifier != null);
        Debug.Assert(Menu != null);
        Debug.Assert(MenuDisplayManager != null);

        identifier.GetIdentityAction = GetIdentity;
        identifier.ShouldDownloadLib = ShouldDownloadLibraries;
        identifier.GetParameters = GetParameterDtos;
    }

    private void Start()
    {
        InitUI();

        UMI3DCollaborationClientServer.Instance.OnConnectionLost.AddListener(OnConnectionLost);
        UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(Hide);
    }

    #endregion

    #region UI Binding

    private void InitUI()
    {
        loader = new LoadingBar(panelRenderer.visualTree);

        connectionScreen = panelRenderer.visualTree.Q<VisualElement>("connection-menu");

        BindPasswordScreen();
        BindAssetsLibrariesRequiredScreen();

        parametersScreen = panelRenderer.visualTree.Q<VisualElement>("parameters-screen");

        HideAllScreens();
    }

    private void BindPasswordScreen()
    {
        passwordScreen = connectionScreen.Q<VisualElement>("password-screen");
        passwordInput = passwordScreen.Q<TextField>("password-input");
        loginInput = passwordScreen.Q<TextField>("login-input");
        connectBtn = passwordScreen.Q<Button>("connect-btn");
        goBackButton = passwordScreen.Q<Button>("go-back-btn");
        goBackButton.clickable.clicked += Leave;

        var passwordVisibleBtn = passwordScreen.Q<VisualElement>("password-visibility");
        passwordVisibleBtn.RegisterCallback<MouseDownEvent>(e =>
        {

            passwordInput.isPasswordField = false;
        });
        passwordVisibleBtn.RegisterCallback<MouseUpEvent>(e =>
        {
            passwordInput.isPasswordField = true;
        });
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
        CircularMenu.Instance.HideMenu();
       
        loader.OnProgressChange(0);
        string url = "http://" + connectionData.ip + UMI3DNetworkingKeys.media;
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

    private void Hide()
    {
        connectionScreen.style.display = DisplayStyle.None;
        CircularMenu.Instance.ShowMenu();
        CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Center);
    }

    public void DisplayParametersScreen(bool val)
    {
        parametersScreen.style.display = val ? DisplayStyle.Flex : DisplayStyle.None;   
    }

    #endregion

    #region Events/Callbacks

    private void GetMediaFailed(string error)
    {
        CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);
        var dialogueBox = dialogueBoxTreeAsset.CloneTree().Q<DialogueBoxElement>();
        panelRenderer.visualTree.Add(dialogueBox);

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
        panelRenderer.visualTree.Add(dialogueBox);

        dialogueBox.Setup("Connection to the server lost",
            "Leave to the connection menu or try again ?",
            "Try again ?",
            "Leave",
            callback);
    }

    /// <summary>
    /// Asks users a login/password to join the environement.
    /// </summary>
    private void GetIdentity(Action<string, string> callback)
    {
        var loadingScreen = panelRenderer.visualTree.Q<VisualElement>("loading-screen");
        loadingScreen.style.display = DisplayStyle.None;

        CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);
        passwordScreen.style.display = DisplayStyle.Flex;

        connectBtn.clickable.clicked += () => {
            passwordScreen.style.display = DisplayStyle.None;
            callback.Invoke(loginInput.value, passwordInput.value);
        };
    }

    /// <summary>
    /// Checks if users need to download libraries to join the environement.
    /// If yes, a screen is displayed to explain that.
    /// </summary>
    private void ShouldDownloadLibraries(List<string> ids, Action<bool> callback)
    {
        if (ids.Count == 0)
        {
            callback.Invoke(true);
        }
        else
        {
            assetsLibrariesScreen.style.display = DisplayStyle.Flex;
            if (ids.Count > 1)
                assetsRequiredWarning.text = ids.Count + " libraries are required to join the environement :";
            else
                assetsRequiredWarning.text = "1 library is required to join the environement :";

            assetsRequiredList.Clear();
            foreach (string id in ids)
            {
                var library = libraryEntryTreeAsset.CloneTree();
                library.Q<Label>("library-name").text = id;
                library.Q<Label>("library-status").text = "Required";
                assetsRequiredList.Add(library);
            }

            CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);

            confirmDLLibrariesBtn.clickable.clicked += () =>
            {
                CloseAssetsLibrariesRequiredScreen(true, callback);
            };
            denyDLLibrariesBtn.clickable.clicked += () =>
            {
                CloseAssetsLibrariesRequiredScreen(false, callback);
            };
        }
    }

    void GetParameterDtos(FormDto form, Action<FormDto> callback)
    {
        var loadingScreen = panelRenderer.visualTree.Q<VisualElement>("loading-screen");
        loadingScreen.style.display = DisplayStyle.None;

        if (form == null)
            callback.Invoke(form);
        else
        {
            //debugForm(form);
            Menu.menu.RemoveAll();
            foreach (var param in form.fields)
            {
                Menu.menu.Add(GetInteractionItem(param));
            }

            ButtonMenuItem send = new ButtonMenuItem() { Name = "Send", toggle = false };
            UnityAction<bool> action = (bool b) => {
                MenuDisplayManager.Hide(true);
                Menu.menu.RemoveAll();
                callback.Invoke(form);
                CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Center);
                loadingScreen.style.display = DisplayStyle.Flex;
            };
            send.Subscribe(action);
            Menu.menu.Add(send);

            MenuDisplayManager.Display(true);
        }
    }

    static MenuItem GetInteractionItem(AbstractInteractionDto dto)
    {
        MenuItem result = null;
        switch (dto)
        {
            case BooleanParameterDto booleanParameterDto:
                var b = new BooleanInputMenuItem() { dto = booleanParameterDto };
                b.Subscribe((x) =>
                {
                    booleanParameterDto.value = x;
                }
                );
                result = b;
                break;
            case FloatRangeParameterDto floatRangeParameterDto:
                var f = new FloatRangeInputMenuItem() { dto = floatRangeParameterDto, max = floatRangeParameterDto.max, min = floatRangeParameterDto.min, value = floatRangeParameterDto.value, increment = floatRangeParameterDto.increment };
                f.Subscribe((x) =>
                {
                    floatRangeParameterDto.value = x;
                }
                );
                result = f;
                break;
            case EnumParameterDto<string> enumParameterDto:
                var en = new DropDownInputMenuItem() { dto = enumParameterDto, options = enumParameterDto.possibleValues };
                en.Subscribe((x) =>
                {
                    enumParameterDto.value = x;
                }
                );
                result = en;
                break;
            case StringParameterDto stringParameterDto:
                var s = new TextInputMenuItem() { dto = stringParameterDto };
                s.Subscribe((x) =>
                {
                    stringParameterDto.value = x;
                }
                );
                result = s;
                break;
            default:
                result = new MenuItem();
                result.Subscribe(() => Debug.Log("hellooo 2"));
                break;
        }
        result.Name = dto.name;
        //icon;
        return result;
    }

    private void CloseAssetsLibrariesRequiredScreen(bool b, Action<bool> callback)
    {
        callback.Invoke(b);
        HideAllScreens();
        CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Center);
    }

    #endregion

    #endregion
}
