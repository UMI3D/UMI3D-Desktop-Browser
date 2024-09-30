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
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using umi3d.baseBrowser.ui.viewController;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.common;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class User_C : BaseVisual_C
    {
        const float userVolumeRangePercent = 3;
        const float logBase = 1.5f;
        const float factor = 5f / 2f;
        const float factor2 = 5f / 2f;

        public new class UxmlFactory : UxmlFactory<User_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            protected UxmlStringAttributeDescription m_name = new UxmlStringAttributeDescription
            {
                name = "user-name",
                defaultValue = null
            };
            protected UxmlBoolAttributeDescription m_isMute = new UxmlBoolAttributeDescription
            {
                name = "is-mute",
                defaultValue = false,
            };
            protected UxmlFloatAttributeDescription m_volume = new UxmlFloatAttributeDescription
            {
                name = "volume",
                defaultValue = 5f
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
                var custom = ve as User_C;

                custom.UserName = m_name.GetValueFromBag(bag, cc);
                custom.IsMute = m_isMute.GetValueFromBag(bag, cc);
                custom.Volume = m_volume.GetValueFromBag(bag, cc);
            }
        }

        public virtual LocalisationAttribute UserName
        {
            get => m_userName;
            set
            {
                m_userName = value;
                UserNameVisual.LocalisedText = value;
            }
        }

        public virtual bool IsMute
        {
            get => m_isMute;
            set
            {
                m_isMute = value;
                if (value)
                {
                    Mute_Icon.RemoveFromClassList(USSCustomClassMute_Off);
                    Mute_Icon.AddToClassList(USSCustomClassMute_On);
                }
                else
                {
                    Mute_Icon.RemoveFromClassList(USSCustomClassMute_On);
                    Mute_Icon.AddToClassList(USSCustomClassMute_Off);
                }
            }
        }

        /// <summary>
        /// Volume is between 0 and userVolumeRangePercent * 100.
        /// <see cref="userVolumeRangePercent"/>
        /// </summary>
        public virtual float Volume
        {
            get => m_isMute ? 0f : m_volume;
            set
            {
                value = Mathf.Clamp(value, 0f, 100f * userVolumeRangePercent);
                m_volume = value;
                User_Audio_Slider.style.width = Length.Percent(value / userVolumeRangePercent);
                if (value == 0f) IsMute = true;
                else IsMute = false;
            }
        }

        public override string StyleSheetPath_MainTheme => $"USS/displayer";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/user";

        public override string UssCustomClass_Emc => "user";
        public virtual string USSCustomClassUserName => $"{UssCustomClass_Emc}__name";
        public virtual string USSCustomClassUserName_Background => $"{UssCustomClass_Emc}__name-background";
        public virtual string USSCustomClassUserName_Slider => $"{UssCustomClass_Emc}__name-audio-slider";
        
        public virtual string USSCustomClassMute => $"{UssCustomClass_Emc}__mute";
        public virtual string USSCustomClassMute_Background => $"{UssCustomClass_Emc}__mute-background";
        public virtual string USSCustomClassMute_Icon => $"{UssCustomClass_Emc}__mute-icon";
        public virtual string USSCustomClassMute_On => $"{UssCustomClass_Emc}__mute-icon__on";
        public virtual string USSCustomClassMute_Off => $"{UssCustomClass_Emc}__mute-icon__off";

        public virtual string USSCustomClassMenu => USSCustomClassMute;
        public virtual string USSCustomClassMenu_Background => USSCustomClassMute_Background;
        public virtual string USSCustomClassMenu_Icon => $"{UssCustomClass_Emc}__menu-icon";



        public event System.Action<bool> LocalMuteValueChanged;
        public UMI3DUser User;
        public Text_C UserNameVisual = new Text_C { name = "user-name" };
        public Visual_C User_Background = new Visual_C { name = "user-background" };
        public Visual_C User_Audio_Slider = new Visual_C { name = "user-audio-slider" };

        public Button_C Menu = new Button_C { name = "menu" };
        public Visual_C Menu_Background = new Visual_C { name = "menu-background" };
        public Visual_C Menu_Icon = new Visual_C { name = "menu-icon" };

        public Button_C Mute = new Button_C { name = "mute" };
        public Visual_C Mute_Background = new Visual_C { name = "mute-background" };
        public Visual_C Mute_Icon = new Visual_C { name = "mute-icon" };

        protected LocalisationAttribute m_userName;
        protected bool m_isMute;
        protected float m_volume
        {
            get => AudioManager.Exists ? VGToUserVolume(AudioManager.Instance.GetVolumeForUser(User) ?? 1, AudioManager.Instance.GetGainForUser(User) ?? 1) : 0;
            set
            {
                if (AudioManager.Exists)
                {
                    if (value != 0f) m_volumeWhenMute = value;
                    var vg = UserVolumeToVG(value);
                    AudioManager.Instance.SetGainForUser(User, vg.gain);
                    AudioManager.Instance.SetVolumeForUser(User, vg.volume);
                }
            }
        }
        protected float m_volumeWhenMute;
        protected TouchManipulator2 m_manipulator = new TouchManipulator2(null, 0, 0);

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            User_Background.AddToClassList(USSCustomClassUserName_Background);
            User_Audio_Slider.AddToClassList(USSCustomClassUserName_Slider);
            UserNameVisual.AddToClassList(USSCustomClassUserName);
            Mute.AddToClassList(USSCustomClassMute);
            Mute_Background.AddToClassList(USSCustomClassMute_Background);
            Mute_Icon.AddToClassList(USSCustomClassMute_Icon);

            Menu.AddToClassList(USSCustomClassMenu);
            Menu_Background.AddToClassList(USSCustomClassMenu_Background);
            Menu_Icon.AddToClassList(USSCustomClassMenu_Icon);

        }

        protected override void InitElement()
        {
            base.InitElement();
            User_Background.AddManipulator(m_manipulator);
            void AnimateInOutSlider(bool isRevert)
            {
                User_Audio_Slider
                    .SetHeight(!isRevert ? Length.Percent(100) : Length.Percent(10))
                    .WithAnimation()
                    .SetCallback(() =>
                    {
                        UserNameVisual.LocalisedText = isRevert ? m_userName : $"{m_volume.ToString("0.00")} %";
                    });
            };
            void ComputeVolume(Vector2 localPosition)
            {
                var xPercent = localPosition.x * 100f / User_Background.layout.width;
                xPercent = Mathf.Clamp(xPercent, 0, 100) * userVolumeRangePercent;
                Volume = xPercent;
                UserNameVisual.LocalisedText = $"{m_volume.ToString("0.00")} %";
            }
            m_manipulator.ClickedDownWithInfo += (evnt, localPosition) =>
            {
                AnimateInOutSlider(false);
                ComputeVolume(localPosition);
            };
            m_manipulator.ClickedUp += () => AnimateInOutSlider(true);
            m_manipulator.MovedWithInfo += (evnt, localPosition) => ComputeVolume(localPosition);
            User_Audio_Slider.style.height = Length.Percent(10);

            Mute.Type = ButtonType.Invisible;
            Mute.Shape = ButtonShape.Round;
            Mute.clicked += () => LocalMuteValueChanged?.Invoke(!m_isMute);

            Menu.Type = ButtonType.Invisible;
            Menu.Shape = ButtonShape.Round;
            Menu.clicked += Menu_clicked;

            ///Globaly mute: to implement in the future.
            //MuteValueChanged += value => User?.SetMicrophoneStatus(!value);
            LocalMuteValueChanged += value =>
            {
                if (value) Volume = 0f;
                else if (m_volumeWhenMute < 10f) Volume = 50f;
                else Volume = m_volumeWhenMute;
                IsMute = value;
            };

            Add(User_Background);
            User_Background.Add(User_Audio_Slider);
            User_Background.Add(UserNameVisual);

            Add(Mute);
            Mute.Add(Mute_Background);
            Mute_Background.Add(Mute_Icon);

            Mute.style.minWidth = 100;

           // Add(Menu);
            Menu.Add(Menu_Background);
            Menu_Background.Add(Menu_Icon);

            Menu.style.minWidth = 100;

            buttons = new();
        }

        private void Menu_clicked()
        {
            throw new System.NotImplementedException();
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            UserName = null;
            IsMute = false;
            Volume = 100f;
        }


        List<VisualElement> buttons;

        public void ClearButtons()
        {
            buttons.ForEach(buttons => buttons.RemoveFromHierarchy());
            buttons.Clear();
        }

        public void AddButton(UserAction userAction)
        {
            Button_C button = new Button_C { name = userAction.name, tooltip = userAction.description };
            Visual_C button_Background = new Visual_C { name = $"{userAction.name}-background" };
            Visual_C button_Icon = new Visual_C { name = $"{userAction.name}-icon" };

            button.AddToClassList(USSCustomClassMute);
            button_Background.AddToClassList(USSCustomClassMute_Background);
            
            button_Icon.AddToClassList(USSCustomClassMute_Icon);

            button.Type = ButtonType.Invisible;
            button.Shape = ButtonShape.Round;
            button.clicked += userAction.Call;

            Add(button);
            button.Add(button_Background);
            button_Background.Add(button_Icon);

            SetBackground(button_Icon, userAction);
            //button_Icon.style.backgroundImage = 
            //button_Background

            button.style.minWidth = 100;
            //button.style.backgroundColor = UnityEngine.Random.ColorHSV();

            buttons.Add(button);
        }

        async void SetBackground(Visual_C button_Icon, UserAction userAction)
        {
            var texture = await userAction.GetTexture();
            if(texture != null)
                button_Icon.style.backgroundImage = Background.FromTexture2D(texture);
        }

        #region Implementation

        /// <summary>
        /// Convert a user volume to a pair of volume and gain
        /// </summary>
        /// <param name="volume">Volume between 0 and <see cref="userVolumeRangePercent">*100</see>/></param>
        /// <returns>a pair of volume and gain. Volume is between 0 and 1 and Gain if between 1 and <see cref="userVolumeRangePercent"> </returns>
        (float volume, float gain) UserVolumeToVG(float volume)
        {
            if (volume <= 100f)
                return (volume / 100f, 1);
            else
                return (1, GainFactor(volume / 100));
        }

        /// <summary>
        /// Convert a user a pair of volume and gain to volume
        /// </summary>
        /// <param name="volume">a pair of volume and gain. Volume is between 0 and 1 and Gain if between 1 and <see cref="userVolumeRangePercent"></param>
        /// <returns>Volume between 0 and <see cref="userVolumeRangePercent">*100</see>/></returns>
        float VGToUserVolume(float volume, float gain)
        {
            if (volume < 1)
                return volume * 100f;
            return InvertGainFactor(gain) * 100;
        }

        /// <summary>
        /// Applying both GainFactor and InvertGainFactor should return the identity in any order.
        /// </summary>
        float GainFactor(float gain) { return (Mathf.Pow(logBase, (gain - 1) * factor) - 1) * factor2 + 1; }

        float InvertGainFactor(float gain) { return (Mathf.Log((gain - 1) / factor2 + 1, logBase) / factor) + 1; }

        #endregion
    }
}
