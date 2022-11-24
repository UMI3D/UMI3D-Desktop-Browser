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
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class CookiesDisplayer : AbstractDisplayer, baseBrowser.Menu.IDisplayerElement
    {
        public string DescriptionLabel;
        public ElementCategory Category;
        public ElementSize Size = ElementSize.Medium;
        public ElemnetDirection Direction = ElemnetDirection.Leading;

        protected LocalInfoRequestInputMenuItem menuItem;
        protected VisualElement m_box;
        protected Text_C m_description;
        protected Toggle_C m_readToggles;
        protected Toggle_C m_WriteToggles;

        private void OnValidate()
        {
            if (m_description == null || m_readToggles == null || m_WriteToggles == null) return;
            m_description.text = DescriptionLabel;

            m_readToggles.Category = Category;
            m_readToggles.Size = Size;
            m_readToggles.Direction = Direction;

            m_WriteToggles.Category = Category;
            m_WriteToggles.Size = Size;
            m_WriteToggles.Direction = Direction;
        }

        public override void SetMenuItem(AbstractMenuItem item)
        {
            if (item is LocalInfoRequestInputMenuItem localInf)
            {
                menuItem = localInf;
                InitAndBindUI();
            }
            else throw new System.Exception("MenuItem must be a BooleanInput");
        }

        public void InitAndBindUI()
        {
            if (m_box != null || m_readToggles != null || m_WriteToggles != null) return;

            m_box = new VisualElement { name = "cookies-box" };

            m_description = new Text_C();
            var dto = (LocalInfoRequestParameterDto)menuItem.dto;
            DescriptionLabel = $"Server {dto.serverName} requests acces to local data : {dto.key}.\n";
            DescriptionLabel += $"{dto.reason}";
            m_description.text = DescriptionLabel;

            m_readToggles = new Toggle_C();
            m_readToggles.Category = Category;
            m_readToggles.Size = Size;
            m_readToggles.Direction = Direction;
            m_readToggles.name = gameObject.name;
            m_readToggles.label = "Read access";
            m_readToggles.RegisterValueChangedCallback(OnReadValueChanged);

            m_WriteToggles = new Toggle_C();
            m_WriteToggles.Category = Category;
            m_WriteToggles.Size = Size;
            m_WriteToggles.Direction = Direction;
            m_WriteToggles.name = gameObject.name;
            m_WriteToggles.label = "Write access";
            m_WriteToggles.RegisterValueChangedCallback(OnWriteValueChanged);

            m_box.Add(m_description);
            m_box.Add(m_readToggles);
            m_box.Add(m_WriteToggles);
        }

        public override void Clear()
        {
            base.Clear();
            m_readToggles.UnregisterValueChangedCallback(OnReadValueChanged);
            m_WriteToggles.UnregisterValueChangedCallback(OnWriteValueChanged);
            m_box.RemoveFromHierarchy();
        }


        public override void Display(bool forceUpdate = false) => m_box.Display();
        public override void Hide() => m_box.Hide();

        public VisualElement GetUXMLContent() => m_box;

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (menu is LocalInfoRequestInputMenuItem) ? 2 : 0;
        }

        private void OnReadValueChanged(ChangeEvent<bool> e)
        {
            if (e.previousValue == e.newValue) return;
            menuItem.NotifyValueChange(new common.interaction.LocalInfoRequestParameterValue(e.newValue, m_WriteToggles.value));
        }

        private void OnWriteValueChanged(ChangeEvent<bool> e)
        {
            if (e.previousValue == e.newValue) return;
            menuItem.NotifyValueChange(new common.interaction.LocalInfoRequestParameterValue(m_readToggles.value, e.newValue));
        }

        private void OnDestroy()
        {
            m_box?.RemoveFromHierarchy();
        }
    }
}
