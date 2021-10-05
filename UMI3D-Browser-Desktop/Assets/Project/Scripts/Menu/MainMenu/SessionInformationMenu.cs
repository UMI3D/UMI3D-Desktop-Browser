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

using System;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.common;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    /// <summary>
    /// This class manages the UI elements which gives information about the current session such as : the name of the environment
    /// (is it a favorite ?), is the microphone working, the session tim, etc.
    /// </summary>
    public class SessionInformationMenu : Singleton<SessionInformationMenu>
    {
        public UIDocument uiDocument;

        #region Top Bar

        //Top Bar
        VisualElement topCenterMenu;
        Label environmentName;
        Button isFavoriteBtn;

        bool isEnvironmentFavorite;

        #endregion

        #region

        //Main Menu
        VisualElement applicationSettings;
        Button microphoneBtn;

        #endregion

        #region Bottom Bar

        //Bottom Bar
        VisualElement sessionInfo;
        Label sessionTime;
        Label participantsCount;

        DateTime startOfSession = new DateTime();
        UserPreferencesManager.Data currentData;
        List<UserPreferencesManager.Data> favorites;

        #endregion

        /// <summary>
        /// Binds the UI
        /// </summary>
        void Start()
        {
            UnityEngine.Debug.Assert(uiDocument != null);
            var root = uiDocument.rootVisualElement;

            //Top Bar
            topCenterMenu = root.Q<VisualElement>("top-center-menu");
            topCenterMenu.style.display = DisplayStyle.None;

            isFavoriteBtn = root.Q<Button>("is-favorite-btn");
            isFavoriteBtn.clickable.clicked += ToggleAddEnvironmentToFavorites;

            //Main Menu
            /*
            applicationSettings = root.Q<VisualElement>("application-settings");
            microphoneBtn = applicationSettings.Q<Button>("microphone-btn");
            microphoneBtn.clickable.clicked += () =>
            {
                ActivateDeactivateMicrophone.Instance.ToggleMicrophoneStatus();
            };
            */

            //Bottom Bar
            sessionInfo = root.Q<VisualElement>("session-info");
            sessionTime = sessionInfo.Q<Label>("session-time");
            participantsCount = sessionInfo.Q<Label>("participants-count");
            //umi3d.cdk.collaboration.UMI3DUser.OnNewUser.AddListener(UpdateParticipantsCount);
            //umi3d.cdk.collaboration.UMI3DUser.OnRemoveUser.AddListener(UpdateParticipantsCount);
            umi3d.cdk.collaboration.UMI3DCollaborationEnvironmentLoader.OnUpdateUserList.AddListener(UpdateParticipantsCount);


            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() =>
            {
                startOfSession = DateTime.Now;
                topCenterMenu.style.display = DisplayStyle.Flex;
            });
        }

        private void Update()
        {
            var time = DateTime.Now - startOfSession;
            sessionTime.text = time.ToString("hh") + ":" + time.ToString("mm") + ":" + time.ToString("ss");
        }


        /// <summary>
        /// Event called when the status of the microphone changes.
        /// </summary>
        /// <param name="val"></param>
        public void OnMicrophoneStatusChanged(bool val)
        {
            if (val)
            {
                microphoneBtn.RemoveFromClassList("btn-mic-off");
                microphoneBtn.AddToClassList("btn-mic-on");
            }

            else
            {
                microphoneBtn.RemoveFromClassList("btn-mic-on");
                microphoneBtn.AddToClassList("btn-mic-off");
            }

        }

        /// <summary>
        /// Initiates the custom title bar with the name of the environment.
        /// </summary>
        /// <param name="media"></param>
        /// <param name="data"></param>
        public void SetEnvironmentName(MediaDto media, UserPreferencesManager.Data data)
        {
            currentData = data;

            environmentName = uiDocument.rootVisualElement.Q<Label>("environment-name");
            environmentName.text = media.name;

            favorites = UserPreferencesManager.GetFavoriteConnectionData();

            isEnvironmentFavorite = favorites.Find(d => "http://" + d.ip == media.connection.httpUrl) != null;

            isFavoriteBtn.ClearClassList();

            if (isEnvironmentFavorite)
            {
                isFavoriteBtn.AddToClassList("is-favorite");
            }
            else
            {
                isFavoriteBtn.AddToClassList("not-favorite");
            }
        }

        public void ToggleAddEnvironmentToFavorites()
        {
            isEnvironmentFavorite = !isEnvironmentFavorite;

            NotificationDto notif = new NotificationDto { duration = 3, title = ""};

            if (isEnvironmentFavorite)
            {
                favorites.Add(currentData);
                notif.content = "Environment added to favorites";
            }
            else
            {
                favorites.Remove(favorites.Find(d => d.ip == currentData.ip));
                notif.content = "Environment removed from favorites";
            }

            NotificationDisplayer.Instance.DisplayNotification(notif);

            isFavoriteBtn.ToggleInClassList("not-favorite");
            isFavoriteBtn.ToggleInClassList("is-favorite");

            UserPreferencesManager.StoreFavoriteConnectionData(favorites);
        }

        /// <summary>
        /// Update the participants count when a user connect or disconnect to the environment
        /// </summary>
        /// <param name="user"></param>
        public void UpdateParticipantsCount()
        {
            int usersCount = umi3d.cdk.collaboration.UMI3DCollaborationEnvironmentLoader.Instance.UserList.Count;

            participantsCount.text = usersCount < 2 ? usersCount + " participant" : usersCount + " participants";
        }
    }
}