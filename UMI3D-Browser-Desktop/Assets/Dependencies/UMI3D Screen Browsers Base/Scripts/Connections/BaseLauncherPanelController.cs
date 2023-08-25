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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.commonScreen;
using umi3d.commonScreen.Displayer;
using umi3d.commonScreen.menu;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.connection
{
    public abstract class BaseLauncherPanelController : MonoBehaviour
    {
        public Launcher_C Launcher;

        [SerializeField]
        protected UIDocument document;
        protected VisualElement root => document.rootVisualElement;
        protected Dialoguebox_C m_connectionDialoguebox;

        List<UMI3DConnectionData> favoriteConnections;

        protected virtual void Start()
        {
            Debug.Assert(document != null);

            favoriteConnections = UMI3DClient.GetFavorites_CD();

            Screen.sleepTimeout = SleepTimeout.SystemSetting;

            root.Add(TooltipsLayer_C.Instance);

            Launcher = root.Q<Launcher_C>();
#if !UNITY_STANDALONE
            Launcher.Version = Application.version;
#endif
            Launcher.Settings.Audio.SetAudio();
            //Launcher.CurrentServer = BaseConnectionProcess.Instance.currentServer;
            //Launcher.SavedServers = BaseConnectionProcess.Instance.savedServers;
            //Launcher.CurrentConnectionData = BaseConnectionProcess.Instance.currentConnectionData;
            Launcher.InitLibraries();
            Launcher.InitTips();
            Launcher.CurrentScreen = LauncherScreens.Home;

            BaseConnectionProcess.Instance.ResetLauncherEvent();
            //BaseConnectionProcess.Instance.DisplaySessions += () => Launcher.AddScreenToStack = LauncherScreens.Session;

            m_connectionDialoguebox = new Dialoguebox_C();
            m_connectionDialoguebox.Type = DialogueboxType.Default;
            m_connectionDialoguebox.Size = ElementSize.Small;
            BaseConnectionProcess.Instance.ConnectionInitialized += ConnectionInitialized;
            BaseConnectionProcess.Instance.ConnectionInitializationFailled += ConnectionFailled;
        }

        private void OnDestroy()
        {
            Dialoguebox_C.ResetAllQueue();
        }

        protected virtual void ConnectionInitialized(string url)
        {
            m_connectionDialoguebox.Title = "Connection to a server:";
            m_connectionDialoguebox.Message = $"Try connecting to \n\n\"{url}\" \n\nIt may take some time.";
            m_connectionDialoguebox.ChoicesContainer.style.display = DisplayStyle.None;
            m_connectionDialoguebox.Enqueue(root);
        }

        protected virtual void ConnectionFailled(string url)
        {
            m_connectionDialoguebox.Title = "Failled to connect to server";
            m_connectionDialoguebox.Message = $"Browser was not able to connect to \n\n\"{url}\"";
            m_connectionDialoguebox.ChoicesContainer.style.display = DisplayStyle.Flex;
            m_connectionDialoguebox.ChoiceAText = "Ok";
        }
    }
}
