using System.Collections.Generic;
using UnityEngine.UIElements;

public struct TooltipElement
{
    public VisualElement Target;
    public string Message;

    public TooltipElement(VisualElement pTarget, string pMessage)
    {
        Target = pTarget; 
        Message = pMessage;
    }
}

public class ScreenTooltip
{
    private VisualElement m_Root;
    private Button m_PrevButton;
    private Button m_NextButton;
    private Button m_DoneButton;
    private TextElement m_Message;

    private List<TooltipElement> m_LstTooltip;
    private int m_CurrentTooltipIndex;

    public List<TooltipElement> LstTooltip { get => m_LstTooltip; set => m_LstTooltip = value; }

    public ScreenTooltip(VisualElement pRoot)
    {
        m_Root = pRoot;

        m_PrevButton = m_Root.Q<Button>("ButtonPrev");
        m_PrevButton.clicked += Previous;
        m_NextButton = m_Root.Q<Button>("ButtonNext");
        m_NextButton.clicked += Next;
        m_DoneButton = m_Root.Q<Button>("ButtonDone");
        m_DoneButton.clicked += () => m_Root.AddToClassList("hidden");
        m_Root.Q<Button>("ButtonClose").clicked += () => m_Root.AddToClassList("hidden");

        m_Message = m_Root.Q<TextElement>("Message");
    }

    public void Show()
    {
        if (m_LstTooltip == null || m_LstTooltip.Count == 0) return;
        Display(0);

        m_Root.RemoveFromClassList("hidden");
    }

    private void Next()
    {
        Display(m_CurrentTooltipIndex + 1);
    }

    private void Previous()
    {
        Display(m_CurrentTooltipIndex - 1);
    }

    private void Display(int index)
    {
        m_CurrentTooltipIndex = index;
        if (m_CurrentTooltipIndex == 0)
            m_PrevButton.AddToClassList("hidden");
        else
            m_PrevButton.RemoveFromClassList("hidden");


        if (m_CurrentTooltipIndex == m_LstTooltip.Count - 1)
        {
            m_NextButton.AddToClassList("hidden");
            m_DoneButton.RemoveFromClassList("hidden");
        }
        else
        {
            m_NextButton.RemoveFromClassList("hidden");
            m_DoneButton.AddToClassList("hidden");
        }

        m_Message.text = m_LstTooltip[m_CurrentTooltipIndex].Message;
        m_Root.style.left = m_LstTooltip[m_CurrentTooltipIndex].Target.worldBound.center.x;
        m_Root.style.top = m_LstTooltip[m_CurrentTooltipIndex].Target.worldBound.yMin - 134;
    }
}