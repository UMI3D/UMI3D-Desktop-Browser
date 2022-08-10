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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.connection
{
    public class BaseLauncher : MonoBehaviour
    {
        protected const string currentScene = "Connection";
        protected const string sceneToLoad = "Environement";

        #region Data
        protected preferences.ServerPreferences.ServerData currentServer;
        protected List<preferences.ServerPreferences.ServerData> savedServers;
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
        #endregion

        protected cdk.collaboration.LaucherOnMasterServer masterServer;

        protected virtual void Start()
        {
            Debug.Assert(document != null);
            Debug.Assert(libraryEntry != null);
            Debug.Assert(sessionEntry != null);
            Debug.Assert(savedServerEntry != null);

            masterServer = new cdk.collaboration.LaucherOnMasterServer();
        }

        /// <summary>
        /// Load the environment scene when it is ready.
        /// </summary>
        /// <returns></returns>
        protected IEnumerator WaitReady(preferences.ServerPreferences.Data data)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad, UnityEngine.SceneManagement.LoadSceneMode.Additive);

            while (!ConnectionMenu.Exists) yield return new WaitForEndOfFrame();

            ConnectionMenu.Instance.Connect(data);
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(currentScene);
        }
    }
}
