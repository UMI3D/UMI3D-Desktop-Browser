using System.Collections.Generic;
using umi3d.commonScreen;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioSettingsPanel_C : BaseVisual_C
{
    public new class UxmlFactory : UxmlFactory<AudioSettingsPanel_C, UxmlTraits> { }

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
            var custom = ve as AudioSettingsPanel_C;
        }
    }

    private ScrollView _settings = new ScrollView();

    private Dropdown_C _micro = new Dropdown_C();
    private RadioButtonGroup _microMode = new RadioButtonGroup();
    private PercentageSlider_C _volumeEnvironment = new PercentageSlider_C();
    private PercentageSlider_C _volumeConversation = new PercentageSlider_C();
    private RadioButtonGroup _useNoiseReduction = new RadioButtonGroup();
    private PercentageSlider_C _noiseThreshold = new PercentageSlider_C();
    //private RadioButtonGroup _delayMuteMicro = new RadioButtonGroup();
    private RadioButtonGroup _loopBack = new RadioButtonGroup();

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

        _micro.AddToClassList("setting");
        _microMode.AddToClassList("setting");
        _volumeEnvironment.AddToClassList("setting");
        _volumeConversation.AddToClassList("setting");
        _useNoiseReduction.AddToClassList("setting");
        _noiseThreshold.AddToClassList("setting");
        _loopBack.AddToClassList("setting");

        _microMode.AddToClassList("radio-3-elements");
        _useNoiseReduction.AddToClassList("radio-2-elements");
        _loopBack.AddToClassList("radio-2-elements");
    }
    #endregion

    protected override void InitElement()
    {
        base.InitElement();

        // Micro
        _micro.label = "Microphone";
        _micro.choices = new List<string>() { "Phone Headset", "Speaker" };
        _micro.value = "Phone Headset";

        // Micro mode
        _microMode.label = "Micro mode";
        var modeAlwaysSend = new RadioButton("Always send");
        _microMode.Add(modeAlwaysSend);
        _microMode.Add(new RadioButton("Amplitude"));
        _microMode.Add(new RadioButton("Push to talk"));
        modeAlwaysSend.value = true;

        // Volumes
        _volumeEnvironment.label = "Volume Environment";
        _volumeConversation.label = "Volume Conversation";

        // Noise Reduction
        _useNoiseReduction.label = "Use Noise Reduction";
        var noiseOn = new RadioButton("On");
        _useNoiseReduction.Add(noiseOn);
        _useNoiseReduction.Add(new RadioButton("Off"));
        noiseOn.value = true;

        _noiseThreshold.label = "Noise Threshold";

        // LoopBack
        _loopBack.label = "Loop back";
        var loopOn = new RadioButton("On");
        _loopBack.Add(loopOn);
        _loopBack.Add(new RadioButton("Off"));
        loopOn.value = true;

        _settings.mode = ScrollViewMode.Vertical;

        _settings.Add(_micro);
        _settings.Add(_microMode);
        _settings.Add(_volumeEnvironment);
        _settings.Add(_volumeConversation);
        _settings.Add(_useNoiseReduction);
        _settings.Add(_noiseThreshold);
        //_settings.Add(_delayMuteMicro);
        _settings.Add(_loopBack);

        Add(_settings);
    }
}
