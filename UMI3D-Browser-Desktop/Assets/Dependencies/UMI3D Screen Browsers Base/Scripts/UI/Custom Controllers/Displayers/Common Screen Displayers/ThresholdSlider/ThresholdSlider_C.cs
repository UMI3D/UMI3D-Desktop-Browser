/*
Copyright 2019 - 2022 Inetum

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class ThresholdSlider_C : CustomThresholdSlider
    {
        public new class UxmlFactory : UxmlFactory<ThresholdSlider_C, UxmlTraits> { }

        public ThresholdSlider_C() => Set();

        public override void InitElement()
        {
            if (SampleTextfield == null) SampleTextfield = new Textfield_C();
            if (SampleTextLabel == null) SampleTextLabel = new Text_C();

            base.InitElement();
        }
    }
}
