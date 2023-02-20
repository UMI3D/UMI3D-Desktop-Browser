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
using System.Collections.Generic;
using umi3d.commonScreen;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonMobile.game
{
    public class Joystick_C : BaseVisual_C
    {
        public new class UxmlFactory : UxmlFactory<Joystick_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlFloatBounds m_magnitudeRestriction = new UxmlFloatBounds { min = 0, max = 1f };
            UxmlFloatBounds m_angleRestriction = new UxmlFloatBounds { min = -180f, max = 180f };

            UxmlFloatAttributeDescription m_magnitude = new UxmlFloatAttributeDescription
            {
                name = "magnitude",
                defaultValue = 0f,
            };

            UxmlFloatAttributeDescription m_angle = new UxmlFloatAttributeDescription
            {
                name = "angle",
                defaultValue = 0f,
            };

            UxmlEnumAttributeDescription<ElementPseudoState> m_state = new UxmlEnumAttributeDescription<ElementPseudoState>
            {
                name = "state",
                defaultValue = ElementPseudoState.Enabled
            };

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="ve"></param>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var custom = ve as Joystick_C;

                custom.Magnitude = m_magnitudeRestriction.Clamp(m_magnitude.GetValueFromBag(bag, cc));
                custom.Angle = m_angleRestriction.Clamp(m_angle.GetValueFromBag(bag, cc));
                custom.State = m_state.GetValueFromBag(bag, cc);
            }
        }

        public virtual float Magnitude
        {
            get => m_magnitude;
            set
            {
                m_magnitude = value;
                SetForegroundPosition();
            }
        }

        public virtual float Angle
        {
            get => m_angle;
            set
            {
                m_angle = value;
                SetForegroundPosition();
            }
        }

        public virtual ElementPseudoState State
        {
            get => m_state;
            set
            {
                RemoveFromClassList(USSCustomClassState(m_state));
                AddToClassList(USSCustomClassState(value));
                m_state = value;
                if (value == ElementPseudoState.Disabled)
                {
                    this.SetEnabled(false);
                    Foreground.style.left = StyleKeyword.Null;
                    Foreground.style.bottom = StyleKeyword.Null;
                    this.AddAnimation
                    (
                        this,
                        () => style.scale = new Scale(Vector3.one),
                        () => style.scale = new Scale(new Vector3(0.5f, 0.5f, 1)),
                        "scale",
                        0.5f
                    );
                }
                else
                {
                    this.SetEnabled(true);
                    this.RemoveAnimation("scale");
                    style.scale = new Scale(Vector3.one);
                }
            }
        }

        public override string StyleSheetPath_MainTheme => $"USS/game";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetGamesFolderPath}/joystick";

        public override string UssCustomClass_Emc => "joystick";
        public virtual string USSCustomClassState(ElementPseudoState state) => $"{UssCustomClass_Emc}-{state}".ToLower();
        public virtual string USSCustomClassForeground => $"{UssCustomClass_Emc}__foreground";
        public virtual string USSCustomClassForegroundIcon => $"{UssCustomClass_Emc}__foreground__icon";

        public VisualElement Foreground = new VisualElement { name = "foreground" };
        public VisualElement ForegroundIcon = new VisualElement { name = "foreground-icon" };

        protected float m_magnitude;
        protected float m_angle;
        protected ElementPseudoState m_state;

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            Foreground.AddToClassList(USSCustomClassForeground);
            ForegroundIcon.AddToClassList(USSCustomClassForegroundIcon);
        }

        protected override void InitElement()
        {
            base.InitElement();
            Add(Foreground);
            Foreground.Add(ForegroundIcon);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Magnitude = 0f;
            Angle = 0f;
            State = ElementPseudoState.Enabled;
        }

        #region Implementation

        public virtual Vector2 Direction => new Vector2(Magnitude * Mathf.Cos(Radian), Magnitude * Mathf.Sin(Radian));
        public float Radian => Angle * Mathf.Deg2Rad;

        /// <summary>
        /// Set the position of the foreground element.
        /// </summary>
        public void SetForegroundPosition()
        {
            ForegroundIcon.WaitUntil(() =>
            {
                return !float.IsNaN(ForegroundIcon.layout.width)
                && !float.IsNaN(Foreground.layout.width)
                && !float.IsNaN(ForegroundIcon.layout.height)
                && !float.IsNaN(Foreground.layout.height);
            }, () =>
            {
                Foreground.style.left = Magnitude * Mathf.Cos(Radian) * (Foreground.layout.width / 2f - ForegroundIcon.layout.width / 2f);

                Foreground.style.bottom = Magnitude * Mathf.Sin(Radian) * (Foreground.layout.height / 2f - ForegroundIcon.layout.height / 2f);
            });

        }

        #endregion
    }
}
