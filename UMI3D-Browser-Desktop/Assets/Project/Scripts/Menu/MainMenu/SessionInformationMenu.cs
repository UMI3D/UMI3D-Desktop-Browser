/*
Copyright 2019 Gfi Informatique

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using inetum.unityUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3d.baseBrowser.ui.viewController;
using umi3d.cdk;
using umi3d.common;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    /// <summary>
    /// This class manages the UI elements which gives information about the current session such as : 
    /// the name of the environment, is the microphone working, the session tim, etc.
    /// </summary>
    public class SessionInformationMenu : SingleBehaviour<SessionInformationMenu>
    {
        #region Fields

        public UIDocument uiDocument;
        VisualElement root;

        #region Top Bar

        VisualElement topCenterMenu;
        VisualElement microphoneSetterContainer;
        VisualElement microphoneSetter;

        Label environmentName;

        #endregion

        #region Bottom Bar

        //MicrophoneSlider GainSlider;
        MicrophoneSlider ThresholdSlider;
        Dropdown_E MicrophoneDropDown;
        Dropdown_E ModeDropDown;

        VisualElement PushToTalk;
        VisualElement Amplitude;

        bool displayMicrophoneSlider = true;

        #endregion

        #endregion

        /// <summary>
        /// Binds the UI
        /// </summary>
        void Start()
        {
            UnityEngine.Debug.Assert(uiDocument != null);
            root = uiDocument.rootVisualElement;

            //Top Bar
            topCenterMenu = root.Q<VisualElement>("top-center-menu");
            topCenterMenu.style.display = DisplayStyle.None;

            //Bottom Bar
            microphoneSetterContainer = root.Q<VisualElement>("microphone-setter-container");
            microphoneSetter = microphoneSetterContainer.Q<VisualElement>("microphone-setter");
            HideMicrophoneSettingsPopUp();
            InitMicrophoneSlider(microphoneSetter);

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() =>
            {
                topCenterMenu.style.display = DisplayStyle.Flex;
            });
        }

        private void Update()
        {
            if (umi3d.cdk.collaboration.MicrophoneListener.Exists)
                if (displayMicrophoneSlider && ThresholdSlider.DisplayedValue != umi3d.cdk.collaboration.MicrophoneListener.Instance.rms)
                {
                    //GainSlider.DisplayedValue = umi3d.cdk.collaboration.MicrophoneListener.Instance.RMS;
                    ThresholdSlider.DisplayedValue = umi3d.cdk.collaboration.MicrophoneListener.Instance.rms;
                }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                if (microphoneSetterContainer.resolvedStyle.display == DisplayStyle.Flex)
                    HideMicrophoneSettingsPopUp();
                else
                    DisplayMicrophoneSettingsPopUp();
            }
        }

        private void HideMicrophoneSettingsPopUp()
        {
            microphoneSetterContainer.style.display = DisplayStyle.None;
            umi3d.cdk.collaboration.MicrophoneListener.Instance.Debug = false;
        }

        private void DisplayMicrophoneSettingsPopUp()
        {
            //GainSlider.Value = GToP(umi3d.cdk.collaboration.MicrophoneListener.Gain);
            MicrophoneDropDown.SetOptions(umi3d.cdk.collaboration.MicrophoneListener.Instance.GetMicrophonesNames().ToList());
            MicrophoneDropDown.SetDefaultValue(umi3d.cdk.collaboration.MicrophoneListener.Instance.GetCurrentMicrophoneName());
            ModeDropDown.SetDefaultValue(umi3d.cdk.collaboration.MicrophoneListener.Instance.GetCurrentMicrophoneMode().ToString());
            microphoneSetterContainer.style.display = DisplayStyle.Flex;
            umi3d.cdk.collaboration.MicrophoneListener.Instance.Debug = true;

        }

        int _i;
        int index { get => _i++; set => _i = value; }
        void InitMicrophoneSlider(VisualElement root)
        {
            index = 0;


            var okColors = new MicrophoneSliderColor(0.5f, new UnityEngine.Color(0f, 1f, 0f));
            var saturatedColors = new MicrophoneSliderColor(0.9f, new UnityEngine.Color(1f, 0f, 0f));
            var colors = new List<MicrophoneSliderColor>()
            {
                new MicrophoneSliderColor(0,new UnityEngine.Color32(244,99,11,255)),
                okColors,
                saturatedColors
            };

            umi3d.cdk.collaboration.MicrophoneListener.OnSaturated.AddListener(
                b =>
                {
                    if (b)
                        saturatedColors.Startvalue = 0;
                    else
                        saturatedColors.Startvalue = 0.9f;
                    //GainSlider.RefreshColor();
                    ThresholdSlider.RefreshColor();
                });

            //GainSlider = new MicrophoneSlider(root.Q<VisualElement>("gain-bar"),"Gain",
            //    (i) => { float r; return (float.TryParse(i, out r), GToP(r)); },
            //    (f) => { return (PToG(f)).ToString(); },
            //    GToP(/*umi3d.cdk.collaboration.MicrophoneListener.Gain*/0), 0f, 0f, 1f, 0.01f, colors);
            //GainSlider.OnValueChanged.AddListener(v =>
            //{
            //    //umi3d.cdk.collaboration.MicrophoneListener.Gain = PToG(v);
            //});

            /// Add Microphone selecter.

            var MicrophoneLabel = new Label_E("Corps", StyleKeys.Text("primaryLight"), "Microphone :");
            MicrophoneLabel.InsertRootAtTo(index, root);

            MicrophoneDropDown = new Dropdown_E("MicrophoneDropdown", StyleKeys.Text_Bg("button"));
            MicrophoneDropDown.SetMenuStyle("MicrophoneEnumBox", StyleKeys.Default_Bg_Border);
            MicrophoneDropDown.SetMenuLabel("CorpsMicrophoneDropdown", StyleKeys.DefaultText);
            MicrophoneDropDown.InsertRootAtTo(index, root);

            MicrophoneDropDown.ValueChanged = (s) =>
            {
                UpdateMicrophone(s);
            };


            /// Add Loop back

            var LoopBack = new Button_E("Corps", StyleKeys.Text("primaryLight"));
            LoopBack.InsertRootAtTo(index, root);
            LoopBack.Text = "LoopBack Off";

            LoopBack.ClickedDown += () =>
            {
                umi3d.cdk.collaboration.MicrophoneListener.Instance.useLocalLoopback = !umi3d.cdk.collaboration.MicrophoneListener.Instance.useLocalLoopback;
                LoopBack.Text = (umi3d.cdk.collaboration.MicrophoneListener.Instance.useLocalLoopback) ? "LoopBack On" : "LoopBack Off";
            };

            /// Add Mode selecter.

            var ModeLabel = new Label_E("Corps", StyleKeys.Text("primaryLight"), "Mode :");
            ModeLabel.InsertRootAtTo(index, root);

            ModeDropDown = new Dropdown_E("MicrophoneDropdown", StyleKeys.Text_Bg("button"));
            ModeDropDown.SetMenuStyle("MicrophoneEnumBox", StyleKeys.Default_Bg_Border);
            ModeDropDown.SetMenuLabel("CorpsMicrophoneDropdown", StyleKeys.DefaultText);
            ModeDropDown.InsertRootAtTo(index, root);
            ModeDropDown.SetOptions(Enum.GetNames(typeof(umi3d.cdk.collaboration.MicrophoneMode)).Where(s=>s != umi3d.cdk.collaboration.MicrophoneMode.MethodBased.ToString()).ToList());
            ModeDropDown.ValueChanged = (s) =>
            {
                UpdateMode(s);
            };


            /// Add Mode Push To Talk info

            PushToTalk = new VisualElement();
            root.Insert(index, PushToTalk);

            var PushToTalkKeycodeLabel = new Label_E("Corps", StyleKeys.Text("primaryLight"), "Push To Talk Key");
            PushToTalkKeycodeLabel.InsertRootTo(PushToTalk);

            var PushToTalkKeycode = new Label_E("Corps", StyleKeys.Text("primaryLight"), $"<{umi3d.cdk.collaboration.MicrophoneListener.Instance.pushToTalkKeycode}>");
            PushToTalkKeycode.InsertRootTo(PushToTalk);

            /// Add Mode Amplitude info

            Amplitude = new VisualElement();
            root.Insert(index, Amplitude);

            var tb = root.Q<VisualElement>("threshold-bar");
            Amplitude.Add(tb);
            ThresholdSlider = new MicrophoneSlider(
                tb,
                "Noise Threshold",
                (i) => { float r; return (float.TryParse(i, out r), r / 100f); },
                (f) => { return (f * 100).ToString(); },
                umi3d.cdk.collaboration.MicrophoneListener.Instance.minAmplitudeToSend, 0f, 0f, 1f, 0.01f, colors
                );

            ThresholdSlider.OnValueChanged.AddListener(v =>
            {
                okColors.Startvalue = v;
                umi3d.cdk.collaboration.MicrophoneListener.Instance.minAmplitudeToSend = v;
            });
        }

        async void  UpdateMicrophone(string name)
        {
            if (!string.IsNullOrEmpty(name) && name != umi3d.cdk.collaboration.MicrophoneListener.Instance.GetCurrentMicrophoneName())
            {
                await umi3d.cdk.collaboration.MicrophoneListener.Instance.SetCurrentMicrophoneName(name);
                MicrophoneDropDown.SetOptions(umi3d.cdk.collaboration.MicrophoneListener.Instance.GetMicrophonesNames().ToList());
                MicrophoneDropDown.SetDefaultValue(umi3d.cdk.collaboration.MicrophoneListener.Instance.GetCurrentMicrophoneName());
            }
        }

         void UpdateMode(string name)
        {
            if (!string.IsNullOrEmpty(name) && Enum.TryParse<umi3d.cdk.collaboration.MicrophoneMode>(name, out var mode))
            {
                if (mode != umi3d.cdk.collaboration.MicrophoneListener.Instance.GetCurrentMicrophoneMode())
                {
                    umi3d.cdk.collaboration.MicrophoneListener.Instance.SetCurrentMicrophoneMode(mode);
                    ModeDropDown.SetDefaultValue(umi3d.cdk.collaboration.MicrophoneListener.Instance.GetCurrentMicrophoneMode().ToString());
                }

                PushToTalk.style.display = mode == umi3d.cdk.collaboration.MicrophoneMode.PushToTalk ? DisplayStyle.Flex : DisplayStyle.None;
                Amplitude.style.display = mode == umi3d.cdk.collaboration.MicrophoneMode.Amplitude ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        float GToP(float f)
        {
            return f / 10f;
        }
        float PToG(float f)
        {
            return f * 10f;
        }

        /// <summary>
        /// Initiates the custom title bar with the name of the environment.
        /// </summary>
        /// <param name="media"></param>
        public void SetEnvironmentName(MediaDto media)
        {
            environmentName = root.Q<Label>("environment-name");
            environmentName.text = media.name;
        }

        /*bool isDisplayed = false;
        void DisplayConsole(bool val)
        {
            isDisplayed = val;
            microphoneSetter.style.display = val ? DisplayStyle.Flex : DisplayStyle.None;
            if (val)
                MainThreadDispatcher.UnityMainThreadDispatcher.Instance().Enqueue(SetValues());
        }*/

        /*IEnumerator SetValues()
        {
            yield return null;
            GainSlider.Value = GToP(umi3d.cdk.collaboration.MicrophoneListener.Gain);
            ThresholdSlider.Value = umi3d.cdk.collaboration.MicrophoneListener.NoiseThreshold;
        }*/

    }
}