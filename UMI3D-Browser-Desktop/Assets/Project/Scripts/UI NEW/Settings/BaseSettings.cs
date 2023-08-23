using UnityEngine.UIElements;

public class BaseSettings
{
    private VisualElement m_Root;

    private string m_Name;

    public string Name => m_Name;

    public BaseSettings(VisualElement pRoot) 
    {
        m_Root = pRoot;
        m_Name = pRoot.name;
    }

    public virtual void Hide() => m_Root.AddToClassList("hidden");
    public virtual void Show() => m_Root.RemoveFromClassList("hidden");
}