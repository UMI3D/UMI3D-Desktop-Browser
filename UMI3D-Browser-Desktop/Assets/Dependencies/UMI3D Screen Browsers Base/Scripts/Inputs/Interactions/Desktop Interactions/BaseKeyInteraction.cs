/*
Copyright 2019 - 2023 Inetum

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

using umi3d.baseBrowser.cursor;
using UnityEngine.InputSystem;
using UnityEngine;

namespace umi3d.baseBrowser.inputs.interactions
{
    public abstract class BaseKeyInteraction : EventInteraction
    {
        public static bool IsEditingTextField;

        public InputAction Key;

        protected bool m_wasHoverBeforeClicked;

        public virtual bool CanProces() => BaseCursor.Movement != BaseCursor.CursorMovement.Free && !IsEditingTextField;

        protected virtual void Start()
        {
            Key.started += KeyStarted;
            Key.canceled += KeyCanceled;
            Key.Enable();

            onInputDown.AddListener(() =>
            {
                m_wasHoverBeforeClicked = BaseCursor.State == BaseCursor.CursorState.Hover;
                if (m_wasHoverBeforeClicked) BaseCursor.State = BaseCursor.CursorState.Clicked;
            });
            onInputUp.AddListener(() =>
            {
                if (m_wasHoverBeforeClicked && BaseCursor.State == BaseCursor.CursorState.Clicked)
                    BaseCursor.State = BaseCursor.CursorState.Hover;
            });
        }

        protected override void CreateMenuItem()
        {
            base.CreateMenuItem();

            //DisplayInput(associatedInteraction.name, InputLayoutManager.GetInputCode(activationButton).ToString());
        }

        protected virtual void DisplayInput(string label, string inputName)
        {
            //Shortcutbox_E.Instance.AddShortcut(label, inputName);
            //Key.activeControl.displayName;
        }

        /// <summary>
        /// Callback when the key is pressed down.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void KeyStarted(InputAction.CallbackContext context)
        {
            //todo check if key is allow to be watch (cursor free, etc.)
            if (!CanProces()) return;

            Pressed(true);
        }
        
        /// <summary>
        /// Callback when the key is pressed up.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void KeyCanceled(InputAction.CallbackContext context)
        {
            //todo check if key is allow to be watch (cursor free, etc.)
            if (!CanProces()) return;

            Pressed(false);
        }
    }
}
