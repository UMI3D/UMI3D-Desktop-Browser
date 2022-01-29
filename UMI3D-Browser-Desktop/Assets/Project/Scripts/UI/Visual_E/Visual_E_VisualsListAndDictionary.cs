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
using System.Collections.Generic;
using umi3DBrowser.UICustomStyle;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.uI.viewController
{
    public partial class Visual_E
    {
        protected List<VisualElement> m_visuals;
        protected Dictionary<VisualElement, (CustomStyle_SO, StyleKeys, UnityAction, EventCallback<MouseOverEvent>, EventCallback<MouseOutEvent>, EventCallback<MouseCaptureEvent>, EventCallback<MouseUpEvent>)> m_visualStyles;

        protected void AddVisualStyle(VisualElement visual, string styleResourcePath, StyleKeys formatAndStyleKeys, bool stopPropagation = true)
            => AddVisualStyle(visual, GetStyleSO(styleResourcePath), formatAndStyleKeys, stopPropagation);
        protected void AddVisualStyle(VisualElement visual, CustomStyle_SO style_SO, StyleKeys formatAndStyleKeys, bool stopPropagation = true)
        {
            if (m_visuals.Contains(visual)) return;
            m_visuals.Add(visual);
            EventCallback<MouseOverEvent> mouseOver = (e) =>
            {
                OnMouseOver(e, style_SO, formatAndStyleKeys, visual.style, stopPropagation);
            };
            EventCallback<MouseOutEvent> mouseOut = (e) =>
            {
                OnMouseOut(e, style_SO, formatAndStyleKeys, visual.style, stopPropagation);
            };
            EventCallback<MouseCaptureEvent> mouseDown = (e) =>
            {
                OnMouseDown(e, style_SO, formatAndStyleKeys, visual.style, stopPropagation);
            };
            EventCallback<MouseUpEvent> mouseUp = (e) =>
            {
                OnMouseUp(e, style_SO, formatAndStyleKeys, visual.style, stopPropagation);
            };
            visual.RegisterCallback(mouseOver);
            visual.RegisterCallback(mouseOut);
            visual.RegisterCallback(mouseDown);
            visual.RegisterCallback(mouseUp);
            if (style_SO != null)
            {
                UnityAction ApplyFormatAndStyleAction = () =>
                {
                    ApplyFormatAndStyle(style_SO, formatAndStyleKeys, visual.style, m_mouseBehaviourFromState);
                };
                style_SO.AppliesFormatAndStyle.AddListener(ApplyFormatAndStyleAction);
                m_visualStyles.Add(visual, (style_SO, formatAndStyleKeys, ApplyFormatAndStyleAction, mouseOver, mouseOut, mouseDown, mouseUp));
                ApplyFormatAndStyleAction.Invoke();
            }
            else
            {
                m_visualStyles.Add(visual, (null, null, null, mouseOver, mouseOut, mouseDown, mouseUp));
            }

        }

        protected void UpdateVisualStyle(VisualElement visual, StyleKeys newFormatAndStyleKeys)
        {
            if (!m_visuals.Contains(visual)) throw new Exception($"Visual unknown [{visual}] wanted to be updated.");
            if (newFormatAndStyleKeys == null) throw new NullReferenceException("FormatAnStyleKeys is null.");
            var (style_SO, formatAndStyleKeys, _, _, _, _, _) = m_visualStyles[visual];
            formatAndStyleKeys.Text = newFormatAndStyleKeys.Text;
            formatAndStyleKeys.TextStyleKey = newFormatAndStyleKeys.TextStyleKey;
            formatAndStyleKeys.BackgroundStyleKey = newFormatAndStyleKeys.BackgroundStyleKey;
            formatAndStyleKeys.BorderStyleKey = newFormatAndStyleKeys.BorderStyleKey;
            ApplyFormatAndStyle(style_SO, formatAndStyleKeys, visual.style, m_mouseBehaviourFromState);
        }

        protected void ResetAllVisualStyle()
        {
            foreach (VisualElement visual in m_visuals)
            {
                var (style, _, ApplyFormatAndStyleAction, mouseOver, mouseOut, mouseDown, mouseUp) = m_visualStyles[visual];
                style.AppliesFormatAndStyle.RemoveListener(ApplyFormatAndStyleAction);
                visual.UnregisterCallback(mouseOver);
                visual.UnregisterCallback(mouseOut);
                visual.UnregisterCallback(mouseDown);
                visual.UnregisterCallback(mouseUp);
            }
            m_visuals.Clear();
            m_visualStyles.Clear();
        }
    }
}