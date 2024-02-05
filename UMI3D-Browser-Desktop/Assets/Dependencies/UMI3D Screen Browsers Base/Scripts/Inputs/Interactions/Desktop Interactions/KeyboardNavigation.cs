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

namespace umi3d.baseBrowser.inputs.interactions
{
    public enum NavigationEnum
    {
        Forward,
        Backward,
        Left,
        Right,
        sprint,
        Jump,
        Crouch,
        FreeView
    }

    public class KeyboardNavigation : BaseKeyInteraction
    {
        public static List<KeyboardNavigation> S_Navigations = new List<KeyboardNavigation>();

        public NavigationEnum Navigation;

        protected override void Start()
        {
            Key.started += KeyStarted;
            Key.canceled += KeyCanceled;
            Key.Enable();
        }

        public static bool IsPressed(NavigationEnum navigation)
        {
            var _key = S_Navigations.Find(key => key.Navigation == navigation);
            return _key != null ? _key.m_isDown : false;
        }

        protected override void KeyCanceled(InputAction.CallbackContext context)
        {
            if (!CanProces() && !m_isDown) return;

            Pressed(false);
        }
    }
}
