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
using umi3DBrowser.UICustomStyle;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.uI.viewController
{
    public partial class Visual_E
    {
        //protected enum MousePressedState
        //{
        //    Unpressed,
        //    Pressed
        //}
        //protected enum MousePositionState
        //{
        //    Out,
        //    Over
        //}

        //protected (MousePressedState, MousePositionState) m_mouseState { get; set; }
        //protected MouseBehaviour m_mouseBehaviourFromState
        //{
        //    get
        //    {
        //        return m_mouseState switch
        //        {
        //            (MousePressedState.Unpressed, MousePositionState.Out) => MouseBehaviour.MouseOut,
        //            (MousePressedState.Unpressed, MousePositionState.Over) => MouseBehaviour.MouseOver,
        //            _ => MouseBehaviour.MousePressed
        //        };
        //    }
        //}

        //protected virtual void OnMouseOver(MouseOverEvent e, CustomStyle_SO style_SO, StyleKeys formatAndStyleKeys, IStyle style, bool stopPropagation)
        //    => MouseBehaviourChanged(e, (m_mouseState.Item1, MousePositionState.Over), style_SO, formatAndStyleKeys, style, stopPropagation);
        //protected virtual void OnMouseOut(MouseOutEvent e, CustomStyle_SO style_SO, StyleKeys formatAndStyleKeys, IStyle style, bool stopPropagation)
        //    => MouseBehaviourChanged(e, (m_mouseState.Item1, MousePositionState.Out), style_SO, formatAndStyleKeys, style, stopPropagation);
        //protected virtual void OnMouseDown(MouseCaptureEvent e, CustomStyle_SO style_SO, StyleKeys formatAndStyleKeys, IStyle style, bool stopPropagation)
        //    => MouseBehaviourChanged(e, (MousePressedState.Pressed, m_mouseState.Item2), style_SO, formatAndStyleKeys, style, stopPropagation);
        //protected virtual void OnMouseUp(MouseUpEvent e, CustomStyle_SO style_SO, StyleKeys formatAndStyleKeys, IStyle style, bool stopPropagation)
        //{
        //    if (e.button != 0) return;
        //    MouseBehaviourChanged(e, (MousePressedState.Unpressed, m_mouseState.Item2), style_SO, formatAndStyleKeys, style, stopPropagation);
        //}
        //protected void MouseBehaviourChanged(EventBase e, (MousePressedState, MousePositionState) mouseState, CustomStyle_SO style_SO, StyleKeys formatAndStyleKeys, IStyle style, bool stopPropagation)
        //{
        //    m_mouseState = mouseState;
        //    ApplyStyle(style_SO, formatAndStyleKeys, style, m_mouseBehaviourFromState);
        //    if (stopPropagation) e.StopPropagation();
        //}
    }
}