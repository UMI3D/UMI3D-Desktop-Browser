using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    /// <summary>
    /// Toggle displayer.
    /// </summary>
    public class BooleanInputDisplayerElement : AbstractBooleanInputDisplayer, IDisplayerElement
    {
        public VisualTreeAsset booleanInputVisualTreeAsset;

        Toggle toggle;

        public override void Display(bool forceUpdate = false)
        {
            InitAndBindUI();

            toggle.style.display = DisplayStyle.Flex;
            toggle.value = GetValue();
            toggle.RegisterValueChangedCallback(OnValueChanged);
        }

        public VisualElement GetUXMLContent()
        {
            InitAndBindUI();
            return toggle;
        }

        public override void Hide()
        {
            toggle.style.display = DisplayStyle.None;
            toggle.UnregisterValueChangedCallback(OnValueChanged);
        }

        public override void Clear()
        {
            base.Clear();
            toggle.UnregisterValueChangedCallback(OnValueChanged);
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (menu is BooleanInputMenuItem) ? 2 : 0;
        }

        private void OnValueChanged(ChangeEvent<bool> e)
        {
            NotifyValueChange(e.newValue);
        }

        public void InitAndBindUI()
        {
            if (toggle == null)
                toggle = booleanInputVisualTreeAsset.CloneTree().Q<Toggle>();
        }
    }
}