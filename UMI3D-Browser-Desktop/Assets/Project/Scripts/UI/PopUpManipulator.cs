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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.uI.viewController
{
    /// <summary>
    /// This class provides a class for a manipulator which enables users to move <see cref="AbstractPopUp"/>.
    /// </summary>
    public class PopUpManipulator : MouseManipulator
    {
        #region Init

        protected Vector2 start;
        protected bool active = false;

        public PopUpManipulator()
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }

        #endregion

        #region Registrations

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

        #endregion

        #region OnMouseDown

        protected virtual void OnMouseDown(MouseDownEvent e)
        {
            if (active)
            {
                e.StopImmediatePropagation();
                return;
            }

            if (CanStartManipulation(e))
            {
                start = e.localMousePosition;
                active = true;
                target.CaptureMouse();
                e.StopPropagation();
            }
        }

        #endregion

        #region OnMouseMove

        /// <summary>
        /// Makes the VisualElement follow user's mouse position.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseMove(MouseMoveEvent e)
        {
            if (!active || !target.HasMouseCapture())
                return;

            Vector2 diff = e.localMousePosition - start;

            float heightDivTwo = -target.layout.height / 2;
            float widthDivTwo = -target.layout.width / 2;

            target.style.top = Mathf.Clamp(target.layout.y + diff.y, heightDivTwo, Screen.height + heightDivTwo);
            target.style.left = Mathf.Clamp(target.layout.x + diff.x, widthDivTwo, Screen.width + widthDivTwo);

            e.StopPropagation();
        }

        #endregion



        #region OnMouseUp

        /// <summary>
        /// Releases the VisualElement currently dragged.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseUp(MouseUpEvent e)
        {
            if (!active || !target.HasMouseCapture() || !CanStopManipulation(e))
                return;

            active = false;
            target.ReleaseMouse();
            e.StopPropagation();
        }

        #endregion
    }
}