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
        void InitMicrophoneSlider(VisualElement root)
        {
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

            /// Add Mode Amplitude info


            //ThresholdSlider = new MicrophoneSlider(
            //    tb,
            //    "Noise Threshold",
            //    (i) => { float r; return (float.TryParse(i, out r), r / 100f); },
            //    (f) => { return (f * 100).ToString(); },
            //    umi3d.cdk.collaboration.MicrophoneListener.Instance.minAmplitudeToSend, 0f, 0f, 1f, 0.01f, colors
            //    );
        }
    }
}