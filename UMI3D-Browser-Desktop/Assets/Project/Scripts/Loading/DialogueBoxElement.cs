using System;
using System.Collections;
using umi3d.cdk;
using UnityEngine;
using UnityEngine.UIElements;


public class DialogueBoxElement : VisualElement
{
    // UxmlFactory and UxmlTraits allow UIBuilder to use CardElement as a building block
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
