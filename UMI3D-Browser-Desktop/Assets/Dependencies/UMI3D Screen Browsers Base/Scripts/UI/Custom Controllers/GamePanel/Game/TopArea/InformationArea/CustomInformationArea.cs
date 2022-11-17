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
                    break;
                case ControllerEnum.Touch:
                    ShortInf.Add(Mic);
                    ShortInf.Add(Sound);
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
            if (m_isExplanded == value && !value)
            {
                NotificationCenter.ResetNewNotificationFilter();
                Main.RemoveFromHierarchy();
                return;
            }
            m_isExplanded = value;
            ExpandUpdate?.Invoke(value);
            Main.WaitUntil
            (
                () => !float.IsNaN(m_shortInfheightLength.value) && !float.IsNaN(m_shortInfWidthgLength.value),
                () =>
                {
                    this.InsertIfNotInHierarchy(0, Main);

                    Main.schedule.Execute(() =>
                    {
                        var padingLength = new Length()
                        {
                            unit = m_shortInfheightLength.unit,
                            value = m_shortInfheightLength.value * 1.2f
                        };
                        var widthPercent = m_shortInfWidthgLength.unit == LengthUnit.Percent
                            ? m_shortInfWidthgLength.value
                            : m_shortInfWidthgLength.value * 100f / this.layout.width;

                        var heightPercent = m_shortInfheightLength.unit == LengthUnit.Percent
                            ? m_shortInfheightLength.value
                            : m_shortInfheightLength.value * 100f / this.layout.width;

                        Main.AddAnimation
                           (
                               this,
                               () => Main.style.marginTop = m_shortInfMargin_PaddingLength,
                               () => Main.style.marginTop = 0f,
                               "margin-top",
                               AnimatorManager.MainDuration,
                               revert: !m_isExplanded
                           );
                        Main.AddAnimation
                        (
                            this,
                            () => Main.style.paddingTop = 0f,
                            () => Main.style.paddingTop = padingLength,
                            "padding-top",
                            AnimatorManager.MainDuration,
                            revert: !m_isExplanded
                        );
                        Main.AddAnimation
                        (
                            this,
                            () => Main.style.width = Length.Percent(widthPercent),
                            () => Main.style.width = Length.Percent(100),
                            "width",
                            AnimatorManager.MainDuration,
                            revert: !m_isExplanded
                        );
                        Main.AddAnimation
                        (
                            this,
                            () => Main.style.height = Length.Percent(heightPercent),
                            () => Main.style.height = Length.Percent(100),
                            "height",
                            AnimatorManager.MainDuration,
                            callback: m_isExplanded ? null : () =>
                            {
                                NotificationCenter.ResetNewNotificationFilter();
                                Main.RemoveFromHierarchy();
                            },
                            revert: !m_isExplanded
                        );
                    });
                }
            );
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
    public virtual string USSCustomClassName => "information-area";
    public virtual string USSCustomClassMain => $"{USSCustomClassName}__main";
    public virtual string USSCustomClassMain_Background => $"{USSCustomClassName}__main-background";
    public virtual string USSCustomClassShortInf => $"{USSCustomClassName}__short__inf";
    public virtual string USSCustomClassMic_Sound => $"{USSCustomClassName}__mic-sound";
    public virtual string USSCustomClassMic_On => $"{USSCustomClassName}__mic-on";
    public virtual string USSCustomClassSound_On => $"{USSCustomClassName}__sound-on";
    public virtual string USSCustomClassMic_Off => $"{USSCustomClassName}__mic-off";
    public virtual string USSCustomClassSound_Off => $"{USSCustomClassName}__sound-off";

    public event Action<bool> ExpandUpdate;
    public event Action MicStatusChanged;
    public event Action SoundStatusChanged;

    public event Action NotificationTitleClicked;
    public string EnvironmentName;
    public Stack<string> NotificationTitleStack = new Stack<string>();
    public CustomText ShortInf;
    public VisualElement Main = new VisualElement { name = "main" };
    public VisualElement Main_Background = new VisualElement { name = "main-background" };
    public CustomUserList UserList;
    public CustomNotificationCenter NotificationCenter;
    public VisualElement Mic = new VisualElement { name = "mic-icon" };
    public VisualElement Sound = new VisualElement { name = "sound-icon" };
    public TouchManipulator2 InfManipulator = new TouchManipulator2(null, 0, 0);
    public TouchManipulator2 MainManipulator = new TouchManipulator2(null, 0, 0);
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
    protected Length m_shortInfMargin_PaddingLength = float.NaN;
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
        Main.AddToClassList(USSCustomClassMain);
        Main_Background.AddToClassList(USSCustomClassMain_Background);
        ShortInf.AddToClassList(USSCustomClassShortInf);
        Mic.AddToClassList(USSCustomClassMic_Sound);
        Sound.AddToClassList(USSCustomClassMic_Sound);

        this.RegisterCallback<CustomStyleResolvedEvent>((evt) =>
        {
            this.TryGetCustomStyle("--size__height-short-inf", out m_shortInfheightLength);
            this.TryGetCustomStyle("--size__width-short-inf", out m_shortInfWidthgLength);
            this.TryGetCustomStyle("--size-margin-and-padding-game", out m_shortInfMargin_PaddingLength);
        });

        this.AddManipulator(InfManipulator);
        InfManipulator.ClickedDownWithInfo += (evt, locaPosition) => m_initialManipulatedPosition = locaPosition;
        InfManipulator.MovedWithInfo += (evt, localPosition) =>
        {
            if (!IsExpanded && m_initialManipulatedPosition.y < localPosition.y) IsExpanded = true;
        };
        Main_Background.AddManipulator(MainManipulator);
        MainManipulator.ClickedDownWithInfo += (evt, locaPosition) => m_initialManipulatedPosition = locaPosition;
        MainManipulator.MovedWithInfo += (evt, localPosition) =>
        {
            if (IsExpanded && m_initialManipulatedPosition.y > localPosition.y) IsExpanded = false;
        };

        ShortInf.name = "short-inf";
        ShortInf.AddManipulator(ShortInfManipulator);
        ShortInfManipulator.LongPressDelay = 400;
        //ShortInfManipulator.ClickedLong += () => IsExpanded = !IsExpanded;
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

            //ShortInf.AddAnimation
            //(
            //    this,
            //    () => ShortInf.style.scale = new Scale(Vector3.one),
            //    () => ShortInf.style.scale = new Scale(new Vector3(1.2f, 1.2f, 1)),
            //    "scale",
            //    0.5f,
            //    forceAnimation: true
            //);
        };
        //ShortInfManipulator.ClickedUp += () =>
        //{
        //    ShortInf.AddAnimation
        //    (
        //        this,
        //        () => { },
        //        () => ShortInf.style.scale = new Scale(Vector3.one),
        //        "scale",
        //        0.5f,
        //        forceAnimation: true
        //    );
        //};

        Mic.AddManipulator(MicManipulator);
        MicManipulator.clicked += () => MicStatusChanged?.Invoke();
        Sound.AddManipulator(SoundManipulator);
        SoundManipulator.clicked += () => SoundStatusChanged?.Invoke();

        Main.Add(Main_Background);
        Main.Add(UserList);
        Main.Add(NotificationCenter);
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
            if (!NotificationTitleStack.TryPeek(out var title))
            {
                AnimateShortInf(true);
                ShortText = EnvironmentName;
            }
            else if (!HideNotification)
            {
                NotificationTitleStack.Pop();
                AnimateShortInf(false);
                var NotifCount = NotificationTitleStack.Count + 1;
                ShortText = NotifCount == 1 ? $"1 notif: {title}" : $"{NotifCount} notifs: {title}";
            }
        }).Every(3000);
    }

    #region Implementation

    public static bool HideNotification;

    public virtual void AddNotification(NotificationDto dto)
    {
        var notification = NotificationCenter.AddNotification(dto);
        NotificationTitleStack.Push(notification.Title);

        var root = this.FindRoot();
        root.schedule.Execute(() =>
        {
            notification.Timestamp = "0min";
            root.schedule.Execute(() =>
            {
                var time = notification.Timestamp.Substring(0, notification.Timestamp.Length - 3);
                notification.Timestamp = $"{int.Parse(time) + 1}min";
            }).Every(60000);
        }).ExecuteLater(60000);
    }

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
