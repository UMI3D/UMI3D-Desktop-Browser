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
using umi3DBrowser.UICustomStyle;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.ui.viewController
{
    public partial class View_E
    {
        #region Public methods

        #region Manage Root format and style

        /// <summary>
        /// Return the VisualManipulator of the Root.
        /// </summary>
        /// <param name="visual"></param>
        /// <returns></returns>
        public VisualManipulator GetRootManipulator()
            => GetVisualManipulator(Root);
        /// <summary>
        /// Update the customStyle, the style keys and the manipulator of the Root.
        /// </summary>
        /// <param name="newStyleResourcePath"></param>
        /// <param name="keys"></param>
        /// <param name="manipulator"></param>
        public void UpdateRootStyleAndKeysAndManipulator(string newStyleResourcePath, StyleKeys keys, VisualManipulator manipulator = null)
            => UpdateStyleAndKeysAndManipulator(Root, newStyleResourcePath, keys, manipulator);
        /// <summary>
        /// Update the customStyle, the style keys and the manipulator of the Root.
        /// </summary>
        /// <param name="style_SO"></param>
        /// <param name="keys"></param>
        /// <param name="manipulator"></param>
        public void UpdateRootStyleAndKeysAndManipulator(CustomStyle_SO style_SO, StyleKeys keys, VisualManipulator manipulator = null)
            => UpdateStyleAndKeysAndManipulator(Root, style_SO, keys, manipulator);
        /// <summary>
        /// Update the VisualManipulator of the root.
        /// </summary>
        /// <param name="newManipulator"></param>
        public void UpdateRootManipulator(VisualManipulator newManipulator)
            => UpdateManipulator(Root, newManipulator);
        /// <summary>
        /// Update the cutomStyle of the root.
        /// </summary>
        /// <param name="newStyleResourcePath"></param>
        public void UpdateRootStyle(string newStyleResourcePath)
            => UpdateStyle(Root, newStyleResourcePath);
        /// <summary>
        /// Update the cutomStyle of the root.
        /// </summary>
        /// <param name="newStyle_SO"></param>
        public void UpdateRootStyle(CustomStyle_SO newStyle_SO)
            => UpdateStyle(Root, newStyle_SO);
        /// <summary>
        /// Update the style keys of the root.
        /// </summary>
        /// <param name="newKeys"></param>
        public void UpdateRootKeys(StyleKeys newKeys)
            => UpdateKeys(Root, newKeys);

        #endregion

        /// <summary>
        /// Return the VisualManipulator of the [visual].
        /// </summary>
        /// <param name="visual"></param>
        /// <returns></returns>
        public VisualManipulator GetVisualManipulator(VisualElement visual)
            => m_visualMap[visual];
        /// <summary>
        /// Return the CustomStyle_SO of the [visual].
        /// </summary>
        /// <param name="visual"></param>
        /// <returns></returns>
        public CustomStyle_SO GetVisualStyle(VisualElement visual)
            => m_visualMap[visual].StyleSO;
        /// <summary>
        /// Return the StyleKeys of the [visual].
        /// </summary>
        /// <param name="visual"></param>
        /// <returns></returns>
        public StyleKeys GetVisualKeys(VisualElement visual)
            => m_visualMap[visual].Keys;
        /// <summary>
        /// Link the mouse behaviour style changement from [sender] to [receiver].
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="receiver"></param>
        public static void LinkMouseBehaviourChanged(View_E sender, View_E receiver)
        {
            var senderManipulator = sender.GetRootManipulator();
            var receiverManipulator = receiver.GetRootManipulator();
            senderManipulator.MouseBehaviourChanged += receiverManipulator.ApplyStyle;
        }

        #endregion

        #region Protected Methods

        #region Add and Reset Visual Style

        protected void AddVisualStyle(VisualElement visual, string styleResourcePath, StyleKeys formatAndStyleKeys, VisualManipulator manipulator = null, bool processDuringBubbleUp = false)
            => AddVisualStyle(visual, GetStyleSO(styleResourcePath), formatAndStyleKeys, manipulator, processDuringBubbleUp);
        protected void AddVisualStyle(VisualElement visual, CustomStyle_SO style_SO, StyleKeys keys, VisualManipulator manipulator = null, bool processDuringBubbleUp = false)
        {
            if (visual == null) throw new NullReferenceException("visual is null");

            if (!m_visualMap.ContainsKey(visual))
            {
                if (manipulator == null)
                    manipulator = new VisualManipulator(processDuringBubbleUp);

                SetManipulator(manipulator, style_SO, keys);
                visual.AddManipulator(manipulator);
                m_visualMap.Add(visual, manipulator);
                manipulator.ApplyFormatAndStyle();
            }
            else
                UpdateStyleAndKeysAndManipulator(visual, style_SO, keys, manipulator);
        }
        protected void ResetAllVisualStyle()
        {
            foreach (VisualElement visual in m_visualMap.Keys)
            {
                var manipulator = m_visualMap[visual];
                visual.RemoveManipulator(manipulator);
                manipulator.Reset();
            }
            m_visualMap.Clear();
        }

        #endregion

        #region Update

        /// <summary>
        /// Update the customStyle, the style keys and the manipulator of the [visual]
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="styleResourcePath"></param>
        /// <param name="keys"></param>
        /// <param name="manipulator"></param>
        protected void UpdateStyleAndKeysAndManipulator(VisualElement visual, string styleResourcePath, StyleKeys keys, VisualManipulator manipulator = null)
            => UpdateStyleAndKeysAndManipulator(visual, GetStyleSO(styleResourcePath), keys, manipulator);
        /// <summary>
        /// Update the customStyle, the style keys and the manipulator of the [visual]
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="style_SO"></param>
        /// <param name="keys"></param>
        /// <param name="manipulator"></param>
        protected void UpdateStyleAndKeysAndManipulator(VisualElement visual, CustomStyle_SO style_SO, StyleKeys keys, VisualManipulator manipulator = null)
        {
            if (!m_visualMap.ContainsKey(visual)) 
                throw new Exception($"Visual [{visual}] unknown wanted to be updated.");

            var oldManipulator = m_visualMap[visual];
            if (manipulator == null)
            {
                manipulator = oldManipulator;
                manipulator.Set(style_SO, keys);
            }
            else
            {
                visual.RemoveManipulator(oldManipulator);
                SetManipulator(manipulator, style_SO, keys);
                visual.AddManipulator(manipulator);
            }
            m_visualMap[visual] = manipulator;
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
            if (!m_visualMap.ContainsKey(visual))
                throw new Exception($"Visual [{visual}] unknown wanted to be updated.");
            var manipulator = m_visualMap[visual];
            manipulator.UpdateStyle(style_SO);
        }

        /// <summary>
        /// Update visual style keys.
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="newKeys"></param>
        protected void UpdateKeys(VisualElement visual, StyleKeys newKeys)
        {
            if (!m_visualMap.ContainsKey(visual))
                throw new Exception($"Visual [{visual}] unknown wanted to be updated.");
            var manipulator = m_visualMap[visual];
            manipulator.UpdateKeys(newKeys);
        }

        /// <summary>
        /// Remove current manipulator and add [newManipulator]
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="newManipulator"></param>
        protected void UpdateManipulator(VisualElement visual, VisualManipulator newManipulator)
        {
            if (!m_visualMap.ContainsKey(visual))
                throw new Exception($"Visual [{visual}] unknown wanted to be updated.");
            if (newManipulator == null) 
                throw new NullReferenceException("Manipulator null");
            var oldManipulator = m_visualMap[visual];
            SetManipulator(newManipulator, oldManipulator.StyleSO, oldManipulator.Keys);
            visual.RemoveManipulator(oldManipulator);
            visual.AddManipulator(newManipulator);
            m_visualMap[visual] = newManipulator;
        }

        #endregion

        protected void SetManipulator(VisualManipulator manipulator, CustomStyle_SO style_SO, StyleKeys keys)
        {
            manipulator.Set(style_SO, keys);
            manipulator.ApplyingFormat += ApplyFormat;
            manipulator.ApplyingStyle += ApplyStyle;
        }

        #endregion
    }
}