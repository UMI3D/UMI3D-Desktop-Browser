using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IconsDictionary", menuName = "ScriptableObjects/IconsDictionary")]
public class Icons_SO : ScriptableObject
{
    [System.Serializable]
    public struct Icon
    {
        [SerializeField]
        private string key;
        public string Key
        {
            get => this.key;
            private set => this.key = value;
        }

        [SerializeField]
        private Sprite sprite;
        public Sprite Sprite
        {
            get => this.sprite;
            private set => this.sprite = value;
        }

        public readonly string SpriteName => sprite.name;

        public Icon(string key, Sprite sprite)
        {
            this.key = key;
            this.sprite = sprite;
        }
    }

    [SerializeField]
    private Icon[] icons;
    public Icon[] Icons => this.icons;

    public Sprite GetSpriteFrom(string key)
    {
        foreach (Icon icon in icons)
        {
            if (icon.Key == key)
                return icon.Sprite;
        }

        Debug.LogWarning("Icon key not found: this shouln't happen");
        return null;
    }
}
