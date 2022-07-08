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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.Controller
{
    public class BaseCursorDisplayer : inetum.unityUtils.SingleBehaviour<BaseCursorDisplayer>
    {
        public UIDocument document;

        [Header("Class names")]
        [SerializeField]
        Sprite defaultCursor = null;
        [SerializeField]
        Sprite hoverCursor = null;
        [SerializeField]
        Sprite followCursor = null;
        [SerializeField]
        Sprite clickedCursor = null;

        protected VisualElement cursorCenter;

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(document != null);
            var root = document.rootVisualElement;
            cursorCenter = root.Q("cursor-center");
        }

        public void DisplayCursor(bool display, BaseCursor.CursorState state)
        {
            cursorCenter.ClearClassList();
            cursorCenter.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;

            switch (state)
            {
                case BaseCursor.CursorState.Default:
                    cursorCenter.style.backgroundImage = new StyleBackground(defaultCursor.texture);
                    break;
                case BaseCursor.CursorState.Hover:
                    cursorCenter.style.backgroundImage = new StyleBackground(hoverCursor.texture);
                    break;
                case BaseCursor.CursorState.Clicked:
                    cursorCenter.style.backgroundImage = new StyleBackground(clickedCursor.texture);
                    break;
                case BaseCursor.CursorState.FollowCursor:
                    cursorCenter.style.backgroundImage = new StyleBackground(followCursor.texture);
                    break;
                default:
                    break;
            }
        }
    }
}
