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
using static UnityEngine.Rendering.DebugUI;

namespace BrowserDesktop.Menu
{
    /// <summary>
    /// This class manages the UI elements which gives information about the current session such as : 
    /// the name of the environment, is the microphone working, the session tim, etc.
    /// </summary>
    public class SessionInformationMenu : SingleBehaviour<SessionInformationMenu>
    {
        const string MicrophoneKey = "last_microphone_used_by_user"; 

        #region Fields

        public UIDocument uiDocument;
        VisualElement root;

        #region Top Bar

        VisualElement microphoneSetterContainer;
        VisualElement microphoneSetter;

        #endregion

        #region Bottom Bar

        FloatField_E TimeToShut;

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

            //Bottom Bar
            microphoneSetterContainer = root.Q<VisualElement>("microphone-setter-container");
            microphoneSetter = microphoneSetterContainer.Q<VisualElement>("microphone-setter");
            HideMicrophoneSettingsPopUp();
            InitMicrophoneSlider(microphoneSetter);
        }

        private void Update()
        {
            

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
            umi3d.cdk.collaboration.MicrophoneListener.Instance.debugSampling = false;
        }

        private void DisplayMicrophoneSettingsPopUp()
        {
            TimeToShut.value = umi3d.cdk.collaboration.MicrophoneListener.Instance.voiceStopingDelaySeconds.ToString();
            microphoneSetterContainer.style.display = DisplayStyle.Flex;
            umi3d.cdk.collaboration.MicrophoneListener.Instance.debugSampling = true;

        }

        int _i;
        int index { get => _i++; set => _i = value; }
        void InitMicrophoneSlider(VisualElement root)
        {
            index = 0;


            //var okColors = new MicrophoneSliderColor(0.5f, new UnityEngine.Color(0f, 1f, 0f));
            //var saturatedColors = new MicrophoneSliderColor(0.9f, new UnityEngine.Color(1f, 0f, 0f));
            //var colors = new List<MicrophoneSliderColor>()
            //{
            //    new MicrophoneSliderColor(0,new UnityEngine.Color32(244,99,11,255)),
            //    okColors,
            //    saturatedColors
            //};

            //umi3d.cdk.collaboration.MicrophoneListener.OnSaturated.AddListener(
            //    b =>
            //    {
            //        if (b)
            //            saturatedColors.Startvalue = 0;
            //        else
            //            saturatedColors.Startvalue = 0.9f;
            //    });

            /// Add Loop back

            var LoopBack = new Button_E("MicrophoneDropdown", StyleKeys.Text_Bg("button"));
            LoopBack.InsertRootAtTo(index, root);
            LoopBack.Text = "LoopBack Off";

            LoopBack.ClickedDown += () =>
            {
                umi3d.cdk.collaboration.MicrophoneListener.Instance.useLocalLoopback = !umi3d.cdk.collaboration.MicrophoneListener.Instance.useLocalLoopback;
                LoopBack.Text = (umi3d.cdk.collaboration.MicrophoneListener.Instance.useLocalLoopback) ? "LoopBack On" : "LoopBack Off";
            };

            /// Add Mode Amplitude info


            //ThresholdSlider = new MicrophoneSlider(
            //    tb,
            //    "Noise Threshold",
            //    (i) => { float r; return (float.TryParse(i, out r), r / 100f); },
            //    (f) => { return (f * 100).ToString(); },
            //    umi3d.cdk.collaboration.MicrophoneListener.Instance.minAmplitudeToSend, 0f, 0f, 1f, 0.01f, colors
            //    );


            var TimeToShutLabel = new Label_E("Corps", StyleKeys.Text("primaryLight"), $"Delay before stopping the microphone (second) :");
            //TimeToShutLabel.InsertRootTo(Amplitude);

            TimeToShut = new FloatField_E("UI/Style/Displayers/InputFloatField",null);
            //TimeToShut.InsertRootTo(Amplitude);
            TimeToShut.ValueChanged += (oldValue, newValue) =>
            {
                //To be changed when floatField will be use in runtime.
                if (float.TryParse(newValue,out float value))
                {
                    if (value > 0f)
                        umi3d.cdk.collaboration.MicrophoneListener.Instance.voiceStopingDelaySeconds = value;
                    else
                        TimeToShut.value = "0";
                }
            };
        }
    }
}