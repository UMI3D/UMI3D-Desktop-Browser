using System.Collections.Generic;
using umi3d.commonScreen;
using UnityEngine;
using UnityEngine.UIElements;

public class GeneralSettingsPanel_C : BaseVisual_C
{
    public new class UxmlFactory : UxmlFactory<GeneralSettingsPanel_C, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="ve"></param>
        /// <param name="bag"></param>
        /// <param name="cc"></param>
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as GeneralSettingsPanel_C;
        }
    }

    private Dropdown_C _languages = new Dropdown_C();
    private Dropdown_C _theme = new Dropdown_C();

    #region USS
    public override string StyleSheetPath_MainTheme => $"UI NEW/USS/Settings/Settings";

    protected override void AttachStyleSheet()
    {
        base.AttachStyleSheet();
    }

    protected override void AttachUssClass()
    {
        base.AttachUssClass();
        AddToClassList("settings-panel");
    }
    #endregion

    protected override void InitElement()
    {
        base.InitElement();

        _languages.label = "Language";
        _languages.choices = new List<string>() { "English", "French" };
        _languages.value = "English";
        _theme.label = "Theme";
        _theme.choices = new List<string>() { "Dark mode", "Light mode" };
        _theme.value = "Dark mode";

        Add(_languages);
        Add(_theme);
    }
}
