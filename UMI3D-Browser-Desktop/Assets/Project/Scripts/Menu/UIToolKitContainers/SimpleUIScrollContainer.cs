﻿/*
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
    public class SimpleUIScrollContainer : SimpleUIContainer2D
    {
        protected ScrollView scrollView;

        protected override void BindUI()
        {
            base.BindUI();
            scrollView = contentElement as ScrollView;
            if (scrollView == null)
            {
                Debug.LogWarning("The content element must be a scrollview for this container");
            }
        }

        public override void Expand(bool forceUpdate = false)
        {
            scrollView.style.display = DisplayStyle.Flex;
            base.Expand(forceUpdate);
        }

        public override void Collapse(bool forceUpdate = false)
        {
            scrollView.style.display = DisplayStyle.None;
            base.Collapse(forceUpdate);
        }

        public override void ExpandAs(umi3d.cdk.menu.view.AbstractMenuDisplayContainer Container, bool forceUpdate = false)
        {
            scrollView.style.display = DisplayStyle.Flex;
            base.ExpandAs(Container, forceUpdate);
        }
    }
}