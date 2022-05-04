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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public enum DisplayerboxType { ToolboxesPopup, ParametersPopup }
    public partial class Displayerbox_E
    {
        public Func<View_E> CreateSeparator { get; set; } = null;

        private static VisualElement m_displayerbox
        {
            get
            {
                var displayerbox = new VisualElement();
                displayerbox.name = "displayerbox";
                return displayerbox;
            }
        }
        private static string s_displayerboxToolboxStyle = "UI/Style/Displayers/ToolboxDisplayerbox";
        private static string s_displayerboxParameterStyle = "UI/Style/Displayers/ParameterDisplayerbox";
        private static string GetDisplayerboxType(DisplayerboxType type)
        {
            switch (type)
            {
                case DisplayerboxType.ToolboxesPopup:
                    return s_displayerboxToolboxStyle;
                case DisplayerboxType.ParametersPopup:
                    return s_displayerboxParameterStyle;
                default:
                    throw new System.Exception();
            }
        }

        protected List<View_E> m_separatorsDisplayed { get; set; } = null;
        protected List<View_E> m_separatorsWaited { get; set; } = null;

        public void AddRange(params View_E[] displayers)
        {
            foreach (View_E displayer in displayers)
                Add(displayer);
        }

        protected virtual void UpdateSeparator()
        {
            if (CreateSeparator == null)
                return;

            m_separatorsDisplayed.ForEach((separator) => separator.RemoveRootFromHierarchy());
            m_separatorsWaited.AddRange(m_separatorsDisplayed);
            m_separatorsDisplayed.Clear();

            m_views.ForEach(delegate (View_E elt)
            {
                int eltIndex = Root.IndexOf(elt.Root);
                if (eltIndex == 0) return;
                ObjectPooling(out View_E separator, m_separatorsDisplayed, m_separatorsWaited, CreateSeparator);
                separator.InsertRootAtTo(eltIndex, Root);
            });
        }
    }

    public partial class Displayerbox_E : View_E
    {
        public Displayerbox_E(DisplayerboxType type) :
            base(m_displayerbox, GetDisplayerboxType(type), StyleKeys.DefaultBackgroundAndBorder)
        {
            string separatorStyle = "UI/Style/Displayers/DisplayerSeparator";
            CreateSeparator = () =>
            {
                var separator = new View_E(separatorStyle, StyleKeys.DefaultBackground);
                separator.Root.name = "displayerSeparator";
                return separator;
            };
        }

        public override void Add(View_E child)
        {
            base.Add(child);
            child.InsertRootTo(Root);
            UpdateSeparator();
        }

        public override void Remove(View_E child)
        {
            base.Remove(child);
            Root.Remove(child.Root);
            UpdateSeparator();
        }

        protected override void Initialize()
        {
            base.Initialize();

            m_separatorsDisplayed = new List<View_E>();
            m_separatorsWaited = new List<View_E>();
        }
    }
}