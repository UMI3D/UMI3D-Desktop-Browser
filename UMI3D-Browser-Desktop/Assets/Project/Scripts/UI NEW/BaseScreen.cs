using UnityEngine.UIElements;

public class BaseScreen
{
    protected VisualElement _root;

    public VisualElement Root => _root;

    public BaseScreen(VisualElement element)
    {
        _root = element;
    }

    public virtual void Hide() => _root.AddToClassList("hidden");
    public virtual void Show() => _root.RemoveFromClassList("hidden");
}