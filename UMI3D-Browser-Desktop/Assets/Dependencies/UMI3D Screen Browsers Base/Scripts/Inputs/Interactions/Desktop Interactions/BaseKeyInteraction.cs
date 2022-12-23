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

namespace umi3d.baseBrowser.inputs.interactions
{
    public abstract class BaseKeyInteraction : EventInteraction
    {
        public static bool IsEditingTextField;

        public InputAction Key;

        protected bool m_wasHoverBeforeClicked;

        /// <summary>
        /// check if key is allow to be watch (cursor free, etc.)
        /// </summary>
        /// <returns></returns>
        public virtual bool CanProces() => BaseCursor.Movement != BaseCursor.CursorMovement.Free && !IsEditingTextField;

        public virtual void UpdateKey(InputAction action)
        {
            if (action.bindings.Count == 0)
            {
                for (int i = 0; i < Key.bindings.Count; i++) Key.ChangeBinding(i).Erase();
            }
            else if (action.bindings.Count == 1)
            {
                for (int i = 1; i < Key.bindings.Count; i++) Key.ChangeBinding(i).Erase();

                UpdateBinding(0, action.bindings[0]);
            }
            else if (action.bindings.Count == 2)
            {
                UpdateBinding(0, action.bindings[0]);
            }

            if (action.bindings.Count > 1) Key.ChangeBinding(1).WithPath(action.bindings[1].path);
        }

        protected virtual void UpdateBinding(int index, InputBinding binding)
        {
            if (binding.isComposite)
            {

            }
            if (Key.bindings.Count <= index) Key.AddBinding(binding.path);
            else Key.ChangeBinding(index).WithPath(binding.path);
        }

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
            if (!CanProces()) return;

            Pressed(true);
        }

        /// <summary>
        /// Callback when the key is pressed up.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void KeyCanceled(InputAction.CallbackContext context)
        {
            if (!CanProces()) return;

            Pressed(false);
        }
    }
}
