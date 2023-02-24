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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.baseBrowser.inputs.interactions
{
    public class KeyboardEmote : BaseKeyInteraction
    {
        public static List<KeyboardEmote> S_Emotes = new List<KeyboardEmote>();
        public static event System.Action<int> EmotePressed;

        public static bool IsPressed(int index)
        {
            if (S_Emotes.Count <= index) return false;
            var _key = S_Emotes[index];
            return _key != null ? _key.m_isDown : false;
        }

        protected override void Awake()
        {
            base.Awake();
            onInputUp.AddListener(() =>
            {
                var index = S_Emotes.IndexOf(this);
                if (index == -1) return;
                EmotePressed?.Invoke(index);
            });
        }
    }
}
