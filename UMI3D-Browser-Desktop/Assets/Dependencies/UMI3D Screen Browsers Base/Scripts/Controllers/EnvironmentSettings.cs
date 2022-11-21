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
using System;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.cdk.userCapture;
using UnityEngine;

public interface ISetting
{
    public bool IsOn { get; }
    event Action<bool> StatusChanged;
    public void Start();
    public void Update();
    public void Toggle();
}

public class AudioSetting : ISetting
{
    public bool IsOn { get; private set; }
    public event Action<bool> StatusChanged;

    /// <summary>
    /// Value is between 0 and 1.
    /// </summary>
    public float GeneralVolume
    {
        get => AudioListener.volume;
        set
        {
            SetGeneralVolumeWithoutNotify(value);
            StatusChanged?.Invoke(IsOn);
        }
    }
    private float m_generalVolume;

    public AudioSetting()
    {
        IsOn = true;
        Start();
    }

    public void Start() => StatusChanged?.Invoke(IsOn);

    public void Toggle()
    {
        SetGeneralVolumeWithoutNotify(IsOn ? 0f : m_generalVolume);
        StatusChanged?.Invoke(IsOn);
    }

    public void Update()
    {
        //if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ToggleAudio)) &&
        //    !TextInputDisplayerElement.isTyping)
        //{
        //    Toggle();
        //}
    }

    public void SetGeneralVolumeWithoutNotify(float value)
    {
        if (value != 0f) m_generalVolume = value;
        AudioListener.volume = value;
        IsOn = value != 0f;
    }
}

public class AvatarSetting : ISetting
{
    public bool IsOn
    {
        get => (userTracking != null) ? userTracking.SendTracking : false;
        private set
        {
            userTracking?.SetTrackingSending(value);
            UMI3DCollaborationEnvironmentLoader.Instance.GetClientUser()?.SetAvatarStatus(value);
        }
    }
    public event Action<bool> StatusChanged;

    private UMI3DClientUserTracking userTracking => UMI3DClientUserTracking.Instance;

    public AvatarSetting()
    {
        IsOn = true;
        Start();
    }

    public void Start()
    {
        UMI3DUser.OnUserAvatarStatusUpdated.AddListener((u) =>
        {
            if (UMI3DCollaborationClientServer.Exists && u.id == UMI3DCollaborationClientServer.Instance.GetUserId())
            {
                IsOn = u.avatarStatus;
                StatusChanged?.Invoke(IsOn);
            }
        });
        StatusChanged?.Invoke(IsOn);
    }

    public void Toggle()
    {
        IsOn = !IsOn;
        StatusChanged?.Invoke(IsOn);
    }

    public void Update()
    {
        //if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ToggleAvatar)) &&
        //    !TextInputDisplayerElement.isTyping)
        //{
        //    Toggle();
        //}
    }
}

public class MicSetting : ISetting
{
    public bool IsOn
    {
        get => !MicrophoneListener.mute;
        private set => MicrophoneListener.mute = !value;
    }
    public event Action<bool> StatusChanged;

    public MicSetting()
    {
        IsOn = false;
        Start();
    }

    public void Start()
    {
        UMI3DUser.OnUserMicrophoneStatusUpdated.AddListener((u) =>
        {
            if (UMI3DCollaborationClientServer.Exists && u.id == UMI3DCollaborationClientServer.Instance.GetUserId())
            {
                IsOn = u.microphoneStatus;
                StatusChanged?.Invoke(IsOn);
            }
        });
        StatusChanged?.Invoke(IsOn);
    }

    public void Toggle()
    {
        IsOn = !IsOn;
        StatusChanged?.Invoke(IsOn);
    }

    public void Update()
    {
        //if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ToggleMicrophone)) &&
        //    !TextInputDisplayerElement.isTyping)
        //{
        //    Toggle();
        //}
    }
}

public class AllMicSetting : ISetting
{
    public bool IsOn
    {
        get => false;
        private set
        {
            UMI3DUser.MuteAllMicrophone();
        }
    }
    public event Action<bool> StatusChanged;

    public AllMicSetting()
    {
        IsOn = false;
        Start();
    }

    public void Start() => StatusChanged?.Invoke(IsOn);

    public void Toggle()
    {
        IsOn = !IsOn;
        StatusChanged?.Invoke(IsOn);
    }

    public void Update()
    {
        //if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.MuteAllMicrophone)) &&
        //    !TextInputDisplayerElement.isTyping)
        //{
        //    Toggle();
        //}
    }
}


//public class UserListSetting
//{
//    public Action<bool> m_statusChanged { get; private set; }

//    public class User : User_item_E
//    {
//        public UMI3DUser user { get; }

//        public User(UMI3DUser user)
//        {
//            this.user = user;
//        }

//        public void ToggleMic()
//        {
//            user.SetMicrophoneStatus(!user.microphoneStatus);
//        }

//        public void ToggleAvatar()
//        {
//            user.SetAvatarStatus(!user.avatarStatus);
//        }

//        public void setValue()
//        {
//            Avatar?.Toggle(user.avatarStatus);
//            Mic?.Toggle(user.microphoneStatus);
//            Sound?.Toggle(true);
//        }


//        public override void Bind(VisualElement element)
//        {
//            base.Bind(element);
//            setValue();
//            Mic.Clicked += ToggleMic;
//            element.Q<Label>("userLabel").text = user.login;
//        }

//        public override void Unbind(VisualElement element)
//        {
//            Mic.Clicked -= ToggleMic;
//            base.Unbind(element);
//        }
//    }

//    User[] Users;

//    public UserListSetting()
//    {
//        UMI3DCollaborationEnvironmentLoader.OnUpdateJoinnedUserList += RefreshList;

//        UMI3DUser.OnUserMicrophoneStatusUpdated.AddListener(UpdateUser);
//        UMI3DUser.OnUserAvatarStatusUpdated.AddListener(UpdateUser);
//        UMI3DUser.OnUserAttentionStatusUpdated.AddListener(UpdateUser);

//        UMI3DUser.OnRemoveUser.AddListener((u) => { Users.FirstOrDefault(U => (U.user == u))?.Unbind(null); });

//        RefreshList();
//    }

//    void UpdateUser(UMI3DUser user)
//    {
//        var _u = Users.FirstOrDefault(U => (U.user == user));
//        if (_u == null)
//            RefreshList();
//        else
//            _u.setValue();
//    }

//    void RefreshList()
//    {
//        Settingbox_E.Instance.UserList.Clear();
//        InitUsers();
//        Settingbox_E.Instance.UserList.AddRange(Users);
//    }


//    void InitUsers()
//    {
//        Users = UMI3DCollaborationEnvironmentLoader.Instance.JoinnedUserList.Where(u => !u.isClient).Select(u => new User(u)).ToArray();
//    }
//}

public sealed class EnvironmentSettings : inetum.unityUtils.SingleBehaviour<EnvironmentSettings>
{
    
    public AudioSetting AudioSetting;
    public AvatarSetting AvatarSetting;
    public MicSetting MicSetting;
    public AllMicSetting AllMicSetting;

    //private UserListSetting userListSetting;

    private bool m_environmentLoaded { get; set; } = false;
    private bool initialized = false;

    protected override void Awake()
    {
        base.Awake();

        AvatarSetting = new AvatarSetting();
        AudioSetting = new AudioSetting();
        MicSetting = new MicSetting();
        AllMicSetting = new AllMicSetting();

        //userListSetting = new UserListSetting();
    }

    void Start()
    {
        UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => {
            m_environmentLoaded = true;
        });
        initialized = true;
    }

    void Update()
    {
        if (!m_environmentLoaded || !initialized) return;
        AvatarSetting.Update();
        AudioSetting.Update();
        MicSetting.Update();
        AllMicSetting.Update();
    }
}
