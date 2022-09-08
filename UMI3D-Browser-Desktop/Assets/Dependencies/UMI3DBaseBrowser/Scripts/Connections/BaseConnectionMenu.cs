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
using BrowserDesktop.Menu;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.connection
{
    public abstract class BaseConnectionMenu : inetum.unityUtils.SingleBehaviour<BaseConnectionMenu>
    {
        public Camera cam;
        public BaseClientIdentifier identifier;
        public cdk.menu.MenuAsset FormMenu;
        public cdk.menu.view.MenuDisplayManager formMenuDisplay;

        protected const string launcherScene = "Connection";
        protected static string url = null;
        protected preferences.ServerPreferences.Data connectionData;
        protected LoadingBar loader;
        protected System.Action nextStep = null;

        #region UI
        public UIDocument document;
        public bool isDisplayed = true;

        protected bool isPasswordVisible = false;
        protected VisualElement connectionScreen;
        protected VisualElement passwordScreen;
        protected VisualElement parametersScreen;
        protected VisualElement loadingScreen;

        protected VisualElement logo;
        protected TextField loginInput;
        protected TextField passwordInput;
        protected Button goBackButton;

        protected virtual void InitUI()
        {
            VisualElement root = document.rootVisualElement;

            loader = LoadingBar.Instance;
            loader.Setup(root);
            loader.Text = "Connection";

            loadingScreen = root.Q<VisualElement>("loading-screen");
            connectionScreen = root.Q<VisualElement>("connection-menu");
            parametersScreen = root.Q<VisualElement>("parameters-screen");
            BindPasswordScreen(root);

            passwordScreen.style.display = DisplayStyle.None;
            connectionScreen.style.display = DisplayStyle.Flex;
        }

        protected virtual void BindPasswordScreen(VisualElement root)
        {
            passwordScreen = root.Q<VisualElement>("password-screen");

            logo = connectionScreen.Q<VisualElement>("logo");
            passwordInput = passwordScreen.Q<TextField>("password-input");
            loginInput = passwordScreen.Q<TextField>("login-input");

            goBackButton = root.Q<Button>("back-menu-btn");
            goBackButton.clickable.clicked += Leave;

            var passwordVisibleBtn = root.Q<VisualElement>("password-visibility");
            passwordVisibleBtn.RegisterCallback<MouseDownEvent>(e =>
            {
                passwordVisibleBtn.ClearClassList();
                isPasswordVisible = !isPasswordVisible;
                if (isPasswordVisible) passwordVisibleBtn.AddToClassList("input-eye-button-on");
                else passwordVisibleBtn.AddToClassList("input-eye-button-off");
                passwordInput.isPasswordField = !isPasswordVisible;

                passwordInput.Focus();
            });
        }

        /// <summary>
        /// Display Menu
        /// </summary>
        protected virtual void Display()
        {
            isDisplayed = true;
            connectionScreen.style.display = DisplayStyle.Flex;
            Controller.BaseCursor.SetMovement(this, Controller.BaseCursor.CursorMovement.Free);
            loader.OnProgressChange(0.01f);
            loader.Text = "Connecting to the new environment";
        }

        /// <summary>
        /// Hide Menu
        /// </summary>
        protected virtual void Hide()
        {
            loader.OnProgressChange(3f);
            isDisplayed = false;
            connectionScreen.style.display = DisplayStyle.None;
            Controller.BaseCursor.SetMovement(this, Controller.BaseCursor.CursorMovement.Center);
        }

        /// <summary>
        /// Resize some elements when the window is resized, to make the UI more responsive.
        /// </summary>
        protected abstract void ResizeElements(GeometryChangedEvent e);

        protected void DisplayScreenToLogin()
        {
            loadingScreen = document.rootVisualElement.Q<VisualElement>("loading-screen");
            loadingScreen.style.display = DisplayStyle.None;
            Controller.BaseCursor.SetMovement(this, Controller.BaseCursor.CursorMovement.Free);
            passwordScreen.style.display = DisplayStyle.Flex;
        }

        /// <summary>
        /// Show or hide the form to set the login.
        /// </summary>
        /// <param name="val"></param>
        protected void AskLogin(bool val)
        {
            passwordScreen.Q<TextField>("login-input").style.display = val ? DisplayStyle.Flex : DisplayStyle.None;
            passwordScreen.Q<Label>("login-label").style.display = val ? DisplayStyle.Flex : DisplayStyle.None;
        }

        /// <summary>
        /// Show or hide the form to set the password.
        /// </summary>
        /// <param name="val"></param>
        protected void AskPassword(bool val)
        {
            passwordScreen.Q<VisualElement>("password-container").style.display = val ? DisplayStyle.Flex : DisplayStyle.None;
            passwordScreen.Q<Label>("password-label").style.display = val ? DisplayStyle.Flex : DisplayStyle.None;
        }

        protected abstract void DisplayDialogueBox(string title, string message, string optionA, string optionB, System.Action<bool> callback);
        protected abstract void DisplayDialogueBox(string title, string message, string option, System.Action callback);
        #endregion

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(cam != null);
            Debug.Assert(document != null);
            Debug.Assert(identifier != null);
            Debug.Assert(FormMenu != null);
            Debug.Assert(formMenuDisplay != null);

            identifier.ShouldDownloadLib = ShouldDownloadLibraries;
            identifier.GetParameters = GetParameterDtos;
        }

        protected virtual void Start()
        {
            InitUI();
            
            cdk.collaboration.UMI3DCollaborationClientServer.Instance.OnRedirectionStarted.AddListener(OnRedirectionStarted);
            cdk.collaboration.UMI3DCollaborationClientServer.Instance.OnRedirectionAborted.AddListener(OnRedirectionAborted);
            cdk.collaboration.UMI3DCollaborationClientServer.Instance.OnConnectionLost.AddListener(OnConnectionLost);
            cdk.UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(OnEnvironmentLoaded);

            cdk.collaboration.UMI3DCollaborationClientServer.Instance.OnLeaving.AddListener(Leave);
            cdk.collaboration.UMI3DCollaborationClientServer.Instance.OnForceLogoutMessage.AddListener(ForcedLeave);
        }

        /// <summary>
        /// Uses the connection data to connect to te server.
        /// </summary>
        /// <param name="connectionData"></param>
        public void Connect(preferences.ServerPreferences.Data connectionData)
        {
            this.connectionData = connectionData;

            loader.OnProgressChange(0);
            var baseUrl = FormatUrl(connectionData.ip, connectionData.port);
            var curentUrl = baseUrl + common.UMI3DNetworkingKeys.media;
            url = curentUrl;
            try
            {
                GetMediaSucces(new common.MediaDto() { url = baseUrl, name = connectionData.environmentName ?? connectionData.ip }, (s) => GetMediaFailed(s));
            }
            catch (System.Exception e)
            {
                GetMediaFailed(e.Message);
            }
        }

        protected static string FormatUrl(string ip, string port)
        {
            string url = ip + (string.IsNullOrEmpty(port) ? "" : (":" + port));

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                return "http://" + url;
            return url;
        }

        public static async System.Threading.Tasks.Task<common.MediaDto> GetMediaDto(preferences.ServerPreferences.ServerData connectionData)
        {
            var curentUrl = FormatUrl(connectionData.serverUrl, null) + common.UMI3DNetworkingKeys.media;
            url = curentUrl;
            try
            {
                return await cdk.collaboration.UMI3DCollaborationClientServer.GetMedia(url, (e) => url == curentUrl && e.count < 3);
            }
            catch
            {
                return null;
            }
        }

        protected void GetMediaSucces(common.MediaDto media, System.Action<string> failed)
        {
            connectionData.environmentName = media.name;
            document.rootVisualElement.Q<Label>("environment-name").text = media.name;

            SessionInformationMenu.Instance.SetEnvironmentName(media);

            cdk.collaboration.UMI3DCollaborationClientServer.Connect(media, failed);
        }

        protected void GetMediaFailed(string error)
        => DisplayDialogueBox("Server error", error, "Leave", Leave);


        public void ForcedLeave(string s)
        {
            cdk.collaboration.UMI3DCollaborationClientServer.Instance.OnForceLogoutMessage.RemoveListener(ForcedLeave);
            cdk.collaboration.UMI3DCollaborationClientServer.Instance.OnLeaving.RemoveListener(Leave);

            DisplayDialogueBox("Forced Deconnection", s, "Leave", Leave);
        }

        /// <summary>
        /// Clears the environment and goes back to the launcher.
        /// </summary>
        public void Leave()
        {
            cdk.collaboration.UMI3DCollaborationClientServer.Instance.OnLeaving.RemoveListener(Leave);

            url = null;
            cam.backgroundColor = new Color(0.196f, 0.196f, 0.196f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cdk.UMI3DEnvironmentLoader.Clear();
            cdk.UMI3DResourcesManager.Instance.ClearCache();
            cdk.collaboration.UMI3DCollaborationClientServer.Logout();
            Destroy(cdk.UMI3DClientServer.Instance.gameObject);

            UnityEngine.SceneManagement.SceneManager.LoadScene(launcherScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        /// <summary>
        /// Inits the UI the environment is loaded.
        /// </summary>
        protected virtual void OnEnvironmentLoaded() => Hide();

        protected virtual void OnRedirectionStarted() => Display();

        protected virtual void OnRedirectionAborted() => Hide();

        protected void OnConnectionLost()
        {
            Controller.BaseController.CanProcess = false;
            System.Action<bool> callback = (b) => {
                if (b) cdk.collaboration.UMI3DCollaborationClientServer.Reconnect();
                else Leave();
                Controller.BaseController.CanProcess = true;
            };

            OnConnectionLost(callback);
        }
        protected void OnConnectionLost(System.Action<bool> callback)
        => DisplayDialogueBox("Connection to the server lost", "Leave to the connection menu or try again ?", "Try again ?", "Leave", callback);

        /// <summary>
        /// Checks if users need to download libraries to join the environement.
        /// If yes, a screen is displayed to explain that.
        /// </summary>
        protected void ShouldDownloadLibraries(List<string> ids, System.Action<bool> callback)
        {
            Display();
            loader.Text = "Loading environment"; // TODO change
            if (ids.Count == 0) callback.Invoke(true);
            else
            {
                string title = (ids.Count == 1) ? "One assets library is required" : ids.Count + " assets libraries are required";

                DisplayDialogueBox
                    (
                        title,
                        "Download libraries and connect to the server ?",
                        "Accept", "Deny",
                        (b) =>
                        {
                            callback.Invoke(b);
                            Controller.BaseCursor.SetMovement(this, Controller.BaseCursor.CursorMovement.Center);
                        }
                     );
            }
        }

        /// <summary>
        /// Asks users some parameters when they join the environement.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="callback"></param>
        protected void GetParameterDtos(common.interaction.FormDto form, System.Action<common.interaction.FormAnswerDto> callback)
        {
            Display();
            loadingScreen.style.display = DisplayStyle.None;
            parametersScreen.style.display = DisplayStyle.Flex;


            if (form == null) callback.Invoke(null);
            else
            {
                common.interaction.FormAnswerDto answer = new common.interaction.FormAnswerDto()
                {
                    boneType = 0,
                    hoveredObjectId = 0,
                    id = form.id,
                    toolId = 0,
                    answers = new List<common.interaction.ParameterSettingRequestDto>()
                };

                FormMenu.menu.RemoveAll();
                FormMenu.menu.Name = form.name;

                foreach (var param in form.fields)
                {
                    var c = cdk.interaction.GlobalToolMenuManager.GetInteractionItem(param);
                    FormMenu.menu.Add(c.Item1);
                    answer.answers.Add(c.Item2);
                }

                ButtonMenuItem send = new ButtonMenuItem() { Name = "Join", toggle = false };
                UnityEngine.Events.UnityAction<bool> action = (bool b) =>
                {
                    parametersScreen.style.display = DisplayStyle.None;
                    formMenuDisplay.Hide(true);
                    FormMenu.menu.RemoveAll();
                    callback.Invoke(answer);
                    Controller.BaseCursor.SetMovement(this, Controller.BaseCursor.CursorMovement.Center);
                    nextStep = null;
                    cdk.collaboration.LocalInfoSender.CheckFormToUpdateAuthorizations(form);
                };
                send.Subscribe(action);
                FormMenu.menu.Add(send);
                nextStep = () => action(true);
                formMenuDisplay.CreateMenuAndDisplay(true, false);
            }
        }
    }
}
