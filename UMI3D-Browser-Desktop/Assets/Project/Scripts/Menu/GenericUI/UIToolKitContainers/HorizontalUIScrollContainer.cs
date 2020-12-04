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

using System.Linq;
using System.Security.Cryptography;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class HorizontalUIScrollContainer : SimpleUIScrollContainer
    {
        
        [Tooltip("UXML tage name of the back button")]
        public string uxmlSubContentTag;

        public VisualTreeAsset itemTreeAsset;

        VisualElement subContent;
        Button iconSelectionButton;

        bool firstOpening = true;

        protected override void BindUI()
        {
            base.BindUI();
            subContent = containerElement.Q<VisualElement>(uxmlSubContentTag);
            subContent.style.display = DisplayStyle.None;
            if (subContent == null)
            {
                Debug.LogError("There is no UXML tag with the name " + uxmlSubContentTag + " to be uses as a subcontainer");
            }
        }

        public override void Hide()
        {
            base.Hide();
            firstOpening = true;
            subContent.style.display = DisplayStyle.None;
            iconSelectionButton?.RemoveFromClassList("btn-tool-selected");
        }

        public override void Insert(AbstractDisplayer element, bool updateDisplay = true)
        {
            InitAndBindUI();

            if (element is IDisplayerElement elt)
            {
                containedDisplayers.Add(element);

                var uxmlContent = elt.GetUXMLContent();
                uxmlContent.style.marginBottom = uxmlContent.resolvedStyle.marginBottom + spaceBetweenElements;

                if (elt is HorizontalUIScrollContainer horizontalScrollView)
                {
                    horizontalScrollView.MakeIconSelectionButton();
                    contentElement.Add(horizontalScrollView.iconSelectionButton);
                }
                subContent.Add(elt.GetUXMLContent());
                element.transform.SetParent(this.transform, false);

                if (updateDisplay)
                    Display();
            }
            else
            {
                throw new System.NotImplementedException("This container is only made to work with IDisplayerElement. " + element.GetType().ToString() + " displayer given");
            }
        }

        public override void Insert(AbstractDisplayer element, int index, bool updateDisplay = true)
        {
            if (element is IDisplayerElement elt)
            {
                containedDisplayers.Add(element);

                if (elt is HorizontalUIScrollContainer horizontalScrollView)
                {
                    horizontalScrollView.MakeIconSelectionButton();
                    contentElement.Add(horizontalScrollView.iconSelectionButton);
                }
                subContent.Insert(index, elt.GetUXMLContent());
                element.transform.SetParent(this.transform, false);
                element.transform.SetSiblingIndex(index);

                if (updateDisplay)
                    Display();
            }
            else
            {
                throw new System.NotImplementedException("This container is only made to work with IDisplayerElement");
            }
        }

        public override void Expand(bool forceUpdate = false)
        {
            if (!isDisplayed)
            {
                Display(forceUpdate);
            }
            if (isExpanded && !forceUpdate)
            {
                return;
            }

            if (parent is HorizontalUIScrollContainer horizontalContainer)
            {
                horizontalContainer.subContent.style.display = DisplayStyle.Flex;
                iconSelectionButton.AddToClassList("btn-tool-selected");
            }
                
            if (VirtualContainer != null && VirtualContainer != this)
            {
                selectButton.text = menu.Name;
                foreach (AbstractDisplayer displayer in VirtualContainer)
                {
                    if (displayer is IDisplayerElement elt)
                    {
                        displayer.Hide();
                        //TODO
                        (VirtualContainer as HorizontalUIScrollContainer)?.subContent.Add(elt.GetUXMLContent());
                        displayer.transform.SetParent((VirtualContainer as SimpleUIContainer2D)?.transform);
                    }
                    else
                    {
                        throw new System.NotImplementedException("This container is only made to work with IDisplayerElement");
                    }
                }
            }
            //contentElement.style.display = DisplayStyle.Flex;
            if(firstOpening && containedDisplayers.Count > 0 && containedDisplayers[0] is AbstractMenuDisplayContainer)
                firstOpening = false;
            else
                subContent.style.display = DisplayStyle.Flex;

            if (containedDisplayers.Count(d => d is AbstractMenuDisplayContainer) > 0)
                scrollView.style.display = DisplayStyle.Flex;
            else
                scrollView.style.display = DisplayStyle.None;

            VirtualContainer = this;

            if (selectButton != null)
                selectButton.clickable.clicked -= Select;

            HideBackButton();

            foreach (AbstractDisplayer displayer in this)
            {
                if (displayer is IDisplayerElement elt)
                    displayer.Display();
                else
                    throw new System.NotImplementedException("This container is only made to work with IDisplayerElement");
            }

            isExpanded = true;
        }

        public override void Collapse(bool forceUpdate = false)
        {
            scrollView.style.display = DisplayStyle.None;

            if (!isExpanded && !forceUpdate)
            {
                return;
            }

            if (VirtualContainer != null && VirtualContainer != this)
            {
                selectButton.text = menu.Name;
                foreach (AbstractDisplayer displayer in VirtualContainer)
                {
                    if (displayer is IDisplayerElement elt)
                    {
                        displayer.Hide();
                        //TODO
                        (VirtualContainer as HorizontalUIScrollContainer)?.subContent.Add(elt.GetUXMLContent());
                        displayer.transform.SetParent((VirtualContainer as SimpleUIContainer2D)?.transform);
                    }
                    else
                    {
                        throw new System.NotImplementedException("This container is only made to work with IDisplayerElement");
                    }
                }
                VirtualContainer = this;
            }

            HideBackButton();

            //contentElement.style.display = DisplayStyle.None;
            iconSelectionButton.RemoveFromClassList("btn-tool-selected");
            subContent.style.display = DisplayStyle.None;

            if (backButton != null)
                backButton.style.display = DisplayStyle.None;
            if (selectButton != null)
                selectButton.clickable.clicked += Select;

            foreach (AbstractDisplayer displayer in this)
            {
                if (displayer is IDisplayerElement)
                    displayer.Hide();
            }
            isExpanded = false;
        }

        public override void ExpandAs(AbstractMenuDisplayContainer Container, bool forceUpdate = false)
        {
            if (isExpanded && !forceUpdate)
            {
                return;
            }
            scrollView.style.display = DisplayStyle.Flex;
            
            if (VirtualContainer != null && VirtualContainer != Container)
            {
                if (backButton != null)
                    backButton.style.display = DisplayStyle.None;
                if (selectButton != null)
                    selectButton.clickable.clicked += Select;


                foreach (AbstractDisplayer displayer in this)
                {
                    if (displayer is IDisplayerElement)
                        displayer.Hide();
                }
            }

            VirtualContainer = Container;
            selectButton.text = Container.menu.Name;

            //contentElement.style.display = DisplayStyle.Flex;
            subContent.style.display = DisplayStyle.Flex;

            if (selectButton != null)
                selectButton.clickable.clicked -= Select;

            foreach (AbstractDisplayer displayer in VirtualContainer)
            {
                if (displayer is IDisplayerElement elt)
                {
                    subContent.Add(elt.GetUXMLContent());
                    //TODO
                    displayer.transform.SetParent(this.transform);
                    displayer.Display();
                }
            }
            isExpanded = true;
        }

        private void MakeIconSelectionButton()
        {
            var item = itemTreeAsset.CloneTree();
            item.name = name + "-scrollview-icon";

            string label = menu.Name;
            if (menu.Name.Length > 8)
            {
                label = label.Substring(0, 6) + "..";
            }
            item.Q<Label>().text = label;
            iconSelectionButton = item.Q<Button>();

            if (menu.icon2D != null)
            {
                item.Q<VisualElement>("icon").style.backgroundImage = menu.icon2D;
            }

            iconSelectionButton.clickable.clicked += () =>
            {
                foreach (var child in parent)
                {
                    if (child != this)
                        child.Hide();
                }
                Select();
            };
        }

    }
}
