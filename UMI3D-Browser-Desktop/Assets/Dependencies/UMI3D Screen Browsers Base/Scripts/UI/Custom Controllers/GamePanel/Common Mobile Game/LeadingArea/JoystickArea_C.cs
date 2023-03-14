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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonMobile.game
{
    public class JoystickArea_C : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<JoystickArea_C, UxmlTraits> { }

        public bool LeftHand
        {
            get => m_leftHand;
            set
            {
                m_leftHand = value;
                if (value)
                {
                    RemoveFromClassList(USSCustomClassName);
                    AddToClassList(USSCustomClassNameReverse);
                    UpdateStaticMode();
                }
                else
                {
                    RemoveFromClassList(USSCustomClassNameReverse);
                    AddToClassList(USSCustomClassName);
                    UpdateStaticMode();
                }
            }
        }

        public virtual string StyleSheetGamePath => $"USS/game";
        public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/joystickArea";
        public virtual string USSCustomClassName => "joystick-area";
        public virtual string USSCustomClassNameReverse => "joystick-area_reverse";

        public static System.Action JoystickStaticModeUpdated;
        public static bool IsJoystickStatic;
        public Joystick_C Joystick = new Joystick_C { name = "joystick" };
        public TouchManipulator2 JoystickManipulator = new TouchManipulator2(null, 0, 0);

        protected bool m_hasBeenInitialized;
        protected bool m_leftHand;

        public JoystickArea_C() => InitElement();

        /// <summary>
        /// Initialize this element
        /// </summary>
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

            JoystickStaticModeUpdated = UpdateStaticMode;
            UpdateStaticMode();

            this.AddManipulator(JoystickManipulator);
            JoystickManipulator.ClickedDownWithInfo += SetActivePosition;
            JoystickManipulator.MovedWithInfo += UpdateJoystick;
            JoystickManipulator.ClickedUp += SetDisableJoystick;

            Joystick.State = ElementPseudoState.Disabled;

            Add(Joystick);
        }

        #region Implementation

        protected virtual void SetActivePosition(EventBase e, Vector2 localPosition)
        {
            Joystick.State = ElementPseudoState.Enabled;

            if (IsJoystickStatic) return;
            var worldPosition = this.LocalToWorld(localPosition);
            var joystickLocal = Joystick.WorldToLocal(worldPosition);

            var left = joystickLocal.x - Joystick.layout.width / 2f;
            var right = -joystickLocal.x + Joystick.layout.width / 2f;
            var bottom = -joystickLocal.y + Joystick.layout.height / 2f;

            if (m_leftHand)
                Joystick.style.right = Mathf.Clamp(right, 0, layout.width - Joystick.layout.width);
            else
                Joystick.style.left = Mathf.Clamp(left, 0, layout.width - Joystick.layout.width);
            Joystick.style.bottom = Mathf.Clamp(bottom, 0, layout.height - Joystick.layout.height);
        }

        protected virtual void UpdateJoystick(EventBase e, Vector2 localPosition)
        {
            var worldPosition = this.LocalToWorld(localPosition);
            var joystickLocal = Joystick.WorldToLocal(worldPosition);

            var joystickWidthHalf = Joystick.layout.width / 2f;
            var joystickHeighHalf = Joystick.layout.height / 2f;

            var joystickLocalCenter = new Vector2
                (
                    Mathf.Clamp(joystickLocal.x - joystickWidthHalf, -joystickWidthHalf, joystickWidthHalf) / joystickWidthHalf,
                    Mathf.Clamp(joystickLocal.y - joystickHeighHalf, -joystickHeighHalf, joystickHeighHalf) / joystickHeighHalf
                );

            Joystick.Magnitude = Mathf.Clamp(Vector2.SqrMagnitude(joystickLocalCenter), 0, 1);
            Joystick.Angle = -Mathf.Sign(joystickLocalCenter.y) * Vector2.Angle(Vector2.right, joystickLocalCenter);
        }

        protected virtual void SetDisableJoystick()
        {
            Joystick.State = ElementPseudoState.Disabled;
            Joystick.Magnitude = 0;
            if (IsJoystickStatic) return;
            Joystick.style.left = StyleKeyword.Null;
            Joystick.style.right = StyleKeyword.Null;
            Joystick.style.bottom = StyleKeyword.Null;
        }

        protected virtual void UpdateStaticMode()
        {
            Joystick.style.right = IsJoystickStatic && m_leftHand ? 100f : StyleKeyword.Null;
            Joystick.style.left = IsJoystickStatic && !m_leftHand ? 100f : StyleKeyword.Null;
            Joystick.style.bottom = IsJoystickStatic ? 100f : StyleKeyword.Null;
        }

        #endregion
    }
}
