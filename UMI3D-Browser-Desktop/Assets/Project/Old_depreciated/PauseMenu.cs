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

using BrowserDesktop.Cursor;
using inetum.unityUtils;
using umi3d.cdk;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class PauseMenu : SingleBehaviour<PauseMenu>
    {
        public UIDocument uiDocument;

        VisualElement pauseMenuContainer;

        [SerializeField]
        float pauseMenuHeight = 52;


        void Display(bool value)
        {
            if (value)
            {
                pauseMenuContainer.experimental.animation.Start(0, pauseMenuHeight, 100, (elt, val) =>
                {
                    elt.style.height = val;
                });
            } else
            {
                pauseMenuContainer.experimental.animation.Start(pauseMenuHeight, 0, 100, (elt, val) =>
                {
                    elt.style.height = val;
                });
            }
        }

    }
}