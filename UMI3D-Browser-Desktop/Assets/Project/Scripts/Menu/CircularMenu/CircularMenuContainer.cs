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
using System.Collections.Generic;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class CircularMenuContainer : SimpleUIContainer2D
    {
        public int radius;
        public int sameTimeDisplayable = 8;
        public int currentFirstDisplayed = 0;

        [Tooltip("Offset from the top left corner to the center of the menu")]
        [SerializeField] Vector2 offset = Vector2.zero;
        [SerializeField] string viewTagName = null;
        VisualElement view = null;
        [SerializeField] string nextButtonTagName = null;
        Button nextButton;
        [SerializeField] string previousButtonTagName = null;
        Button previousButton;

        int count;

        protected override void BindUI()
        {
            base.BindUI();

            containerElement.style.flexGrow = 1;

            view = containerElement.Q<VisualElement>(viewTagName);
            previousButton = containerElement.Q<Button>(previousButtonTagName);
            nextButton = containerElement.Q<Button>(nextButtonTagName);

            nextButton.clickable.clicked += Next;
            previousButton.clickable.clicked += Previous;
        }


        public override void Collapse(bool forceUpdate = false)
        {
            base.Collapse(forceUpdate);
            view.style.display = DisplayStyle.None;
        }

        public override void Display(bool forceUpdate)
        {
            CircularMenu.Setup(this);
            base.Display(forceUpdate);
            currentFirstDisplayed = 0;
            OrganiseChildren();
        }

        public void ShowMenu()
        {
            parentElement.style.display = DisplayStyle.Flex;
        }

        public void HideMenu()
        {
            parentElement.style.display = DisplayStyle.None;
        }

        public override void Expand(bool forceUpdate = false)
        {
            base.Expand(forceUpdate);
            currentFirstDisplayed = 0;
            OrganiseChildren();
            view.style.display = DisplayStyle.Flex;
        }

        public override void ExpandAs(AbstractMenuDisplayContainer Container, bool forceUpdate = false)
        {
            base.ExpandAs(Container, forceUpdate);
            OrganiseChildren();
        }

        /// <summary>
        /// Organises items along the circular menu.
        /// </summary>
        public void OrganiseChildren()
        {
            if (VirtualContainer == null)
                VirtualContainer = this;
            count = VirtualContainer.Count();

            if (count == 0) return;
            if (count > sameTimeDisplayable)
            {
                int diff = count - currentFirstDisplayed;
                if (diff < sameTimeDisplayable)
                {
                    count = diff + 1;
                }
                else
                    count = sameTimeDisplayable + 1;

                nextButton.style.display = DisplayStyle.Flex;
                previousButton.style.display = DisplayStyle.Flex;
            } else
            {
                nextButton.style.display = DisplayStyle.None;
                previousButton.style.display = DisplayStyle.None;
            }

            float angle = 360 / (count);
            for (int i = 0; i < VirtualContainer.Count(); i++)
            {
                if (i >= currentFirstDisplayed && i < (currentFirstDisplayed + sameTimeDisplayable))
                {
                    StartCoroutine(SetDisplayerPosition(VirtualContainer[i], i, angle));
                }
                else
                {
                    HideDisplayer(VirtualContainer[i]);
                }
            }
        }

        /// <summary>
        /// Sets the position of an item along the circular menu and displays it.
        /// </summary>
        private IEnumerator SetDisplayerPosition(AbstractDisplayer displayer, int i, float angle)
        {
            yield return null;

            if (displayer != null) // The gameobject could have been destroyed at th end of the previous frame;
            {
                if (displayer is IDisplayerElement displayerElement)
                {
                    displayer.Display(true);
                    VisualElement elt = displayerElement.GetUXMLContent();
                    elt.style.position = Position.Absolute;

                    var angleRes = (-i + currentFirstDisplayed - 1) * - angle;

                    Vector3 dir = (Quaternion.AngleAxis(angleRes , Vector3.forward) * Vector3.down).normalized;
                    Vector3 center = - new Vector3(elt.worldBound.width, elt.worldBound.height , 0)/2;

                    //elt.transform.position = (Vector3)offset - dir * radius;  
                    elt.style.left = 0;
                    elt.style.top = 0;

                    yield return null;

                    center = -new Vector3(elt.worldBound.width, elt.worldBound.height, 0) / 2;
                    elt.style.left = ((Vector3)offset + center - dir * radius).x;
                    elt.style.top = ((Vector3)offset + center - dir * radius).y;

                }
                else
                {
                    throw new System.NotImplementedException("This container is only made to work with IDisplayerElement");
                }
            }
        }

        private void HideDisplayer(AbstractDisplayer displayer)
        {
            if (displayer is IDisplayerElement displayerElement)
            {
                displayer.Hide();
            }
            else
            {
                throw new System.NotImplementedException("This container is only made to work with IDisplayerElement");
            }
        }

        private void Next()
        {
            currentFirstDisplayed += sameTimeDisplayable;
            if (currentFirstDisplayed > VirtualContainer.Count()) currentFirstDisplayed = 0;
            OrganiseChildren();
        }

        private void Previous()
        {
            currentFirstDisplayed -= sameTimeDisplayable;
            int count = VirtualContainer.Count();
            if (currentFirstDisplayed < 0) currentFirstDisplayed = count - (count % sameTimeDisplayable);
            OrganiseChildren();
        }

        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            OrganiseChildren();
        }
    }
}