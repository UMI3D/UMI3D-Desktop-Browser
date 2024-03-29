﻿/*
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
using TMPro;
using UnityEngine.UI;

namespace BrowserDesktop.preferences.Theme
{

    public class PrincipalColorItem : ThemeListener
    {
        Image Image;
        Text Text;
        TMP_Text Text2;

        public override void ApplyTheme()
        {
            if(Image == null) Image = GetComponent<Image>();
            if (Text == null) Text = GetComponent<Text>();
            if (Text2 == null) Text2 = GetComponent<TMP_Text>();

            if (Image != null) Image.color = Theme.PrincipalColor;
            if (Text != null) Text.color = Theme.PrincipalTextColor;
            if (Text2 != null) Text2.color = Theme.PrincipalTextColor;
        }
    }

}