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

namespace BrowserDesktop.Menu
{
    /// <summary>
    /// This class manages the shortcut displayer 
    /// </summary>
    public class Shortcuts : MonoBehaviour
    {
        public UIDocument uiDocument;

        [SerializeField]
        private VisualTreeAsset shortcutTreeAsset;

        VisualElement shortcutArea; //Where shortcut's button and labels are positions in the footer.
        VisualElement shortcutDisplayer; //Where the shortcuts are displayed.
        Button openShortcutBtn;

        bool isDisplayed = true; //is shortcutDisplayer visible.

        public static UnityEvent OnClearShortcut = new UnityEvent();

        void Start()
        {
            Debug.Assert(uiDocument != null);
            Debug.Assert(shortcutTreeAsset != null);

            var root = uiDocument.rootVisualElement;
            openShortcutBtn = root.Q<Button>("open-shortcuts-button");
            openShortcutBtn.clickable.clicked += ()=> DisplayShortcut(!isDisplayed);
            openShortcutBtn.AddToClassList("btn-shortcut");

            DisplayShortcut(false); //Default: shortcuts are hidden.
        }

        void Update()
        {

        }

        public void AddShortcut()
        {
            var shortcutElement = shortcutTreeAsset.CloneTree().Q<ShortcutElement>();
            OnClearShortcut.AddListener(shortcutElement.RemoveShortcut);
            //TODO
        }

        public void ClearShortcut()
        {
            //TODO
            OnClearShortcut.Invoke();
        }

        private void DisplayShortcut(bool val)
        {
            isDisplayed = val;

            //TODO display or hide shortcutDisplayer
        }
    }
}
