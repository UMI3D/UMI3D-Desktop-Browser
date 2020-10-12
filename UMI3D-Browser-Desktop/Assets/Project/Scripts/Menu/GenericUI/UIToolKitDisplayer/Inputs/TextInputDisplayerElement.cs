using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class TextInputDisplayerElement : AbstractTextInputDisplayer, IDisplayerElement
    {
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

            if (textInput.resolvedStyle.display == DisplayStyle.None)
                textInput.style.display = DisplayStyle.Flex;

            textInput.label = menuItem.ToString();
            textInput.value = menuItem.GetValue();

            textInput.RegisterValueChangedCallback(OnValueChanged);

            if(textInput.enabledInHierarchy && (textInput.resolvedStyle.display == DisplayStyle.Flex))
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
            StopAllCoroutines();
        }

        public override void Hide()
        {
            textInput.UnregisterValueChangedCallback(OnValueChanged);
            StopCoroutine(networkMessageSender());

            if (textInput.resolvedStyle.display == DisplayStyle.Flex)
                textInput.style.display = DisplayStyle.None;
        }

        public VisualElement GetUXMLContent()
        {
            InitAndBindUI();
            return textInput;
        }

        public void InitAndBindUI()
        {
            if (textInput == null)
                textInput = new TextField();
        }
    }
}