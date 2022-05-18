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
    public partial class ProgressBar_E
    {
        public Icon_E Bar { get; protected set; } = null;

        public void SetBar(string partialStylePath, StyleKeys keys)
            => Bar.UpdateRootStyleAndKeysAndManipulator(partialStylePath, keys);
    }

    public partial class ProgressBar_E : Box_E
    {
        public ProgressBar_E(string partialStylePath, StyleKeys keys) :
            base(partialStylePath, keys)
        { }

        protected override void Initialize()
        {
            base.Initialize();

            Root.name = "progressBar";
            Bar = new Icon_E();
            Bar.InsertRootTo(Root);
        }
    } 
}
