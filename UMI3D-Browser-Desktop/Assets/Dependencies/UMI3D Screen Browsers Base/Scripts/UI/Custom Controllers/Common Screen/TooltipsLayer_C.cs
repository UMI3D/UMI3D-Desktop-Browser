/*
Copyright 2019 - 2023 Inetum

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
using System.Collections;
using System.Collections.Generic;
using umi3d.commonScreen.Displayer;
using umi3d.commonScreen.game;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen
{
    public class TooltipsLayer_C : BaseVisual_C
    {
        public static TooltipsLayer_C Instance
        {
            get
            {
                if (s_instance != null) return s_instance;

                s_instance = new TooltipsLayer_C();
                return s_instance;
            }
            private set => s_instance = value;
        }
        protected static TooltipsLayer_C s_instance;

        public static string Text
        {
            get => Tooltip_Text.text;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Tooltip_Text.RemoveFromHierarchy();
                    Instance.style.display = DisplayStyle.None;
                }
                else
                {
                    Instance.Add(Tooltip_Text);
                    Instance.style.display = DisplayStyle.Flex;
                }
                Tooltip_Text.LocalisedText = value;
            }
        }

        public static Text_C Tooltip_Text = new Text_C { name = "tooltip" };

        protected override void AttachStyleSheet()
        {
        }
        protected override void AttachUssClass()
        {
        }

        protected override void InitElement()
        {
            base.InitElement();
            style.position = Position.Absolute;
            style.top = 0;
            style.left = 0;
            style.width = 0;
            style.height = 0;
            style.flexShrink = 1;
            style.display = DisplayStyle.None;

            Tooltip_Text.style.position = Position.Absolute;
            Tooltip_Text.Color = TextColor.Menu;
            Tooltip_Text.style.backgroundColor = Color.black;

            this.RemoveManipulator(TooltipManipulator);
        }

        #region Implementation

        public static void SetTextPosition(VisualElement target)
        {
            Tooltip_Text.style.left = target.worldBound.center.x;
            Tooltip_Text.style.top = target.worldBound.yMin;
        }

        #endregion
    }
}
