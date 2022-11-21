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

namespace umi3d.commonScreen.Container
{
    public class ButtonGroup_C : CustomButtonGroup
    {
        public new class UxmlFactory : UxmlFactory<ButtonGroup_C, UxmlTraits> { }

        public ButtonGroup_C() => Set();

        protected override CustomButton CreateButton()
            => new Displayer.Button_C();
    }

    public class ButtonGroup_C<ButtonEnum> : CustomButtonGroup<ButtonEnum>
    where ButtonEnum : struct, System.Enum
    {
        public ButtonGroup_C() => Set();

        protected override CustomButton CreateButton()
            => new Displayer.Button_C();

        protected override string EnumToString(ButtonEnum enumValue)
            => enumValue.ToString();
    }
}