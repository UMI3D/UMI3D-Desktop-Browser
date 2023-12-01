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
using System.Collections.Generic;
using umi3d.baseBrowser.connection;
using umi3d.cdk.collaboration;
using umi3d.common;
using umi3d.common.interaction;

namespace umi3d.browserRuntime.connection
{
    public class ConnectionEvents : IConnectionEvents
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action<IConnectionStateParam> ConnectionStateChanged;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action<IConnectionStartedParam> ConnectionOrRedirectionStarted;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action<IMediaDtoFoundParam> MediaDtoFound;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action<IConnectionSucceededParam> ConnectionOrRedirectionSucceeded;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action<IConnectionFailedParam> ConnectionOrRedirectionFailed;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action EnvironmentLoaded;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action ConnectionLost;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action UserAsksToLeave;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action<string> BrowserForceDisconnection;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action<List<string>, Action<bool>> ServerAsksToLoadLib;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action<ConnectionFormDto, Action<FormAnswerDto>> ServerAsksForConnectionForm;

        BaseClientIdentifier identifier;

        public ConnectionEvents(BaseClientIdentifier identifier)
        {
            this.identifier = identifier;
        }

        public void OnConnectionStateChanged(IConnectionStateParam param)
        {
            ConnectionStateChanged?.Invoke(param);
        }

        public void OnConnectionOrRedirectionStarted(IConnectionStartedParam param)
        {
            ConnectionOrRedirectionStarted?.Invoke(param);
        }

        public void OnMediaDtoFound(IMediaDtoFoundParam param)
        {
            MediaDtoFound?.Invoke(param);
        }

        public void OnConnectionOrRedirectionSucceeded(IConnectionSucceededParam param)
        {
            ConnectionOrRedirectionSucceeded?.Invoke(param);
        }

        public void OnConnectionOrRedirectionFailed(IConnectionFailedParam param)
        {
            ConnectionOrRedirectionFailed?.Invoke(param);
        }

        public void OnEnvironmentLoaded()
        {
            EnvironmentLoaded?.Invoke();
        }

        public void OnConnectionLost()
        {
            ConnectionLost?.Invoke();
        }

        public void OnUserAsksToLeave()
        {
            UserAsksToLeave?.Invoke();
        }

        public void OnBrowserForceDisconnection(string reason)
        {
            BrowserForceDisconnection?.Invoke(reason);
        }

        public void OnServerAsksToLoadLib(List<string> libs, Action<bool> callback)
        {
            ServerAsksToLoadLib?.Invoke(libs, callback);
        }

        public void OnServerAsksForConnectionForm(ConnectionFormDto connectionForm, Action<FormAnswerDto> answer)
        {
            ServerAsksForConnectionForm?.Invoke(connectionForm, answer);
        }

        public IEnumerator SetManagerEvent()
        {
            UMI3DEnvironmentClient.ConnectionState.AddListener(stateDescription =>
            {
                OnConnectionStateChanged(new ConnectionStateParam(null, stateDescription));
            });
            UMI3DEnvironmentClient.EnvironementLoaded.AddListener(OnEnvironmentLoaded);

            identifier.ShouldDownloadLib = (libs, callback) =>
            {
                if (libs == null || libs.Count == 0)
                {
                    callback.Invoke(true);
                }
                else
                {
                    OnServerAsksToLoadLib(libs, callback);
                }
            };
            identifier.GetParameters = OnServerAsksForConnectionForm;

            bool allSet = false;
            do
            {
                if (UMI3DCollaborationClientServer.Exists)
                {
                    UMI3DCollaborationClientServer.Instance.OnRedirectionStarted.AddListener(() =>
                    {
                        OnConnectionOrRedirectionStarted(new ConnectionStartedParam());
                    });
                    UMI3DCollaborationClientServer.Instance.OnRedirectionAborted.AddListener(() =>
                    {
                        OnConnectionOrRedirectionFailed(new ConnectionFailedParam(null, "Aborted"));
                    });
                    UMI3DCollaborationClientServer.Instance.OnConnectionLost.AddListener(OnConnectionLost);
                    UMI3DCollaborationClientServer.Instance.OnForceLogoutMessage.AddListener(OnBrowserForceDisconnection);
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
