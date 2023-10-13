using UnityEngine.UIElements;

public class BaseSettings
{
    protected VisualElement m_Root;

    protected string m_Name;

    public string Name => m_Name;

    public BaseSettings(VisualElement pRoot) 
    {
        m_Root = pRoot;
        m_Name = pRoot.name;
    }

    public virtual void Hide() => m_Root.AddToClassList("hidden");
    public virtual void Show() => m_Root.RemoveFromClassList("hidden");
}