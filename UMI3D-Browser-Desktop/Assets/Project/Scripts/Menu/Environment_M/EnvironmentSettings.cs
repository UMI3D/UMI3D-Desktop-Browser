/*
Copyright 2019 - 2021 Inetum

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
using BrowserDesktop.Controller;
using BrowserDesktop.Menu;
using DesktopBrowser.UIControllers;
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.cdk.userCapture;
using UnityEngine;

public interface ISetting
{
    public bool EnvironmentLoaded { get; set; }
    public bool IsOn { get; }
    public MenuBar_UIController menuBar { get; }
    public void Start();
    public void Update();
    public void Toggle();
}

public struct AudioSetting : ISetting
{
    public bool EnvironmentLoaded { get; set; }
    public bool IsOn { get;  private set; }
    public MenuBar_UIController menuBar { get; }

    public AudioSetting(MenuBar_UIController menuBar)
    {
        this.menuBar = menuBar;
        EnvironmentLoaded = false;
        IsOn = true;
        Start();
    }

    public void Start()
    {
        menuBar.ToggleAudio = Toggle;
        menuBar.OnAudioStatusChanged(IsOn);
    }

    public void Toggle()
    {
        if (!EnvironmentLoaded) return;

        IsOn = !IsOn;
        if (IsOn) AudioListener.volume = 1f;
        else AudioListener.volume = 0f;

        menuBar.OnAudioStatusChanged(IsOn);
    }

    public void Update()
    {
        if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ToggleAudio)) && 
            !TextInputDisplayerElement.isTyping)
        {
            Toggle();
        }
    }
}

public struct AvatarSetting : ISetting
{
    public bool EnvironmentLoaded { get; set; }
    public bool IsOn 
    { 
        get => UMI3DClientUserTracking.Instance.SendTracking; 
        private set => UMI3DClientUserTracking.Instance.setTrackingSending(value);
    }
    public MenuBar_UIController menuBar { get; }

    public AvatarSetting(MenuBar_UIController menuBar)
    {
        this.menuBar = menuBar;
        EnvironmentLoaded = false;
        //IsOn = true;
        Start();
    }

    public void Start()
    {
        menuBar.ToggleAvatarTracking = Toggle;
        menuBar.OnAvatarTrackingChanged(IsOn);
    }

    public void Toggle()
    {
        if (!EnvironmentLoaded) return;

        IsOn = !IsOn;
        menuBar.OnAvatarTrackingChanged(IsOn);
    }

    public void Update()
    {
        if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ToggleAvatar)) && 
            !TextInputDisplayerElement.isTyping)
        {
            Toggle();
        }
    }
}

public struct MicSetting : ISetting
{
    public bool EnvironmentLoaded { get; set; }
    public bool IsOn 
    { 
        get => !MicrophoneListener.IsMute; 
        private set => MicrophoneListener.IsMute = !value; 
    }
    public MenuBar_UIController menuBar { get; }

    public MicSetting(MenuBar_UIController menuBar)
    {
        this.menuBar = menuBar;
        EnvironmentLoaded = false;
        IsOn = false;
        Start();
    }

    public void Start()
    {
        menuBar.ToggleMic = Toggle;
        menuBar.OnMicrophoneStatusChanged(IsOn);
    }

    public void Toggle()
    {
        if (!EnvironmentLoaded) return;

        IsOn = !IsOn;
        menuBar.OnMicrophoneStatusChanged(IsOn);
    }

    public void Update()
    {
        if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ToggleMicrophone)) && 
            !TextInputDisplayerElement.isTyping)
        {
            Toggle();
        }
    }
}

public sealed class EnvironmentSettings : MonoBehaviour
{
    private AudioSetting audioSetting;
    private AvatarSetting avatarSetting;
    private MicSetting micSetting;
    private MenuBar_UIController menuBar;
    private bool initialized = false;

    void Start()
    {
        menuBar = UIController.GetUIController("menuBar") as MenuBar_UIController;
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        while (!menuBar.Initialized) yield return null;
        avatarSetting = new AvatarSetting(menuBar);
        audioSetting = new AudioSetting(menuBar);
        micSetting = new MicSetting(menuBar);
        UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => {
            audioSetting.EnvironmentLoaded = true;
            avatarSetting.EnvironmentLoaded = true;
            micSetting.EnvironmentLoaded = true;
        });
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized) return;
        avatarSetting.Update();
        audioSetting.Update();
        micSetting.Update();
    }
}
