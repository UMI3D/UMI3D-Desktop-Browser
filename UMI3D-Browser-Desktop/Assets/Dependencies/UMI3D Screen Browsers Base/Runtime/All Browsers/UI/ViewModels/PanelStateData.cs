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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.browserRuntime.ui
{
    public class PanelStateData : IPanelStateData
    {
        public IPanelState this[int index]
        {
            get
            {
                return States[index];
            }
        }

        public NotifyingVariable<IPanelState> CurrentPanel { get; }
        public NotifyingList<IPanelState> States { get; }

        public PanelStateData()
        {
            CurrentPanel = new();
            States = new();
        }

        public bool Add<T>(T state)
            where T : IPanelState
        {
            if (States.Count == 0)
            {
                States.Add(state);
                CurrentPanel.value = state;
                return true;
            }
            else
            {
                if (States[States.Count - 1] is T)
                {
                    return false;
                }
                else
                {
                    States.Add(state);
                    CurrentPanel.value = state;
                    return true;
                }
            }
        }

        public (IPanelState oldLastState, IPanelState newLastState) Pop()
        {
            throw new System.NotImplementedException();
        }
    }
}
