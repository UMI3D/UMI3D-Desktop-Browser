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
using umi3d.cdk;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueBoxElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<DialogueBoxElement, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits { }

    public void Setup(string title, string message, string optionA, string optionB, Action<bool> choiceCallback) {
        this.style.position = Position.Absolute;
        
        this.Q<Label>("dialogue-box-title").text = title;
        this.Q<Label>("dialogue-box-message").text = message;

        Button optionABtn = this.Q<Button>("dialogue-box-btn1");
        Button optionBBtn = this.Q<Button>("dialogue-box-btn2");

        optionABtn.text = optionA;
        optionABtn.clickable.clicked += () =>
        {
            this.RemoveFromHierarchy();
            choiceCallback.Invoke(true);
        };

        optionBBtn.text = optionB;
        optionBBtn.clickable.clicked += () =>
        {
            this.RemoveFromHierarchy();
            choiceCallback.Invoke(false);
        };

        UMI3DResourcesManager.Instance.StartCoroutine(SetPosition());
    }


    IEnumerator SetPosition()
    {
        //Coroutine required because when the VisualElement is instantiate, its worldBound and style.heigh/width are null.
        yield return null;
        style.top = Screen.height / 2 - resolvedStyle.height / 2;
        style.left = Screen.width / 2 - resolvedStyle.width / 2;
    }
}
