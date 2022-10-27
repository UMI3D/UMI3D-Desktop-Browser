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

    public virtual string USSCustomClassEmote => "emote-window";
    public virtual string USSCustomClassEmoteIcon => $"{USSCustomClassEmote}__icon";

    public List<Emote> Emotes;
    public List<CustomButton> EmoteButtons = new List<CustomButton>();

    public override void InitElement()
    {
        base.InitElement();
        AddToClassList(USSCustomClassEmote);
    }

    public override void Set() => Set(ElementCategory.Game, "Emotes", null);

    public void OnEmoteReceived(List<Emote> emotes)
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
            icon.AddToClassList(USSCustomClassEmoteIcon);
            icon.style.backgroundImage = new StyleBackground(emote.icon);
            emoteButton.Add(icon);

            Add(emoteButton);
            if (!emote.available) emoteButton.Hide();

            emoteButton.Body.RegisterCallback<GeometryChangedEvent>(evt => emoteButton.Body.style.width = emoteButton.layout.height);

            emoteButton.clicked += () => emote.PlayEmote(emote);
        }
    }

    public void Reset()
    {
        Emotes = null;
        EmoteButtons.ForEach(emote => emote.RemoveFromHierarchy());
        EmoteButtons.Clear();
    }

    public void OnUpdateEmote(Emote emote)
    {
        var emoteButton = EmoteButtons.Find(button => (Emote)button.userData == emote);
        if (emote.available) emoteButton.Display();
        else emoteButton.Hide();
    }

    protected abstract CustomButton CreateButton();
}
