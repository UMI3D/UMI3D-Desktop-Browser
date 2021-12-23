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
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.cdk.userCapture;
using UnityEngine;

public interface ISetting
{
    public bool EnvironmentLoaded { get; set; }
    public bool IsOn { get; }
    public MenuBar_UIController MenuBar { get; }
    public void Start();
    public void Update();
    public void Toggle();
}

public struct AudioSetting : ISetting
{
    public bool EnvironmentLoaded { get; set; }
    public bool IsOn { get;  private set; }
    public MenuBar_UIController MenuBar { get; }

    public AudioSetting(MenuBar_UIController menuBar)
    {
        this.MenuBar = menuBar;
        EnvironmentLoaded = false;
        IsOn = true;
        Start();
    }

    public void Start()
    {
        MenuBar.ToggleAudio = Toggle;
        MenuBar.OnAudioStatusChanged(IsOn);
    }

    public void Toggle()
    {
        if (!EnvironmentLoaded) return;

        IsOn = !IsOn;
        if (IsOn) AudioListener.volume = 1f;
        else AudioListener.volume = 0f;

        MenuBar.OnAudioStatusChanged(IsOn);
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
        get => (userTracking != null) ? userTracking.SendTracking : false;
        private set => userTracking?.setTrackingSending(value);
    }
    public MenuBar_UIController MenuBar { get; }

    private UMI3DClientUserTracking userTracking;

    public AvatarSetting(MenuBar_UIController menuBar, UMI3DClientUserTracking userTracking)
    {
        this.MenuBar = menuBar;
        this.userTracking = userTracking;
        EnvironmentLoaded = false;
        IsOn = true;
        Start();
    }

    public void Start()
    {
        MenuBar.ToggleAvatarTracking = Toggle;
        MenuBar.OnAvatarTrackingChanged(IsOn);
    }

    public void Toggle()
    {
        if (!EnvironmentLoaded) return;

        IsOn = !IsOn;
        MenuBar.OnAvatarTrackingChanged(IsOn);
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
    public MenuBar_UIController MenuBar { get; }

    public MicSetting(MenuBar_UIController menuBar)
    {
        this.MenuBar = menuBar;
        EnvironmentLoaded = false;
        IsOn = false;
        Start();
    }

    public void Start()
    {
        MenuBar.ToggleMic = Toggle;
        MenuBar.OnMicrophoneStatusChanged(IsOn);
    }

    public void Toggle()
    {
        if (!EnvironmentLoaded) return;

        IsOn = !IsOn;
        MenuBar.OnMicrophoneStatusChanged(IsOn);
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
    [SerializeField]
    private UMI3DEnvironmentLoader environmentLoader;
    [SerializeField]
    private UMI3DClientUserTracking userTracking;

    private AudioSetting audioSetting;
    private AvatarSetting avatarSetting;
    private MicSetting micSetting;

    private MenuBar_UIController menuBar;

    private bool initialized = false;

    private void Awake()
    {
        Debug.Assert(environmentLoader != null, "environmentLoader null in EnvironmentSettings.");
        Debug.Assert(userTracking != null, "userTracking null in EnvironmentSettings.");
    }

    void Start()
    {
        menuBar = UIController.GetUIController("menuBar") as MenuBar_UIController;
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        while (!menuBar.Initialized) yield return null;
        avatarSetting = new AvatarSetting(menuBar, userTracking);
        audioSetting = new AudioSetting(menuBar);
        micSetting = new MicSetting(menuBar);
        environmentLoader?.onEnvironmentLoaded.AddListener(() => {
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
