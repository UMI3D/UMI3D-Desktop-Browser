/*
Copyright 2019 - 2021 Inetum

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
using umi3DBrowser.UICustomStyle;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    /// <summary>
    /// This class provides a class for a manipulator which enables visual styles to change according to mouse behaviour.
    /// </summary>
    public partial class VisualManipulator
    {
        public event Action<MouseBehaviour> OnMouseBehaviourChanged;

        //protected VisualElement m_visual { get; set; } = null;
        protected CustomStyle_SO m_styleSO { get; set; } = null;
        protected StyleKeys m_keys { get; set; } = null;
        protected bool m_stopPropagation { get; set; } = false;
        protected Action<CustomStyle_SO, StyleKeys, VisualElement> m_applyFormat { get; set; } = null;
        protected Action<CustomStyle_SO, StyleKeys, IStyle, MouseBehaviour> m_applyStyle { get; set; } = null;

        #region Mouse

        protected enum MousePressedState
        {
            Unpressed,
            Pressed
        }
        protected enum MousePositionState
        {
            Out,
            Over
        }

        protected (MousePressedState, MousePositionState) m_mouseState { get; set; }
        protected MouseBehaviour m_mouseBehaviourFromState
        {
            get
            {
                return m_mouseState switch
                {
                    (MousePressedState.Unpressed, MousePositionState.Out) => MouseBehaviour.MouseOut,
                    (MousePressedState.Unpressed, MousePositionState.Over) => MouseBehaviour.MouseOver,
                    _ => MouseBehaviour.MousePressed
                };
            }
        }

        #endregion
    }

    public partial class VisualManipulator
    {
        public VisualManipulator(VisualElement visualTarget, CustomStyle_SO style_SO, StyleKeys keys, bool stopPropagation, Action<CustomStyle_SO, StyleKeys, VisualElement> applyFormat, Action<CustomStyle_SO, StyleKeys, IStyle, MouseBehaviour> applyStyle)
        {
            //m_visual = visualTarget;
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            Set(style_SO, keys, applyFormat, applyStyle);
            m_stopPropagation = stopPropagation;
        }

        public void Set(CustomStyle_SO style_SO, StyleKeys keys)
        {
            m_styleSO?.AppliesFormatAndStyle.RemoveListener(AppliesFormatAndStyle);
            m_styleSO = style_SO;
            m_keys = keys;
            m_styleSO?.AppliesFormatAndStyle.AddListener(AppliesFormatAndStyle);
        }
        public void Set(CustomStyle_SO style_SO, StyleKeys keys, Action<CustomStyle_SO, StyleKeys, VisualElement> applyFormat, Action<CustomStyle_SO, StyleKeys, IStyle, MouseBehaviour> applyStyle)
        {
            Set(style_SO, keys);
            m_applyFormat = applyFormat;
            m_applyStyle = applyStyle;
        }

        public void UpdatesKeys(StyleKeys newKeys)
        {
            m_keys = newKeys;
            AppliesFormatAndStyle();
        }

        public void AppliesStyle()
            => m_applyStyle(m_styleSO, m_keys, target.style, m_mouseBehaviourFromState);
        public void ApplyStyle(MouseBehaviour mouseBehaviour)
            => m_applyStyle(m_styleSO, m_keys, target.style, mouseBehaviour);

        public void AppliesFormatAndStyle()
        {
            m_applyFormat(m_styleSO, m_keys, target);
            m_applyStyle(m_styleSO, m_keys, target.style, m_mouseBehaviourFromState);
        }
    }

    public partial class VisualManipulator : MouseManipulator
    {
        #region Registrations

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseOverEvent>(OnMouseOver);
            target.RegisterCallback<MouseOutEvent>(OnMouseOut);
            target.RegisterCallback<MouseCaptureEvent>(OnMouseCapture);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);

            m_styleSO?.AppliesFormatAndStyle.AddListener(AppliesFormatAndStyle);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseOverEvent>(OnMouseOver);
            target.UnregisterCallback<MouseOutEvent>(OnMouseOut);
            target.UnregisterCallback<MouseCaptureEvent>(OnMouseCapture);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);

            m_styleSO?.AppliesFormatAndStyle.RemoveListener(AppliesFormatAndStyle);
        }

        #endregion

        protected virtual void OnMouseOver(MouseOverEvent e)
            => MouseBehaviourChanged(e, (m_mouseState.Item1, MousePositionState.Over), target.style);

        protected virtual void OnMouseOut(MouseOutEvent e)
            => MouseBehaviourChanged(e, (m_mouseState.Item1, MousePositionState.Out), target.style);

        protected virtual void OnMouseCapture(MouseCaptureEvent e)
            => MouseBehaviourChanged(e, (MousePressedState.Pressed, m_mouseState.Item2), target.style);

        protected virtual void OnMouseUp(MouseUpEvent e)
            => MouseBehaviourChanged(e, (MousePressedState.Unpressed, m_mouseState.Item2), target.style);

        protected void MouseBehaviourChanged(EventBase e, (MousePressedState, MousePositionState) mouseState, IStyle style)
        {
            m_mouseState = mouseState;
            m_applyStyle(m_styleSO, m_keys, style, m_mouseBehaviourFromState);
            if (m_stopPropagation) e.StopPropagation();
            OnMouseBehaviourChanged?.Invoke(m_mouseBehaviourFromState);
        }
    }
}