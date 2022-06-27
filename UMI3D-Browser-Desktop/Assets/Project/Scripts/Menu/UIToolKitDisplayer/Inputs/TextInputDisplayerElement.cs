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

using System.Collections;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class TextInputDisplayerElement : AbstractTextInputDisplayer, IDisplayerElement
    {
        public static bool isTyping { get => elementFocus != null; }

        public static TextInputDisplayerElement elementFocus = null;

        public VisualTreeAsset textFieldTreeAsset;

        VisualElement textInputContainer;
        TextField textInput;

        /// <summary>
        /// Frame rate applied to message emission through network (high values can cause network flood).
        /// </summary>
        public float networkFrameRate = 30;

        /// <summary>
        /// Launched coroutine for network message sending (if any).
        /// </summary>
        /// <see cref="networkMessageSender"/>
        protected Coroutine messageSenderCoroutine;

        protected bool valueChanged = false;

        protected IEnumerator networkMessageSender()
        {
            while (true)
            {
                if (valueChanged)
                {
                    NotifyValueChange(textInput.text);
                    valueChanged = false;
                }
                yield return new WaitForSeconds(1f / networkFrameRate);
            }
        }

        public override void Display(bool forceUpdate = false)
        {
            InitAndBindUI();

            if (textInputContainer.resolvedStyle.display == DisplayStyle.None)
                textInputContainer.style.display = DisplayStyle.Flex;

            if(!string.IsNullOrEmpty(menuItem.ToString()))
                textInput.label = menuItem.ToString();
            textInput.value = menuItem.GetValue();

            textInput.RegisterValueChangedCallback(OnValueChanged);

            if(textInputContainer.enabledInHierarchy && (textInputContainer.resolvedStyle.display == DisplayStyle.Flex))
                messageSenderCoroutine = StartCoroutine(networkMessageSender());

        }

        private void OnValueChanged(ChangeEvent<string> e)
        {
            valueChanged = true;
        }

        public override void Clear()
        {
            base.Clear();
            textInput.UnregisterValueChangedCallback(OnValueChanged);
            textInputContainer.RemoveFromHierarchy();
            StopAllCoroutines();
        }

        public override void Hide()
        {
            textInput.UnregisterValueChangedCallback(OnValueChanged);
            StopCoroutine(networkMessageSender());

            if (textInputContainer.resolvedStyle.display == DisplayStyle.Flex)
                textInputContainer.style.display = DisplayStyle.None;

            if (elementFocus == this)
                elementFocus = null;
        }

        public VisualElement GetUXMLContent()
        {
            InitAndBindUI();
            return textInputContainer;
        }

        public void InitAndBindUI()
        {
            if (textInputContainer == null)
            {
                textInputContainer = textFieldTreeAsset.CloneTree();
                textInputContainer.name = gameObject.name;
                textInput = textInputContainer.Q<TextField>();
                textInput.RegisterCallback<FocusInEvent>((e) => { elementFocus = this;});
                textInput.RegisterCallback<FocusOutEvent>((e) => { elementFocus = null; ;});
            }
        }

        private void OnDestroy()
        {
            textInputContainer?.RemoveFromHierarchy();
        }
    }
}