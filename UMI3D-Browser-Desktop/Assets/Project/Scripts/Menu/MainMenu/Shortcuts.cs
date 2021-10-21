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
        VisualElement shortcutDisplayer; //Where the shortcuts are displayed.
        ScrollView shortcutsScrollView; //ListView of shortcuts
        Button openShortcutBtn;

        #endregion

        [SerializeField]
        private Shortcut[] shortcuts; //Shortcuts dictionary

        bool isDisplayed = true; //is shortcutDisplayer visible.

        private List<ShortcutElement> shortcutsDisplayedList = new List<ShortcutElement>();
        private List<ShortcutElement> shortcutsWaitedList = new List<ShortcutElement>();
        public readonly List<ShortcutIconElement> ShortcutIconsDisplayedList = new List<ShortcutIconElement>();
        public readonly List<ShortcutIconElement> ShortcutIconsWaitedList = new List<ShortcutIconElement>();
        public readonly List<Label> ShortcutPlusLabelDisplayList = new List<Label>();
        public readonly List<Label> ShortcutPlusLabelWaitedList = new List<Label>();
        
        //public UnityEvent<Shortcuts> OnClearShortcut = new UnityEvent<Shortcuts>();
        //public UnityEvent OnResizeIconsArea = new UnityEvent();

        #endregion

        void Start()
        {
            Debug.Assert(uiDocument != null);
            Debug.Assert(shortcutTreeAsset != null);
            Debug.Assert(shortcutIconTreeAsset != null);

            var root = uiDocument.rootVisualElement;

            openShortcutBtn = root.Q<Button>("open-shortcuts-button");
            openShortcutBtn.clickable.clicked += ()=> DisplayShortcut(!isDisplayed);
            openShortcutBtn.AddToClassList("btn-shortcut");

            shortcutDisplayer = root.Q<VisualElement>("shortcut-displayer");
            shortcutsScrollView = shortcutDisplayer.Q<ScrollView>("shortcuts");

            DisplayShortcut(false); //Default: shortcuts are hidden.
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                String[] test = { "t" };
                AddShortcut("test", test);
                //OnResizeIconsArea.Invoke();
                AddShortcuts();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                String[] test = { "t", "s" };
                AddShortcut("test2", test);
                //OnResizeIconsArea.Invoke();
                AddShortcuts();
            }
            if (Input.GetKeyDown(KeyCode.M))
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
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                ClearShortcut();
            }
        }

        private IEnumerator GetIconsAreaWidth()
        {
            yield return null;
            shortcutsDisplayedList.ForEach((sE) => sE.GetIconsAreaWidth());
        }

        private IEnumerator ResizeIconsArea()
        {
            yield return GetIconsAreaWidth();
            shortcutsDisplayedList.ForEach((sE) => sE.ResizeIconsArea());
        }


        public void AddShortcuts()
        {
            //TODO Add shortcuts
            //TODO Resize
            //OnResizeIconsArea.Invoke();
            StartCoroutine(ResizeIconsArea());
        }

        /// <summary>
        /// Add a shortcut to the shortcuts displayer.
        /// </summary>
        /// <param name="shortcutName">What the shortcut do.</param>
        /// <param name="shortcutkeys">Keys to press to trigger the shortcut.</param>
        private void AddShortcut(String shortcutName, string[] shortcutkeys)
        {
            ShortcutElement shortcutElement;
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

            //TODO increase the height of the shortcutDisplayer.

            Sprite[] shortcutIcons = new Sprite[shortcutkeys.Length];
            for (int i = 0; i < shortcutkeys.Length; ++i)
            {
                shortcutIcons[i] = GetShortcutSprite(shortcutkeys[i]);
            }

            shortcutElement.Setup(shortcutName, shortcutIcons, shortcutIconTreeAsset, this);
            shortcutsScrollView.Add(shortcutElement);
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

        /// <summary>
        /// Show or hide shortcuts.
        /// </summary>
        /// <param name="val">if true: show shortcuts, else hide.</param>
        private void DisplayShortcut(bool val)
        {
            isDisplayed = val;

            //TODO display or hide shortcutDisplayer
        }

        /// <summary>
        /// Get the sprite corresponding to the shortcut key.
        /// </summary>
        /// <param name="shortcutKey">one of the keys use in a shortcut.</param>
        /// <returns></returns>
        private Sprite GetShortcutSprite(string shortcutKey)
        {
            foreach (Shortcut shortcut in shortcuts)
            {
                if (shortcut.ShortcutKey == shortcutKey)
                    return shortcut.ShortcutIcon;
            }

            Debug.LogError("Shortcut key not found: this shouln't happen");
            return null;
        }
    }
}
