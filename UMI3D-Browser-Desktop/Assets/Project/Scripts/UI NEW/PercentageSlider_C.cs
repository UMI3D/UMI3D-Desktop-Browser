using System;
using System.Linq;
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
            _percentageText.value = value.ToString();
        }
    }

    private VisualElement _percentage = new VisualElement();

    public TextField _percentageText = new TextField();

    private Label _percentageLabel = new Label();

    private string _integers = "0123456789";

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

        _percentageLabel.text = "%";
        _percentageText.value = "0";
        _percentageText.RegisterValueChangedCallback(e =>
        {
            // Check if is a number
            var r = "";
            foreach (var str in e.newValue)
            {
                if (_integers.Contains(str))
                    r += str;
            }
            if (r != e.newValue)
            {
                _percentageText.value = r;
                return;
            }

            // Max 3 number
            if (e.newValue.Count() > 3) 
            {
                _percentageText.value = e.newValue.Substring(0, 3);
                return;
            }

            // Between 0 and 100
            var v = Int32.Parse(e.newValue);
            if (v > 100)
            {
                _percentageText.value = "100";
                return;
            }
            if (v < 0)
            {
                _percentageText.value = "0";
                return;
            }

            // Apply value
            value = v;
        });

        _percentage.Add(_percentageText);
        _percentage.Add(_percentageLabel);

        Add(_percentage);
    }
}