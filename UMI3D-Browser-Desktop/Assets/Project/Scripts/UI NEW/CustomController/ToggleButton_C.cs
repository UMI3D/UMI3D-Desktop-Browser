using UnityEngine.UIElements;

public class ToggleButton_C : BaseBoolField
{
    public new class UxmlFactory : UxmlFactory<ToggleButton_C, UxmlTraits> { }

    private VisualElement m_Background = new VisualElement();
    private VisualElement m_Circle = new VisualElement();

    #region USS
    public virtual string UssMainClass => $"toggle-button";

    /// <summary>
    /// Add style and theme style sheets to this element.
    /// </summary>
    protected virtual void AttachStyleSheet_Impl()
    {

    }

    private void AttachStyleSheet()
    {
        try
        {
            AttachStyleSheet_Impl();
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
    }

    /// <summary>
    /// Add Uss custom classes to this element and its children.
    /// </summary>
    protected virtual void AttachUssClass()
    {
        RemoveFromClassList("unity-toggle");
        RemoveFromClassList("unity-text-element");
        AddToClassList(UssMainClass);
        m_Background.AddToClassList(UssMainClass + "-background");
        m_Circle.AddToClassList(UssMainClass + "-circle");
    }
    #endregion

    public ToggleButton_C() : base("")
    {
        AttachStyleSheet();
        AttachUssClass();
        InitElement();
    }

    /// <summary>
    /// Initialise this element.
    /// </summary>
    protected virtual void InitElement()
    {
        m_CheckMark.RemoveFromHierarchy();

        m_Background.Add(m_Circle);

        Add(m_Background);
    }
}