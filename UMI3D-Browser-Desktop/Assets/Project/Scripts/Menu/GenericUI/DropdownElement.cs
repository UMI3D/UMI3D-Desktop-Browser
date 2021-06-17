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
using UnityEngine.UIElements;

public class DropdownElement : VisualElement
{
    /// <summary>
    /// The USS class which is used to display the label of the different choices.
    /// </summary>
    string labelClassName;

    /// <summary>
    /// To be recognized by UI Builder
    /// </summary>
    public new class UxmlFactory : UxmlFactory<DropdownElement, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits { }

    /// <summary>
    /// Event sent teh value of this element changes.
    /// </summary>
    /// <param name="val"></param>
    public delegate void OnValueChangedDelegate(int val);
    public event OnValueChangedDelegate OnValueChanged;

    int currentChoiceId = 0;

    /// <summary>
    /// The choices of the dropown will be displayed at the root of the renderer.
    /// </summary>
    UIDocument uiDocument;

    Button openChoiceButton;
    Label currentChoice;
    VisualElement choicesDropdown;

    List<string> options;

    bool areChoicesVisible = false;

    public void SetLabel(string label)
    {
        this.Q<Label>("label").text = label;
    }

    public void SetUp(UIDocument uiDocument, string labelClassName)
    {
        this.uiDocument = uiDocument;
        this.labelClassName = labelClassName;

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
            if (areChoicesVisible && options.Count > 1)
                umi3d.cdk.UMI3DResourcesManager.Instance.StartCoroutine(OpenDropdown());
            else
            {
                choicesDropdown.style.display = DisplayStyle.None;
                choicesDropdown.RemoveFromHierarchy();
            }
        };
    }

    /// <summary>
    /// Add choices to the dropdown.
    /// Pre-conditon : 
    ///     - DropdownElement.Setup() must be called before using this method,
    ///     - this contains a tag named "dropdown-open-choice" wich contains a child.
    /// </summary>
    /// <param name="options"></param>
    public void SetOptions(List<string> options)
    {
        if (options.Count > 0)
        {
            this.options = new List<string>(options);

            for (int i = 0; i < options.Count; i++)
            {
                var labelEntry = new Label { text = options[i] };

                labelEntry.userData = i;
                labelEntry.AddToClassList("dark-grey3-bck-hover");
                labelEntry.AddToClassList("white-txt");
                labelEntry.AddToClassList(labelClassName);
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

            if (options.Count == 1)
                this.Q<VisualElement>("dropdown-open-choice")[0].style.display = DisplayStyle.None;
            else
                this.Q<VisualElement>("dropdown-open-choice")[0].style.display = DisplayStyle.Flex;
        } else
        {
            throw new ArgumentException("Option list can not be empty");
        }
    }

    IEnumerator OpenDropdown()
    {
        yield return null;
        this.Focus();

        uiDocument.rootVisualElement.Add(choicesDropdown);
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
