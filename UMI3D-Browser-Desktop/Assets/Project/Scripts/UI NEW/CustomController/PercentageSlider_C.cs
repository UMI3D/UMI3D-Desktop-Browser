using UnityEngine;
using UnityEngine.UIElements;

public class PercentageSlider_C : SliderInt
{
    public new class UxmlFactory : UxmlFactory<PercentageSlider_C, UxmlTraits> { }

    public override int value { 
        get => base.value;
        set
        {
            base.value = value;
            _percentage.value = value.ToString();
        }
    }

    private Numeral_C _percentage = new Numeral_C() { name = "Percentage" };

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
        _percentage.AddToClassList("percentage");
    }
    #endregion

    public PercentageSlider_C()
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

        _percentage.Type = Numeral_C.NumeralType.Percentage;

        _percentage.OnChanged += newValue =>
        {
            value = newValue;
        };

        Add(_percentage);
    }
}