using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class BaseScreen
{
    private const int k_buttonCooldown = 1500;

    protected VisualElement m_Root;

    protected InfoBox m_InfoBox;
    protected InfoBox m_ErrorBox;

    protected Tooltip m_Tooltip;

    protected bool m_IsAButtonAlreadyPressed;

    protected List<TooltipElement> m_LstTooltip;

    public VisualElement Root => m_Root;
    public List<TooltipElement> LstTooltip => m_LstTooltip;


    public BaseScreen(VisualElement pRoot)
    {
        m_Root = pRoot;

        m_InfoBox = new InfoBox(m_Root.parent.Q("InfoBox"));
        m_ErrorBox = new InfoBox(m_Root.parent.Q("ErrorBox"));

        m_InfoBox.OnOpened += m_ErrorBox.Hide;
        m_ErrorBox.OnOpened += m_InfoBox.Hide;

        m_Tooltip = new Tooltip(m_Root.parent.Q("Tooltip"));

        m_LstTooltip = new();
    }

    protected async void ButtonActivated()
    {
        m_IsAButtonAlreadyPressed = true;
        await Task.Delay(k_buttonCooldown);
        m_IsAButtonAlreadyPressed = false;
    }

    public virtual void Hide() => m_Root.AddToClassList("hidden");
    public virtual void Show() => m_Root.RemoveFromClassList("hidden");
}