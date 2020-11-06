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
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
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

    private UserPreferencesManager.Data connectionData;
    private UserPreferencesManager.Data favoriteEnvironments;

    public ClientPCIdentifier identifier;

    public MenuAsset Menu;
    public MenuDisplayManager MenuDisplayManager;

    [SerializeField]
    private string launcherScene;

    [SerializeField]
    private VisualTreeAsset dialogueBoxTreeAsset;

    private LoadingBar loader;

    public bool isDisplayed = true;

    #region UI Fields

    public PanelRenderer panelRenderer;

    private VisualElement connectionScreen;

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
        previousStep = Leave;

        InitUI();

        UMI3DCollaborationClientServer.Instance.OnConnectionLost.AddListener(OnConnectionLost);
        UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(OnEnvironmentLoaded);
    }

    Action nextStep = null;
    Action previousStep = null;

    private void Update()
    {
        ManageInputs();   
    }

    /// <summary>
    /// Manages the Return and Escape inputs to navigate through the menu.
    /// </summary>
    private void ManageInputs()
    {
        if (!isDisplayed)
            return;

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

    #endregion

    #region UI Binding

    private void InitUI()
    {
        VisualElement root = panelRenderer.visualTree;

        loader = new LoadingBar(root);

        loadingScreen = panelRenderer.visualTree.Q<VisualElement>("loading-screen");

        connectionScreen = root.Q<VisualElement>("connection-menu");

        root.Q<Label>("version").text = umi3d.UMI3DVersion.version;

        BindPasswordScreen();
        passwordScreen.style.display = DisplayStyle.None;

        parametersScreen = panelRenderer.visualTree.Q<VisualElement>("parameters-screen");
        connectionScreen.style.display = DisplayStyle.Flex;
    }

    bool isPasswordVisible = false;
    private void BindPasswordScreen()
    {
        passwordScreen = connectionScreen.Q<VisualElement>("password-screen");
        passwordInput = passwordScreen.Q<TextField>("password-input");
        loginInput = passwordScreen.Q<TextField>("login-input");
        connectBtn = passwordScreen.Q<Button>("connect-btn");
        goBackButton = panelRenderer.visualTree.Q<Button>("back-menu-btn");
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

    #endregion

    #region Actions

    /// <summary>
    /// Uses the connection data to connect to te server.
    /// </summary>
    /// <param name="connectionData"></param>
    public void Connect(UserPreferencesManager.Data connectionData)
    {
        this.connectionData = connectionData;

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
        this.connectionData.environmentName = media.name;
        this.panelRenderer.visualTree.Q<Label>("environment-name").text = media.name;

        SessionInformationMenu.Instance.SetEnvironmentName(media, connectionData);

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

        connectBtn.clickable.clicked += () => SendIdentity(callback);

        nextStep = () => SendIdentity(callback);
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
        nextStep = null;
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
        } else
        {
            CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);

            string title = (ids.Count == 1) ? "One assets library is required" : ids.Count + " assets libraries are required";

            DialogueBoxElement dialogue = dialogueBoxTreeAsset.CloneTree().Q<DialogueBoxElement>();
            dialogue.Setup(title, "Download libraries and connect to the server ?", "Accept", "Denied", (b) =>
            {
                callback.Invoke(b);
                CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Center);
            },
            true);

            panelRenderer.visualTree.Add(dialogue);
        }


        /*if (ids.Count == 0)
        {
            callback.Invoke(true);
        }
        else
        {
            HideLoadingScreen();

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
        }*/
    }

    /// <summary>
    /// Asks users some parameters when they join the environement.
    /// </summary>
    /// <param name="form"></param>
    /// <param name="callback"></param>
    void GetParameterDtos(FormDto form, Action<FormDto> callback)
    {
        loadingScreen.style.display = DisplayStyle.None;
        parametersScreen.style.display = DisplayStyle.Flex;

        CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Free);

        if (form == null)
            callback.Invoke(form);
        else
        {
            Menu.menu.RemoveAll();
            foreach (var param in form.fields)
            {
                Menu.menu.Add(GetInteractionItem(param));
            }

            ButtonMenuItem send = new ButtonMenuItem() { Name = "Send", toggle = false };
            UnityAction<bool> action = (bool b) => {
                parametersScreen.style.display = DisplayStyle.None;
                MenuDisplayManager.Hide(true);
                Menu.menu.RemoveAll();
                callback.Invoke(form);
                CursorHandler.SetMovement(this, CursorHandler.CursorMovement.Center);
                nextStep = null;
            };
            send.Subscribe(action);
            Menu.menu.Add(send);
            nextStep = () => action(true);
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

    #endregion

    #endregion
}
