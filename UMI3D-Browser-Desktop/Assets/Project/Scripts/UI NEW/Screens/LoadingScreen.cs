using umi3d.cdk;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadingScreen : BaseScreen
{
    private ProgressBar m_ProgressBar;
    private TextElement m_ProressText;
    private TextElement m_Message;

    public float ProgressValue
    {
        get => m_ProgressBar.value;
        set 
        {
            m_ProgressBar.value = value;
        }
    }

    public string ProgressValueText
    {
        get => m_ProressText.text;
        set => m_ProressText.text = value;
    }

    public string Message
    {
        get => m_Message.text;
        set => m_Message.text = value;
    }

    public LoadingScreen(VisualElement pRoot) : base(pRoot)
    {
        m_ProgressBar = m_Root.Q<ProgressBar>("LoadingProgress");
        m_ProressText = m_Root.Q<TextElement>("ProgressText");
        m_Message = m_Root.Q<TextElement>("ProgressInfo");
    }
}