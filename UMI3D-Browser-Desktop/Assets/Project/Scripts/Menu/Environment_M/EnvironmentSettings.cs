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
using System.Linq;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.cdk.userCapture;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;
using UnityEngine.UIElements;

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
        m_statusChanged = Settingbox_E.Instance.Sound.Toggle;
        Settingbox_E.Instance.Sound.Clicked += Toggle;
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
        private set
        {
            userTracking?.setTrackingSending(value);
            UMI3DCollaborationEnvironmentLoader.Instance.GetClientUser()?.SetAvatarStatus(value);
        }
    }
    public Action<bool> m_statusChanged { get; private set; }

    private UMI3DClientUserTracking userTracking => UMI3DClientUserTracking.Instance;

    public AvatarSetting()
    {
        IsOn = true;
        m_statusChanged = Settingbox_E.Instance.Avatar.Toggle;
        Settingbox_E.Instance.Avatar.Clicked += Toggle;
        Start();
    }

    public void Start()
    {
        UMI3DUser.OnUserAvatarStatusUpdated.AddListener((u) =>
        {
            if (UMI3DCollaborationClientServer.Exists && u.id == UMI3DCollaborationClientServer.Instance.GetUserId())
            {
                IsOn = u.avatarStatus;
                m_statusChanged?.Invoke(IsOn);
            }
        });
        m_statusChanged?.Invoke(IsOn);
    }

    public void Toggle()
    {
        IsOn = !IsOn;
        m_statusChanged?.Invoke(IsOn);
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
        m_statusChanged = Settingbox_E.Instance.Mic.Toggle;
        Settingbox_E.Instance.Mic.Clicked += Toggle;
        Start();
    }

    public void Start()
    {
        UMI3DUser.OnUserMicrophoneStatusUpdated.AddListener((u) =>
        {
            if (UMI3DCollaborationClientServer.Exists && u.id == UMI3DCollaborationClientServer.Instance.GetUserId())
            {
                IsOn = u.microphoneStatus;
                m_statusChanged?.Invoke(IsOn);
            }
        });
        m_statusChanged?.Invoke(IsOn);
    }

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


public class AllMicSetting : ISetting
{
    public bool IsOn
    {
        get => false;
        private set{
            UMI3DUser.MuteAllMicrophone();
        }
    }
    public Action<bool> m_statusChanged { get; private set; }

    public AllMicSetting()
    {
        IsOn = false;
        m_statusChanged = Settingbox_E.Instance.AllMic.Toggle;
        Settingbox_E.Instance.AllMic.Clicked += Toggle;
        Start();
    }

    public void Start()
    {
        m_statusChanged?.Invoke(IsOn);
    }

    public void Toggle()
    {
        IsOn = !IsOn;
        m_statusChanged(IsOn);
    }

    public void Update()
    {
        if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.MuteAllMicrophone)) &&
            !TextInputDisplayerElement.isTyping)
        {
            Toggle();
        }
    }
}


public class UserListSetting
{
    public Action<bool> m_statusChanged { get; private set; }

    public class User : User_item_E
    {
        public UMI3DUser user {get;}

        public User(UMI3DUser user)
        {
            this.user = user;
        }

        public void ToggleMic()
        {
            user.SetMicrophoneStatus(!user.microphoneStatus);
        }

        public void ToggleAvatar()
        {
            user.SetMicrophoneStatus(!user.avatarStatus);
        }

        public void setValue() {
            Avatar?.Toggle(user.avatarStatus);
            Mic?.Toggle(user.microphoneStatus);
            Sound?.Toggle(true);
        }


        public override void Bind(VisualElement element)
        {
            base.Bind(element);
            setValue();
            Mic.Clicked += ToggleMic;
            element.Q<Label>("userLabel").text = user.login;
        }

        public override void Unbind(VisualElement element)
        {
            Mic.Clicked -= ToggleMic;
            base.Unbind(element);
        }
    }

    User[] Users;

    public UserListSetting()
    {
        UMI3DCollaborationEnvironmentLoader.OnUpdateUserList += RefreshList;

        UMI3DUser.OnUserMicrophoneStatusUpdated.AddListener((u) => { Users.FirstOrDefault(U => (U.user == u))?.setValue(); });
        UMI3DUser.OnUserAvatarStatusUpdated.AddListener((u) => { Users.FirstOrDefault(U => (U.user == u))?.setValue(); });
        UMI3DUser.OnUserAttentionStatusUpdated.AddListener((u) => { Users.FirstOrDefault(U => (U.user == u))?.setValue(); });

        UMI3DUser.OnRemoveUser.AddListener((u) => { Users.FirstOrDefault(U => (U.user == u))?.Unbind(null); });

        RefreshList();
    }

    void RefreshList()
    {
        Settingbox_E.Instance.UserList.Clear();
        InitUsers();
        Settingbox_E.Instance.UserList.AddRange(Users);
    }


    void InitUsers()
    {
        Users = UMI3DCollaborationEnvironmentLoader.Instance.UserList.Where(u=>!u.isClient).Select(u => new User(u)).ToArray();
    }
}

public sealed class EnvironmentSettings : MonoBehaviour
{
    private UMI3DEnvironmentLoader m_environmentLoader => UMI3DEnvironmentLoader.Instance;

    private bool m_environmentLoaded { get; set; } = false;
    private AudioSetting audioSetting;
    private AvatarSetting avatarSetting;
    private MicSetting micSetting;
    private AllMicSetting allMicSetting;

    private UserListSetting userListSetting;


    private bool initialized = false;

    void Start()
    {
        avatarSetting = new AvatarSetting();
        audioSetting = new AudioSetting();
        micSetting = new MicSetting();
        allMicSetting = new AllMicSetting();

        userListSetting = new UserListSetting();

        m_environmentLoader.onEnvironmentLoaded.AddListener(() => {
            m_environmentLoaded = true;
        });
        initialized = true;
    }

    void Update()
    {
        if (!m_environmentLoaded || !initialized) return;
        avatarSetting.Update();
        audioSetting.Update();
        micSetting.Update();
        allMicSetting.Update();
    }
}
