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

public class CustomJoystick : VisualElement, ICustomElement
{
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

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomJoystick;
            
            custom.Set
                (
                    m_magnitudeRestriction.Clamp(m_magnitude.GetValueFromBag(bag, cc)),
                    m_angleRestriction.Clamp(m_angle.GetValueFromBag(bag, cc)),
                    m_state.GetValueFromBag(bag, cc)
                );
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

    public virtual string StyleSheetGamePath => $"USS/game";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/joystick";
    public virtual string USSCustomClassName => "joystick";
    public virtual string USSCustomClassState(ElementPseudoState state) => $"{USSCustomClassName}-{state}".ToLower();
    public virtual string USSCustomClassForeground => $"{USSCustomClassName}__foreground";
    public virtual string USSCustomClassForegroundIcon => $"{USSCustomClassName}__foreground__icon";

    public virtual Vector2 Direction => new Vector2(Magnitude * Mathf.Cos(Radian), Magnitude * Mathf.Sin(Radian));
    public float Radian => Angle * Mathf.Deg2Rad;
    public VisualElement Foreground = new VisualElement { name = "foreground" };
    public VisualElement ForegroundIcon = new VisualElement { name = "foreground-icon" };

    protected bool m_hasBeenInitialized;
    protected float m_magnitude;
    protected float m_angle;
    protected ElementPseudoState m_state;

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
        Foreground.AddToClassList(USSCustomClassForeground);
        ForegroundIcon.AddToClassList(USSCustomClassForegroundIcon);

        Add(Foreground);
        Foreground.Add(ForegroundIcon);
    }

    public virtual void Set() => Set(0f, 0f, ElementPseudoState.Enabled);

    public virtual void Set(float magnitude, float angle, ElementPseudoState state)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Magnitude = magnitude;
        Angle = angle;
        State = state;
    }

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
}
