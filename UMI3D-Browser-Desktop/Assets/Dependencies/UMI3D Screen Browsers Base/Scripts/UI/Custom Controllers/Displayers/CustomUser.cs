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
using umi3d.baseBrowser.ui.viewController;
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class CustomUser : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlStringAttributeDescription m_name = new UxmlStringAttributeDescription
        {
            name = "user-name",
            defaultValue = null
        };
        protected UxmlBoolAttributeDescription m_isMute = new UxmlBoolAttributeDescription
        {
            name = "is-mic-on",
            defaultValue = false,
        };
        protected UxmlFloatAttributeDescription m_volume = new UxmlFloatAttributeDescription
        {
            name = "volume",
            defaultValue = 5f
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomUser;

            custom.Set
            (
                m_name.GetValueFromBag(bag, cc),
                m_isMute.GetValueFromBag(bag, cc),
                m_volume.GetValueFromBag(bag, cc)
            );
        }
    }

    public virtual string UserName
    {
        get => m_userName;
        set 
        {
            m_userName = value;
            UserNameVisual.text = value;
        }
    }

    public virtual bool IsMute
    {
        get => m_isMute;
        set
        {
            UnityEngine.Debug.Log($"mute {value}");
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

    public virtual float Volume
    {
        get => m_isMute ? 0f : m_volume;
        set
        {
            UnityEngine.Debug.Log($"volume {value}");
            value = Mathf.Clamp(value, 0f, 100f);
            m_volume = value;
            User_Audio_Slider.style.width = Length.Percent(value);
            if (value == 0f) MuteValueChanged?.Invoke(true);
            else if (m_isMute) MuteValueChanged?.Invoke(true);
        }
    }

    public virtual string StyleSheetDisplayerPath => $"USS/displayer";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/user";
    public virtual string USSCustomClassName => "user";
    public virtual string USSCustomClassUserName => $"{USSCustomClassName}__name";
    public virtual string USSCustomClassUserName_Background => $"{USSCustomClassName}__name-background";
    public virtual string USSCustomClassUserName_Slider => $"{USSCustomClassName}__name-audio-slider";
    public virtual string USSCustomClassMute => $"{USSCustomClassName}__mute";
    public virtual string USSCustomClassMute_Background => $"{USSCustomClassName}__mute-background";
    public virtual string USSCustomClassMute_Icon => $"{USSCustomClassName}__mute-icon";
    public virtual string USSCustomClassMute_On => $"{USSCustomClassName}__mute-icon__on";
    public virtual string USSCustomClassMute_Off => $"{USSCustomClassName}__mute-icon__off";

    public event System.Action<bool> MuteValueChanged;
    public UMI3DUser User;
    public CustomText UserNameVisual;
    public VisualElement User_Background = new VisualElement { name = "user-background" };
    public VisualElement User_Audio_Slider = new VisualElement { name = "user-audio-slider" };
    public CustomButton Mute;
    public VisualElement Mute_Background = new VisualElement { name = "mute-background" };
    public VisualElement Mute_Icon = new VisualElement { name = "mute-icon" };

    protected bool m_hasBeenInitialized;
    protected string m_userName;
    protected bool m_isMute;
    protected float m_volume;
    protected TouchManipulator2 m_manipulator = new TouchManipulator2(null, 0, 0);

    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetDisplayerPath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);
        User_Background.AddToClassList(USSCustomClassUserName_Background);
        User_Audio_Slider.AddToClassList(USSCustomClassUserName_Slider);
        UserNameVisual.AddToClassList(USSCustomClassUserName);
        Mute.AddToClassList(USSCustomClassMute);
        Mute_Background.AddToClassList(USSCustomClassMute_Background);
        Mute_Icon.AddToClassList(USSCustomClassMute_Icon);

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
                callback: () => UserNameVisual.text = isRevert ? m_userName : $"{m_volume.ToString("0.00")} %",
                revert: isRevert
            );
        };
        m_manipulator.ClickedDown += () =>
        {
            AnimateInOutSlider(false);
        };
        m_manipulator.ClickedUp += () =>
        {
            AnimateInOutSlider(true);
        };
        m_manipulator.MovedWithInfo += (evnt, localPosition) =>
        {
            var xPercent = localPosition.x * 100f / User_Background.layout.width;
            xPercent = Mathf.Clamp(xPercent, 0, 100);
            Volume = xPercent;
            UserNameVisual.text = $"{m_volume.ToString("0.00")} %";
        };
        User_Audio_Slider.style.height = Length.Percent(10);

        Mute.Type = ButtonType.Invisible;
        Mute.Shape = ButtonShape.Round;
        Mute.clicked += () => MuteValueChanged?.Invoke(!m_isMute);

        MuteValueChanged += value => User?.SetMicrophoneStatus(value);

        Add(User_Background);
        User_Background.Add(User_Audio_Slider);
        User_Background.Add(UserNameVisual);
        
        Add(Mute);
        Mute.Add(Mute_Background);
        Mute_Background.Add(Mute_Icon);
    }

    public virtual void Set() => Set(null, false, 0f);

    public virtual void Set(string name, bool isMute, float volume) 
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        UserName = name;
        IsMute = isMute;
        Volume = volume;
    }
}
