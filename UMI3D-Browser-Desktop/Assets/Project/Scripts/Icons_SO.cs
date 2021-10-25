using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IconsDictionary", menuName = "ScriptableObjects/IconsDictionary")]
public class Icons_SO : ScriptableObject
{
    [System.Serializable]
    public class Icon
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
        private Sprite sprite;
        public Sprite Sprite
        {
            get => this.sprite;
            private set => this.sprite = value;
        }

        public Icon(string key, KeyCode keyCode, Sprite sprite)
        {
            this.key_string = key;
            this.key_code = keyCode;
            this.sprite = sprite;
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
    }

    [SerializeField]
    private Icon[] icons;
    public Icon[] Icons => this.icons;

    public Sprite GetSpriteFrom(string key)
    {
        foreach (Icon icon in icons)
        {
            if (icon.Key_string == key)
                return icon.Sprite;
        }

        Debug.LogWarning("Icon key not found: this shouln't happen");
        return null;
    }

    public Sprite GetSpriteFrom(KeyCode key)
    {
        foreach (Icon icon in icons)
        {
            if (icon.Key_code == key)
                return icon.Sprite;
        }

        Debug.LogWarning("Icon key not found: this shouln't happen");
        return null;
    }

    private void GetKeyCodeFromString()
    {
        foreach (Icon icon in icons)
        {
            icon.GetKeyCodeFromString();
        }
    }

    private void GetKeyStringFromCode()
    {
        foreach (Icon icon in icons)
        {
            icon.GetKeyStringFromCode();
        }
    }
}
