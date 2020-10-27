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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DropdownElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<DropdownElement, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits { }

    public delegate void OnValueChangedDelegate(int val);
    public event OnValueChangedDelegate OnValueChanged;

    int currentChoiceId = 0;

    Button openChoiceButton;
    Label currentChoice;
    VisualElement choicesDropdown;

    List<string> options;

    bool areChoicesVisible = false;

    bool isMini;

    public void SetLabel(string label)
    {
        this.Q<Label>("label").text = label;
    }

    public void SetUp(bool isMini = false)
    {
        this.isMini = isMini;

        this.RegisterCallback<FocusOutEvent>(e => choicesDropdown.RemoveFromHierarchy());

        openChoiceButton = this.Q<Button>("dropdown-open-choice");
        currentChoice = this.Q<Label>("dropdown-current-choice-label");

        choicesDropdown = this.Q<VisualElement>("dropdown-choices");
        choicesDropdown.style.position = Position.Absolute;
        choicesDropdown.style.display = DisplayStyle.None;

        currentChoice.RegisterCallback<MouseDownEvent>(e =>
        {
            CloseChoices(currentChoice.text, currentChoiceId);
        });

        openChoiceButton.clickable.clicked += () =>
        {
            areChoicesVisible = !areChoicesVisible;
            if (areChoicesVisible)
                ConnectionMenu.Instance.StartCoroutine(OpenDropdown());
            else
            {
                choicesDropdown.style.display = DisplayStyle.None;
                choicesDropdown.RemoveFromHierarchy();
            }
        };
    }

    public void SetOptions(List<string> options)
    {
        if (options.Count > 0)
        {
            this.options = new List<string>(options);

            for (int i = 0; i < options.Count; i++)
            {
                var labelEntry = new Label { text = options[i] };

                labelEntry.style.fontSize = isMini ? 11 : 15;
                labelEntry.style.color = Color.white;
                labelEntry.userData = i;
                labelEntry.style.height = isMini ? 20 : 25;
                labelEntry.style.paddingLeft = 5;
                labelEntry.style.unityTextAlign = TextAnchor.MiddleLeft; 
                labelEntry.RegisterCallback<MouseDownEvent>(e => {
                    CloseChoices(options[(int) labelEntry.userData], (int)labelEntry.userData);
                });
                choicesDropdown.Add(labelEntry);

                if(i == 0)
                {
                    currentChoiceId = 0;
                    currentChoice.text = options[0];
                }
            }
        } else
        {
            throw new ArgumentException("Option list can not be empty");
        }
    }

    IEnumerator OpenDropdown()
    {
        yield return null;
        this.Focus();

        ConnectionMenu.Instance.panelRenderer.visualTree.Add(choicesDropdown);
        choicesDropdown.style.display = DisplayStyle.Flex;
        choicesDropdown.style.top = currentChoice.worldBound.y + currentChoice.worldBound.height;
        choicesDropdown.style.left = currentChoice.worldBound.x;
        choicesDropdown.style.width = currentChoice.worldBound.width + openChoiceButton.worldBound.width;

        choicesDropdown.BringToFront();
    }

    public void ClearOptions()
    {
        choicesDropdown?.Clear();
        options?.Clear();
    }

    public void SetValue(int val)
    {
        if (val >= 0 && val < options.Count)
        {
            currentChoiceId = val;
            currentChoice.text = options[val];
        } 
    }

    private void CloseChoices(string name, int i)
    {
        if (i != currentChoiceId)
        {
            OnValueChanged?.Invoke(i);
            currentChoiceId = i;
            currentChoice.text = name;
        }
        choicesDropdown.style.display = DisplayStyle.None;
    }
}
