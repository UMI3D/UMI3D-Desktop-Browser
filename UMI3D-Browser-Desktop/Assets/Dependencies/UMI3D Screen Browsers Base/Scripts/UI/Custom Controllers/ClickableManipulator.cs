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
    public class TouchManipulator2 : Clickable
    {
        /// <summary>
        /// Clicked down with eventBase and localPosition.
        /// </summary>
        public event Action<EventBase, Vector2> ClickedDownWithInfo;
        /// <summary>
        /// Clicked up with eventBase and localPosition.
        /// </summary>
        public event Action<EventBase, Vector2> ClickedUpWithInfo;
        /// <summary>
        /// Clicked long with eventBase and localPosition.
        /// </summary>
        public event Action<EventBase, Vector2> ClickedLongWithInfo;
        /// <summary>
        /// Moved with eventBase and localPosition.
        /// </summary>
        public event Action<EventBase, Vector2> MovedWithInfo;

        public event Action ClickedDown;
        public event Action ClickedUp;
        public event Action ClickedLong;
        public event Action Moved;
        public long LongPressDelay;

        protected int? m_pointerId;

        public TouchManipulator2() : this(null, 0, 0)
        {

        }

        public TouchManipulator2(Action handler, long delay, long interval) :
            base(handler, delay, interval)
        {

        }

        /// <summary>
        /// Invoke clicked down event.
        /// </summary>
        public void OnClickedDown() => ClickedDown?.Invoke();
        /// <summary>
        /// Invoke clicked up event.
        /// </summary>
        public void OnClickedUp() => ClickedUp?.Invoke();
        /// <summary>
        /// Invoke clicked long event.
        /// </summary>
        public void OnClickedLong() => ClickedLong?.Invoke();
        /// <summary>
        /// Invoke moved event.
        /// </summary>
        public void OnMoved() => Moved?.Invoke();
        /// <summary>
        /// Invoke clicked down event with information.
        /// </summary>
        public void OnClickedDownWithInf(EventBase evt, Vector2 localPosition) => ClickedDownWithInfo?.Invoke(evt, localPosition);
        /// <summary>
        /// Invoke clicked up event with information.
        /// </summary>
        public void OnClickedUpWithInf(EventBase evt, Vector2 localPosition) => ClickedUpWithInfo?.Invoke(evt, localPosition);
        /// <summary>
        /// Invoke clicked long event with information.
        /// </summary>
        public void OnClickedLongWithInf(EventBase evt, Vector2 localPosition) => ClickedLongWithInfo?.Invoke(evt, localPosition);
        /// <summary>
        /// Invoke moved event with information.
        /// </summary>
        public void OnMovedWithInf(EventBase evt, Vector2 localPosition) => MovedWithInfo?.Invoke(evt, localPosition);

        protected override void ProcessDownEvent(EventBase evt, Vector2 localPosition, int pointerId)
        {
            ClickedDown?.Invoke();
            ClickedDownWithInfo?.Invoke(evt, localPosition);
            m_pointerId = pointerId;
            base.ProcessDownEvent(evt, localPosition, pointerId);
            base.target.schedule.Execute(() => LongPressed(evt, localPosition)).StartingIn(LongPressDelay);
        }

        protected override void ProcessMoveEvent(EventBase evt, Vector2 localPosition)
        {
            Moved?.Invoke();
            MovedWithInfo?.Invoke(evt, localPosition);
            base.ProcessMoveEvent(evt, localPosition);
        }

        protected override void ProcessUpEvent(EventBase evt, Vector2 localPosition, int pointerId)
        {
            ClickedUp?.Invoke();
            ClickedUpWithInfo?.Invoke(evt, localPosition);
            base.ProcessUpEvent(evt, localPosition, pointerId);
        }

        protected override void ProcessCancelEvent(EventBase evt, int pointerId)
        {
            base.ProcessCancelEvent(evt, pointerId);
        }

        protected virtual void LongPressed(EventBase evt, Vector2 localPosition)
        {
            if (!active) return;
            ClickedLong?.Invoke();
            ClickedLongWithInfo?.Invoke(evt, localPosition);
        }
    }
}
