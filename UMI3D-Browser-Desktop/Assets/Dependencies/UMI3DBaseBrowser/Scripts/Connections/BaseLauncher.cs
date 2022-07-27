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

        protected VisualElement root => document.rootVisualElement;
        #endregion

        protected cdk.collaboration.LaucherOnMasterServer masterServer;

        protected virtual void Start()
        {
            Debug.Assert(document != null);

            masterServer = new cdk.collaboration.LaucherOnMasterServer();
        }
    }
}
