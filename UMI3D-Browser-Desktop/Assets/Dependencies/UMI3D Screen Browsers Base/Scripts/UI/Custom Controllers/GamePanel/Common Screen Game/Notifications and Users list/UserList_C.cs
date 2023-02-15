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
using umi3d.cdk.collaboration;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public class UserList_C : Visual_C
    {
        public new class UxmlFactory : UxmlFactory<UserList_C, UxmlTraits> { }

        public override string StyleSheetPath_MainTheme => $"USS/game";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetGamesFolderPath}/userList";

        public override string UssCustomClass_Emc => "user__list";
        public virtual string USSCustomClassFilterLabel => $"{UssCustomClass_Emc}-filter__label";

        public Textfield_C FilterTextField = new Textfield_C { name = "filter" };
        public Text_C FilterLabel = new Text_C { name = "filter" };
        public ScrollView_C ScrollView = new ScrollView_C { name = "scroll-view" };

        protected User_C[] m_users;
        protected List<User_C> m_filteredUser = new List<User_C>();

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            FilterLabel.AddToClassList(USSCustomClassFilterLabel);
        }

        protected override void InitElement()
        {
            base.InitElement();
            FilterTextField.Category = ElementCategory.Game;
            FilterTextField.RegisterValueChangedCallback(ce => Filter());

            FilterLabel.LocaliseText = new LocalisationAttribute("Search by user name:", "NotificationCenter", "UserList_FilterLabel");

            Add(FilterTextField);
            FilterTextField.Add(FilterLabel);
            Add(ScrollView);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override VisualElement contentContainer => IsSet ? ScrollView.contentContainer : this;

        #region Implementation

        /// <summary>
        /// Stores all audio settings set for each users (memory for a same environment).
        /// </summary>
        protected Dictionary<ulong, User_C> intraUserAudioSettingsMemory = new Dictionary<ulong, User_C>();

        /// <summary>
        /// Stores all audio settings set for each users (memory between environments).
        /// </summary>
        protected Dictionary<string, User_C> interUserAudioSettingsMemory = new Dictionary<string, User_C>();

        public virtual void OnEnvironmentChanged()
        {
            interUserAudioSettingsMemory = new Dictionary<string, User_C>();

            foreach (var u in intraUserAudioSettingsMemory.Values)
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

        public virtual void RemoveUser(UMI3DUser u)
        {
            m_users.FirstOrDefault
            (U =>
            {
                if (U == null) return false;
                return U.User == u;
            })?.RemoveFromHierarchy();
            Filter();
        }

        protected virtual void Filter()
        {
            ScrollView.Clear();
            m_filteredUser.Clear();

            if (string.IsNullOrEmpty(FilterTextField.value)) FilterTextField.Add(FilterLabel);
            else FilterLabel.RemoveFromHierarchy();

            if (m_users == null) return;

            foreach (var user in m_users) if (user.UserName.Value.Contains(FilterTextField.value)) m_filteredUser.Add(user);

            foreach (var user in m_filteredUser) ScrollView.Add(user);
        }

        protected virtual User_C CreateUser(UMI3DUser user)
        {
            var newUser = new User_C();
            newUser.User = user;
            newUser.UserName = user.login;

            User_C previous = intraUserAudioSettingsMemory.ContainsKey(user.id) ? intraUserAudioSettingsMemory[user.id] : null;

            if (previous == null)
                previous = interUserAudioSettingsMemory.ContainsKey(user.login) ? interUserAudioSettingsMemory[user.login] : null;

            if (previous != null)
            {
                newUser.Volume = previous.Volume;
                newUser.IsMute = previous.IsMute;
            }
            else
            {
                newUser.Volume = 100f;
                newUser.IsMute = false;
            }

            return newUser;
        }

        #endregion


    }
}
