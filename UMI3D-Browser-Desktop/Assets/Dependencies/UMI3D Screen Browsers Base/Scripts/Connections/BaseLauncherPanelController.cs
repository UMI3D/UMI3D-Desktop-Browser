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
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.connection
{
    public abstract class BaseLauncherPanelController : MonoBehaviour
    {
        public CustomLauncher Launcher;

        [SerializeField]
        protected UIDocument document;
        protected VisualElement root => document.rootVisualElement;

        protected virtual void Start()
        {
            Debug.Assert(document != null);

            Launcher = root.Q<CustomLauncher>();
#if UNITY_STANDALONE
            Launcher.Version = BrowserDesktop.BrowserVersion.Version;
#else
            Launcher.Version = Application.version;
#endif
            Launcher.Connect = async (value) => await BaseConnectionProcess.Instance.InitConnect(value);
            Launcher.StoreCurrentConnectionDataAndConnect = (ip, port) => BaseConnectionProcess.Instance.ConnectWithMediaDto(ip, port);
            Launcher.CurrentServer = BaseConnectionProcess.Instance.currentServer;
            Launcher.SavedServers = BaseConnectionProcess.Instance.savedServers;
            Launcher.CurrentConnectionData = BaseConnectionProcess.Instance.currentConnectionData;
            Launcher.InitLibraries();
            Launcher.CurrentScreen = LauncherScreens.Home;

            BaseConnectionProcess.Instance.ResetLauncherEvent();
            //BaseConnectionProcess.Instance.DisplaySessions += () => Launcher.AddScreenToStack = LauncherScreens.Session;
        }
    }
}
