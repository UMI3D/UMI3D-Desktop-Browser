using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Numeral_C : TextField
{
    public enum NumeralType
    {
        Custom,
        Percentage,
        Second,
    }

    public new class UxmlFactory : UxmlFactory<Numeral_C, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription()
        {
            name = "label",
        };

        protected UxmlIntAttributeDescription m_Value = new UxmlIntAttributeDescription()
        {
            name = "value",
        };

        protected UxmlEnumAttributeDescription<NumeralType> m_Type = new UxmlEnumAttributeDescription<NumeralType>()
        {
            name= "type",
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
            var custom = ve as Numeral_C;
            custom.label = m_Label.GetValueFromBag(bag, cc);
            custom.value = m_Value.GetValueFromBag(bag, cc).ToString();
            custom.Type = m_Type.GetValueFromBag(bag, cc);
        }
    }

    private Label m_UnitText = new Label() { name = "Unit" };

    private string m_Integers = "0123456789.";

    private string m_Unit;

    private NumeralType m_Type = NumeralType.Custom;

    public string Unit { 
        get => m_Unit; 
        set
        {
            if (value != "")
            {
                m_Unit = value;
                if (m_UnitText.parent == null) Add(m_UnitText);
                m_UnitText.text = m_Unit;
            } else
            {
                m_UnitText.RemoveFromHierarchy();
            }
        } 
    }

    public NumeralType Type { 
        get => m_Type; 
        set {
            m_Type = value;
            switch (m_Type)
            {
                case NumeralType.Custom:
                    break;
                case NumeralType.Percentage:
                    Unit = "%";
                    break;
                case NumeralType.Second:
                    m_UnitText.text = "s";
                    break;
                default: 
                    break;
            }
        }
    }

    public event Action<float> OnChanged;

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
        AddToClassList("numeral");
        m_UnitText.AddToClassList("unit");
    }
    #endregion

    public Numeral_C()
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
        value = "0";
        this.RegisterValueChangedCallback(e =>
        {
            if (e.newValue == "")
            {
                SetValueWithoutNotify("0");
                OnChanged?.Invoke(0);
            }

            // Check if is a number
            var r = "";
            var foundDot = false;
            foreach (var str in e.newValue)
            {
                if (m_Integers.Contains(str))
                {
                    if (str == '.')
                        if (foundDot)
                            continue;
                        else
                            foundDot = true;
                    r += str;
                }
            }
            if (r != e.newValue)
            {
                value = r;
                return;
            }

            float.TryParse(e.newValue, out var v);

            if (m_Type == NumeralType.Percentage)
            {
                if (v < 0)
                {
                    value = "0";
                    return;
                }
                if (v > 100)
                {
                    value = "100";
                    return;
                }
            }

            // Apply value
            OnChanged?.Invoke(v);
        });

        Add(m_UnitText);
    }
}