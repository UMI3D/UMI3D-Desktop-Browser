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
using umi3d.cdk.collaboration;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public sealed class UserList_C : CustomUserList
    {
        public new class UxmlFactory : UxmlFactory<UserList_C, UxmlTraits> { }

        public UserList_C() => Set();

        public override void InitElement()
        {
            if (FilterTextField == null) FilterTextField = new Displayer.Textfield_C();
            if (ScrollView == null) ScrollView = new Container.ScrollView_C();

            base.InitElement();
        }

        protected override CustomUser CreateUser(UMI3DUser user)
        {
            var newUser = new Displayer.User_C();
            newUser.User = user;
            newUser.UserName = user.login;
            newUser.IsMute = !user.microphoneStatus;
            newUser.Volume = 100f;
            return newUser;
        }
    }
}
