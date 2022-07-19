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

        /// <summary>
        /// Where the drag started.
        /// </summary>
        protected Vector2? m_startPosition;
        /// <summary>
        /// Required to get canvas size.
        /// </summary>
        protected VisualElement m_visual;

        protected virtual void OnMouseDown(MouseDownEvent e)
        {
            if (!IsPointerIdCompatible(PointerId.mousePointerId) || !CanStartManipulation(e)) return;
            ProcessDownEvent(e, e.localMousePosition, PointerId.mousePointerId);
        }
        protected virtual void OnPointerDown(PointerDownEvent e)
        {
            if (!IsPointerIdCompatible(e.pointerId)) return;
            ProcessDownEvent(e, e.position, e.pointerId);
        }
        protected virtual void ProcessDownEvent(EventBase evt, Vector2 localPosition, int pointerId)
        {
            m_startPosition = localPosition;
            target.CapturePointer(pointerId);
            MouseDown?.Invoke();
        }

        /// <summary>
        /// Makes the VisualElement follow user's mouse position.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseMove(MouseMoveEvent e)
        {
            if (!IsPointerIdUsed(PointerId.mousePointerId) || !target.HasMouseCapture()) return;
            ProcessMoveEvent(e, e.localMousePosition, PointerId.mousePointerId);
        }
        protected virtual void OnPointerMove(PointerMoveEvent e)
        {
            if (!IsPointerIdUsed(e.pointerId) || !target.HasPointerCapture(e.pointerId)) return;
            ProcessMoveEvent(e, e.position, e.pointerId);
        }

        /// <summary>
        /// Releases the VisualElement currently dragged.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseUp(MouseUpEvent e)
        {
            if (!IsPointerIdUsed(PointerId.mousePointerId) || !target.HasMouseCapture() || !CanStopManipulation(e)) return;
            ProcessUpEvent(e, PointerId.mousePointerId);
        }
        protected virtual void OnPointerUp(PointerUpEvent e)
        {
            if (!IsPointerIdUsed(e.pointerId) || !target.HasPointerCapture(e.pointerId)) return;
            ProcessUpEvent(e, e.pointerId);
        }
        protected virtual void ProcessUpEvent(EventBase evt, int pointerId)
        {
            target.ReleasePointer(pointerId);
            m_startPosition = null;
            MouseUp?.Invoke();
        }
    }

    public abstract partial class AbstractDraggableManipulator
    {
        protected abstract void ProcessMoveEvent(EventBase e, Vector2 localPosition, int pointerId);
        protected abstract bool IsPointerIdCompatible(int pointerId);
        protected abstract bool IsPointerIdUsed(int pointerId);
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
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }
    }
}