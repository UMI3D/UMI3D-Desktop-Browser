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

using umi3d.cdk;
using umi3d.common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace umi3d.baseBrowser.inputs.interactions
{
    public abstract class BaseKeyInteraction : EventInteraction
    {
        public InputAction Key;

        protected virtual void Start()
        {
            Key.performed += KeyPressed;

            //Todo change cursor state when key press
            //onInputDown.AddListener(() =>
            //{
            //    SwichOnDown = (BaseCursor.State == BaseCursor.CursorState.Hover);
            //    if (SwichOnDown) BaseCursor.State = BaseCursor.CursorState.Clicked;
            //});
            //onInputUp.AddListener(() =>
            //{
            //    if (SwichOnDown && BaseCursor.State == BaseCursor.CursorState.Clicked)
            //        BaseCursor.State = BaseCursor.CursorState.Hover;
            //});
        }

        protected override void CreateMenuItem()
        {
            base.CreateMenuItem();

            //DisplayInput(associatedInteraction.name, InputLayoutManager.GetInputCode(activationButton).ToString());
        }

        protected virtual void DisplayInput(string label, string inputName)
        {
            //Shortcutbox_E.Instance.AddShortcut(label, inputName);
        }

        protected virtual void KeyPressed(InputAction.CallbackContext context)
        {
            //todo check if key is allow to be watch (cursor free, etc.)

            if (context.started) Pressed(true);
            else if (context.canceled) Pressed(false);
        }
    }
}
