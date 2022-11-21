/*
Copyright 2019 - 2021 Inetum

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
using umi3DBrowser.UICustomStyle;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.ui.viewController
{
    public class Icon_E : View_E
    {
        public Icon_E() :
            base()
        { }
        public Icon_E(string partialStylePath, StyleKeys keys) :
            base(partialStylePath, keys)
        { }
        public Icon_E(VisualElement visual) :
            base(visual)
        { }
        public Icon_E(VisualElement visual, string partialStylePath, StyleKeys keys) :
            base(visual, partialStylePath, keys)
        { }
        public Icon_E(string visualResourcePath, string partialStylePath, StyleKeys keys) :
            base(visualResourcePath, partialStylePath, keys)
        { }

        protected override CustomStyle_SO GetStyleSO(string resourcePath)
        {
            var path = (resourcePath == null) ? null : $"UI/Style/Icons/{resourcePath}";
            return base.GetStyleSO(path);
        }
    }
}