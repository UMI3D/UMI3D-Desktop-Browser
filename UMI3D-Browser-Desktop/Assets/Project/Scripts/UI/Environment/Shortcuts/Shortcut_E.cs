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
using umi3d.cdk.menu;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class Shortcut_E
    {
        protected Box_E m_iconbox { get; set; } = null;
        protected Label_E m_title { get; set; } = null;

        protected List<Label_E> m_plusDisplayed;
        protected static List<Label_E> m_plusWaited;
        protected List<ShortcutIcon_E> m_iconsDisplayed;
        protected static List<ShortcutIcon_E> m_iconsWaited;

        private static string m_shortcutStyle = "UI/Style/Shortcuts/Shortcut";
    }

    public partial class Shortcut_E
    {
        public Shortcut_E() :
            base("UI/UXML/Shortcuts/shortcut", m_shortcutStyle, null)
        { }

        public void Setup(string title, params Sprite[] icons)
        {
            m_title.value = title;

            for (int i = 0; i < icons.Length; ++i)
            {
                if (i != 0 && i < icons.Length)
                {
                    ObjectPooling(out Label_E plus, m_plusDisplayed, m_plusWaited, () 
                        => new Label_E("Corps", StyleKeys.Text("primaryLight"), "+"));
                    plus.InsertRootTo(m_iconbox.Root);
                }

                ObjectPooling(out ShortcutIcon_E icon, m_iconsDisplayed, m_iconsWaited, () => new ShortcutIcon_E());
                icon.Setup(icons[i]);
                icon.InsertRootTo(m_iconbox.Root);
            }
        }
    }

    public partial class Shortcut_E : View_E
    {
        protected override void Initialize()
        {
            base.Initialize();

            m_title = new Label_E(Root.Q<Label>("title"), "CorpsShortcut", StyleKeys.DefaultText);
            m_iconbox = new Box_E(QR("iconbox"), "KeyIconbox", null);

            m_plusDisplayed = new List<Label_E>();
            m_plusWaited = new List<Label_E>();
            m_iconsDisplayed = new List<ShortcutIcon_E>();
            m_iconsWaited = new List<ShortcutIcon_E>();
        }

        public override void RemoveRootFromHierarchy()
        {
            base.RemoveRootFromHierarchy();

            Action<View_E> removeVEFromHierarchy = (vE) =>
            {
                vE.RemoveRootFromHierarchy();
            };

            m_iconsDisplayed.ForEach(removeVEFromHierarchy);
            m_iconsWaited.AddRange(m_iconsDisplayed);
            m_iconsDisplayed.Clear();

            m_plusDisplayed.ForEach(removeVEFromHierarchy);
            m_plusWaited.AddRange(m_plusDisplayed);
            m_plusDisplayed.Clear();
        }
    }
}
