using umi3d.commonScreen;
using UnityEngine.UIElements;

public class Button_C : Button
{
    public new class UxmlFactory : UxmlFactory<Button_C, UxmlTraits> { }

    public new class UxmlTraits : Button.UxmlTraits
    {
        protected UxmlLocaliseAttributeDescription m_localisedText = new UxmlLocaliseAttributeDescription
        {
            name = "localised-text"
        };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as Button_C;

            custom.LocaliseText = m_localisedText.GetValueFromBag(bag, cc);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <remarks> Use <see cref="LocaliseText"/> instead. </remarks>
    public override string text { get => base.text; set => base.text = value; }
       
    public virtual LocalisationAttribute LocaliseText
    {
        get => _text.LocalisedText;
        set
        {
            if (value.IsEmpty) _text.RemoveFromHierarchy();
            else Add(_text);
            _text.LocalisedText = value;
        }
    }

    public virtual VectorImage VectorImage
    {
        get => _image.vectorImage;
        set
        {
            if (value == null) _image.RemoveFromHierarchy();
            else Add(_image);
            _image.vectorImage = value;
        }
    }

    private Text_C _text = new Text_C();
    private Image _image = new Image();

    #region USS
    public virtual string UssMainClass => $"button";

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
        RemoveFromClassList("unity-button");
        RemoveFromClassList("unity-text-element");
        AddToClassList(UssMainClass);
    }
    #endregion

    public Button_C()
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

    }
}