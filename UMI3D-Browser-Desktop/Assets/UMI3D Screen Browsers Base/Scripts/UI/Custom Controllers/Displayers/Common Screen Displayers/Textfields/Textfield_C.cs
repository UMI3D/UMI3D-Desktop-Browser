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

namespace umi3d.commonScreen.Displayer
{
    public class Textfield_C : CustomTextfield
    {
        public new class UxmlFactory : UxmlFactory<Textfield_C, UxmlTraits> { }

        public Textfield_C() => Set();

        public Textfield_C(ElementCategory category, ElementSize size, ElemnetDirection direction, bool maskToggle, bool submitButton) => Set(category, size, direction, maskToggle, submitButton);

        public override void InitElement()
        {
            if (SampleTextLabel == null) SampleTextLabel = new Text_C();
            if (MaskToggle == null) MaskToggle = new Toggle_C();
            if (SubmitButton == null) SubmitButton = new Button_C();

            base.InitElement();
        }
    }
}
