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
using UnityEngine.UIElements;

namespace umi3d.mobileBrowser.Container
{
    public class Form_C : CustomForm
    {
        public new class UxmlFactory : UxmlFactory<Form_C, UxmlTraits> { }

        public Form_C() => Set();

        public Form_C(ElementCategory category, string title, string iconPath) => Set(category, title, iconPath);

        public override void InitElement()
        {
            if (TitleLabel == null) TitleLabel = new Displayer.Text_C();
            if (VScroll == null) VScroll = new ScrollView_C();

            base.InitElement();
        }
    }
}
