using UnityEngine.UIElements;

public class BaseScreen
{
    protected VisualElement _root;
    protected Button _back;
    protected Button _next;

    public VisualElement Root => _root;
    public Button Back => _back;
    public Button Next => _next;

    public BaseScreen(VisualElement element)
    {
        _root = element;
        _back = _root.Q<Button>("ButtonBack");
        _next = _root.Q<Button>("ButtonSubmit");
    }

    public virtual void Hide() => _root.AddToClassList("hidden");
    public virtual void Show() => _root.RemoveFromClassList("hidden");
}