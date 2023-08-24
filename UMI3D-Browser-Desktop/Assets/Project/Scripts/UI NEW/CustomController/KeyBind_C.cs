using UnityEngine;
using UnityEngine.UIElements;
using static Numeral_C;

public class KeyBind_C : VisualElement
{
    public new class UxmlFactory : UxmlFactory<KeyBind_C, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription()
        {
            name = "label",
        };

        protected UxmlStringAttributeDescription m_Key1 = new UxmlStringAttributeDescription()
        {
            name = "key1",
        };

        protected UxmlStringAttributeDescription m_Key2 = new UxmlStringAttributeDescription()
        {
            name = "key2",
        };

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="ve"></param>
        /// <param name="bag"></param>
        /// <param name="cc"></param>
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as KeyBind_C;
            custom.Label = m_Label.GetValueFromBag(bag, cc);
            custom.Key1 = m_Key1.GetValueFromBag(bag, cc);
            custom.Key2 = m_Key2.GetValueFromBag(bag, cc);
        }
    }

    private Label m_Label = new Label();
    private TextField m_Key1 = new TextField();
    private TextField m_Key2 = new TextField();

    public string Label { get => m_Label.text; set => m_Label.text = value; }
    public string Key1 { get => m_Key1.value; set => m_Key1.value = value; }
    public string Key2 { get => m_Key2.value; set => m_Key2.value = value; }

    #region USS
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
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// Add Uss custom classes to this element and its children.
    /// </summary>
    protected virtual void AttachUssClass()
    {
        AddToClassList("unity-base-field");
        AddToClassList("key-bind");
        m_Label.AddToClassList("unity-base-field__label");
        m_Key1.AddToClassList("key-bind__input");
        m_Key2.AddToClassList("key-bind__input");
    }
    #endregion

    public KeyBind_C()
    {
        AttachStyleSheet();
        AttachUssClass();
        InitElement();
    }

    /// <summary>
    /// Initialize this element.
    /// </summary>
    protected virtual void InitElement()
    {
        Add(m_Label);
        Add(m_Key1);
        Add(m_Key2);
    }
}