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
using umi3d.cdk;
using umi3d.common;
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
        VisualElement microphoneSetter;

        Label environmentName;

        #endregion

        #region Bottom Bar

        MicrophoneSlider GainSlider;
        MicrophoneSlider ThresholdSlider;

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


            /*DisplayConsole(false);
            microphoneBtn.RegisterCallback<MouseDownEvent>(e => { 
                if(e.pressedButtons == 2)
                    DisplayConsole(!isDisplayed);
            });

            InitMicrophoneSlider(microphoneSetter);*/

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() =>
            {
                topCenterMenu.style.display = DisplayStyle.Flex;
            });
        }

        private void Update()
        {
            /*if(umi3d.cdk.collaboration.MicrophoneListener.Exists)
                if(displayMicrophoneSlider && GainSlider.DisplayedValue != umi3d.cdk.collaboration.MicrophoneListener.Instance.RMS)
                {
                    GainSlider.DisplayedValue = umi3d.cdk.collaboration.MicrophoneListener.Instance.RMS;
                    ThresholdSlider.DisplayedValue = umi3d.cdk.collaboration.MicrophoneListener.Instance.RMS;
                }*/
        }

        /*void InitMicrophoneSlider(VisualElement root)
        {
            var okColors = new MicrophoneSliderColor(0.5f, new UnityEngine.Color(0f, 1f, 0f));
            var saturatedColors = new MicrophoneSliderColor(0.9f, new UnityEngine.Color(1f, 0f, 0f));
            var colors = new List<MicrophoneSliderColor>()
            {
                new MicrophoneSliderColor(0,new UnityEngine.Color32(244,99,11,255)),
                okColors,
                saturatedColors
            };

            umi3d.cdk.collaboration.MicrophoneListener.OnSaturated.AddListener(
                b => {
                    if (b)
                        saturatedColors.Startvalue = 0;
                    else
                        saturatedColors.Startvalue = 0.9f;
                    GainSlider.RefreshColor();
                    ThresholdSlider.RefreshColor();
                });


            *//*GainSlider = new MicrophoneSlider(root.Q<VisualElement>("gain-bar"),"Gain",
                (i) => { float r; return (float.TryParse(i, out r), GToP(r)); },
                (f) => { return (PToG(f)).ToString(); },
                GToP(umi3d.cdk.collaboration.MicrophoneListener.Gain), 0f, 0f, 1f, 0.01f, colors);
            GainSlider.OnValueChanged.AddListener(v =>
            {
                umi3d.cdk.collaboration.MicrophoneListener.Gain = PToG(v);
            });
            ThresholdSlider = new MicrophoneSlider(root.Q<VisualElement>("threshold-bar")
                , "Noise Threshold",
                (i) => { float r; return (float.TryParse(i, out r), r / 100f); },
                (f) => { return (f * 100).ToString(); },
                umi3d.cdk.collaboration.MicrophoneListener.NoiseThreshold, 0f, 0f, 1f, 0.01f, colors);
            ThresholdSlider.OnValueChanged.AddListener(v =>
            {
                okColors.Startvalue = v;
                umi3d.cdk.collaboration.MicrophoneListener.NoiseThreshold = v;
            });*//*
        }*/

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