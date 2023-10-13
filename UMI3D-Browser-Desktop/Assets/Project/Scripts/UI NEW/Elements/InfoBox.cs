using System;
using UnityEngine.UIElements;

public class InfoBox
{
    private VisualElement m_Root;

    public event Action OnOpened;

    public InfoBox(VisualElement pRoot)
    {
        m_Root = pRoot;
        m_Root.Q<Button>("ButtonClose").clicked += Hide;
    }

    public void Show(string pTitle, string pMessage, params Button[] pButtons)
    {
        m_Root.Q<TextElement>("Title").text = pTitle;
        m_Root.Q<TextElement>("Message").text = pMessage;

        var buttons = m_Root.Q("Buttons");
        buttons.Clear();
        foreach (var button in pButtons)
        {
            buttons.Add(button);
        }

        OnOpened?.Invoke();
        m_Root.RemoveFromClassList("hidden");
    }

    public void Hide() => m_Root.AddToClassList("hidden");
}