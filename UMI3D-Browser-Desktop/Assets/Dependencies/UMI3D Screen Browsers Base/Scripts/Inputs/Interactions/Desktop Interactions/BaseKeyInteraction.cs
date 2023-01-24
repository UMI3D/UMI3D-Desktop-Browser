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
using System.Collections.Generic;
using System.Linq;
using umi3d.baseBrowser.cursor;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using static umi3d.baseBrowser.inputs.interactions.BaseKeyInteraction;

namespace umi3d.baseBrowser.inputs.interactions
{
    public abstract class BaseKeyInteraction : EventInteraction
    {
        public enum ControllerInputEnum { Gamepad, Keyboard, Mouse }

        public enum MappingType
        {
            Keyboard, Mouse,
            //TODO add gamepad 
        }

        public static bool IsEditingTextField;

        public InputAction Key;

        protected bool m_wasHoverBeforeClicked;

        /// <summary>
        /// check if key is allow to be watch (cursor free, etc.)
        /// </summary>
        /// <returns></returns>
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

        #region Rebinding

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

        #endregion
    }

    public static class InputActionExtensions
    {
        /// <summary>
        /// Get the index of the corresponding InputControl from the <paramref name="bindingIndex"/>.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="bindingIndex"></param>
        /// <returns></returns>
        public static int GetControlIndexFromBindingIndex(this InputAction action, int bindingIndex)
        {
            var bindings = action.bindings;
            int result = -1;

            for (int i = 0; i <= bindingIndex; i++)
            {
                result++;
                if (i > 0 && bindings[i - 1].isComposite) result--;
            }

            return result;
        }

        /// <summary>
        /// Get the corresponding InputControl from the <paramref name="bindingIndex"/>
        /// </summary>
        /// <param name="action"></param>
        /// <param name="bindingIndex"></param>
        /// <returns></returns>
        public static InputControl GetControlFromBindingIndex(this InputAction action, int bindingIndex)
            => action.controls[action.GetControlIndexFromBindingIndex(bindingIndex)];

        /// <summary>
        /// return the first binding index after <paramref name="lowIndex"/> that match <paramref name="controllers"/> and set <paramref name="inputBinding"/> as this binging. If there is no match return -1 and set <paramref name="inputBinding"/> equals new InputBinding.
        /// </summary>
        /// /// <param name="action"></param>
        /// <param name="inputBinding"></param>
        /// <param name="lowIndex"></param>
        /// <param name="controllers"></param>
        /// <returns></returns>
        public static int FirstBindingIndex(this InputAction action, out InputBinding inputBinding, int lowIndex, params ControllerInputEnum[] controllers)
        {
            ReadOnlyArray<InputBinding> bindings = action.bindings;

            if (lowIndex >= bindings.Count)
            {
                inputBinding = new InputBinding();
                return -1;
            }

            var currentIndex = 0;
            inputBinding = bindings.FirstOrDefault(_binding =>
            {
                currentIndex = bindings.IndexOf(item => item == _binding);
                if (currentIndex < lowIndex) return false;

                if (_binding.isComposite)
                {
                    if (_binding.path == "ButtonWithOneModifier")
                    {
                        bool matchController1 = false;
                        foreach (var controller in controllers)
                            if (bindings[currentIndex + 1].path.Contains(controller.ToString())) matchController1 = true;
                        bool matchController2 = false;
                        foreach (var controller in controllers)
                            if (bindings[currentIndex + 2].path.Contains(controller.ToString())) matchController2 = true;
                        return matchController1 && matchController2;
                    }
                    else if (_binding.path == "ButtonWithTwoModifier")
                    {
                        bool matchController1 = false;
                        foreach (var controller in controllers)
                            if (bindings[currentIndex + 1].path.Contains(controller.ToString())) matchController1 = true;
                        bool matchController2 = false;
                        foreach (var controller in controllers)
                            if (bindings[currentIndex + 2].path.Contains(controller.ToString())) matchController2 = true;
                        bool matchController3 = false;
                        foreach (var controller in controllers)
                            if (bindings[currentIndex + 2].path.Contains(controller.ToString())) matchController3 = true;
                        return matchController1 && matchController2 && matchController3;
                    }
                    else return false;
                }
                else if (_binding.isPartOfComposite) return false;
                else
                {
                    bool matchController = false;
                    foreach (var controller in controllers)
                        if (_binding.path.Contains(controller.ToString())) matchController = true;

                    return matchController;
                }
            });

            return inputBinding.Equals(new InputBinding()) ? -1 : currentIndex;
        }

        /// <summary>
        /// Get the controller type and control diplay name.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="bindingIndex"></param>
        /// <returns></returns>
        public static (ControllerInputEnum, string) GetSimpleMappingFromBindingIndex(this InputAction action, int bindingIndex)
        {
            InputControl inputControl = action.GetControlFromBindingIndex(bindingIndex);

            if (inputControl.path.Contains(ControllerInputEnum.Keyboard.ToString())) 
                return (ControllerInputEnum.Keyboard, inputControl.displayName);
            else if (inputControl.path.Contains(ControllerInputEnum.Mouse.ToString()))
                return (ControllerInputEnum.Mouse, inputControl.displayName);
            else return (ControllerInputEnum.Gamepad, inputControl.displayName);
        }

        /// <summary>
        /// Get the list of controller type and control diplay name that form a composit binding.
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="ControlBinding"></param>
        /// <param name="index"></param>
        /// <param name="controlIndex"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        public static List<(ControllerInputEnum, string)> GetCompositMappingFromBindingIndex(this InputAction action, int bindingIndex)
        {
            InputBinding binding = action.bindings[bindingIndex];
            ReadOnlyArray<InputControl> controls = action.controls;

            List<(ControllerInputEnum, string)> result = new List<(ControllerInputEnum, string)>();

            if (binding.path == "ButtonWithOneModifier")
            {
                result.Add(action.GetSimpleMappingFromBindingIndex(bindingIndex + 2));
                result.Add(action.GetSimpleMappingFromBindingIndex(bindingIndex));
            }
            else if (binding.path == "ButtonWithTwoModifier")
            {
                result.Add(action.GetSimpleMappingFromBindingIndex(bindingIndex + 2));
                result.Add(action.GetSimpleMappingFromBindingIndex(bindingIndex + 3));
                result.Add(action.GetSimpleMappingFromBindingIndex(bindingIndex));
            }

            return result;
        }

        /// <summary>
        /// Get the first mapping that match <paramref name="controllers"/>
        /// </summary>
        /// <param name="action"></param>
        /// <param name="controllers"></param>
        /// <returns></returns>
        public static List<(ControllerInputEnum, string)> GetFirstMappingFromController(this InputAction action, params ControllerInputEnum[] controllers)
        {
            int bindingIndex = action.FirstBindingIndex(out var binding, 0, controllers);
            List<(ControllerInputEnum, string)> result = new List<(ControllerInputEnum, string)>();

            if (binding.isComposite) result.AddRange(action.GetCompositMappingFromBindingIndex(bindingIndex));
            else result.Add(action.GetSimpleMappingFromBindingIndex(bindingIndex));

            return result;
        }
    }
}
