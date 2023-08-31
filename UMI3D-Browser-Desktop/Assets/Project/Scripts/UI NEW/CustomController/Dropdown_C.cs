using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class Dropdown_C : DropdownField
{
    public new class UxmlFactory : UxmlFactory<Dropdown_C, UxmlTraits> { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <remarks> Use <see cref="LocalisedLabel"/> instead. </remarks>
    public new string label { get => base.label; set => base.label = value; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <remarks> Use <see cref="LocalisedOptions"/> instead. </remarks>
    public override List<string> choices { 
        get => base.choices; 
        set 
        { 
            base.choices = value; 
            if (m_AllChoices == null) m_AllChoices = new List<string>();
            if (m_AllChoices.Count == 0)
                m_AllChoices = value; 
        } 
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <remarks> Use <see cref="LocalisedValue"/> instead. </remarks>
    public override string value
    { 
        get => base.value;
        set
        {
            if (base.value == value || !choices.Contains(value)) return;

            base.choices = new List<string>(m_AllChoices);
            base.choices.Remove(value);

            base.value = value;
        }
    }

    private List<string> m_AllChoices = new List<string>();

    public Dropdown_C()
    {
        AttachStyleSheet();
        AttachUssClass();
        InitElement();
    }

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

    }

    /// <summary>
    /// Initialise this element.
    /// </summary>
    protected virtual void InitElement()
    {

    }
}