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
using System;
using System.Collections;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.cdk.userCapture;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;

public interface ISetting
{
    public bool IsOn { get; }
    Action<bool> m_statusChanged { get; }
    public void Start();
    public void Update();
    public void Toggle();
}

public class AudioSetting : ISetting
{
    public bool IsOn { get;  private set; }
    public Action<bool> m_statusChanged { get; private set; }

    public AudioSetting()
    {
        IsOn = true;
        m_statusChanged = MenuBar_E.Instance.Sound.Toggle;
        MenuBar_E.Instance.Sound.OnClicked = Toggle;
        Start();
    }

    public void Start()
        => m_statusChanged(IsOn);

    public void Toggle()
    {
        IsOn = !IsOn;
        if (IsOn) AudioListener.volume = 1f;
        else AudioListener.volume = 0f;

        m_statusChanged(IsOn);
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

public class AvatarSetting : ISetting
{
    public bool IsOn 
    { 
        get => (userTracking != null) ? userTracking.SendTracking : false;
        private set => userTracking?.setTrackingSending(value);
    }
    public Action<bool> m_statusChanged { get; private set; }

    private UMI3DClientUserTracking userTracking => UMI3DClientUserTracking.Instance;

    public AvatarSetting()
    {
        IsOn = true;
        m_statusChanged = MenuBar_E.Instance.Avatar.Toggle;
        MenuBar_E.Instance.Avatar.OnClicked = Toggle;
        Start();
    }

    public void Start()
        => m_statusChanged(IsOn);

    public void Toggle()
    {
        IsOn = !IsOn;
        m_statusChanged(IsOn);
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

public class MicSetting : ISetting
{
    public bool IsOn 
    { 
        get => !MicrophoneListener.IsMute; 
        private set => MicrophoneListener.IsMute = !value; 
    }
    public Action<bool> m_statusChanged { get; private set; }

    public MicSetting()
    {
        IsOn = false;
        m_statusChanged = MenuBar_E.Instance.Mic.Toggle;
        MenuBar_E.Instance.Mic.OnClicked = Toggle;
        Start();
    }

    public void Start()
        => m_statusChanged(IsOn);

    public void Toggle()
    {
        IsOn = !IsOn;
        m_statusChanged(IsOn);
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
    private UMI3DEnvironmentLoader m_environmentLoader => UMI3DEnvironmentLoader.Instance;

    private bool m_environmentLoaded { get; set; } = false;
    private AudioSetting audioSetting;
    private AvatarSetting avatarSetting;
    private MicSetting micSetting;

    private bool initialized = false;

    void Start()
    {
        //StartCoroutine(Initialize());
        avatarSetting = new AvatarSetting();
        audioSetting = new AudioSetting();
        micSetting = new MicSetting();
        m_environmentLoader.onEnvironmentLoaded.AddListener(() => {
            m_environmentLoaded = true;
        });
        initialized = true;
    }

    private IEnumerator Initialize()
    {
        //while (!MenuBar_E.Instance.Initialized) yield return null;
        
        yield return null;
    }

    void Update()
    {
        if (!m_environmentLoaded || !initialized) return;
        avatarSetting.Update();
        audioSetting.Update();
        micSetting.Update();
    }
}