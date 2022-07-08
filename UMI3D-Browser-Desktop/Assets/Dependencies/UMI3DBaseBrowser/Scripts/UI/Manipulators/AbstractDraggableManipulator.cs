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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.ui.viewController
{
    /// <summary>
    /// This class is manipulator which enables users to drag the target.
    /// </summary>
    public partial class AbstractDraggableManipulator
    {
        public event Action MouseDown;
        public event Action MouseUp;

        protected Vector2 start;
        protected VisualElement m_visual;

        protected virtual void OnMouseDown(MouseDownEvent e)
        {
            if (!CanStartManipulation(e))
                return;

            start = e.localMousePosition;
            target.CaptureMouse();
            MouseDown?.Invoke();
        }

        

        /// <summary>
        /// Releases the VisualElement currently dragged.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseUp(MouseUpEvent e)
        {
            if (!target.HasMouseCapture() || !CanStopManipulation(e))
                return;

            target.ReleaseMouse();
            MouseUp?.Invoke();
        }
    }

    public abstract partial class AbstractDraggableManipulator
    {
        protected abstract void OnMouseMove(MouseMoveEvent e);
    }

    public partial class AbstractDraggableManipulator : MouseManipulator
    {
        public AbstractDraggableManipulator(VisualElement visual)
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            m_visual = visual;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }
    }
}