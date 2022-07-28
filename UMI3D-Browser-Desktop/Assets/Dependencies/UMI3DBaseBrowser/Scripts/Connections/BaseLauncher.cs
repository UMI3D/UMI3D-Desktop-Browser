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
        protected preferences.ServerPreferences.ServerData currentServerConnectionData;
        protected List<preferences.ServerPreferences.ServerData> serverConnectionData = new List<preferences.ServerPreferences.ServerData>();
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
