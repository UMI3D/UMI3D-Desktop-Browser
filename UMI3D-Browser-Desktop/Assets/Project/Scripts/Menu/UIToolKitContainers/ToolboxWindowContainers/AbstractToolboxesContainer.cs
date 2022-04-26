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
using System.Linq;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;

namespace umi3d.desktopBrowser.menu.Container
{
    public abstract partial class AbstractToolboxesContainer
    {
        public enum ItemType { Undefine, Tool, Toolbox }

        public ToolboxItem_E ToolboxItem { get; protected set; } = null;
        public Toolbox_E Toolbox { get; protected set; } = null;
        public Displayerbox_E Displayerbox { get; protected set; } = null;
        public ItemType ToolType { get; protected set; } = ItemType.Undefine;
    }

    public abstract partial class AbstractToolboxesContainer
    {
        protected virtual void Awake()
        {
            m_virtualContainer = this;
            isExpanded = false;
        }
        protected virtual void OnDisable()
        {
            Collapse();
        }
        /// <summary>
        /// Set the container as a toolbox.
        /// </summary>
        protected virtual void SetContainerAsToolbox()
            => ToolType = ItemType.Toolbox;
        /// <summary>
        /// Set the container as a tool.
        /// </summary>
        protected virtual void SetContainerAsTool()
            => ToolType = ItemType.Tool;
    }

    public abstract partial class AbstractToolboxesContainer : Abstract2DContainer
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="menu"></param>
        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            this.name = menu.Name + "-" + GetType().ToString().Split('.').Last();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="updateDisplay"></param>
        public override void Insert(AbstractDisplayer element, bool updateDisplay = true)
        {
            if (element is AbstractToolboxesContainer menuContainer)
                menuContainer.parent = this;
            base.Insert(element, updateDisplay);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="index"></param>
        /// <param name="updateDisplay"></param>
        public override void Insert(AbstractDisplayer element, int index, bool updateDisplay = true)
        {
            if (element is AbstractMenuDisplayContainer menuContainer)
                menuContainer.parent = this;

            element.transform.SetParent(this.transform);
            element.transform.SetSiblingIndex(index);

            m_currentDisplayers.Insert(index, element);
            element.Hide();
            if (updateDisplay)
                Display(true);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="updateDisplay"></param>
        /// <returns></returns>
        public override bool Remove(AbstractDisplayer element, bool updateDisplay = true)
        {
            if (!m_currentDisplayers.Remove(element))
                return false;
            if (element is AbstractToolboxesContainer menuContainer)
                menuContainer.parent = null;
            if (updateDisplay)
                Display(true);

            return true;
        }
    }
}