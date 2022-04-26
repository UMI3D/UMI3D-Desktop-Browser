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
        /// <summary>
        /// Event raised when the behaviour of the mouse (over, out, down, up) changed.
        /// </summary>
        public event Action<MouseBehaviour> MouseBehaviourChanged;
        /// <summary>
        /// Event raised when the customStyle'format will be applied on the target.
        /// </summary>
        public event Action<CustomStyle_SO, StyleKeys, VisualElement> ApplyingFormat;
        /// <summary>
        /// Event raised when the customStyle'style will be applied on the target.
        /// </summary>
        public event Action<CustomStyle_SO, StyleKeys, IStyle, MouseBehaviour> ApplyingStyle;

        public bool StopPropagation { get; set; } = false;

        protected CustomStyle_SO m_styleSO { get; set; } = null;
        protected StyleKeys m_keys { get; set; } = null;
        
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
        public VisualManipulator () : 
            this(true) 
        { }
        public VisualManipulator(bool stopPropagation) : 
            this(new ManipulatorActivationFilter { button = MouseButton.LeftMouse }, stopPropagation)
        { }
        public VisualManipulator(ManipulatorActivationFilter activationFilter, bool stopPropagation)
        {
            activators.Add(activationFilter);
            StopPropagation = stopPropagation;
        }
        
        /// <summary>
        /// Bind custom style ans keys.
        /// </summary>
        /// <param name="style_SO"></param>
        /// <param name="keys"></param>
        public void Set(CustomStyle_SO style_SO, StyleKeys keys)
        {
            m_styleSO?.AppliesFormatAndStyle.RemoveListener(ApplyFormatAndStyle);
            m_styleSO = style_SO;
            m_keys = keys;
            m_styleSO?.AppliesFormatAndStyle.AddListener(ApplyFormatAndStyle);
        }

        /// <summary>
        /// Reset Manipulator, Unset events.
        /// </summary>
        public virtual void Reset()
        {
            MouseBehaviourChanged = null;
            ApplyingFormat = null;
            ApplyingStyle = null;
        }

        public void UpdateStyle(CustomStyle_SO style_SO)
        {
            m_styleSO = style_SO;
            ApplyFormatAndStyle();
        }
        public void UpdateKeys(StyleKeys newKeys)
        {
            m_keys = newKeys;
            ApplyFormatAndStyle();
        }

        public void ApplyStyle()
            => ApplyingStyle?.Invoke(m_styleSO, m_keys, target.style, m_mouseBehaviourFromState);
        public void ApplyStyle(MouseBehaviour mouseBehaviour)
            => ApplyingStyle?.Invoke(m_styleSO, m_keys, target.style, mouseBehaviour);

        public void ApplyFormatAndStyle()
        {
            ApplyingFormat?.Invoke(m_styleSO, m_keys, target);
            ApplyingStyle?.Invoke(m_styleSO, m_keys, target.style, m_mouseBehaviourFromState);
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
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);

            m_styleSO?.AppliesFormatAndStyle.AddListener(ApplyFormatAndStyle);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseOverEvent>(OnMouseOver);
            target.UnregisterCallback<MouseOutEvent>(OnMouseOut);
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);

            m_styleSO?.AppliesFormatAndStyle.RemoveListener(ApplyFormatAndStyle);

            Reset();
        }

        #endregion

        protected virtual void OnMouseOver(MouseOverEvent e)
            => OnMouseBehaviourChanged(e, (m_mouseState.Item1, MousePositionState.Over), target.style);

        protected virtual void OnMouseOut(MouseOutEvent e)
            => OnMouseBehaviourChanged(e, (m_mouseState.Item1, MousePositionState.Out), target.style);

        protected virtual void OnMouseDown(MouseDownEvent e)
            => OnMouseBehaviourChanged(e, (MousePressedState.Pressed, m_mouseState.Item2), target.style);

        protected virtual void OnMouseUp(MouseUpEvent e)
            => OnMouseBehaviourChanged(e, (MousePressedState.Unpressed, m_mouseState.Item2), target.style);

        protected void OnMouseBehaviourChanged(EventBase e, (MousePressedState, MousePositionState) mouseState, IStyle style)
        {
            m_mouseState = mouseState;
            ApplyingStyle?.Invoke(m_styleSO, m_keys, style, m_mouseBehaviourFromState);
            if (StopPropagation) e.StopPropagation();
            MouseBehaviourChanged?.Invoke(m_mouseBehaviourFromState);
        }
    }
}