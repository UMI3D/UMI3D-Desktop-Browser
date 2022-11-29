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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class CustomUserList : VisualElement, ICustomElement
{
    public virtual string StyleSheetGamePath => $"USS/game";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/userList";
    public virtual string USSCustomClassName => "user__list";
    public virtual string USSCustomClassFilterLabel => $"{USSCustomClassName}-filter__label";

    public CustomTextfield FilterTextField;
    public CustomText FilterLabel;
    public CustomScrollView ScrollView;

    protected bool m_hasBeenInitialized;
    protected bool m_isSet = false;
    protected CustomUser[] m_users;
    protected List<CustomUser> m_filteredUser = new List<CustomUser>();

    /// <summary>
    /// Stores all audio settings set for each users (memory for a same environment).
    /// </summary>
    protected Dictionary<ulong, CustomUser> intraUserAudioSettingsMemory = new Dictionary<ulong, CustomUser>();

    /// <summary>
    /// Stores all audio settings set for each users (memory between environments).
    /// </summary>
    protected Dictionary<string, CustomUser> interUserAudioSettingsMemory = new Dictionary<string, CustomUser>();

    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetGamePath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }

        AddToClassList(USSCustomClassName);
        FilterLabel.AddToClassList(USSCustomClassFilterLabel);
        UMI3DEnvironmentClient.EnvironementJoinned.AddListener(OnEnvironmentChanged);
        UMI3DUser.OnUserMicrophoneStatusUpdated.AddListener(UpdateUser);
        UMI3DUser.OnUserAvatarStatusUpdated.AddListener(UpdateUser);
        UMI3DUser.OnUserAttentionStatusUpdated.AddListener(UpdateUser);
        UMI3DUser.OnRemoveUser.AddListener((u) => 
        {
            m_users.FirstOrDefault(U => (U.User == u))?.RemoveFromHierarchy();
            Filter();
        });

        FilterTextField.Category = ElementCategory.Game;
        FilterTextField.RegisterValueChangedCallback(ce => Filter());

        FilterLabel.text = "Search user name:";

        Add(FilterTextField);
        FilterTextField.Add(FilterLabel);
        Add(ScrollView);
    }

    public virtual void Set()
    {
        m_isSet = false;

        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        m_isSet = true;
    }

    public override VisualElement contentContainer => m_isSet ? ScrollView.contentContainer : this;

    protected abstract CustomUser CreateUser(UMI3DUser user);

    #region Implementation

    private void OnEnvironmentChanged()
    {
        interUserAudioSettingsMemory = new Dictionary<string, CustomUser>();

        foreach(var u in intraUserAudioSettingsMemory.Values)
        {
            if (!string.IsNullOrEmpty(u.User.login))
                interUserAudioSettingsMemory[u.User.login] = u;
        }

        intraUserAudioSettingsMemory.Clear();
    }

    public virtual void RefreshList()
    {
        if (m_users != null)
        {
            foreach (var u in m_users)
            {
                intraUserAudioSettingsMemory[u.User.id] = u;
            }
        }

        var usersList = UMI3DCollaborationEnvironmentLoader.Instance.JoinnedUserList
            .Where(u => !u.isClient)
            .Select(u => CreateUser(u))
            .ToList();

        usersList.Sort((user0, user1) => string.Compare(user0.UserName, user1.UserName));
            
        m_users = usersList.ToArray();

        Filter();
    }

    public virtual void UpdateUser(UMI3DUser user)
    {
        var _u = m_users.FirstOrDefault(U => (U.User == user));
        if (_u == null) RefreshList();
        //else _u.IsMute = !user.microphoneStatus;
    }

    protected virtual void Filter()
    {
        ScrollView.Clear();
        m_filteredUser.Clear();

        if (string.IsNullOrEmpty(FilterTextField.value)) FilterTextField.Add(FilterLabel);
        else FilterLabel.RemoveFromHierarchy();

        if (m_users == null) return;

        foreach (var user in m_users) if (user.UserName.Contains(FilterTextField.value)) m_filteredUser.Add(user);

        foreach (var user in m_filteredUser) ScrollView.Add(user);
    }

    #endregion
}
