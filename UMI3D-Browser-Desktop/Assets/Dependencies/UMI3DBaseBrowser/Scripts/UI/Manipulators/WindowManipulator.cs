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

namespace umi3d.baseBrowser.ui.viewController
{
    public class WindowManipulator : AbstractDraggableManipulator
    {
        public WindowManipulator(VisualElement window) :
            base(window)
        { }

        protected override bool IsPointerIdCompatible(int pointID)
        {
            return true;
        }

        protected override bool IsPointerIdUsed(int pointerId)
        {
            return true;
        }

        protected override void ProcessMoveEvent(EventBase e, Vector2 localPosition, int pointerId)
        {
            if (m_startPosition == null || !target.HasMouseCapture()) return;

            Vector2 diff = localPosition - m_startPosition.Value;

            float heightDivTwo = m_visual.resolvedStyle.height / 2;
            float widthDivTwo = m_visual.resolvedStyle.width / 2;

            m_visual.style.top = Mathf.Clamp(m_visual.resolvedStyle.top + diff.y, 0f, Screen.height - heightDivTwo);
            m_visual.style.left = Mathf.Clamp(m_visual.resolvedStyle.left + diff.x, -widthDivTwo, Screen.width - widthDivTwo);
        }
    }
}
