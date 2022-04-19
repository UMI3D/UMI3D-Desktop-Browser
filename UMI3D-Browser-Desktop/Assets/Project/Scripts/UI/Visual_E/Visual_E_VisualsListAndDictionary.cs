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

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class Visual_E
    {
        /// <summary>
        /// List of all the VisualElements that this Visual_E manage.
        /// </summary>
        protected List<VisualElement> m_visuals;
        protected Dictionary<VisualElement, (CustomStyle_SO, StyleKeys, VisualManipulator)> m_visualStyles;

        public VisualManipulator GetVisualManipulator(VisualElement visual)
            => m_visualStyles[visual].Item3;

        /// <summary>
        /// Update the manipulator of the root visualElement.
        /// </summary>
        /// <param name="newManipulator"></param>
        public void UpdateVisualManipulator(VisualManipulator newManipulator)
            => UpdateManipulator(Root, newManipulator);

        protected void AddVisualStyle(VisualElement visual, string styleResourcePath, StyleKeys formatAndStyleKeys, VisualManipulator manipulator = null, bool stopPropagation = true)
            => AddVisualStyle(visual, GetStyleSO(styleResourcePath), formatAndStyleKeys, manipulator, stopPropagation);
        protected void AddVisualStyle(VisualElement visual, CustomStyle_SO style_SO, StyleKeys keys, VisualManipulator manipulator = null, bool stopPropagation = true)
        {
            if (visual == null) throw new NullReferenceException("visual is null");

            if (!m_visuals.Contains(visual))
            {
                m_visuals.Add(visual);

                if (manipulator == null)
                    manipulator = new VisualManipulator(stopPropagation);
                
                SetManipulator(manipulator, style_SO, keys);
                visual.AddManipulator(manipulator);

                m_visualStyles.Add(visual, (style_SO, keys, manipulator));
                manipulator.ApplyFormatAndStyle();
            }
            else
                UpdateStyleAndKeysAndManipulator(visual, style_SO, keys, manipulator);
        }

        protected void UpdateStyleAndKeysAndManipulator(VisualElement visual, CustomStyle_SO style_SO, StyleKeys keys, VisualManipulator manipulator = null)
        {
            if (!m_visuals.Contains(visual)) throw new Exception($"Visual unknown [{visual}] wanted to be updated.");

            var (_, _, oldManipulator) = m_visualStyles[visual];
            if (manipulator == null)
            {
                manipulator = oldManipulator;
                manipulator.Set(style_SO, keys);
            }
            else
            {
                oldManipulator.Reset();
                SetManipulator(manipulator, style_SO, keys);
            }
            m_visualStyles[visual] = (style_SO, keys, manipulator);
            manipulator.ApplyFormatAndStyle();
        }

        /// <summary>
        /// Update Visual Style.
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="styleResourcePath"></param>
        protected void UpdateStyle(VisualElement visual, string styleResourcePath)
            => UpdateStyle(visual, GetStyleSO(styleResourcePath));
        /// <summary>
        /// Update Visual Style.
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="style_SO"></param>
        protected void UpdateStyle(VisualElement visual, CustomStyle_SO style_SO)
        {
            if (!m_visuals.Contains(visual)) throw new Exception($"Visual unknown [{visual}] wanted to be updated.");
            var (_, keys, manipulator) = m_visualStyles[visual];
            manipulator.UpdateStyle(style_SO);
            m_visualStyles[visual] = (style_SO, keys, manipulator);
        }

        /// <summary>
        /// Update visual style keys.
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="newKeys"></param>
        protected void UpdateKeys(VisualElement visual, StyleKeys newKeys)
        {
            if (!m_visuals.Contains(visual)) throw new Exception($"Visual unknown [{visual}] wanted to be updated.");
            var (styleSO, _, manipulator) = m_visualStyles[visual];
            manipulator.UpdateKeys(newKeys);
            m_visualStyles[visual] = (styleSO, newKeys, manipulator);
        }

        /// <summary>
        /// Remove current manipulator and add [newManipulator]
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="newManipulator"></param>
        protected void UpdateManipulator(VisualElement visual, VisualManipulator newManipulator)
        {
            if (!m_visuals.Contains(visual)) throw new Exception($"Visual unknown [{visual}] wanted to be updated.");
            if (newManipulator == null) throw new NullReferenceException("Manipulator null");
            var (styleSO, keys, oldManipulator) = m_visualStyles[visual];
            visual.RemoveManipulator(oldManipulator);
            SetManipulator(newManipulator, styleSO, keys);
            visual.AddManipulator(newManipulator);
            m_visualStyles[visual] = (styleSO, keys, newManipulator);
        }

        protected void SetManipulator(VisualManipulator manipulator, CustomStyle_SO style_SO, StyleKeys keys)
        {
            manipulator.Set(style_SO, keys);
            manipulator.ApplyingFormat += ApplyFormat;
            manipulator.ApplyingStyle += ApplyStyle;
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