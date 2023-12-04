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
using System.Collections.Generic;

namespace umi3d.browserRuntime.connection
{
    public class ConnectionStateData : IConnectionStateData
    {
        public event Action StateAdded;
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

        public bool Add<T>(T data) where T : IConnectionState
        {
            if (states.Count == 0)
            {
                states.Add(data);
                return true;
            }
            else
            {
                if (states[states.Count - 1] is T)
                {
                    UnityEngine.Debug.Log($"message");
                    return false;
                }
                else
                {
                    UnityEngine.Debug.Log($"pomme");
                    states.Add(data);
                    return true;
                }
            }
        }

        public void Clear()
        {
            states.Clear();
        }

        public bool ContainsStateByType<T>() where T : IConnectionState
        {
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i] is T)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
