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
        protected Dictionary<VisualElement, (CustomStyle_SO, StyleKeys, VisualManipulator)> m_visualStyles;

        protected void AddVisualStyle(VisualElement visual, string styleResourcePath, StyleKeys formatAndStyleKeys, bool stopPropagation = true)
            => AddVisualStyle(visual, GetStyleSO(styleResourcePath), formatAndStyleKeys, stopPropagation);
        protected void AddVisualStyle(VisualElement visual, CustomStyle_SO style_SO, StyleKeys keys, bool stopPropagation = true, params VisualElement[] otherVisualTriggers)
        {
            if (visual == null) throw new NullReferenceException("visual is null");

            if (m_visuals.Contains(visual))
            {
                var (oldStyle, oldKeys, manipulator) = m_visualStyles[visual];
                manipulator.Set(style_SO, keys);

                m_visualStyles[visual] = (style_SO, keys, manipulator);
                manipulator.AppliesFormatAndStyle();
            }
            else
            {
                m_visuals.Add(visual);

                var manipulator = new VisualManipulator(style_SO, keys, stopPropagation, ApplyFormat, ApplyStyle);
                visual.AddManipulator(manipulator);

                m_visualStyles.Add(visual, (style_SO, keys, manipulator));
                manipulator.AppliesFormatAndStyle();
            }
        }

        protected void UpdateVisualKeys(VisualElement visual, StyleKeys newKeys)
        {
            if (!m_visuals.Contains(visual)) throw new Exception($"Visual unknown [{visual}] wanted to be updated.");
            if (newKeys == null) throw new NullReferenceException("FormatAnStyleKeys is null.");
            var (_, keys, manipulator) = m_visualStyles[visual];
            keys.Text = newKeys.Text;
            keys.TextStyleKey = newKeys.TextStyleKey;
            keys.BackgroundStyleKey = newKeys.BackgroundStyleKey;
            keys.BorderStyleKey = newKeys.BorderStyleKey;
            manipulator.AppliesFormatAndStyle();
        }

        protected void UpdateVisualManipulator(VisualElement visual, VisualManipulator newManipulator)
        {
            if (!m_visuals.Contains(visual)) throw new Exception($"Visual unknown [{visual}] wanted to be updated.");
            if (newManipulator == null) throw new NullReferenceException("Manipulator null");
            var (styleSO, keys, oldManipulator) = m_visualStyles[visual];
            visual.RemoveManipulator(oldManipulator);
            newManipulator.Set(styleSO, keys, ApplyFormat, ApplyStyle);
            visual.AddManipulator(newManipulator);
            m_visualStyles[visual] = (styleSO, keys, newManipulator);
        }

        protected void ResetAllVisualStyle()
        {
            foreach (VisualElement visual in m_visuals)
            {
                var (_, _, manipulator) = m_visualStyles[visual];
                visual.RemoveManipulator(manipulator);
            }
            m_visuals.Clear();
            m_visualStyles.Clear();
        }
    }
}