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
using umi3d.baseBrowser.ui.viewController;
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class User_C : Visual_C
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
                UserNameVisual.LocaliseText = value;
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

        public event System.Action<bool> LocalMuteValueChanged;
        public UMI3DUser User;
        public Text_C UserNameVisual = new Text_C { name = "user-name" };
        public VisualElement User_Background = new VisualElement { name = "user-background" };
        public VisualElement User_Audio_Slider = new VisualElement { name = "user-audio-slider" };
        public Button_C Mute = new Button_C { name = "mute" };
        public VisualElement Mute_Background = new VisualElement { name = "mute-background" };
        public VisualElement Mute_Icon = new VisualElement { name = "mute-icon" };

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
        }

        protected override void InitElement()
        {
            base.InitElement();
            User_Background.AddManipulator(m_manipulator);
            void AnimateInOutSlider(bool isRevert)
            {
                User_Audio_Slider.AddAnimation
                (
                    this,
                    () => User_Audio_Slider.style.height = Length.Percent(10),
                    () => User_Audio_Slider.style.height = Length.Percent(100),
                    "height",
                    1f,
                    callback: () => UserNameVisual.LocaliseText = isRevert ? m_userName : $"{m_volume.ToString("0.00")} %",
                    revert: isRevert
                );
            };
            void ComputeVolume(Vector2 localPosition)
            {
                var xPercent = localPosition.x * 100f / User_Background.layout.width;
                xPercent = Mathf.Clamp(xPercent, 0, 100) * userVolumeRangePercent;
                Volume = xPercent;
                UserNameVisual.LocaliseText = $"{m_volume.ToString("0.00")} %";
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
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            UserName = null;
            IsMute = false;
            Volume = 100f;
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
