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
using System.Threading.Tasks;
using umi3d.baseBrowser.cursor;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using UnityEngine;

namespace umi3d.baseBrowser.connection
{
    public class BaseConnectionProcess : inetum.unityUtils.PersistentSingleBehaviour<BaseConnectionProcess>
    {
        protected override void Awake()
        {
            base.Awake();

            //currentConnectionData = preferences.ServerPreferences.GetPreviousConnectionData() ?? new preferences.ServerPreferences.Data();

            Debug.Log("TODO : Not force mono speaker mode. For now forced for bluetooth headset.");
        }

        void Start()
        {
            UMI3DCollaborationClientServer.EnvironmentProgress = () =>
            {
                var p = new MultiProgress("Join Environment");
                //p.ResumeAfterFail = ResumeAfterFail;
                p.ResumeAfterFail = async (e) =>
                {
                    await Task.Delay(10000);
                    UnityEngine.Debug.Log("<color=Orange>Join environment fail: </color>" + $"{e}");
                    return true;
                };

                return p;
            };

            UMI3DCollaborationClientServer.Instance.OnLeavingEnvironment.AddListener(() => LeaveWithoutNotify());
        }

        #region Launcher

        [HideInInspector]
        public event System.Action<float> LoadingEnvironment;
        [HideInInspector]
        public event System.Action LoadedLauncher;

        public void ResetLauncherEvent()
        {
            LoadingEnvironment = null;
            LoadedLauncher = null;
        }


        //protected void StoreServer()
        //{
        //    if (savedServers.Find((server) => server.serverName == currentServer.serverName) == null)
        //        savedServers.Add(currentServer);
        //    preferences.ServerPreferences.StoreRegisteredServerData(savedServers);
        //}

        ///// <summary>
        ///// Store current connection data and Load environment.
        ///// </summary>
        //protected void StoreCurrentConnectionDataAndConnect()
        //{
        //    preferences.ServerPreferences.StoreUserData(currentConnectionData);
        //    LoadEnvironment();
        //}

        //protected async void LoadEnvironment()
        //{
        //    var loadAsync = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GamePanelScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);

        //    while (!loadAsync.isDone || !cdk.collaboration.UMI3DCollaborationClientServer.Exists)
        //    {
        //        LoadingEnvironment?.Invoke(loadAsync.progress);
        //        await UMI3DAsyncManager.Yield();
        //    }

        //    cdk.UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => LoadedEnvironment?.Invoke());

        //    cdk.collaboration.UMI3DCollaborationClientServer.Instance.Clear();
        //    Connect();

        //    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(LauncherPanelScene);
        //}

        #endregion

        #region Environment

        [HideInInspector]
        public event System.Action LoadedEnvironment;
        [HideInInspector]
        public event System.Action EnvironmentLeave;
        [HideInInspector]
        public event System.Action<string> ForcedLeave;
        [HideInInspector]
        public event System.Action<float> LoadingLauncher;
        [HideInInspector]
        public event System.Action<string, string, System.Action<int>> DisplayPopUpAfterLoadingFailed;
        [HideInInspector]
        public event System.Action EnvironmentLoaded;
        [HideInInspector]
        public event System.Action<int> UserCountUpdated;

        bool rememberNe = false;
        bool rememberLe = false;
        bool rememberUe = false;

        async Task<bool> ResumeAfterFail(System.Exception e)
        {
            switch (e)
            {
                case Umi3dNetworkingException ne:
                    if (rememberNe) return true;
                    (bool, bool) c = (false, false);
                    switch (ne.errorCode)
                    {
                        case 404:
                            //c = await DisplayPopUp("Resources not found",ne.Message +"\n"+ne.url);
                            Debug.Log("TODO : display pop up again");
                            return true;
                            break;
                        case 204:
                            c = await DisplayPopUp("Resources Downloaded was empty", ne.Message + "\n" + ne.url);
                            break;
                        case 501:
                            c = await DisplayPopUp("Resources acess was denied", ne.Message + "\n" + ne.url);
                            break;
                        default:
                            c = await DisplayPopUp($"Networking error {ne.errorCode}", $"{ne.Message}\n{ne.url}");
                            break;
                    }
                    rememberNe = c.Item2;
                    return c.Item1;
                case Umi3dLoadingException le:
                    if (rememberLe) return true;
                    (bool, bool) lc = await DisplayPopUp("Loading error", $"{le.Message}");
                    rememberLe = lc.Item2;
                    return lc.Item1;
                case Umi3dException ue:
                    if (rememberUe) return true;
                    (bool, bool) uc = await DisplayPopUp("Error", $"{ue.Message}");
                    rememberUe = uc.Item2;
                    return uc.Item1;
            }
            (bool, bool) u = await DisplayPopUp("Other Error", $"{e.Message}");
            rememberUe = u.Item2;
            return true;
        }

        async Task<(bool, bool)> DisplayPopUp(string title, string message)
        {
            UnityEngine.Debug.Log(title + " \n" + message + " \n" + "Ignore and resume loading ? ");

            bool? choise = null;
            System.Action<int> action = (index) => { choise = index == 0; };

            DisplayPopUpAfterLoadingFailed?.Invoke(title, message, action);

            while (choise == null) await UMI3DAsyncManager.Yield();
            return (choise.Value, false);
        }

        public void ResetEnvironmentEvents()
        {
            LoadedEnvironment = null;
            EnvironmentLeave = null;
            ForcedLeave = null;
            DisplayPopUpAfterLoadingFailed = null;
            EnvironmentLoaded = null;
            UserCountUpdated = null;
        }


        public void Leave() => UMI3DCollaborationClientServer.Logout();

        /// <summary>
        /// Clears the environment and goes back to the launcher.
        /// </summary>
        public void LeaveWithoutNotify()
        {
            cdk.UMI3DEnvironmentLoader.Clear();
            cdk.UMI3DResourcesManager.Instance.ClearCache();

            BaseCursor.SetMovement(this, BaseCursor.CursorMovement.Free);

            EnvironmentLeave?.Invoke();

            //LoadLauncher();
        }

        public void TryReconnecting() => cdk.collaboration.UMI3DCollaborationClientServer.Reconnect();

        //protected async void LoadLauncher()
        //{
        //    var loadAsync = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(LauncherPanelScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);

        //    while (!loadAsync.isDone)
        //    {
        //        LoadingLauncher?.Invoke(loadAsync.progress);
        //        await UMI3DAsyncManager.Yield();
        //    }

        //    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(GamePanelScene);
        //}

        #endregion
    }
}
