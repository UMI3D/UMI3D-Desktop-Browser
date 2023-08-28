using System.Threading.Tasks;
using UnityEngine.UIElements;

public class BaseScreen
{
    private const int k_TooltipDelay = 1000;

    protected VisualElement m_Root;
    private VisualElement m_Tooltip;

    private bool m_MustShowTooltip;

    private const int k_buttonCooldown = 1500;
    protected bool m_IsAButtonAlreadyPressed;

    public VisualElement Root => m_Root;

    public BaseScreen(VisualElement pElement)
    {
        m_Root = pElement;
        m_Tooltip = m_Root.parent.Q("Tooltip");
    }

    public async void ShowTooltip(VisualElement pTarget, string pMessage)
    {
        m_MustShowTooltip = true;

        m_Tooltip.Q<TextElement>("Message").text = pMessage;
        m_Tooltip.style.left = pTarget.worldBound.center.x - 8;
        m_Tooltip.style.top = pTarget.worldBound.yMin + 4;

        await Task.Delay(k_TooltipDelay);

        if (!m_MustShowTooltip) return;

        m_Tooltip.RemoveFromClassList("hidden");
    }

    public void HideTooltip()
    {
        m_MustShowTooltip = false;
        m_Tooltip.AddToClassList("hidden");
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