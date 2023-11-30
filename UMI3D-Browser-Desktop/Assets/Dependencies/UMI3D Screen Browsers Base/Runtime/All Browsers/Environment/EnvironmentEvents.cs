/*
Copyright 2019 - 2023 Inetum

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
using System.Collections;
using umi3d.cdk.collaboration;

namespace umi3d.browserRuntime.environment
{
    public class EnvironmentEvents : IEnvironmentEvents
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action UserJoinedEnvironment;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action UserLeftEnvironment;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action UserUnmute;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action UserMute;

        IUsersCount usersCount;

        public EnvironmentEvents(IUsersCount usersCount) 
        {
            this.usersCount = usersCount;
        }

        public void OnUserJoinedEnvironment()
        {
            UserJoinedEnvironment?.Invoke();
        }

        public void OnUserLeftEnvironment() 
        {
            UserLeftEnvironment?.Invoke();
        }

        public IEnumerator SetManagerEvents()
        {
            bool allSet = false;

            do
            {
                if (UMI3DCollaborationEnvironmentLoader.Exists)
                {
                    UMI3DCollaborationEnvironmentLoader.Instance.OnUpdateJoinnedUserList += () =>
                    {
                        if (usersCount.Count < UMI3DCollaborationEnvironmentLoader.Instance.JoinnedUserList.Count)
                        {
                            usersCount.Count = UMI3DCollaborationEnvironmentLoader.Instance.JoinnedUserList.Count;
                            OnUserJoinedEnvironment();
                        }
                        else if (usersCount.Count > UMI3DCollaborationEnvironmentLoader.Instance.JoinnedUserList.Count)
                        {
                            usersCount.Count = UMI3DCollaborationEnvironmentLoader.Instance.JoinnedUserList.Count;
                            OnUserLeftEnvironment();
                        }
                    };
                    allSet = true;
                }
                else
                {
                    yield return null;
                }
            } while (!allSet);
        }
    }
}
