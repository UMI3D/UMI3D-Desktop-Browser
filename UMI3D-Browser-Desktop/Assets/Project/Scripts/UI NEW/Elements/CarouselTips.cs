using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CarouselTips
{
    private VisualElement m_Root;
    private RadioButtonGroup m_NavigationButtons;
    private TextElement m_Title;
    private TextElement m_Message;
    private VisualElement m_PreviousImage;
    private VisualElement m_MainImage;
    private VisualElement m_NextImage;

    private List<RadioButton> m_NavigationButtonsList;

    private List<Tip> m_LstTip;
    private int m_CurrentTipIndex;

    private Tip m_CurrentTip => m_LstTip[m_CurrentTipIndex];

    public CarouselTips(VisualElement pRoot)
    {
        m_Root = pRoot;
        var tipsTables = Resources.LoadAll<TipsTable>("");
        m_LstTip = new List<Tip>();

        foreach (var tipsTable in tipsTables)
        {
            m_LstTip.AddRange(tipsTable.Tips);
        }

        m_NavigationButtons = m_Root.Q<RadioButtonGroup>();
        m_Title = m_Root.Q<TextElement>("Title");
        m_Message = m_Root.Q<TextElement>("Message");
        m_PreviousImage = m_Root.Q("PrevTip").Q("Background");
        m_MainImage = m_Root.Q("MainTip").Q("Background");
        m_NextImage = m_Root.Q("NextTip").Q("Background");

        InitializeNavigationButtons();
        GoTo(0);
    }

    private void InitializeNavigationButtons()
    {
        m_NavigationButtonsList = new();
        m_NavigationButtons.Clear();
        foreach (Tip tip in m_LstTip)
        {
            var button = new RadioButton();
            button.RegisterValueChangedCallback(e =>
            {
                if (e.newValue)
                    GoTo(m_LstTip.IndexOf(tip));
            });
            m_NavigationButtons.Add(button);
            m_NavigationButtonsList.Add(button);
        }

        m_Root.Q<Button>("ButtonPrevious").clicked += () =>
        {
            GoTo(m_CurrentTipIndex - 1);
        };

        m_Root.Q<Button>("ButtonNext").clicked += () =>
        {
            GoTo(m_CurrentTipIndex + 1);
        };
    }

    private void GoTo(int index)
    {
        m_CurrentTipIndex = index;
        if (m_CurrentTipIndex < 0) m_CurrentTipIndex = m_LstTip.Count - 1;
        if (m_CurrentTipIndex >= m_LstTip.Count) m_CurrentTipIndex = 0;

        m_Title.text = m_CurrentTip.Title;
        m_Message.text = m_CurrentTip.Message;
        m_MainImage.style.backgroundImage = m_CurrentTip.Image;
        m_NavigationButtonsList[m_CurrentTipIndex].value = true;

        Debug.Log(m_PreviousImage);
        m_PreviousImage.style.backgroundImage = m_CurrentTipIndex > 0 ? m_LstTip[m_CurrentTipIndex-1].Image : null;
        m_NextImage.style.backgroundImage = m_CurrentTipIndex < m_LstTip.Count - 1 ? m_LstTip[m_CurrentTipIndex+1].Image : null;
    }
}