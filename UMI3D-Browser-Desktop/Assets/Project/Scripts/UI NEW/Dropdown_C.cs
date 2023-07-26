using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class Dropdown_C : DropdownField
{
    public new class UxmlFactory : UxmlFactory<Dropdown_C, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as Dropdown_C;
        }
    }

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
            if (_allChoices.Count == 0)
                _allChoices = value; 
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

            base.choices = new List<string>(_allChoices);
            base.choices.Remove(value);

            base.value = value;
        }
    }

    private List<string> _allChoices = new List<string>();

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

    private void UpdateChoices()
    {

    }
}