/*
Copyright 2019 Gfi Informatique

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

using UnityEngine;

namespace BrowserDesktop.Controller
{
    [CreateAssetMenu(fileName = "keyBindings", menuName = "ScriptableObjects/KeyBindings")]
    public class KeyBindings_SO : ScriptableObject
    {
        [System.Serializable]
        public class KeyBinding
        {
            [SerializeField]
            [ContextMenuItem("GetKeyStringFromCode", "GetKeyStringFromCode")]
            private string key_string;
            public string Key_string
            {
                get => this.key_string;
                private set => this.key_string = value;
            }

            [SerializeField]
            [ContextMenuItem("GetKeyCodeFromString", "GetKeyCodeFromString")]
            private KeyCode key_code;
            public KeyCode Key_code
            {
                get => this.key_code;
                private set => this.key_code = value;
            }

            [SerializeField]
            [ContextMenuItem("SetKeyCodeAndStringFromSprite", "SetKeyCodeAndStringFromSprite")]
            private Sprite icon;
            public Sprite Icon
            {
                get => this.icon;
                private set => this.icon = value;
            }

            public KeyBinding(string key, KeyCode keyCode, Sprite icon)
            {
                this.key_string = key;
                this.key_code = keyCode;
                this.icon = icon;
            }

            public void GetKeyCodeFromString()
            {
                if (!string.IsNullOrEmpty(Key_string))
                {
                    Debug.Log("key code test = " + key_string);
                    System.Enum.TryParse<KeyCode>(Key_string.Trim().Replace(" ", string.Empty), true, out key_code);
                }
            }

            public void GetKeyStringFromCode()
            {
                if (Key_code != KeyCode.None)
                {
                    Debug.Log("key string test = " + Key_code.ToString());
                    key_string = Key_code.ToString();
                }
            }

            public void SetKeyCodeAndStringFromSprite()
            {
                if (Icon != null)
                {
                    string iconName = Icon.name.Replace("key_", "");
                    Debug.Log("Icon name test = " + iconName);
                    bool hasParse = System.Enum.TryParse<KeyCode>(iconName.Trim().Replace(" ", string.Empty), true, out key_code);
                    if (hasParse)
                    {
                        key_string = Key_code.ToString();
                    }
                }
            }
        }

        [SerializeField]
        private KeyBinding[] keyBindings;
        public KeyBinding[] KeyBindings => this.keyBindings;

        public Sprite GetSpriteFrom(string key)
        {
            foreach (KeyBinding keyBinding in keyBindings)
            {
                if (keyBinding.Key_string == key)
                    return keyBinding.Icon;
            }

            Debug.LogWarning("Icon key not found: this shouln't happen");
            return null;
        }

        public Sprite GetSpriteFrom(KeyCode key)
        {
            foreach (KeyBinding keyBinding in keyBindings)
            {
                if (keyBinding.Key_code == key)
                    return keyBinding.Icon;
            }

            Debug.LogWarning("Icon key not found: this shouln't happen");
            return null;
        }

        private void GetKeyCodeFromString()
        {
            foreach (KeyBinding keyBinding in keyBindings)
            {
                keyBinding.GetKeyCodeFromString();
            }
        }

        private void GetKeyStringFromCode()
        {
            foreach (KeyBinding keyBinding in keyBindings)
            {
                keyBinding.GetKeyStringFromCode();
            }
        }

        private void SetKeyCodeAndStringFromSprite()
        {
            foreach (KeyBinding keyBinding in keyBindings)
            {
                keyBinding.SetKeyCodeAndStringFromSprite();
            }
        }
    }
}

