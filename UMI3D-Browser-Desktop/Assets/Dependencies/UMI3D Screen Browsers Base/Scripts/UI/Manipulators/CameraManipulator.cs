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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.ui.viewController
{
    public class CameraManipulator : AbstractDraggableManipulator
    {
        private Vector2 m_localPosition;

        /// <summary>
        /// Direction of the swipe.
        /// </summary>
        public Vector2 Direction
        {
            get
            {
                if (!m_cameraMoved) return Vector2.zero;
                m_cameraMoved = false;
                m_startPosition = m_localPosition;
                return m_direction;
            }
        }
        protected Vector2 m_direction;
        /// <summary>
        /// Id of the pointer currently captured by this manipulator.
        /// </summary>
        protected List<int> m_pointersId = new List<int>();
        protected bool m_cameraMoved;

        public CameraManipulator(VisualElement visual) :
            base(visual)
        { }

        protected override void OnMouseMove(MouseMoveEvent e)
        {}

        protected override bool IsPointerIdCompatible(int pointerId)
        {
            if (pointerId == PointerId.mousePointerId) return false;
            if (m_pointersId.Count == 0) m_pointersId.Add(pointerId);
            else return false;
            return true;
        }

        protected override bool IsPointerIdUsed(int pointerId)
            => m_pointersId.Contains(pointerId);

        protected override void ProcessMoveEvent(EventBase e, Vector2 localPosition, int pointerId)
        {
            m_localPosition = localPosition;
            m_direction = m_localPosition - m_startPosition.Value;
            m_direction.x /= m_visual.worldBound.width;
            m_direction.y /= -m_visual.worldBound.height;
            m_direction *= 50;
            m_cameraMoved = true;
        }

        protected override void ProcessUpEvent(EventBase evt, int pointerId)
        {
            base.ProcessUpEvent(evt, pointerId);
            m_direction = Vector2.zero;
            m_pointersId.Clear();
        }
    }
}
