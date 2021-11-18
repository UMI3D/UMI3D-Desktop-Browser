/*
Copyright 2019 Gfi Informatique

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

using BrowserDesktop.Menu;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace BrowserDesktop.UI.GenericElement
{
    public class ShortcutGenericElement : GenericAndCustomElement
    {
        #region Fields

        public new class UxmlFactory : UxmlFactory<ShortcutGenericElement, UxmlTraits> { }

        VisualElement iconsArea_VE; //Where icons are displays.
        Label shortcutName_L; //the shortcut's name.

        private string shortcutNameText;

        private static int shortcutsCount = 0;
        /// <summary>
        /// The number of shortcuts.
        /// </summary>
        public static int ShortcutsCount
        {
            get => shortcutsCount;
            private set
            {
                if (shortcutsCount - value == 1 || value - shortcutsCount == 1) //the equivalent of --ShortcutsCount or ++Shortcut
                {
                    shortcutsCount = value;
                }

                if (shortcutsCount == 0)
                    IconsAreaWidth = 0;
            }
        }

        private static float iconsAreaWidth = 0;
        /// <summary>
        /// The maximum width in px of all the icons area.
        /// </summary>
        public static float IconsAreaWidth
        {
            get => iconsAreaWidth;
            private set
            {
                if (value == 0) //Should only happen when there is no shortcup to display.
                {
                    iconsAreaWidth = 0;
                }
                else if (iconsAreaWidth < value) //The icons area width has increased.
                {
                    iconsAreaWidth = value;
                }
            }
        }

        private static float shortcutNameWidth = 0;
        /// <summary>
        /// The maximum width in px of all the shortcut's name label.
        /// </summary>
        public static float ShortcutNameWidth
        {
            get => shortcutNameWidth;
            private set
            {
                if (value == 0) //Should only happen when there is no shortcup to display.
                {
                    shortcutNameWidth = 0;
                }
                else if (shortcutNameWidth < value) //The shortcut name width has increased.
                {
                    shortcutNameWidth = value;
                }
            }
        }

        /// <summary>
        /// The size of the icons area and the shortut's name label.
        /// </summary>
        //private float maxIconsAreaWidth = 151;

        #endregion

        protected override void Initialize()
        {
            base.Initialize();

            shortcutName_L = this.Q<Label>("shortcut-name");
            iconsArea_VE = this.Q<VisualElement>("Icons-layer");
        }

        /// <summary>
        /// Set the icons and name of this shortcut.
        /// </summary>
        /// <param name="shortcutName">The name of this shortcut.</param>
        /// <param name="shortcutIcons">The icons (sprite) of this shortcut.</param>
        /// <param name="shortcutIcon_VTA">The template of shortcut icon.</param>
        /// <param name="shortcutsClass">The instance of the Shortcuts class.</param>
        public ShortcutGenericElement Setup(string shortcutName, Sprite[] shortcutIcons, VisualTreeAsset shortcutIcon_VTA, VisualTreeAsset label_VTA)
        {
            Initialize();

            ++ShortcutsCount;

            shortcutNameText = shortcutName;

            for (int i = 0; i < shortcutIcons.Length; ++i)
            {
                if (i != 0 && i < shortcutIcons.Length)
                {
                    Label_GE plus;
                    Shortcuts.ObjectPooling(out plus, Shortcuts.ShortcutPlusLabelDisplayList, Shortcuts.ShortcutPlusLabelWaitedList, label_VTA);
                    plus.
                        Setup("+", "label").
                        AddTo(iconsArea_VE);

                    /*plus.AddToClassList("label-shortcut");
                    plus.AddToClassList("label-shortcut-plus");*/
                }

                ShortcutIcon_GE icon_GE;
                Shortcuts.ObjectPooling(out icon_GE, Shortcuts.ShortcutIconsDisplayedList, Shortcuts.ShortcutIconsWaitedList, shortcutIcon_VTA);
                icon_GE.
                    Setup(shortcutIcons[i]).
                    AddTo(iconsArea_VE);
            }

            //OnApplyUserPreferences();

            return this;
        }

        public void ComputeShortcutWidth()
        {
            IconsAreaWidth = iconsArea_VE.resolvedStyle.width;
            //ShortcutNameWidth = shortcutName_L.resolvedStyle.width;

            Debug.Log($"[{shortcutNameText}] icons max width = {IconsAreaWidth}, Icons layer width = {iconsArea_VE.resolvedStyle.width}");
        }

        public void ResizeShortcutWidth()
        {
            iconsArea_VE.style.width = IconsAreaWidth;
            //shortcutName_L.style.width = ShortcutNameWidth;

            //Debug.Log("resizement");
        }

        
        public override void Remove()
        {
            base.Remove();

            --ShortcutsCount;

            iconsArea_VE.style.width = StyleKeyword.Auto; //Unset the icons area width.
            shortcutName_L.style.width = StyleKeyword.Auto; //Unset the shortcut name width.
        }

        public override void OnApplyUserPreferences()
        {
            if (!displayed) return;

            UserPreferences.UserPreferences.TextAndIconPref.ApplyTextPref(shortcutName_L, "label", shortcutNameText);
        }
    }
}