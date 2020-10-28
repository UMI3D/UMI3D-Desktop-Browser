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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using umi3d.cdk;
using umi3d.common;
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class SessionInformationMenu : Singleton<SessionInformationMenu>
{
    public PanelRenderer panelRenderer;

    VisualElement sessionInfo;

    Label sessionTime;
    Button activeSound;
    Button activeAvatar;
    Button muteBtn;
    Button hangUpBtn;
    Label environmentName;
    Button isFavoriteBtn;

    bool isEnvironmentFavorite;

    DateTime startOfSession = new DateTime();

    LauncherManager.Data currentData;
    List<LauncherManager.Data> favorites;

    void Start()
    {
        var root = panelRenderer.visualTree;

        UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() =>
        {
            startOfSession = DateTime.Now;
        });

        sessionInfo = root.Q<VisualElement>("session-info");
        sessionTime = sessionInfo.Q<Label>("session-time");
        activeSound = sessionInfo.Q<Button>("active-sound");
        activeSound.clickable.clicked += () =>
        {
            throw new NotImplementedException("TODO : activeSound button");
        };

        activeAvatar = sessionInfo.Q<Button>("active-avatar");
        activeAvatar.clickable.clicked += () =>
        {
            throw new NotImplementedException("TODO : activeAvatar button");
        };

        muteBtn = sessionInfo.Q<Button>("mute-btn");
        muteBtn.clickable.clicked += () =>
        {
            ActivateDeactivateMicrophone.Instance.ToggleMicrophoneStatus();
        };

        hangUpBtn = sessionInfo.Q<Button>("hang-up-btn");
        hangUpBtn.clickable.clicked += () =>
        {
            throw new NotImplementedException("TODO : hang up button");
        };

        isFavoriteBtn = root.Q<Button>("is-favorite-btn");
        isFavoriteBtn.clickable.clicked += () =>
        {
            isEnvironmentFavorite = !isEnvironmentFavorite;

            if (isEnvironmentFavorite)
            {
                favorites.Add(currentData);
            }
            else
            {
                favorites.Remove(currentData);
            }
            isFavoriteBtn.ToggleInClassList("not-favorite");
            isFavoriteBtn.ToggleInClassList("is-favorite");

            StoreFavoriteConnectionData();
        };
    }

    /// <summary>
    /// Stores the connection data about the favorite environments.
    /// </summary>
    void StoreFavoriteConnectionData()
    {
        string path = umi3d.common.Path.Combine(Application.persistentDataPath, LauncherManager.favoriteDataFile);
        FileStream file;
        if (File.Exists(path)) file = File.OpenWrite(path);
        else file = File.Create(path);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, favorites);
        file.Close();
    }

    private void Update()
    {
        var time = DateTime.Now - startOfSession;
        sessionTime.text = time.ToString("hh") + ":" + time.ToString("mm") + ":" + time.ToString("ss");
    }

    public static void Display(bool val)
    {
        if (Exists)
        {
            Instance._Display(val);
        } else
        {
            Debug.LogError("SessionInformationMenu does not exist in the scene");
        }
    }

    private void _Display(bool val)
    {
        sessionInfo.style.display = val ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void OnMicrophoneStatusChanged(bool val)
    {
        if (val)
        {
            muteBtn.RemoveFromClassList("session-info-btn-mute-off");
            muteBtn.AddToClassList("session-info-btn-mute-on");
        }
            
        else
        {
            muteBtn.RemoveFromClassList("session-info-btn-mute-on");
            muteBtn.AddToClassList("session-info-btn-mute-off");
        }
           
    }

    public void SetEnvironmentName(MediaDto media, LauncherManager.Data data)
    {
        currentData = data;

        environmentName = panelRenderer.visualTree.Q<Label>("environment-name");
        environmentName.text = media.name;

        favorites = GetFavoriteConnectionData();

        isEnvironmentFavorite = favorites.Find(d => d.ip == media.httpUrl) != null;

        if (isEnvironmentFavorite)
        {
            isFavoriteBtn.AddToClassList("is-favorite");
        }
        else
        {
            isFavoriteBtn.AddToClassList("not-favorite");
        }
    }
     private List<LauncherManager.Data> GetFavoriteConnectionData()
    {
        string path = umi3d.common.Path.Combine(Application.persistentDataPath, LauncherManager.favoriteDataFile);
        if (File.Exists(path))
        {
            FileStream file;
            file = File.OpenRead(path);
            BinaryFormatter bf = new BinaryFormatter();
            List<LauncherManager.Data> data = (List<LauncherManager.Data>)bf.Deserialize(file);
            file.Close();
            return data;
        }
        return new List<LauncherManager.Data>();
    }
}
