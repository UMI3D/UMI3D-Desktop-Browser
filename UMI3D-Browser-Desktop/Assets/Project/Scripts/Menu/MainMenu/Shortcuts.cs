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

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using System.Collections;

namespace BrowserDesktop.Menu
{
    /// <summary>
    /// This class manages the shortcut displayer 
    /// </summary>
    public class Shortcuts : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public struct Shortcut
        {
            public string ShortcutKey;
            public Sprite ShortcutIcon;
        }

        #region Fields

        #region UI

        public UIDocument uiDocument;

        [SerializeField]
        private VisualTreeAsset shortcutTreeAsset;
        [SerializeField]
        private VisualTreeAsset shortcutIconTreeAsset;

        VisualElement shortcutArea; //Where shortcut's button and labels are positions in the footer.
        VisualElement shortcutDisplayer_VE; //Where the shortcuts are displayed.
        ScrollView shortcuts_SV; //ScrollView of shortcuts
        VisualElement shortcuts_VE;
        //Button openShortcutBtn;

        #endregion

        #region Data

        [SerializeField]
        private Shortcut[] shortcuts; //Shortcuts dictionary To be replace

        [SerializeField]
        private Icons_SO shortcutsIcons; //Shortcuts Icons dictionary

        bool isDisplayed = true; //is shortcutDisplayer visible.

        //Object Pooling
        private List<ShortcutElement> shortcutsDisplayedList = new List<ShortcutElement>();
        private List<ShortcutElement> shortcutsWaitedList = new List<ShortcutElement>();
        public readonly List<ShortcutIconElement> ShortcutIconsDisplayedList = new List<ShortcutIconElement>();
        public readonly List<ShortcutIconElement> ShortcutIconsWaitedList = new List<ShortcutIconElement>();
        public readonly List<Label> ShortcutPlusLabelDisplayList = new List<Label>();
        public readonly List<Label> ShortcutPlusLabelWaitedList = new List<Label>();

        #endregion

        #endregion

        void Start()
        {
            Debug.Assert(uiDocument != null);
            Debug.Assert(shortcutsIcons != null);
            Debug.Assert(shortcutTreeAsset != null);
            Debug.Assert(shortcutIconTreeAsset != null);

            var root = uiDocument.rootVisualElement;

            shortcutDisplayer_VE = root.Q<VisualElement>("shortcut-displayer");
            shortcuts_SV = shortcutDisplayer_VE.Q<ScrollView>("shortcuts");

            DisplayShortcut(false); //Default: shortcuts are hidden.
        }

        void Update()
        {
            //To be improved.
            if (Input.GetKeyDown(KeyCode.F1))
            {
                DisplayShortcut(!isDisplayed);
            }

            //Debug
            if (Input.GetKeyDown(KeyCode.K))
            {
                String[] test = { "1" };
                AddShortcut("test", test);
                AddShortcuts();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                String[] test = { "ctrl", "1" };
                AddShortcut("test2", test);
                AddShortcuts();
            }
            /*if (Input.GetKeyDown(KeyCode.M))
            {
                String[] test0 = { "t", "s" };
                String[] test1 = { "t" };
                String[] test2 = { "t", "s", "d" };
                String[] test3 = { "t", "s" };
                AddShortcut("test", test0);
                AddShortcut("test01", test1);
                AddShortcut("test002", test2);
                AddShortcut("test0003", test3);
                //OnResizeIconsArea.Invoke();
                AddShortcuts();
            }*/
            if (Input.GetKeyDown(KeyCode.J))
            {
                String[] test = { "shift", "shift", "shift" };
                AddShortcut("test shift", test);
                AddShortcuts();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                ClearShortcut();
            }
        }

        /// <summary>
        /// Wait one frame and compute the max width of shortcuts.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ComputeShortcutsWidth()
        {
            yield return null;
            shortcutsDisplayedList.ForEach((sE) => sE.ComputeShortcutWidth());
        }

        /// <summary>
        /// Wait for ComputeShortcutsWidth() and resize shortcuts width.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ResizeShortcutsWidth()
        {
            yield return ComputeShortcutsWidth();
            shortcutsDisplayedList.ForEach((sE) => sE.ResizeShortcutWidth());
            AnimeVisualElement(shortcuts_SV, 1f, true, (elt, val) => { elt.style.opacity = val; }); //Display shortcuts when the resizement is done.
            Debug.Log("Shortcut displayer = " + shortcutDisplayer_VE.resolvedStyle.width);
        }

        #region Add and Remove Shortcuts

        public void AddShortcuts()
        {
            AnimeVisualElement(shortcuts_SV, 1f, false, (elt, val) => { elt.style.opacity = val; }); //Hide shortcuts while they are added.
            //TODO Add shortcuts
            //TODO Resize
            //OnResizeIconsArea.Invoke();
            StartCoroutine(ResizeShortcutsWidth());
        }

        /// <summary>
        /// Add a shortcut to the shortcuts displayer.
        /// </summary>
        /// <param name="shortcutName">What the shortcut do.</param>
        /// <param name="shortcutkeys">Keys to press to trigger the shortcut.</param>
        private void AddShortcut(String shortcutName, string[] shortcutkeys)
        {
            ShortcutElement shortcutElement;
            //Object Pooling for ShortcutElement.
            if (shortcutsWaitedList.Count == 0)
            {
                shortcutElement = shortcutTreeAsset.CloneTree().Q<ShortcutElement>();
            }
            else
            {
                shortcutElement = shortcutsWaitedList[shortcutsWaitedList.Count - 1];
                shortcutsWaitedList.RemoveAt(shortcutsWaitedList.Count - 1);
            }
            shortcutsDisplayedList.Add(shortcutElement);

            Sprite[] shortcutIcons = new Sprite[shortcutkeys.Length];
            for (int i = 0; i < shortcutkeys.Length; ++i)
            {
                shortcutIcons[i] = shortcutsIcons.GetSpriteFrom(shortcutkeys[i]);
                    //GetShortcutSprite(shortcutkeys[i]);
            }

            shortcutElement.Setup(shortcutName, shortcutIcons, shortcutIconTreeAsset, this);
            shortcuts_SV.Add(shortcutElement);
        }

        /// <summary>
        /// Remove all shortcuts from the shortcuts displayer.
        /// </summary>
        public void ClearShortcut()
        {
            //TO Test
            Action<VisualElement> removeVEFromHierarchy = (vE) => vE.RemoveFromHierarchy();

            ShortcutIconsDisplayedList.ForEach(removeVEFromHierarchy);
            ShortcutIconsWaitedList.AddRange(ShortcutIconsDisplayedList);
            ShortcutIconsDisplayedList.Clear();

            ShortcutPlusLabelDisplayList.ForEach(removeVEFromHierarchy);
            ShortcutPlusLabelWaitedList.AddRange(ShortcutPlusLabelDisplayList);
            ShortcutPlusLabelDisplayList.Clear();

            shortcutsDisplayedList.ForEach((sE)=> sE.RemoveShortcut(this));
            shortcutsWaitedList.AddRange(shortcutsDisplayedList);
            shortcutsDisplayedList.Clear();
        }

        #endregion

        /// <summary>
        /// Show or hide shortcuts.
        /// </summary>
        /// <param name="val">if true: show shortcuts, else hide.</param>
        private void DisplayShortcut(bool value)
        {
            isDisplayed = value;

            //Display or hide shortcutDisplayer with animation.
            AnimeVisualElement(shortcutDisplayer_VE, 300f, value, 
                                (elt, val) => { elt.style.right = 300f - val; });
        }

        /// <summary>
        /// Anime the VisualElement.
        /// </summary>
        /// <param name="vE">the VisualElement to be animated.</param>
        /// <param name="value">The animation will be trigger from 0 to this value when isShowing is true. Else, from this value to 0.</param>
        /// <param name="isShowing">The VisualElement should be displayed if true.</param>
        /// <param name="animation">The animation to be perform.</param>
        private void AnimeVisualElement(VisualElement vE, float value, bool isShowing, Action<VisualElement, float> animation)
        {
            Debug.LogWarning("Use of Unity experimental API. May not work in the future.");
            if (isShowing)
            {
                vE.experimental.animation.Start(0, value, 100, animation);
            }
            else
            {
                vE.experimental.animation.Start(value, 0, 100, animation);
            }
        }

        /// <summary>
        /// Get the sprite corresponding to the shortcut key.
        /// </summary>
        /// <param name="shortcutKey">one of the keys use in a shortcut.</param>
        /// <returns></returns>
        private Sprite GetShortcutSprite(string shortcutKey)
        {
            /*foreach (Shortcut shortcut in shortcuts)
            {
                if (shortcut.ShortcutKey == shortcutKey)
                    return shortcut.ShortcutIcon;
            }

            Debug.LogError("Shortcut key not found: this shouln't happen");
            return null;*/
            return shortcutsIcons.GetSpriteFrom(shortcutKey);
        }
    }
}
