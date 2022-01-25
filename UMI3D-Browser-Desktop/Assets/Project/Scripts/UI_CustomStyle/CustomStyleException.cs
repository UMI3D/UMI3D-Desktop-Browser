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
using System;

namespace umi3DBrowser.UICustomStyle
{
    public class KeyNotFoundException : Exception
    {
        private string m_key;

        public KeyNotFoundException(string key) : base($"[{key}] not found.")
        {
            this.m_key = key;
        }

        public KeyNotFoundException(string key, string keyUsingObject) : base($"[{key}] not found in [{keyUsingObject}].")
        {
            this.m_key = key;
        }

        public string Key => m_key;
    }

    public class ThemeNotFoundException : Exception
    {
        private string m_theme;

        public ThemeNotFoundException(Theme_SO theme) : base($"[{theme.name}] not found.")
        {
            this.m_theme = theme.name;
        }

        public ThemeNotFoundException(Theme_SO theme, string keyUsingObject) : base($"[{theme.name}] not found in [{keyUsingObject}].")
        {
            this.m_theme = theme.name;
        }

        public string Theme => m_theme;
    }
}