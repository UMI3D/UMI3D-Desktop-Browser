using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    public TMP_Text Title;
    public TMP_Text Info;
    public TMP_Text optionALabel;
    public Button optionA;
    public TMP_Text optionBLabel;
    public Button optionB;

    public void Setup(string title,string info,string optionA,string optionB,Action<bool> callback)
    {

        Title.text = title;
        Info.text = info;
        optionALabel.text = optionA;
        optionBLabel.text = optionB;
        this.optionA.onClick.RemoveAllListeners();
        this.optionB.onClick.RemoveAllListeners();
        this.optionA.onClick.AddListener(()=>callback(true));
        this.optionB.onClick.AddListener(() => callback(true));

    }

}
