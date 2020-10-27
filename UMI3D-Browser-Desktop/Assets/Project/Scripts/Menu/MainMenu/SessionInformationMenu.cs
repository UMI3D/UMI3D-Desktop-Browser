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

    DateTime startOfSession = new DateTime();

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
}
