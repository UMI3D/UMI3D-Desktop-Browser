using System.Threading.Tasks;
using umi3d.commonScreen;
using UnityEngine;
using UnityEngine.UIElements;

public class Text_C : Label
{
    public new class UxmlFactory : UxmlFactory<Text_C, UxmlTraits> { }

    public new class UxmlTraits : Label.UxmlTraits
    {
        UxmlLocaliseAttributeDescription m_localisedText = new UxmlLocaliseAttributeDescription
        {
            name = "localised-text"
        };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as Text_C;

            custom.LocalisedText = m_localisedText.GetValueFromBag(bag, cc);
        }
    }

    public virtual LocalisationAttribute LocalisedText
    {
        get => m_localisation;
        set
        {
            m_localisation = value;
            ChangedLanguage();
            if (!Application.isPlaying) text = value.DefaultText;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <remarks>!! Use <see cref="LocalisedText"/> instead</remarks>
    public override string text { get => base.text; set => base.text = value; }

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
    }
    #endregion

    public Text_C()
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
        
    }

    #region Localisation

    public LocalisationAttribute m_localisation;
    public static event System.Action LanguageChanged;

    /// <summary>
    /// Raise <see cref="LanguageChanged"/> event.
    /// </summary>
    /// <returns></returns>
    public static void OnLanguageChanged()
    {
        if (!Application.isPlaying) return;

        LanguageChanged?.Invoke();
    }

    /// <summary>
    /// Change language of the text.
    /// </summary>
    public void ChangedLanguage() => _ = _ChangedLanguage();

    protected virtual async Task _ChangedLanguage()
    {
        text = m_localisation.DefaultText;

        if (m_localisation.CanBeLocalised)
        {
            while (!LocalisationManager.Exists) await UMI3DAsyncManager.Yield();

            text = LocalisationManager.Instance.GetTranslation(m_localisation);
        }
    }
    #endregion
}