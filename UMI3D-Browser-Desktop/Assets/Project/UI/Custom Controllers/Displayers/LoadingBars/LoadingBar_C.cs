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

namespace umi3d.mobileBrowser.Displayer
{
    public class LoadingBar_C : CustomLoadingBar
    {
        public new class UxmlFactory : UxmlFactory<LoadingBar_C, UxmlTraits> { }

        public LoadingBar_C() => Set();

        public LoadingBar_C(ElementCategory category, ElementSize size, string message)
            => Set(category, size, message, 0f);

        public override void InitElement()
        {
            if (SampleTitleLabel == null) SampleTitleLabel = new Text_C();
            if (MessageLabel == null) MessageLabel = new Text_C();
            base.InitElement();
        }
    }
}
