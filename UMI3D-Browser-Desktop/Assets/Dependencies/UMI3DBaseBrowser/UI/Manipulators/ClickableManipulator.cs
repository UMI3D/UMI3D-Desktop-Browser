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
using static umi3d.baseBrowser.ui.viewController.VisualManipulator;

namespace umi3d.baseBrowser.ui.viewController
{
    public class ClickableManipulator : Clickable
    {
        public event Action ClickedDown;
        public event Action ClickedUp;
        public event Action<EventBase, MousePressedState> MouseBehaviourChanged;

        public ClickableManipulator() :
            base((Action)null)
        { }
        public ClickableManipulator(Action<EventBase> handler)
            : base(handler)
        { }
        public ClickableManipulator(Action handler)
            : base(handler)
        { }
        public ClickableManipulator(Action handler, long delay, long interval)
            : base(handler, delay, interval)
        { }

        protected override void ProcessDownEvent(EventBase evt, Vector2 localPosition, int pointerId)
        {
            base.ProcessDownEvent(evt, localPosition, pointerId);
            ClickedDown?.Invoke();
            MouseBehaviourChanged?.Invoke(evt, MousePressedState.Pressed);
        }

        protected override void ProcessUpEvent(EventBase evt, Vector2 localPosition, int pointerId)
        {
            base.ProcessUpEvent(evt, localPosition, pointerId);
            ClickedUp?.Invoke();
            MouseBehaviourChanged?.Invoke(evt, MousePressedState.Unpressed);
        }
    }
}
