using System.Threading.Tasks;
using UnityEngine.UIElements;

public class Tooltip
{
    private const int k_TooltipDelay = 1000;

    private VisualElement m_Root;

    private bool m_MustShowTooltip;

    public Tooltip(VisualElement pRoot)
    {
        m_Root = pRoot;
    }
 
    public async void Show(VisualElement pTarget, string pMessage)
    {
        m_MustShowTooltip = true;

        m_Root.Q<TextElement>("Message").text = pMessage;
        m_Root.style.left = pTarget.worldBound.center.x - 8;
        m_Root.style.top = pTarget.worldBound.yMin + 4;

        await Task.Delay(k_TooltipDelay);

        if (!m_MustShowTooltip) return;

        m_Root.RemoveFromClassList("hidden");
    }

    public void Hide()
    {
        m_MustShowTooltip = false;
        m_Root.AddToClassList("hidden");
    }
}