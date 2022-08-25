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
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.connection
{
    public abstract class BaseLauncher : MonoBehaviour
    {
        protected const string currentScene = "Connection";
        protected const string sceneToLoad = "Environement";

        #region Data
        protected preferences.ServerPreferences.ServerData currentServer;
        protected System.Collections.Generic.List<preferences.ServerPreferences.ServerData> savedServers;
        protected preferences.ServerPreferences.Data currentConnectionData;
        #endregion

        #region UI
        [SerializeField]
        protected UIDocument document;
        [SerializeField]
        protected VisualTreeAsset libraryEntry;
        [SerializeField]
        protected VisualTreeAsset sessionEntry;
        [SerializeField]
        protected VisualTreeAsset savedServerEntry;

        protected VisualElement root => document.rootVisualElement;
        //Session Screen
        protected bool ShouldDisplaySessionScreen = false;

        protected abstract void InitUI();

        /// <summary>
        /// Displays advanced connection screen.
        /// </summary>
        public abstract void DisplayHomeScreen();
        /// <summary>
        /// Displays session screen
        /// </summary>
        public abstract void DisplaySessionScreen();
        /// <summary>
        /// Displays advanced connection screen.
        /// </summary>
        public abstract void DisplayAdvConnectionScreen();
        /// <summary>
        /// Displays Libraries manager screen.
        /// </summary>
        public abstract void DisplayLibManagerScreen();
        /// <summary>
        /// Displays a dialogue box
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="optionA"></param>
        /// <param name="optionB"></param>
        /// <param name="choiceCallback"></param>
        public abstract void DisplayDialogueBox(string title, string message, string optionA, string optionB, System.Action<bool> choiceCallback);
        #endregion

        protected cdk.collaboration.LaucherOnMasterServer masterServer;

        protected virtual void Start()
        {
            Debug.Assert(document != null);
            Debug.Assert(libraryEntry != null);
            Debug.Assert(sessionEntry != null);
            Debug.Assert(savedServerEntry != null);

            currentServer = preferences.ServerPreferences.GetPreviousServerData() ?? new preferences.ServerPreferences.ServerData();
            currentConnectionData = preferences.ServerPreferences.GetPreviousConnectionData() ?? new preferences.ServerPreferences.Data();
            savedServers = preferences.ServerPreferences.GetRegisteredServerData() ?? new System.Collections.Generic.List<preferences.ServerPreferences.ServerData>();
            masterServer = new cdk.collaboration.LaucherOnMasterServer();

            InitUI();
        }

        /// <summary>
        /// Initiates the connection to the forge master server.
        /// </summary>
        protected async void Connect(bool saveInfo = false)
        {
            void StoreServer()
            {
                if (savedServers.Find((server) => server.serverName == currentServer.serverName) == null) savedServers.Add(currentServer);
                preferences.ServerPreferences.StoreRegisteredServerData(savedServers);
            }

            bool mediaDtoFound = false;
            bool masterServerFound = false;

            //1. Try to find a master server
            masterServer.ConnectToMasterServer(() =>
            {
                if (mediaDtoFound) return;

                masterServer.RequestInfo((name, icon) =>
                {
                    if (mediaDtoFound) return;
                    masterServerFound = true;

                    currentServer.serverName = name;
                    currentServer.serverIcon = icon;
                    preferences.ServerPreferences.StoreUserData(currentServer);
                    if (saveInfo) StoreServer();
                });

                ShouldDisplaySessionScreen = true;
            }, currentServer.serverUrl);

            //2. try to get a mediaDto
            var media = await ConnectionMenu.GetMediaDto(currentServer);
            if (media == null || masterServerFound) return;
            mediaDtoFound = true;

            currentServer.serverName = media.name;
            currentServer.serverIcon = media?.icon2D?.variants?.FirstOrDefault()?.url;
            preferences.ServerPreferences.StoreUserData(currentServer);
            if (saveInfo) StoreServer();

            currentConnectionData.environmentName = media.name;
            currentConnectionData.ip = media.url;
            currentConnectionData.port = null;
            StoreCurrentConnectionDataAndConnect();
        }

        /// <summary>
        /// Store current connection data and initiates the connection to the environment.
        /// </summary>
        protected void StoreCurrentConnectionDataAndConnect()
        {
            preferences.ServerPreferences.StoreUserData(currentConnectionData);
            StartCoroutine(WaitReady(currentConnectionData));
        }

        /// <summary>
        /// Load the environment scene when it is ready.
        /// </summary>
        /// <returns></returns>
        protected System.Collections.IEnumerator WaitReady(preferences.ServerPreferences.Data data)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad, UnityEngine.SceneManagement.LoadSceneMode.Additive);

            while (!ConnectionMenu.Exists) yield return new WaitForEndOfFrame();
            while (!cdk.collaboration.UMI3DCollaborationClientServer.Exists) yield return new WaitForEndOfFrame();
            cdk.collaboration.UMI3DCollaborationClientServer.Instance.Clear();
            ConnectionMenu.Instance.Connect(data);
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(currentScene);
        }
    }
}
