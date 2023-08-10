using System.Threading.Tasks;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.FilterWindow;

public class BaseScreen
{
    private const int k_tooltipDelay = 1000;

    protected VisualElement _root;

    public VisualElement Root => _root;
    private VisualElement _tooltip;
    private bool _mustShowTooltip;

    public BaseScreen(VisualElement pElement)
    {
        _root = pElement;
        _tooltip = _root.parent.Q("Tooltip");
    }

    public async void ShowTooltip(VisualElement pTarget, string pMessage)
    {
        _mustShowTooltip = true;

        _tooltip.Q<TextElement>("Message").text = pMessage;
        _tooltip.style.left = pTarget.worldBound.center.x - 8;
        _tooltip.style.top = pTarget.worldBound.yMin + 4;

        await Task.Delay(k_tooltipDelay);

        if (!_mustShowTooltip) return;

        _tooltip.RemoveFromClassList("hidden");
    }

    public void HideTooltip()
    {
        _mustShowTooltip = false;
        _tooltip.AddToClassList("hidden");
    }

    public virtual void Hide() => _root.AddToClassList("hidden");
    public virtual void Show() => _root.RemoveFromClassList("hidden");
}