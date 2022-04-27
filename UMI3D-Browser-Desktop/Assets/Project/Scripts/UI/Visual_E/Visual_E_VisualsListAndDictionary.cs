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