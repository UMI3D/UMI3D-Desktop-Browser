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

using inetum.unityUtils;
using System;
using System.Collections.Generic;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    /// <summary>
    /// This class manages the shortcut displayer 
    /// </summary>
    public class Shortcuts : SingleBehaviour<Shortcuts>
    {
        #region Fields

        #region UI

        public UIDocument uiDocument;

        [Tooltip("Visual Tree Asset of a shortcut.")]
        [SerializeField]
        private VisualTreeAsset shortcut_ge_VTA;
        [Tooltip("Visual Tree Asset of an icon of a shortcut.")]
        [SerializeField]
        private VisualTreeAsset shortcutIcon_ge_VTA;
        [Tooltip("Visual Tree Asset of a label.")]
        [SerializeField]
        private VisualTreeAsset label_ge_VTA;

        VisualElement shortcutDisplayer_VE; //Where the shortcuts are displayed.
        ScrollView shortcuts_SV; //ScrollView of shortcuts.

        #endregion

        #region Data

        [Tooltip("Display shortcut at start.")]
        [SerializeField]
        private bool displayAtStart = false;

        [Tooltip("Shortcuts Icons dictionary.")]
        [SerializeField]
        private Controller.KeyBindings_SO keyBindings;

        private float shortcutDisplayerWidth = 350;

        /// <summary>
        /// True if the shortcutDisplayer is visible.
        /// </summary>
        private bool isDisplayed;

        //Object Pooling
        public static readonly List<ShortcutGenericElement> Shortcut_ge_DisplayedList = new List<ShortcutGenericElement>();
        public static readonly List<ShortcutGenericElement> Shortcut_ge_WaitedList = new List<ShortcutGenericElement>();
        public static readonly List<ShortcutIcon_GE> ShortcutIcon_ge_DisplayedList = new List<ShortcutIcon_GE>();
        public static readonly List<ShortcutIcon_GE> ShortcutIcon_ge_WaitedList = new List<ShortcutIcon_GE>();
        //public static readonly List<Label_GE> ShortcutLabel_ge_DisplayedList = new List<Label_GE>();
        //public static readonly List<Label_GE> ShortcutLabel_ge_WaitedList = new List<Label_GE>();

        #endregion

        #endregion

        void Start()
        {

            var root = uiDocument.rootVisualElement;

            shortcutDisplayer_VE = root.Q<VisualElement>("shortcut-displayer");
            shortcuts_SV = shortcutDisplayer_VE.Q<ScrollView>("shortcuts");


            shortcutDisplayer_VE.style.width = shortcutDisplayerWidth;
        }

        void Update()
        {
            //Debug
            if (Input.GetKeyDown(KeyCode.K))
            {
                String[] test = { "1" };
                AddShortcut("test", test);
                //AddShortcuts();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                String[] test = { "ctrl", "1" };
                AddShortcut("test2", test);
                //AddShortcuts();
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
                AddShortcut("Test shift bla bla blabla bla bla test.", test);
                //AddShortcuts();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                ClearShortcut();
            }
        }

        #region Add and Remove Shortcuts
        /// <summary>
        /// Add a shortcut to the shortcuts displayer.
        /// </summary>
        /// <param name="shortcutName">What the shortcut do.</param>
        /// <param name="shortcutkeys">Keys to press to trigger the shortcut.</param>
        public void AddShortcut(string shortcutName, string[] shortcutkeys)
        {
            ShortcutGenericElement shortcut_GE;
            //ObjectPooling(out shortcut_GE, Shortcut_ge_DisplayedList, Shortcut_ge_WaitedList, shortcut_ge_VTA);

            Sprite[] shortcutIcons = new Sprite[shortcutkeys.Length];
            for (int i = 0; i < shortcutkeys.Length; ++i)
                shortcutIcons[i] = keyBindings.GetSpriteFrom(shortcutkeys[i]);

            //shortcut_GE.
            //    Setup(shortcutName, shortcutIcons, shortcutIcon_ge_VTA, label_ge_VTA).
            //    InsertRootTo(shortcuts_SV);
        }

        /// <summary>
        /// Remove all shortcuts from the shortcuts displayer.
        /// </summary>
        public void ClearShortcut()
        {
            Action<Visual_E> removeVEFromHierarchy = (vE) => vE.Remove();

            ShortcutIcon_ge_DisplayedList.ForEach(removeVEFromHierarchy);
            ShortcutIcon_ge_WaitedList.AddRange(ShortcutIcon_ge_DisplayedList);
            ShortcutIcon_ge_DisplayedList.Clear();

            //ShortcutLabel_ge_DisplayedList.ForEach(removeVEFromHierarchy);
            //ShortcutLabel_ge_WaitedList.AddRange(ShortcutLabel_ge_DisplayedList);
            //ShortcutLabel_ge_DisplayedList.Clear();

            Shortcut_ge_DisplayedList.ForEach(removeVEFromHierarchy);
            Shortcut_ge_WaitedList.AddRange(Shortcut_ge_DisplayedList);
            Shortcut_ge_DisplayedList.Clear();
        }

        #endregion

        #region OP
        public static void ObjectPooling<T>(out T vE, List<T> listDisplayed, List<T> listWaited, VisualTreeAsset visualTreeAsset) where T : VisualElement
        {
            if (listWaited.Count == 0)
                vE = visualTreeAsset.CloneTree().Q<T>();
            else
            {
                vE = listWaited[listWaited.Count - 1];
                listWaited.RemoveAt(listWaited.Count - 1);
            }
            listDisplayed.Add(vE);
        }

        public static void ObjectPooling(out Label label_L, List<Label> listDisplayed, List<Label> listWaited, string text)
        {
            if (listWaited.Count == 0)
                label_L = new Label(text);
            else
            {
                label_L = listWaited[listWaited.Count - 1];
                listWaited.RemoveAt(listWaited.Count - 1);
            }
            listDisplayed.Add(label_L);
        }
        #endregion
    }
}
