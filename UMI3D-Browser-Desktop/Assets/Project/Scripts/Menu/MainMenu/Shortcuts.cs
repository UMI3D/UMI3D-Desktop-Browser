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

namespace BrowserDesktop.Menu
{
    public class Shortcuts : MonoBehaviour
    {
        public UIDocument uiDocument;

        VisualElement shortcutArea;
        VisualElement shortcutDisplayer;
        Button openShortcutBtn;

        bool isDisplayed = true; //is shortcutDisplayer visible

        void Start()
        {
            var root = uiDocument.rootVisualElement;
            openShortcutBtn = root.Q<Button>("open-shortcuts-button");
            openShortcutBtn.clickable.clicked += ()=> DisplayShortcut(!isDisplayed);
            openShortcutBtn.AddToClassList("btn-shortcut");

            DisplayShortcut(false); //Default: shortcut are hide
        }

        void Update()
        {

        }

        private void DisplayShortcut(bool val)
        {
            isDisplayed = val;

            //TODO display or hide shortcutDisplayer

            /*if (val)
            {
                openShortcutBtn.AddToClassList("btn_shortcut_ENABLE");
                openShortcutBtn.RemoveFromClassList("btn-notif-off");
            }
            else
            {
                openShortcutBtn.AddToClassList("btn-notif-off");
                openShortcutBtn.RemoveFromClassList("btn_shortcut_ENABLE");
            }*/
        }
    }
}
