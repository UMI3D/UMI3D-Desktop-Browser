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
using umi3d.commonScreen.Displayer;
using umi3d.commonScreen.game;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonDesktop.game
{
    public class BottomArea_C : VisualElement
    {
        public enum BottomBarButton
        {
            None,
            Avatar,
            Emote,
            Mic,
            Sound,
        }

        public new class UxmlFactory : UxmlFactory<BottomArea_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlBoolAttributeDescription m_isAvatarOn = new UxmlBoolAttributeDescription
            {
                name = "is-avatar-on",
                defaultValue = true,
            };

            UxmlBoolAttributeDescription m_isMicOn = new UxmlBoolAttributeDescription
            {
                name = "is-mic-on",
                defaultValue = false,
            };

            UxmlBoolAttributeDescription m_isSoundOn = new UxmlBoolAttributeDescription
            {
                name = "is-sound-on",
                defaultValue = true,
            };

            UxmlEnumAttributeDescription<BottomBarButton> m_buttonSelected = new UxmlEnumAttributeDescription<BottomBarButton>
            {
                name = "button-selected",
                defaultValue = BottomBarButton.None,
            };

            UxmlBoolAttributeDescription m_displayNotifAndUsersArea = new UxmlBoolAttributeDescription
            {
                name = "display-notif-users-area",
                defaultValue = false,
            };

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="ve"></param>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                if (Application.isPlaying) return;

                base.Init(ve, bag, cc);
                var custom = ve as BottomArea_C;

                custom.IsAvatarOn = m_isAvatarOn.GetValueFromBag(bag, cc);
                custom.IsMicOn = m_isMicOn.GetValueFromBag(bag, cc);
                custom.IsSoundOn = m_isSoundOn.GetValueFromBag(bag, cc);
                custom.ButtonSelected = m_buttonSelected.GetValueFromBag(bag, cc);
                custom.DisplayNotifUsersArea = m_displayNotifAndUsersArea.GetValueFromBag(bag, cc);
            }
        }

        public virtual bool IsAvatarOn
        {
            get => m_isAvatarOn;
            set
            {
                m_isAvatarOn = value;
                if (value)
                {
                    Avatar.RemoveFromClassList(USSCustomClassAvatar_Icon_off);
                    Avatar.AddToClassList(USSCustomClassAvatar_Icon_on);
                }
                else
                {
                    Avatar.RemoveFromClassList(USSCustomClassAvatar_Icon_on);
                    Avatar.AddToClassList(USSCustomClassAvatar_Icon_off);
                }
            }
        }
        public virtual bool IsMicOn
        {
            get => m_isMicOn;
            set
            {
                m_isMicOn = value;
                if (value)
                {
                    Mic.RemoveFromClassList(USSCustomClassMic_Icon_off);
                    Mic.AddToClassList(USSCustomClassMic_Icon_on);
                }
                else
                {
                    Mic.RemoveFromClassList(USSCustomClassMic_Icon_on);
                    Mic.AddToClassList(USSCustomClassMic_Icon_off);
                }
            }
        }
        public virtual bool IsSoundOn
        {
            get => m_isSoundOn;
            set
            {
                m_isSoundOn = value;
                if (value)
                {
                    Sound.RemoveFromClassList(USSCustomClassSound_Icon_off);
                    Sound.AddToClassList(USSCustomClassSound_Icon_on);
                }
                else
                {
                    Sound.RemoveFromClassList(USSCustomClassSound_Icon_on);
                    Sound.AddToClassList(USSCustomClassSound_Icon_off);
                }
            }
        }

        public virtual BottomBarButton ButtonSelected
        {
            get => m_buttonSelected;
            set
            {
                if (m_buttonSelected == value) m_buttonSelected = BottomBarButton.None;
                else m_buttonSelected = value;
                switch (m_buttonSelected)
                {
                    case BottomBarButton.None:
                        Avatar.RemoveFromClassList(USSCustomClassButtonSelected);
                        Emote.RemoveFromClassList(USSCustomClassButtonSelected);
                        Mic.RemoveFromClassList(USSCustomClassButtonSelected);
                        Sound.RemoveFromClassList(USSCustomClassButtonSelected);
                        DisplayEmoteWindow = false;
                        break;
                    case BottomBarButton.Avatar:
                        Avatar.AddToClassList(USSCustomClassButtonSelected);
                        Emote.RemoveFromClassList(USSCustomClassButtonSelected);
                        Mic.RemoveFromClassList(USSCustomClassButtonSelected);
                        Sound.RemoveFromClassList(USSCustomClassButtonSelected);
                        DisplayEmoteWindow = false;
                        break;
                    case BottomBarButton.Emote:
                        Avatar.RemoveFromClassList(USSCustomClassButtonSelected);
                        Emote.AddToClassList(USSCustomClassButtonSelected);
                        Mic.RemoveFromClassList(USSCustomClassButtonSelected);
                        Sound.RemoveFromClassList(USSCustomClassButtonSelected);
                        DisplayEmoteWindow = true;
                        break;
                    case BottomBarButton.Mic:
                        Avatar.RemoveFromClassList(USSCustomClassButtonSelected);
                        Emote.RemoveFromClassList(USSCustomClassButtonSelected);
                        Mic.AddToClassList(USSCustomClassButtonSelected);
                        Sound.RemoveFromClassList(USSCustomClassButtonSelected);
                        DisplayEmoteWindow = false;
                        break;
                    case BottomBarButton.Sound:
                        Avatar.RemoveFromClassList(USSCustomClassButtonSelected);
                        Emote.RemoveFromClassList(USSCustomClassButtonSelected);
                        Mic.RemoveFromClassList(USSCustomClassButtonSelected);
                        Sound.AddToClassList(USSCustomClassButtonSelected);
                        DisplayEmoteWindow = false;
                        break;
                    default:
                        break;
                }
            }
        }

        protected virtual bool DisplayEmoteWindow
        {
            get
            {
                if (Application.isPlaying) return Game_C.S_displayEmoteWindow;
                else return m_displayEmoteWindow;
            }
            set
            {
                if (!Application.isPlaying) m_displayEmoteWindow = value;
                if (value)
                {
                    this.AddIfNotInHierarchy(EmoteWindow);
                    EmoteWindow.style.visibility = Visibility.Hidden;
                    EmoteWindow.UpdateFilter();
                }

                if
                (
                    !value
                    && EmoteWindow.FindRoot() == null
                ) return;

                EmoteWindow.schedule.Execute(() =>
                {
                    EmoteWindow.style.visibility = StyleKeyword.Null;
                    EmoteWindow.AddAnimation
                    (
                        this,
                        () => EmoteWindow.style.width = 0,
                        () => EmoteWindow.style.width = 400,
                        "width",
                        0.5f,
                        revert: !value,
                        callback: value ? null : EmoteWindow.RemoveFromHierarchy
                    );
                });
            }
        }

        public virtual bool DisplayNotifUsersArea
        {
            get
            {
                if (Application.isPlaying) return Game_C.S_displayNotifUserArea;
                else return m_displayNotifAndUserArea;
            }
            set
            {
                if (!Application.isPlaying) m_displayNotifAndUserArea = value;
                if (value)
                {
                    NotifAndUsers.AddToClassList(USSCustomClassButtonSelected);
                    NotifAndUsers.RemoveFromClassList(USSCustomClassNotifAndUsers_Icon_Off);
                    NotifAndUsers.AddToClassList(USSCustomClassNotifAndUsers_Icon_On);
                }
                else
                {
                    NotifAndUsers.RemoveFromClassList(USSCustomClassButtonSelected);
                    NotifAndUsers.RemoveFromClassList(USSCustomClassNotifAndUsers_Icon_On);
                    NotifAndUsers.AddToClassList(USSCustomClassNotifAndUsers_Icon_Off);
                }
            }
        }

        public virtual string StyleSheetGamePath => $"USS/game";
        public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/bottomArea";
        public virtual string USSCustomClassName => "bottom__area";
        public virtual string USSCustomClassBar => $"{USSCustomClassName}-bar";
        public virtual string USSCustomClassRightBox => $"{USSCustomClassName}-right__box";
        public virtual string USSCustomClassAvatar => $"{USSCustomClassName}-avatar";
        public virtual string USSCustomClassEmote => $"{USSCustomClassName}-emote";
        public virtual string USSCustomClassMic => $"{USSCustomClassName}-mic";
        public virtual string USSCustomClassSound => $"{USSCustomClassName}-sound";
        public virtual string USSCustomClassNotifAndUsers => $"{USSCustomClassName}-notif__users";
        public virtual string USSCustomClassNotifAndUsers_Icon_On => $"{USSCustomClassName}-notif__users__icon-on";
        public virtual string USSCustomClassNotifAndUsers_Icon_Off => $"{USSCustomClassName}-notif__users__icon-off";
        public virtual string USSCustomClassAvatar_Icon_on => $"{USSCustomClassName}-avatar__icon-on";
        public virtual string USSCustomClassMic_Icon_on => $"{USSCustomClassName}-mic__icon-on";
        public virtual string USSCustomClassSound_Icon_on => $"{USSCustomClassName}-sound__icon-on";
        public virtual string USSCustomClassAvatar_Icon_off => $"{USSCustomClassName}-avatar__icon-off";
        public virtual string USSCustomClassMic_Icon_off => $"{USSCustomClassName}-mic__icon-off";
        public virtual string USSCustomClassSound_Icon_off => $"{USSCustomClassName}-sound__icon-off";
        public virtual string USSCustomClassButtonSelected => $"{USSCustomClassName}-button__selected";
        public virtual string USSCustomClassEmote_Window => $"{USSCustomClassName}-emote__window";

        public System.Action<bool> NotifUsersValueChanged;
        public System.Action EmoteClicked;

        public VisualElement BottomBar = new VisualElement { name = "bottom-bar" };
        public VisualElement LeftBox = new VisualElement { name = "left-box" };

        public VisualElement RightBox = new VisualElement { name = "right-box" };
        public Button_C Avatar = new Button_C { name = "avatar" };
        public Button_C Emote = new Button_C {name = "emote"};
        public Button_C Mic = new Button_C {name = "mic"};
        public Button_C Sound = new Button_C {name = "sound"};
        public Button_C NotifAndUsers = new Button_C {name = "notifs-and-users"};

        public EmoteWindow_C EmoteWindow;

        protected bool m_isAvatarOn;
        protected bool m_isMicOn;
        protected bool m_isSoundOn;

        protected BottomBarButton m_buttonSelected;
        protected bool m_isMicSettingsOpen;
        protected bool m_isSoundSettingsOpen;
        protected bool m_isEmoteOpen;
        protected bool m_displayNotifAndUserArea;
        protected bool m_displayEmoteWindow;

        public BottomArea_C() => InitElement();

        public virtual void InitElement()
        {
            if (EmoteWindow == null)
            {
                if (Application.isPlaying) EmoteWindow = EmoteWindow_C.Instance;
                else EmoteWindow = new EmoteWindow_C();
                EmoteWindow.name = "emote-window";
            }

            try
            {
                this.AddStyleSheetFromPath(StyleSheetGamePath);
                this.AddStyleSheetFromPath(StyleSheetPath);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            AddToClassList(USSCustomClassName);
            BottomBar.AddToClassList(USSCustomClassBar);
            RightBox.AddToClassList(USSCustomClassRightBox);
            Avatar.AddToClassList(USSCustomClassAvatar);
            Emote.AddToClassList(USSCustomClassEmote);
            Mic.AddToClassList(USSCustomClassMic);
            Sound.AddToClassList(USSCustomClassSound);
            NotifAndUsers.AddToClassList(USSCustomClassNotifAndUsers);
            EmoteWindow.AddToClassList(USSCustomClassEmote_Window);

            Avatar.name = "avatar";
            Emote.name = "emote";
            Mic.name = "mic";
            Sound.name = "sound";

            Avatar.Height = ElementSize.Small;
            Emote.Height = ElementSize.Small;
            Mic.Height = ElementSize.Small;
            Sound.Height = ElementSize.Small;
            NotifAndUsers.Height = ElementSize.Small;
            Avatar.Width = ElementSize.Small;
            Emote.Width = ElementSize.Small;
            Mic.Width = ElementSize.Small;
            Sound.Width = ElementSize.Small;
            NotifAndUsers.Width = ElementSize.Small;

            Avatar.Type = ButtonType.Invisible;
            Emote.Type = ButtonType.Invisible;
            Mic.Type = ButtonType.Invisible;
            Sound.Type = ButtonType.Invisible;
            NotifAndUsers.Type = ButtonType.Invisible;

            Emote.clicked += () => EmoteClicked?.Invoke();
            NotifAndUsers.clicked += () => NotifUsersValueChanged?.Invoke(!Game_C.S_displayNotifUserArea);

            Add(BottomBar);

            BottomBar.Add(LeftBox);

            BottomBar.Add(RightBox);
            //RightBox.Add(Avatar);
            RightBox.Add(Emote);
            RightBox.Add(Mic);
            RightBox.Add(Sound);
            RightBox.Add(NotifAndUsers);

            IsAvatarOn = true;
            IsMicOn = false;
            IsSoundOn = true;
            ButtonSelected = BottomBarButton.None;
            DisplayNotifUsersArea = false;
        }
    }
}
