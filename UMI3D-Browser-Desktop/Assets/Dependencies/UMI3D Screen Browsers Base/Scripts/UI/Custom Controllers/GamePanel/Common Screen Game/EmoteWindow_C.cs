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

using static umi3d.baseBrowser.emotes.EmoteManager;
using System.Collections.Generic;
using UnityEngine.UIElements;
using umi3d.commonScreen.Displayer;
using umi3d.commonScreen.Container;
using umi3d.baseBrowser.emotes;

namespace umi3d.commonScreen.game
{
    public class EmoteWindow_C : Form_C
    {
        public static EmoteWindow_C Instance
        {
            get
            {
                if (s_instance != null) return s_instance;

                s_instance = new EmoteWindow_C();
                return s_instance;
            }
            set
            {
                s_instance = value;
            }
        }
        protected static EmoteWindow_C s_instance;

        public virtual string USSCustomClassEmote => "emote__window";
        public virtual string USSCustomClassEmoteIcon => $"{USSCustomClassEmote}-icon";

        public static List<Emote> Emotes;
        public static List<Button_C> EmoteButtons = new List<Button_C>();

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            AddToClassList(USSCustomClassEmote);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Title = "Emotes";
            Category = ElementCategory.Game;
        }

        protected override void AttachedToPanel(AttachToPanelEvent evt)
        {
            base.AttachedToPanel(evt);
            WillUpdateFilter += UpdateFilter;
        }

        protected override void DetachedFromPanel(DetachFromPanelEvent evt)
        {
            base.DetachedFromPanel(evt);
            WillUpdateFilter -= UpdateFilter;
        }

        #region Implementation

        public static event System.Action WillUpdateFilter;

        public static void OnEmoteConfigReceived(List<Emote> emotes)
        {
            Reset();
            Emotes = emotes;
            foreach (var emote in Emotes)
            {
                var emoteButton = new Button_C();
                EmoteButtons.Add(emoteButton);
                emoteButton.userData = emote;

                emoteButton.name = emote.Label.ToLower();
                emoteButton.LocalisedLabel = string.IsNullOrEmpty(emote.Label) ? " " : emote.Label;

                emoteButton.LabelAndInputDirection = ElementAlignment.Trailing;
                emoteButton.Type = ButtonType.Invisible;

                var icon = new VisualElement { name = "icon" };
                icon.style.backgroundImage = new StyleBackground(emote.icon);
                emoteButton.Add(icon);

                WillUpdateFilter?.Invoke();

                emoteButton.Body.RegisterCallback<GeometryChangedEvent>(evt => emoteButton.Body.style.width = emoteButton.layout.height);

                emoteButton.clicked += () => EmoteManager.Instance.PlayEmote(emote);
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
}
