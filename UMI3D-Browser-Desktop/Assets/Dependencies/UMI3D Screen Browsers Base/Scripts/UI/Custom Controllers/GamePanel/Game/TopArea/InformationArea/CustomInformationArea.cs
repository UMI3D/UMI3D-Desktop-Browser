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
using System.Collections;
using System.Collections.Generic;
using umi3d.baseBrowser.ui.viewController;
using umi3d.common;
using umi3d.commonMobile.game;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomInformationArea : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlEnumAttributeDescription<ControllerEnum> m_controller = new UxmlEnumAttributeDescription<ControllerEnum>
        {
            name = "controller",
            defaultValue = ControllerEnum.MouseAndKeyboard
        };

        UxmlStringAttributeDescription m_shortText = new UxmlStringAttributeDescription
        {
            name = "short-text",
            defaultValue = null,
        };

        UxmlBoolAttributeDescription m_isExpanded = new UxmlBoolAttributeDescription
        {
            name = "is-expanded",
            defaultValue = false,
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

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as CustomInformationArea;

            custom.Set
                (
                    m_controller.GetValueFromBag(bag, cc),
                    m_shortText.GetValueFromBag(bag, cc),
                    m_isExpanded.GetValueFromBag(bag, cc),
                    m_isMicOn.GetValueFromBag(bag, cc),
                    m_isSoundOn.GetValueFromBag(bag, cc)
                );
        }
    }

    public ControllerEnum Controller
    {
        get => m_controller;
        set
        {
            m_controller = value;
            switch (value)
            {
                case ControllerEnum.MouseAndKeyboard:
                    Mic.RemoveFromHierarchy();
                    Sound.RemoveFromHierarchy();
                    ShortInf.style.paddingLeft = m_gameMargin_Padding;
                    ShortInf.style.paddingRight = m_gameMargin_Padding;
                    break;
                case ControllerEnum.Touch:
                    ShortInf.Add(Mic);
                    ShortInf.Add(Sound);
                    ShortInf.style.paddingLeft = m_gameMargin_Padding;
                    ShortInf.style.paddingRight = m_shortInf_Padding;
                    break;
                case ControllerEnum.GameController:
                    ShortInf.Add(Mic);
                    ShortInf.Add(Sound);
                    break;
                default:
                    break;
            }
        }
    }

    public virtual string ShortText
    {
        get => ShortInf.text;
        set 
        {
            if (value == ShortInf.text) return;
            var color = ShortInf.resolvedStyle.color;
            ShortInf.AddAnimation
            (
                this,
                () => ShortInf.style.color = new Color(color.r, color.g, color.b, color.a),
                () => ShortInf.style.color = new Color(color.r, color.g, color.b, 0),
                "color",
                AnimatorManager.TextFadeDuration,
                callback: () =>
                {
                    ShortInf.text = value;
                    ShortInf.AddAnimation
                    (
                        this,
                        () => ShortInf.style.color = new Color(color.r, color.g, color.b, 0),
                        () => ShortInf.style.color = new Color(color.r, color.g, color.b, color.a),
                        "color",
                        AnimatorManager.TextFadeDuration,
                        callback: () => ShortInf.style.color = StyleKeyword.Null
                    );
                }
            );
        }
    }

    public virtual bool IsExpanded
    {
        get => m_isExplanded;
        set
        {
            m_isExplanded = value;
            ExpandUpdate?.Invoke(value);
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
                Mic.RemoveFromClassList(USSCustomClassMic_Off);
                Mic.AddToClassList(USSCustomClassMic_On);
            }
            else
            {
                Mic.RemoveFromClassList(USSCustomClassMic_On);
                Mic.AddToClassList(USSCustomClassMic_Off);
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
                Sound.RemoveFromClassList(USSCustomClassSound_Off);
                Sound.AddToClassList(USSCustomClassSound_On);
            }
            else
            {
                Sound.RemoveFromClassList(USSCustomClassSound_On);
                Sound.AddToClassList(USSCustomClassSound_Off);
            }
        }
    }

    public virtual string StyleSheetGamePath => $"USS/game";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/informationArea";
    public virtual string USSCustomClassName => "information__area";
    public virtual string USSCustomClassShortInf => $"{USSCustomClassName}-short__inf";
    public virtual string USSCustomClassMic_Sound => $"{USSCustomClassName}-mic-sound";
    public virtual string USSCustomClassMic_On => $"{USSCustomClassName}-mic__on";
    public virtual string USSCustomClassSound_On => $"{USSCustomClassName}-sound__on";
    public virtual string USSCustomClassMic_Off => $"{USSCustomClassName}-mic__off";
    public virtual string USSCustomClassSound_Off => $"{USSCustomClassName}-sound__off";

    public event Action<bool> ExpandUpdate;
    public event Action MicStatusChanged;
    public event Action SoundStatusChanged;

    public event Action NotificationTitleClicked;
    public string EnvironmentName;
    public CustomText ShortInf;
    public VisualElement Mic = new VisualElement { name = "mic-icon" };
    public VisualElement Sound = new VisualElement { name = "sound-icon" };
    public TouchManipulator2 InfManipulator = new TouchManipulator2(null, 0, 0);
    public TouchManipulator2 ShortInfManipulator = new TouchManipulator2(null, 0, 0);
    public TouchManipulator2 MicManipulator = new TouchManipulator2(null, 0, 0);
    public TouchManipulator2 SoundManipulator = new TouchManipulator2(null, 0, 0);

    protected ControllerEnum m_controller;
    protected Vector2 m_initialManipulatedPosition;
    protected bool m_isExplanded;
    protected bool m_isMicOn;
    protected bool m_isSoundOn;
    protected bool m_hasBeenInitialized;
    protected Length m_shortInfheightLength = float.NaN;
    protected Length m_shortInfWidthgLength = float.NaN;
    protected Length m_gameMargin_Padding = float.NaN;
    protected Length m_shortInf_Padding = float.NaN;
    protected bool m_shortInfExpended = true;

    public virtual void InitElement()
    {
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
        ShortInf.AddToClassList(USSCustomClassShortInf);
        Mic.AddToClassList(USSCustomClassMic_Sound);
        Sound.AddToClassList(USSCustomClassMic_Sound);

        this.RegisterCallback<CustomStyleResolvedEvent>((evt) =>
        {
            this.TryGetCustomStyle("--size__height-short-inf", out m_shortInfheightLength);
            this.TryGetCustomStyle("--size__width-short-inf", out m_shortInfWidthgLength);
            this.TryGetCustomStyle("--size-margin-and-padding-game", out m_gameMargin_Padding);
            this.TryGetCustomStyle("--padding-short-inf", out m_shortInf_Padding);
        });

        this.AddManipulator(InfManipulator);
        InfManipulator.ClickedDownWithInfo += (evt, locaPosition) => m_initialManipulatedPosition = locaPosition;
        InfManipulator.MovedWithInfo += (evt, localPosition) =>
        {
            if (!IsExpanded && 10f < localPosition.y - m_initialManipulatedPosition.y) IsExpanded = true;
            if (IsExpanded && -10f > localPosition.y - m_initialManipulatedPosition.y) IsExpanded = false;
        };

        ShortInf.name = "short-inf";
        ShortInf.AddManipulator(ShortInfManipulator);
        ShortInfManipulator.ClickedDownWithInfo += (e, localPosition) =>
        {
            var localToWorld = ShortInf.LocalToWorld(localPosition);
            if (Mic.ContainsPoint(Mic.WorldToLocal(localToWorld)))
            {
                MicManipulator.OnClickedUp();
                return;
            }
            if (Sound.ContainsPoint(Sound.WorldToLocal(localToWorld)))
            {
                SoundManipulator.OnClickedUp();
                return;
            }
            if (ShortInf.text != "" && !ShortInf.text.Equals(EnvironmentName))
            {
                NotificationTitleClicked?.Invoke();
                return;
            }
        };

        Mic.AddManipulator(MicManipulator);
        MicManipulator.clicked += () => MicStatusChanged?.Invoke();
        Sound.AddManipulator(SoundManipulator);
        SoundManipulator.clicked += () => SoundStatusChanged?.Invoke();

        Add(ShortInf);
    }

    public virtual void Set() => Set(ControllerEnum.MouseAndKeyboard, null, false, false, true);

    public virtual void Set(ControllerEnum controller, string shortText, bool isExpanded, bool isMicOn, bool isSoundOn)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Controller = controller;
        ShortText = shortText;
        IsExpanded = isExpanded;
        IsMicOn = isMicOn;
        IsSoundOn = isSoundOn;

        ShortInf.schedule.Execute(() =>
        {
            if (!CustomNotificationCenter.NotificationTitleStack.TryPeek(out var title))
            {
                //AnimateShortInf(true);
                ShortText = EnvironmentName;
            }
            else if (!HideNotification)
            {
                CustomNotificationCenter.NotificationTitleStack.Pop();
                //AnimateShortInf(false);
                ShortText = $"Notif: {title}";
            }
        }).Every(3000);
    }

    #region Implementation

    public static bool HideNotification;

    protected void AnimateShortInf(bool isRevert)
    {
        if (isRevert != m_shortInfExpended) return;
        ShortInf.AddAnimation
        (
            this,
            () => ShortInf.style.width = Length.Percent(10),
            () => ShortInf.style.width = Length.Percent(40),
            "width",
            0.5f,
            delay: isRevert ? 0.5f : 0f,
            revert: isRevert,
            callback: () => m_shortInfExpended = !isRevert
        );
    }

    #endregion
}
