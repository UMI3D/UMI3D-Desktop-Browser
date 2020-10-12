using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class DropDownInputDisplayerElement : AbstractDropDownInputDisplayer, IDisplayerElement
    {

        Label label; //TO REPLACE

        public override void Clear()
        {
            base.Clear();
            //TODO
            //dropdown.onValueChanged.RemoveAllListeners();
        }

        public override void Display(bool forceUpdate = false)
        {
            // TODO
            /*dropdown.gameObject.SetActive(true);
            dropdown.ClearOptions();
            dropdown.AddOptions(menuItem.options);
            dropdown.value = menuItem.options.IndexOf(GetValue());
            dropdown.onValueChanged.AddListener((i) => NotifyValueChange(menuItem.options[i]));
            label.text = menuItem.ToString();*/

            InitAndBindUI();

            label.style.display = DisplayStyle.Flex;
            label.text = menuItem.ToString();
        }

        public VisualElement GetUXMLContent()
        {
            InitAndBindUI();
            return label;
        }

        public override void Hide()
        {
            // TODO
            /*dropdown.onValueChanged.RemoveAllListeners();
            dropdown.gameObject.SetActive(false);*/
            label.style.display = DisplayStyle.None;
        }

        public void InitAndBindUI()
        {
            if (label == null)
                label = new Label();
        }
    }
}