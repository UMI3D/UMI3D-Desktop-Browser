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
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace umi3d.baseBrowser.inputs.interactions
{
    public enum KeyboardInteractionType
    {
        /// <summary>
        /// Action is for key that can be associated to an interaction
        /// </summary>
        Action,
        /// <summary>
        /// Shortcut is for key that cannot be associated to an interaction
        /// </summary>
        Shortcut
    }

    public class KeyboardInteraction : BaseKeyInteraction
    {
        public static List<KeyboardInteraction> S_Interactions = new List<KeyboardInteraction>();

        public KeyboardInteractionType Type = KeyboardInteractionType.Shortcut;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void KeyStarted(InputAction.CallbackContext context)
        {
            base.KeyStarted(context);

            UnityEngine.Debug.Log($"key started : {context.control.aliases}");
        }

        protected override void KeyCanceled(InputAction.CallbackContext context)
        {
            base.KeyCanceled(context);

            UnityEngine.Debug.Log($"key canceled : {context.control.displayName}");
        }

        public override bool IsAvailable()
            => base.IsAvailable() && Type != KeyboardInteractionType.Shortcut;
    }
}