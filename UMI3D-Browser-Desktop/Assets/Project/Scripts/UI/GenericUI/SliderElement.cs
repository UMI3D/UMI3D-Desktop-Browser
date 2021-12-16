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
using System.Linq;
using umi3d.cdk;
using UnityEngine.UIElements;

public class SliderElement
{
    int itemsPerPage = 4;

    List<VisualElement> items;

    VisualElement container;
    List<VisualElement> subContainers;
    List<Button> navigationCircles;
    VisualElement circlesContainer;

    int currentPageDisplayed = 0;

    public void SetUp(VisualElement root)
    {
        items = new List<VisualElement>();

        this.container = root.Q<VisualElement>("slider-container");
        this.circlesContainer = root.Q<VisualElement>("slider-circles");

        container.Clear();
        circlesContainer.Clear();
    }

    public void AddElement(VisualElement item)
    {
        items.Add(item);
        UMI3DResourcesManager.Instance.StartCoroutine(DisplayItems());
    }

    public void RemoveElement(VisualElement item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
        UMI3DResourcesManager.Instance.StartCoroutine(DisplayItems());
    }

    public void ClearItems()
    {
        items.Clear();
    }

    public IEnumerator DisplayItems()
    {
        yield return null;

        container.Clear();
        circlesContainer.Clear();
        subContainers = new List<VisualElement>();
        navigationCircles = new List<Button>();

        for (int i = 0; i < NbPagesDisplayed(); i++)
        {
            VisualElement subContainer = new VisualElement();
            subContainer.style.flexDirection = FlexDirection.Row;
            subContainer.style.flexWrap = Wrap.Wrap;
            subContainer.style.justifyContent = Justify.SpaceBetween;
            subContainer.style.width = container.resolvedStyle.width;
            subContainer.style.minWidth = container.resolvedStyle.width;

            for (int j = 0; j < itemsPerPage; j++)
            {
                int index = i * itemsPerPage + j;
                if (index < items.Count)
                {
                    subContainer.Add(items[index]);
                }
            }

            Button circleButton = new Button();
            circleButton.userData = i;
            navigationCircles.Add(circleButton);
            if (i == 0)
                circleButton.AddToClassList("slider-circle-active");
            else
                circleButton.AddToClassList("slider-circle-desactivate");

            circleButton.clickable.clicked += () => MoveToPage((int)circleButton.userData);

            circlesContainer.Add(circleButton);
            container.Add(subContainer);
            subContainers.Add(subContainer);
        }

        if (NbPagesDisplayed() == 1)
            navigationCircles.First().style.display = DisplayStyle.None;

        MoveToPage(currentPageDisplayed);
    }

    private int NbPagesDisplayed()
    {
        if (items.Count() % itemsPerPage == 0)
        {
            return (int)(items.Count() / itemsPerPage);
        } else
        {
            return (int)(items.Count() / itemsPerPage) + 1;
        }
    }

    private void MoveToPage(int pageToGo)
    {
        if (pageToGo == currentPageDisplayed)
            return;

        float previousOffset = subContainers[0].resolvedStyle.right;
        float offset = (pageToGo - currentPageDisplayed) * container.resolvedStyle.width - previousOffset;

        subContainers[0].experimental.animation.Start(previousOffset, offset, 200, (elt, val) =>
        {
            foreach (var subContainer in subContainers)
            {
                subContainer.style.right = val;
            }
        });

        navigationCircles.ForEach(c =>
        {
            c.ClearClassList();
            if ((int)c.userData == pageToGo)
                c.AddToClassList("slider-circle-active");
            else
                c.AddToClassList("slider-circle-desactivate");
        });

        currentPageDisplayed = pageToGo;
    }
}
