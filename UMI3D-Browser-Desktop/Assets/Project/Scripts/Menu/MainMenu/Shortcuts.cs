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

        VisualElement shortcutArea; //Where shortcut's button and labels are positions in the footer.
        VisualElement shortcutDisplayer; //Where the shortcuts are displayed.
        ListView shortcutsListView; //ListView of shortcuts
        Button openShortcutBtn;

        #endregion

        [SerializeField]
        private String pathToShortcutsIcons;

        [SerializeField]
        private Shortcut[] shortcuts; //Shortcuts dictionary

        bool isDisplayed = true; //is shortcutDisplayer visible.
        
        public static UnityEvent OnClearShortcut = new UnityEvent();

        #endregion

        void Start()
        {
            Debug.Assert(uiDocument != null);
            Debug.Assert(shortcutTreeAsset != null);

            var root = uiDocument.rootVisualElement;

            openShortcutBtn = root.Q<Button>("open-shortcuts-button");
            openShortcutBtn.clickable.clicked += ()=> DisplayShortcut(!isDisplayed);
            openShortcutBtn.AddToClassList("btn-shortcut");

            shortcutDisplayer = root.Q<VisualElement>("shortcut-displayer");
            shortcutsListView = shortcutDisplayer.Q<ListView>("shortcuts");

            DisplayShortcut(false); //Default: shortcuts are hidden.
        }

        void Update()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shortcutName"></param>
        /// <param name="shortcutskeys"></param>
        public void AddShortcut(String shortcutName, string[] shortcutskeys)
        {
            var shortcutElement = shortcutTreeAsset.CloneTree().Q<ShortcutElement>();
            OnClearShortcut.AddListener(shortcutElement.RemoveShortcut);

            Dictionary<String, String> shortcutsToAdd = new Dictionary<string, string>();
            foreach (String shortcutKey in shortcutskeys)
            {
                shortcutsToAdd.Add(shortcutKey, GetShortcutIconName(shortcutKey));
            }

            shortcutElement.Setup(shortcutName, shortcutsToAdd);
            shortcutsListView.Add(shortcutElement);
        }

        /// <summary>
        /// Remove all shortcuts from the shortcuts displayer.
        /// </summary>
        public void ClearShortcut()
        {
            //TO Test
            OnClearShortcut.Invoke();
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

        private string GetShortcutIconName(string shortcutKey)
        {
            foreach (Shortcut shortcut in shortcuts)
            {
                if (shortcut.ShortcutKey == shortcutKey)
                    return shortcut.ShortcutIcon.name;
            }

            Debug.LogError("Shortcut key not found: this should'n happen");
            return "";
        }
    }
}
