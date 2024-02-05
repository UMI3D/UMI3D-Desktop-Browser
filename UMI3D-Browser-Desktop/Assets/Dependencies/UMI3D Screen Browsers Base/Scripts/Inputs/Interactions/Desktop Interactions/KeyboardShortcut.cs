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
using UnityEngine.Events;

namespace umi3d.baseBrowser.inputs.interactions
{
    public enum ShortcutEnum
    {
        MuteUnmuteMic, PushToTalk,
        MuteUnmuteGeneralVolume, DecreaseVolume, IncreaseVolume,
        Cancel, Submit,
        DisplayHideGameMenu, 
        DisplayHideContextualMenu, 
        DisplayHideNotifications, 
        DisplayHideUsersList, 
        DisplayHideEmoteWindow,
        FreeCursor,
        SwitchNextManipulation
    }

    public class KeyboardShortcut : BaseKeyInteraction
    {
        public static List<KeyboardShortcut> S_Shortcuts = new List<KeyboardShortcut>();

        public ShortcutEnum Shortcut;

        public override bool CanProces() => !IsEditingTextField;

        public static bool IsPressed(ShortcutEnum shortcut)
        {
            var _key = S_Shortcuts.Find(key => key.Shortcut == shortcut);

            if (_key == null)
            {
                UnityEngine.Debug.LogError($"No key shortcut found for {shortcut.ToString()}");
                return false;
            }
            else if (!_key.CanProces()) return false;

            return _key.m_isDown;
        }

        public static bool WasPressedThisFrame(ShortcutEnum shortcut)
        {
            var _key = S_Shortcuts.Find(key => key.Shortcut == shortcut);

            if (_key == null)
            {
                UnityEngine.Debug.LogError($"No key shortcut found for {shortcut.ToString()}");
                return false;
            }
            else if (!_key.CanProces()) return false;

            return _key.Key.WasPressedThisFrame();
        }

        public static void AddDownListener(ShortcutEnum shortcut, UnityAction action)
        {
            var _key = S_Shortcuts.Find(key => key.Shortcut == shortcut);
            if (_key == null)
            {
                UnityEngine.Debug.LogError($"No key shortcut found for {shortcut.ToString()}");
                return;
            }

            _key.onInputDown.AddListener(action);
        }

        public static void RemoveDownListener(ShortcutEnum shortcut, UnityAction action)
        {
            var _key = S_Shortcuts.Find(key => key.Shortcut == shortcut);
            if (_key == null)
            {
                UnityEngine.Debug.LogError($"No key shortcut found for {shortcut.ToString()}");
                return;
            }

            _key.onInputDown.RemoveListener(action);
        }

        public static void AddUpListener(ShortcutEnum shortcut, UnityAction action)
        {
            var _key = S_Shortcuts.Find(key => key.Shortcut == shortcut);
            if (_key == null)
            {
                UnityEngine.Debug.LogError($"No key shortcut found for {shortcut.ToString()}");
                return;
            }

            _key.onInputUp.AddListener(action);
        }

        public static void RemoveUpListener(ShortcutEnum shortcut, UnityAction action)
        {
            var _key = S_Shortcuts.Find(key => key.Shortcut == shortcut);
            if (_key == null)
            {
                UnityEngine.Debug.LogError($"No key shortcut found for {shortcut.ToString()}");
                return;
            }

            _key.onInputUp.RemoveListener(action);
        }
    }
}
