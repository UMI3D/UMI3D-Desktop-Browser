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
using UnityEngine.InputSystem;

namespace umi3d.baseBrowser.Controller
{
    public partial class BaseController
    {
        public void OnPressFirst(InputValue value)
        {
            UnityEngine.Debug.Log($"is pressed {value.isPressed}");
        }

        public void OnPressSecond(InputValue value)
        {
            UnityEngine.Debug.Log($"is pressed {value.isPressed}");
        }

        public void OnPressMic(InputValue value)
        {
            UnityEngine.Debug.Log($"Mic is pressed {value.isPressed}");
        }

        public void OnPushToTalk(InputValue value)
        {
            UnityEngine.Debug.Log($"Push to talck is pressed {value.isPressed}");
        }

        public void OnPressAudio(InputValue value)
        {
            UnityEngine.Debug.Log($"Audio is pressed {value.isPressed}");
        }

        public void OnSubmit(InputValue value)
        {
            UnityEngine.Debug.Log($"Enter is pressed {value.isPressed}");
            OnEnterKeyPressed();
        }

        public void OnPressEmote1(InputValue value)
        {
            UnityEngine.Debug.Log($"Emote1 is pressed {value.isPressed}");
        }
        public void OnPressEmote2(InputValue value)
        {
            UnityEngine.Debug.Log($"Emote2 is pressed {value.isPressed}");
        }
        public void OnPressEmote3(InputValue value)
        {
            UnityEngine.Debug.Log($"Emote3 is pressed {value.isPressed}");
        }

        public void OnMove(InputValue value)
        {
            // Read value from control. The type depends on what type of controls.
            // the action is bound to.
            var v = value.Get<Vector2>();
            UnityEngine.Debug.Log($"on move = {v}");
            // IMPORTANT: The given InputValue is only valid for the duration of the callback.
            //            Storing the InputValue references somewhere and calling Get<T>()
            //            later does not work correctly.
        }
    }
}
