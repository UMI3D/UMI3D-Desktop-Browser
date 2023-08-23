using System;
using System.Collections.Generic;
using umi3d.baseBrowser.preferences;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.preferences.SettingsPreferences;
using static UnityEngine.Rendering.DebugUI;
using System.Linq;

public class GraphicsSettings : BaseSettings
{
    private RadioButton m_GameResolutionHigh;
    private RadioButton m_GameResolutionMedium;
    private RadioButton m_GameResolutionLow;
    private RadioButton m_GameResolutionCustom;
    private Dropdown_C m_FullScreenResolution;
    private Dropdown_C m_QualitySettings;
    private RadioButton m_UnableHdrOn;
    private RadioButton m_UnableHdrOff;
    private SliderFloat_C m_RenderScale;

    private RadioButton m_UiSizeHigh;
    private RadioButton m_UiSizeMedium;
    private RadioButton m_UiSizeLow;
    private RadioButton m_UiSizeCustom;
    private SliderInt_C m_Zoom;

    private ResolutionData m_ResolutionData;
    private List<string> m_Resolutions;
    private UniversalRenderPipelineAsset m_RenderPipeline;
    public PanelSettings m_UiPanelSettings;

    public GraphicsSettings(VisualElement pRoot) : base(pRoot)
    {
        m_UiPanelSettings = Resources.Load<PanelSettings>("PanelSettings");

        SetupFullScrernResolution();
        SetupQualitySettings();
        SetupUnableHdr();
        SetupRenderScale();
        SetupGameResolution();

        SetupUiSize();

        SetValues();
    }

    private void SetValues()
    {
        if (TryGetResolutionData(out m_ResolutionData))
        {
            // Game Resolution
            if (m_ResolutionData.GameResolution == ResolutionEnum.Custom)
            {
                m_FullScreenResolution.value = m_ResolutionData.FullScreenResolution;
                m_QualitySettings.value = m_ResolutionData.Quality.ToString();
                if (m_ResolutionData.HDR)
                    m_UnableHdrOn.value = true;
                else
                    m_UnableHdrOff.value = true;
                m_RenderScale.value = m_ResolutionData.RenderScale;
            }
            switch (m_ResolutionData.GameResolution)
            {
                case ResolutionEnum.Low:
                    m_GameResolutionLow.value = true;
                    OnGameResolutionChanged(true, ResolutionEnum.Low);
                    break;
                case ResolutionEnum.Medium:
                    m_GameResolutionMedium.value = true;
                    OnGameResolutionChanged(true, ResolutionEnum.Medium);
                    break;
                case ResolutionEnum.High:
                    m_GameResolutionHigh.value = true;
                    OnGameResolutionChanged(true, ResolutionEnum.High);
                    break;
                case ResolutionEnum.Custom:
                    m_GameResolutionCustom.value = true;
                    OnGameResolutionChanged(true, ResolutionEnum.Custom);
                    break;
                default:
                    break;
            }
            // Ui Size
            if (m_ResolutionData.UISize == UIZoom.Custom)
                m_Zoom.value = (int)m_ResolutionData.DPI;
            switch (m_ResolutionData.UISize)
            {
                case UIZoom.Small:
                    m_UiSizeLow.value = true;
                    OnUiSizeChanged(true, UIZoom.Small);
                    break;
                case UIZoom.Medium:
                    m_UiSizeMedium.value = true;
                    OnUiSizeChanged(true, UIZoom.Medium);
                    break;
                case UIZoom.Large:
                    m_UiSizeHigh.value = true;
                    OnUiSizeChanged(true, UIZoom.Large);
                    break;
                case UIZoom.Custom:
                    m_UiSizeCustom.value = true;
                    OnUiSizeChanged(true, UIZoom.Custom);
                    break;
                default:
                    break;
            }
        }
        else
        {
            m_GameResolutionMedium.value = true;
            OnGameResolutionChanged(true, ResolutionEnum.Medium);
            m_UiSizeMedium.value = true;
            OnUiSizeChanged(true, UIZoom.Medium);
        }
    }

    #region Game Resolution
    private void SetupFullScrernResolution()
    {
        m_FullScreenResolution = m_Root.Q<Dropdown_C>("FullScreenResolution");

        m_Resolutions = new List<string>();
        foreach (var resolution in Screen.resolutions)
            m_Resolutions.Insert(0, $"{resolution.width}x{resolution.height}");
        m_FullScreenResolution.choices = m_Resolutions;

        m_FullScreenResolution.RegisterValueChangedCallback(e 
            => OnFullScreenResolutionChanged(e.newValue));
    }

    private void OnFullScreenResolutionChanged(string pValue)
    {
        var resolution = pValue.Split('x');
        int.TryParse(resolution[0], out var width);
        int.TryParse(resolution[1], out var height);

        if (Screen.fullScreen) Screen.SetResolution(width, height, true);

        m_ResolutionData.FullScreenResolution = pValue;
        StoreResolutionData(m_ResolutionData);
    }

    private void SetupQualitySettings()
    {
        m_QualitySettings = m_Root.Q<Dropdown_C>("QualitySettings");

        m_QualitySettings.choices = (from quality in (QualityEnum[])Enum.GetValues(typeof(QualityEnum))
                                     select quality.ToString()).ToList();

        m_QualitySettings.RegisterValueChangedCallback(e
            => OnQualitySettingsChanged(e.newValue));
    }

    private void OnQualitySettingsChanged(string pValue)
    {
        var value = Enum.Parse<QualityEnum>(pValue);
        switch (value)
        {
            case QualityEnum.VLow:
                QualitySettings.SetQualityLevel(0, false);
                break;
            case QualityEnum.Low:
                QualitySettings.SetQualityLevel(1, false);
                break;
            case QualityEnum.Medium:
                QualitySettings.SetQualityLevel(2, false);
                break;
            case QualityEnum.High:
                QualitySettings.SetQualityLevel(3, false);
                break;
            case QualityEnum.VHigh:
                QualitySettings.SetQualityLevel(4, false);
                break;
            case QualityEnum.Ultra:
                QualitySettings.SetQualityLevel(5, false);
                break;
            default:
                break;
        }
        m_RenderPipeline = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;

        m_ResolutionData.Quality = value;
        StoreResolutionData(m_ResolutionData);
    }

    private void SetupUnableHdr()
    {
        m_UnableHdrOn = m_Root.Q("UnableHDR").Q<RadioButton>("On");
        m_UnableHdrOff = m_Root.Q("UnableHDR").Q<RadioButton>("Off");

        m_UnableHdrOn.RegisterValueChangedCallback(e => OnUnableHdrChanged(e.newValue));
    }

    private void OnUnableHdrChanged(bool pValue)
    {
        m_RenderPipeline.supportsHDR = pValue;
        m_ResolutionData.HDR = pValue;
        StoreResolutionData(m_ResolutionData);
    }

    private void SetupRenderScale()
    {
        m_RenderScale = m_Root.Q<SliderFloat_C>("RenderScale");

        m_RenderScale.RegisterValueChangedCallback(e => OnRenderScaleChanged(e.newValue));
    }

    private void OnRenderScaleChanged(float pValue)
    {
        m_RenderPipeline.renderScale = pValue;
        m_ResolutionData.RenderScale = pValue;
        StoreResolutionData(m_ResolutionData);
    }

    private void SetupGameResolution()
    {
        m_GameResolutionHigh = m_Root.Q("GameResolution").Q<RadioButton>("High");
        m_GameResolutionMedium = m_Root.Q("GameResolution").Q<RadioButton>("Medium");
        m_GameResolutionLow = m_Root.Q("GameResolution").Q<RadioButton>("Low");
        m_GameResolutionCustom = m_Root.Q("GameResolution").Q<RadioButton>("Custom");

        m_GameResolutionHigh.RegisterValueChangedCallback(e => 
        OnGameResolutionChanged(e.newValue, ResolutionEnum.High));
        m_GameResolutionMedium.RegisterValueChangedCallback(e =>
        OnGameResolutionChanged(e.newValue, ResolutionEnum.Medium));
        m_GameResolutionLow.RegisterValueChangedCallback(e =>
        OnGameResolutionChanged(e.newValue, ResolutionEnum.Low));
        m_GameResolutionCustom.RegisterValueChangedCallback(e =>
        OnGameResolutionChanged(e.newValue, ResolutionEnum.Custom));
    }

    private void OnGameResolutionChanged(bool pValue, ResolutionEnum pResolution)
    {
        if (!pValue) return;
        switch (pResolution)
        {
            case ResolutionEnum.Low:
                m_FullScreenResolution.value = m_Resolutions[0];
                m_QualitySettings.value = QualityEnum.Low.ToString();
                m_UnableHdrOff.value = true;
                m_RenderScale.value = 0.7f;
                m_RenderPipeline.shadowDistance = 20f;

                m_FullScreenResolution.AddToClassList("hidden");
                m_QualitySettings.AddToClassList("hidden");
                m_UnableHdrOn.parent.AddToClassList("hidden");
                m_RenderScale.AddToClassList("hidden");
                break;
            case ResolutionEnum.Medium:
                m_FullScreenResolution.value = m_Resolutions[0];
                m_QualitySettings.value = QualityEnum.Medium.ToString();
                m_UnableHdrOff.value = true;
                m_RenderScale.value = 1f;
                m_RenderPipeline.shadowDistance = 35f;

                m_FullScreenResolution.AddToClassList("hidden");
                m_QualitySettings.AddToClassList("hidden");
                m_UnableHdrOn.parent.AddToClassList("hidden");
                m_RenderScale.AddToClassList("hidden");
                break;
            case ResolutionEnum.High:
                m_FullScreenResolution.value = m_Resolutions[0];
                m_QualitySettings.value = QualityEnum.High.ToString();
                m_UnableHdrOn.value = true;
                m_RenderScale.value = 1.3f;
                m_RenderPipeline.shadowDistance = 50f;

                m_FullScreenResolution.AddToClassList("hidden");
                m_QualitySettings.AddToClassList("hidden");
                m_UnableHdrOn.parent.AddToClassList("hidden");
                m_RenderScale.AddToClassList("hidden");
                break;
            case ResolutionEnum.Custom:
                m_FullScreenResolution.RemoveFromClassList("hidden");
                m_QualitySettings.RemoveFromClassList("hidden");
                m_UnableHdrOn.parent.RemoveFromClassList("hidden");
                m_RenderScale.RemoveFromClassList("hidden");
                break;
            default:
                break;
        }

        OnFullScreenResolutionChanged(m_FullScreenResolution.value);
        OnQualitySettingsChanged(m_QualitySettings.value);
        OnUnableHdrChanged(m_UnableHdrOn.value);
        OnRenderScaleChanged(m_RenderScale.value);

        m_ResolutionData.GameResolution = pResolution;
        StoreResolutionData(m_ResolutionData);
    }
    #endregion

    #region UI Size
    private void SetupUiSize()
    {
        m_UiSizeHigh = m_Root.Q("UISize").Q<RadioButton>("High");
        m_UiSizeMedium = m_Root.Q("UISize").Q<RadioButton>("Medium");
        m_UiSizeLow = m_Root.Q("UISize").Q<RadioButton>("Low");
        m_UiSizeCustom = m_Root.Q("UISize").Q<RadioButton>("Custom");

        m_UiSizeHigh.RegisterValueChangedCallback(e 
            => OnUiSizeChanged(e.newValue, UIZoom.Large));
        m_UiSizeMedium.RegisterValueChangedCallback(e
            => OnUiSizeChanged(e.newValue, UIZoom.Medium));
        m_UiSizeLow.RegisterValueChangedCallback(e
            => OnUiSizeChanged(e.newValue, UIZoom.Small));
        m_UiSizeCustom.RegisterValueChangedCallback(e
            => OnUiSizeChanged(e.newValue, UIZoom.Custom));

        m_Zoom = m_Root.Q<SliderInt_C>("Zoom");

        m_Zoom.RegisterValueChangedCallback(e => OnDpiValueChanged(e.newValue));
    }

    private void OnUiSizeChanged(bool pValue, UIZoom pZoom)
    {
        if (!pValue) return;
        switch (pZoom)
        {
            case UIZoom.Small:
                OnDpiValueChanged(170f);
                m_Zoom.AddToClassList("hidden");
                break;
            case UIZoom.Medium:
                OnDpiValueChanged(150f);
                m_Zoom.AddToClassList("hidden");
                break;
            case UIZoom.Large:
                OnDpiValueChanged(120f);
                m_Zoom.AddToClassList("hidden");
                break;
            case UIZoom.Custom:
                m_Zoom.RemoveFromClassList("hidden");
                break;
            default:
                break;
        }

        m_ResolutionData.UISize = pZoom;
        StoreResolutionData(m_ResolutionData);
    }

    private void OnDpiValueChanged(float pValue)
    {
        m_UiPanelSettings.referenceDpi = pValue;
        m_ResolutionData.DPI = pValue;
        StoreResolutionData(m_ResolutionData);
    }
    #endregion

    public static bool TryGetResolutionData(out ResolutionData data) => PreferencesManager.TryGet(out data, c_resolutionPath, c_dataFolderPath);
    public static void StoreResolutionData(ResolutionData data) => PreferencesManager.StoreData(data, c_resolutionPath, c_dataFolderPath);
}