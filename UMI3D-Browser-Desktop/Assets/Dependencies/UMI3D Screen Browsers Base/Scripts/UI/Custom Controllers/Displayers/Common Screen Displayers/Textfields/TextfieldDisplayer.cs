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
        public ElemnetDirection Direction = ElemnetDirection.Leading;
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
            textfield.Set(Category, Size, Direction, MaskToggle, SubmitButton);
        }

        public override void SetMenuItem(AbstractMenuItem item)
        {
            if (item is TextInputMenuItem)
            {
                menuItem = item as TextInputMenuItem;
                InitAndBindUI();
            }
            else throw new System.Exception("MenuItem must be a TextInput");
        }

        public void InitAndBindUI()
        {
            if (textfield != null) return;

            textfield = new Textfield_C(Category, Size, Direction, MaskToggle, SubmitButton);
            textfield.name = gameObject.name;
            textfield.label = menuItem.ToString();
            textfield.isDelayed = true;
            textfield.RegisterValueChangedCallback(OnValueChanged);

            if (menuItem.dto.privateParameter)
            {
                textfield.isPasswordField = true;
                textfield.DisplayMaskToggle = true;
            }
        }

        public override void Clear()
        {
            base.Clear();
            textfield.UnregisterValueChangedCallback(OnValueChanged);
            textfield.RemoveFromHierarchy();

            StopAllCoroutines();
        }

        public override void Display(bool forceUpdate = false)
        {
            textfield.Display();


            //if(textInputContainer.enabledInHierarchy && (textInputContainer.resolvedStyle.display == DisplayStyle.Flex))
            //    messageSenderCoroutine = StartCoroutine(NetworkMessageSender());

        }

        public override void Hide()
        {
            textfield.Hide();

            //StopCoroutine(NetworkMessageSender());
        }

        public VisualElement GetUXMLContent() => textfield;

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
        public void Blur()
        {
            textfield.Blur();
        }
    }
}