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
using umi3d.baseBrowser.ui.viewController;
using umi3d.cdk.menu;
using umi3d.commonScreen.Displayer;
using umi3d.commonScreen.game;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public class InformationArea_C : BaseVisual_C
    {
        public new class UxmlFactory : UxmlFactory<InformationArea_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            protected UxmlEnumAttributeDescription<ControllerEnum> m_controller = new UxmlEnumAttributeDescription<ControllerEnum>
            {
                name = "controller",
                defaultValue = ControllerEnum.MouseAndKeyboard
            };

            UxmlLocaliseAttributeDescription m_shortText = new UxmlLocaliseAttributeDescription
            {
                name = "short-text",
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
                var custom = ve as InformationArea_C;

                custom.Controller = m_controller.GetValueFromBag(bag, cc);
                custom.ShortText = m_shortText.GetValueFromBag(bag, cc);
                custom.IsExpanded = m_isExpanded.GetValueFromBag(bag, cc);
                custom.IsMicOn = m_isMicOn.GetValueFromBag(bag, cc);
                custom.IsSoundOn = m_isSoundOn.GetValueFromBag(bag, cc);
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

        public virtual LocalisationAttribute ShortText
        {
            get => ShortInf.LocaliseText;
            set
            {
                UnityEngine.Debug.Log($"value = {value}, {ShortInf.IsAttachedToPanel}");
                if (value == ShortInf.LocaliseText) return;
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
                        ShortInf.LocaliseText = value;
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

        public override string StyleSheetPath_MainTheme => $"USS/game";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetGamesFolderPath}/informationArea";

        public override string UssCustomClass_Emc => "information__area";
        public virtual string USSCustomClassShortInf => $"{UssCustomClass_Emc}-short__inf";
        public virtual string USSCustomClassMic_Sound => $"{UssCustomClass_Emc}-mic-sound";
        public virtual string USSCustomClassMic_On => $"{UssCustomClass_Emc}-mic__on";
        public virtual string USSCustomClassSound_On => $"{UssCustomClass_Emc}-sound__on";
        public virtual string USSCustomClassMic_Off => $"{UssCustomClass_Emc}-mic__off";
        public virtual string USSCustomClassSound_Off => $"{UssCustomClass_Emc}-sound__off";
        public virtual string USSCustomToolbox => $"{UssCustomClass_Emc}-toolbox";

        public event Action<bool> ExpandUpdate;
        public event Action MicStatusChanged;
        public event Action SoundStatusChanged;
        public event Action NotificationTitleClicked;

        public string EnvironmentName;

        public Text_C ShortInf = new Text_C { name = "short-inf" };
        public VisualElement Mic = new VisualElement { name = "mic-icon" };
        public VisualElement Sound = new VisualElement { name = "sound-icon" };
        public Button_C Toolbox = new Button_C { name = "toolbox" };

        public TouchManipulator2 InfManipulator = new TouchManipulator2(null, 0, 0);
        public TouchManipulator2 ShortInfManipulator = new TouchManipulator2(null, 0, 0);
        public TouchManipulator2 MicManipulator = new TouchManipulator2(null, 0, 0);
        public TouchManipulator2 SoundManipulator = new TouchManipulator2(null, 0, 0);

        protected ControllerEnum m_controller;
        protected Vector2 m_initialManipulatedPosition;
        protected bool m_isExplanded;
        protected bool m_isMicOn;
        protected bool m_isSoundOn;
        protected Length m_shortInfheightLength = float.NaN;
        protected Length m_shortInfWidthgLength = float.NaN;
        protected Length m_gameMargin_Padding = float.NaN;
        protected Length m_shortInf_Padding = float.NaN;
        protected bool m_shortInfExpended = true;

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            ShortInf.AddToClassList(USSCustomClassShortInf);
            Mic.AddToClassList(USSCustomClassMic_Sound);
            Sound.AddToClassList(USSCustomClassMic_Sound);
            Toolbox.AddToClassList(USSCustomToolbox);
        }

        protected override void InitElement()
        {
            base.InitElement();
            GlobalToolsMenu = Resources.Load<MenuAsset>("Scriptables/GamePanel/GlobalToolsMenu");

            this.AddManipulator(InfManipulator);
            InfManipulator.ClickedDownWithInfo += (evt, locaPosition) =>
            {
                if (Controller != ControllerEnum.Touch) return;
                m_initialManipulatedPosition = locaPosition;
                if (IsExpanded) IsExpanded = false;
            };
            InfManipulator.MovedWithInfo += (evt, localPosition) =>
            {
                if (Controller != ControllerEnum.Touch) return;
                if (!IsExpanded && 10f < localPosition.y - m_initialManipulatedPosition.y) IsExpanded = true;
            };

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
                    m_isExplanded = true;
                    NotificationTitleClicked?.Invoke();
                    return;
                }
                else if (!IsExpanded)
                {
                    if (Controller != ControllerEnum.Touch) return;
                    IsExpanded = true;
                }
                else
                {
                    if (Controller != ControllerEnum.Touch) return;
                    IsExpanded = false;
                }
            };

            Mic.AddManipulator(MicManipulator);
            MicManipulator.clicked += () => MicStatusChanged?.Invoke();
            Sound.AddManipulator(SoundManipulator);
            SoundManipulator.clicked += () => SoundStatusChanged?.Invoke();
            Add(ShortInf);

            ShortInf.schedule.Execute(() =>
            {
                if (!NotificationCenter_C.NotificationTitleStack.TryPeek(out var title))
                {
                    //AnimateShortInf(true);
                    ShortText = EnvironmentName;
                }
                else if (!HideNotification)
                {
                    NotificationCenter_C.NotificationTitleStack.Pop();
                    //AnimateShortInf(false);
                    ShortText = $"Notif: {title}";
                }
            }).Every(3000);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Controller = ControllerEnum.MouseAndKeyboard;
            ShortText = null;
            IsExpanded = false;
            IsMicOn = false;
            IsSoundOn = false;
        }

        protected override void AttachedToPanel(AttachToPanelEvent evt)
        {
            base.AttachedToPanel(evt);
            GlobalToolsMenu.menu.onContentChange.AddListener(ToolsMenuContentChanged);
            ToolsMenuContentChanged();
        }

        protected override void DetachedFromPanel(DetachFromPanelEvent evt)
        {
            base.DetachedFromPanel(evt);
            GlobalToolsMenu.menu.onContentChange.RemoveListener(ToolsMenuContentChanged);
        }

        protected override void CustomStyleResolved(CustomStyleResolvedEvent evt)
        {
            base.CustomStyleResolved(evt);
            this.TryGetCustomStyle("--size__height-short-inf", out m_shortInfheightLength);
            this.TryGetCustomStyle("--size__width-short-inf", out m_shortInfWidthgLength);
            this.TryGetCustomStyle("--size-margin-and-padding-game", out m_gameMargin_Padding);
            this.TryGetCustomStyle("--padding-short-inf", out m_shortInf_Padding);
            Controller = Controller;
        }

        #region Implementation

        public static bool HideNotification;

        public MenuAsset GlobalToolsMenu;

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

        protected void ToolsMenuContentChanged()
        {
            if (GlobalToolsMenu.menu.Count > 0) this.AddIfNotInHierarchy(Toolbox);
            else if (GlobalToolsMenu.menu.Count == 0) Toolbox.RemoveFromHierarchy();
        }

        #endregion
    }
}
