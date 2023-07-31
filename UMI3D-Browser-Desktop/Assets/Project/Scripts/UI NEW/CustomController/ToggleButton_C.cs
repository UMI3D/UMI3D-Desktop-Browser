using UnityEngine.UIElements;

public class ToggleButton_C : BaseBoolField
{
    public new class UxmlFactory : UxmlFactory<ToggleButton_C, UxmlTraits> { }

    public new class UxmlTraits : Button.UxmlTraits
    {

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as ToggleButton_C;
        }
    }

    private VisualElement _background = new VisualElement();
    private VisualElement _circle = new VisualElement();

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
        _background.AddToClassList(UssMainClass + "-background");
        _circle.AddToClassList(UssMainClass + "-circle");
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

        _background.Add(_circle);

        Add(_background);
    }
}