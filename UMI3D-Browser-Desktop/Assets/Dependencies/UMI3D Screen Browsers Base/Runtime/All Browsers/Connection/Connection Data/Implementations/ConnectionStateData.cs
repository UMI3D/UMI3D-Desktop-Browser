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
using inetum.unityUtils;
using System;
using System.Collections.Generic;

namespace umi3d.browserRuntime.connection
{
    public class ConnectionStateData : IConnectionStateData
    {
        public event Action<IConnectionState> StateAdded;
        public event Action Cleared;

        public IConnectionState this[int index]
        {
            get
            {
                return states[index];
            }
        }

        public IEnumerator<IConnectionState> States
        {
            get
            {
                return states.GetEnumerator();
            }
        }

        List<IConnectionState> states = new();
        List<Guid> stateIds = new();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Contains(Guid id)
        {
            return stateIds.Contains(id);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool Add(IConnectionState data, Guid id)
        {
            if (states.Count == 0)
            {
                states.Add(data);
                stateIds.Add(id);
                StateAdded?.Invoke(data);
                return true;
            }
            else
            {
                if (stateIds.Contains(id))
                {
                    return false;
                }
                else
                {
                    states.Add(data);
                    stateIds.Add(id);
                    StateAdded?.Invoke(data);
                    return true;
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Clear()
        {
            states.Clear();
            stateIds.Clear();
            Cleared?.Invoke();
        }
    }
}
