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
using umi3d.baseBrowser.Menu;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Container
{
    public partial class FormContainer : IDisplayerElement
    {
        public UIDocument Document;
        public string ParentUXML;
        public ElementCategory Category;
        public string Title;

        protected Form_C m_form;

        public VisualElement GetUXMLContent() => m_form;

        public void InitAndBindUI()
        {
            m_form = new Form_C(Category, Title, null);
        }

        private void Start()
        {
            Debug.Assert(Document != null, "Document is null");
            InitAndBindUI();
        }
    }

    public partial class FormContainer : AbstractSimpleContainer
    {
        public override void Display(bool forceUpdate = false) => m_form.Display();

        public override void Hide() => m_form.Hide();

        public override void Insert(AbstractDisplayer element, int index, bool updateDisplay = true)
        {
            if (!(element is IDisplayerElement displayer)) return;

            m_form.VScroll.Insert(index, displayer.GetUXMLContent());
            m_displayers.Insert(index, element);
        }

        public override bool Remove(AbstractDisplayer element, bool updateDisplay = true)
        {
            if (!(element is IDisplayerElement displayer)) return false;
            if (!Contains(element)) return false;

            m_form.VScroll.Remove(displayer.GetUXMLContent());
            m_displayers.Remove(element);
            return true;
        }
    }
}
