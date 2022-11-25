/*
Copyright 2019 - 2022 Inetum

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
using UnityEngine.UIElements;
using static umi3d.baseBrowser.emotes.EmoteManager;

public abstract class CustomEmoteWindow : CustomForm
{
    public new class UxmlTraits : CustomForm.UxmlTraits
    {
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as CustomEmoteWindow;

            custom.Set
                (
                    ElementCategory.Game,
                    "Emotes",
                    null
                );
        }
    }

    public virtual string USSCustomClassEmote => "emote__window";
    public virtual string USSCustomClassEmoteIcon => $"{USSCustomClassEmote}-icon";

    public static List<Emote> Emotes;
    public static List<CustomButton> EmoteButtons = new List<CustomButton>();

    public override void InitElement()
    {
        base.InitElement();
        AddToClassList(USSCustomClassEmote);

        WillUpdateFilter += UpdateFilter;
    }

    public override void Set() => Set(ElementCategory.Game, "Emotes", null);

    ~CustomEmoteWindow()
    {
        WillUpdateFilter = null;
    }

    protected static System.Func<CustomButton> CreateButton;

    #region Implementation

    public static event System.Action WillUpdateFilter;

    public static void OnEmoteReceived(List<Emote> emotes)
    {
        Reset();
        Emotes = emotes;
        foreach (var emote in Emotes)
        {
            var emoteButton = CreateButton();
            EmoteButtons.Add(emoteButton);
            emoteButton.userData = emote;

            emoteButton.name = emote.Label.ToLower();
            emoteButton.Label = string.IsNullOrEmpty(emote.Label) ? " " : emote.Label;

            emoteButton.LabelDirection = ElemnetDirection.Trailing;
            emoteButton.Type = ButtonType.Invisible;

            var icon = new VisualElement { name = "icon" };
            icon.style.backgroundImage = new StyleBackground(emote.icon);
            emoteButton.Add(icon);

            WillUpdateFilter?.Invoke();

            emoteButton.Body.RegisterCallback<GeometryChangedEvent>(evt => emoteButton.Body.style.width = emoteButton.layout.height);

            emoteButton.clicked += () => emote.PlayEmote(emote);
        }
    }

    public static void Reset()
    {
        Emotes = null;
        EmoteButtons.ForEach(emote => emote.RemoveFromHierarchy());
        EmoteButtons.Clear();
    }

    public static void OnUpdateEmote(Emote emote)
    {
        var emoteButton = EmoteButtons.Find(button => (Emote)button.userData == emote);
        if (emote.available) emoteButton.Display();
        else emoteButton.Hide();
    }

    public virtual void UpdateFilter()
    {
        if (this.FindRoot() == null) return;
        foreach (var emoteButton in EmoteButtons)
        {
            var emote = (Emote)emoteButton.userData;
            Add(emoteButton);
            if (!emote.available) emoteButton.Hide();
            emoteButton.Q("icon").AddToClassList(USSCustomClassEmoteIcon);
        }
    }

    #endregion
}
