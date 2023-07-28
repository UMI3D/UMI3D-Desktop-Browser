using System.Collections.Generic;
using umi3d.commonScreen;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphicsSettingsPanel_C : BaseVisual_C
{
    public new class UxmlFactory : UxmlFactory<GraphicsSettingsPanel_C, UxmlTraits> { }

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
            var custom = ve as GraphicsSettingsPanel_C;
        }
    }

    private RadioButtonGroup _gameResolution = new RadioButtonGroup();
    private RadioButtonGroup _uiSize = new RadioButtonGroup();
    private RadioButtonGroup _reduceAnimation = new RadioButtonGroup();

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

        _reduceAnimation.AddToClassList("setting");
        _uiSize.AddToClassList("setting");
        _gameResolution.AddToClassList("setting");

        _reduceAnimation.AddToClassList("radio-2-elements");
        _uiSize.AddToClassList("radio-4-elements");
        _gameResolution.AddToClassList("radio-4-elements");
    }
    #endregion

    protected override void InitElement()
    {
        base.InitElement();

        _gameResolution.label = "Game Resolution";
        var resolutionMedium = new RadioButton("Medium");
        _gameResolution.Add(new RadioButton("High"));
        _gameResolution.Add(resolutionMedium);
        _gameResolution.Add(new RadioButton("Low"));
        _gameResolution.Add(new RadioButton("Custom"));
        resolutionMedium.value = true;

        _uiSize.label = "UI Size";
        var uiSizeMedium = new RadioButton("Medium");
        _uiSize.Add(new RadioButton("High"));
        _uiSize.Add(uiSizeMedium);
        _uiSize.Add(new RadioButton("Low"));
        _uiSize.Add(new RadioButton("Custom"));
        uiSizeMedium.value = true;

        _reduceAnimation.label = "UI Size";
        var animationOn = new RadioButton("On");
        _reduceAnimation.Add(animationOn);
        _reduceAnimation.Add(new RadioButton("Off"));
        animationOn.value = true;

        Add(_gameResolution);
        Add(_uiSize);
        Add(_reduceAnimation);
    }
}
