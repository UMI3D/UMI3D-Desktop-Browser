using UnityEngine;
using UnityEngine.UIElements;
using static Numeral_C;

public class SliderFloat_C : Slider
{
    public new class UxmlFactory : UxmlFactory<SliderFloat_C, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription()
        {
            name = "label",
        };

        protected UxmlFloatAttributeDescription m_LowValue = new UxmlFloatAttributeDescription()
        {
            name = "lowValue",
        };

        protected UxmlFloatAttributeDescription m_HighValue = new UxmlFloatAttributeDescription()
        {
            name = "highValue",
        };

        protected UxmlFloatAttributeDescription m_Value = new UxmlFloatAttributeDescription()
        {
            name = "value",
        };

        protected UxmlEnumAttributeDescription<NumeralType> m_Type = new UxmlEnumAttributeDescription<NumeralType>()
        {
            name = "type",
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
            var custom = ve as SliderFloat_C;
            custom.label = m_Label.GetValueFromBag(bag, cc);
            custom.value = m_Value.GetValueFromBag(bag, cc);
            custom.lowValue = m_LowValue.GetValueFromBag(bag, cc);
            custom.highValue = m_HighValue.GetValueFromBag(bag, cc);
            custom.Number.Type = m_Type.GetValueFromBag(bag, cc);
        }
    }

    public override float value { 
        get => base.value;
        set
        {
            base.value = value;
            m_Number.value = value.ToString();
        }
    }

    private Numeral_C m_Number = new Numeral_C() { name = "Number" };

    public Numeral_C Number => m_Number;

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
        RemoveFromClassList("unity-label");
        m_Number.AddToClassList("slider-number");
    }
    #endregion

    public SliderFloat_C()
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
        highValue = 100;

        m_Number.OnChanged += newValue =>
        {
            value = (float)System.Math.Round(newValue, 2);
        };

        Add(m_Number);
    }
}