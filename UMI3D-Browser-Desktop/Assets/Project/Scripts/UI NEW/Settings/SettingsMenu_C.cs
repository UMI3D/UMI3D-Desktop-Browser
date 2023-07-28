using System;
using System.Collections.Generic;
using System.Linq;
using umi3d.commonScreen;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenu_C : BaseVisual_C
{
    public new class UxmlFactory : UxmlFactory<SettingsMenu_C, UxmlTraits> { }

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
            var custom = ve as SettingsMenu_C;
        }
    }

    private VisualElement _leftPanel = new VisualElement() { name = "LeftPanel" };
    private VisualElement _rightPanel = new VisualElement() { name = "RightPanel" };

    private Button_C _backButton = new Button_C() { name = "BackButton" };
    private Image _icon = new Image() { name = "SettingsIcon " };
    private Text_C _title = new Text_C() { name = "Title" };

    private RadioButtonGroup _sectionButtons = new RadioButtonGroup();
    private Dictionary<RadioButton, VisualElement> _sections = new ();

    #region USS
    public override string StyleSheetPath_MainTheme => $"UI NEW/USS/Settings/Settings";

    protected override void AttachStyleSheet()
    {
        base.AttachStyleSheet();
    }

    protected override void AttachUssClass()
    {
        base.AttachUssClass();
        AddToClassList("settings");

        _leftPanel.AddToClassList("panel");
        _leftPanel.AddToClassList("left");
        _rightPanel.AddToClassList("panel");
        _rightPanel.AddToClassList("right");

        _backButton.AddToClassList("button-base");
        _backButton.AddToClassList("back");
        _title.AddToClassList("title");
    }
    #endregion

    protected override void InitElement()
    {
        base.InitElement();

        _backButton.LocaliseText = "< BACK";
        _icon.vectorImage = Resources.Load<VectorImage>("UI NEW/Icons/settings");
        _title.LocalisedText = "Settings";

        _leftPanel.Add(_backButton);
        _leftPanel.Add(_icon);
        _leftPanel.Add(_title);
        _leftPanel.Add(_sectionButtons);

        Add(_leftPanel);
        Add(_rightPanel);

        AddSection("General", new GeneralSettingsPanel_C());
        AddSection("Audio", new AudioSettingsPanel_C());
        AddSection("Graphics", new GraphicsSettingsPanel_C());
        AddSection("Controls", new GeneralSettingsPanel_C());
        AddSection("Notifications", new NotificationSettingsPanel_C());
    }

    private void AddSection(string sectionName, VisualElement panel)
    {
        var section = new RadioButton() { name = sectionName };
        section.label = sectionName;
        _sectionButtons.Add(section);

        if (sectionName == "General")
            section.value = true;
        else
            panel.AddToClassList("hidden");
        _rightPanel.Add(panel);

        _sections.Add(section, panel);

        section.RegisterValueChangedCallback(e =>
        {
            if (e.newValue)
                _sections[section].RemoveFromClassList("hidden");
            else
                _sections[section].AddToClassList("hidden");
        });
    }
}
