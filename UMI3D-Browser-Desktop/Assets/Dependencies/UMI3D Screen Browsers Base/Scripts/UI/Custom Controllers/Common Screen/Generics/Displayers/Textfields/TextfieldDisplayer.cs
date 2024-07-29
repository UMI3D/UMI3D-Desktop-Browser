/*
Copyright 2019 - 2022 Inetum

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
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine.UIElements;
using static UnityEngine.UIElements.VisualElement;

namespace umi3d.commonScreen.Displayer
{
    public class TextfieldDisplayer : AbstractDisplayer, baseBrowser.Menu.IDisplayerElement
    {
        protected TextInputMenuItem menuItem;

        public ElementCategory Category;
        public ElementSize Size = ElementSize.Medium;
        public ElementDirection Direction = ElementDirection.Leading;
        public bool MaskToggle;
        public bool SubmitButton;

        protected Textfield_C textfield;

        ///// <summary>
        ///// Frame rate applied to message emission through network (high values can cause network flood).
        ///// </summary>
        //public float networkFrameRate = 30;
        ///// <summary>
        ///// Launched coroutine for network message sending (if any).
        ///// </summary>
        ///// <see cref="NetworkMessageSender"/>
        //protected Coroutine messageSenderCoroutine;

        //protected bool valueChanged = false;

        //protected IEnumerator NetworkMessageSender()
        //{
        //    while (true)
        //    {
        //        if (valueChanged)
        //        {
        //            menuItem.NotifyValueChange(textfield.text);
        //            valueChanged = false;
        //        }
        //        yield return new WaitForSeconds(1f / networkFrameRate);
        //    }
        //}

        private void OnValidate()
        {
            if (textfield == null) return;
            textfield.Category = Category;
            textfield.Size = Size;
            textfield.Direction = Direction;
            textfield.DisplayMaskToggle = MaskToggle;
            textfield.DisplaySubmitButton = SubmitButton;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="System.Exception"></exception>
        public override void SetMenuItem(AbstractMenuItem item)
        {
            if (item is TextInputMenuItem)
            {
                menuItem = item as TextInputMenuItem;
                InitAndBindUI();
            }
            else throw new System.Exception("MenuItem must be a TextInput");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void InitAndBindUI()
        {
            if (textfield != null) return;

            textfield = new Textfield_C
            {
                Category = Category,
                Size = Size,
                Direction = Direction,
                DisplayMaskToggle = MaskToggle,
                DisplaySubmitButton = SubmitButton
            };
            textfield.name = gameObject.name;
            textfield.LocaliseLabel = menuItem.ToString();
            textfield.isDelayed = true;
            textfield.SetValueWithoutNotify(menuItem.GetValue());
            textfield.RegisterValueChangedCallback(OnValueChanged);

            if (menuItem.dto.privateParameter)
            {
                textfield.isPasswordField = true;
                textfield.DisplayMaskToggle = true;
            }

            if (menuItem.isDisplayer)
            {
                textfield.HideTextInput();
                textfield.LocaliseLabel = textfield.label.ToLower();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            textfield.UnregisterValueChangedCallback(OnValueChanged);
            textfield.RemoveFromHierarchy();

            StopAllCoroutines();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Display(bool forceUpdate = false)
        {
            textfield.Display();


            //if(textInputContainer.enabledInHierarchy && (textInputContainer.resolvedStyle.display == DisplayStyle.Flex))
            //    messageSenderCoroutine = StartCoroutine(NetworkMessageSender());

        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Hide()
        {
            textfield.Hide();

            //StopCoroutine(NetworkMessageSender());
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public VisualElement GetUXMLContent() => textfield;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public override int IsSuitableFor(umi3d.cdk.menu.AbstractMenuItem menu)
        {
            return (menu is TextInputMenuItem) ? 2 : 0;
        }

        private void OnValueChanged(ChangeEvent<string> e)
        {
            //valueChanged = true;
            if (e.previousValue == e.newValue) return;
            menuItem.NotifyValueChange(e.newValue);
        }

        private void OnDestroy()
        {
            textfield?.RemoveFromHierarchy();
        }

        /// <summary>
        /// Get the focus of this element.
        /// </summary>
        public void Focus() 
        {
            
            textfield.schedule.Execute(() =>
            {
                textfield.schedule.Execute(() =>
                {
                    textfield.Focus();
                });
            });
        }
        /// <summary>
        /// Unfocus this element.
        /// </summary>
        public void Blur()
        {
            textfield.Blur();
        }
    }
}