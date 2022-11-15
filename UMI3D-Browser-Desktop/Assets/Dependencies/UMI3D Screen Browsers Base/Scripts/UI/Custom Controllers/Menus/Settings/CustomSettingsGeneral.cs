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

public class CustomSettingsGeneral : CustomSettingScreen
{
    public override string USSCustomClassName => "setting-general";

    public CustomDropdown LanguageDropdown;
    public CustomDropdown ThemeDropdown;

    public override void InitElement()
    {
        base.InitElement();

        LanguageDropdown.label = "language";
        LanguageDropdown.choices = new List<string>() { "English" };
        LanguageDropdown.value = "English";

        ThemeDropdown.label = "Theme";
        ThemeDropdown.choices = new List<string>() { "Default" };
        ThemeDropdown.value = "Default";

        ScrollView.Add(LanguageDropdown);
        ScrollView.Add(ThemeDropdown);
    }

    public override void Set() => Set("General");
}
