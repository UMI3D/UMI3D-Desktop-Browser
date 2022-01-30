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
namespace umi3DBrowser.UICustomStyle
{
    public enum CustomStyleKeyword
    {
        //Null = 1,
        //Auto = 2,
        //None = 3,
        //Initial = 4,

        /// <summary>
        /// No style will be applied.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Default style.
        /// </summary>
        Default = 1,
        /// <summary>
        /// Apply a custom style.
        /// </summary>
        Custom = 2,

        /// <summary>
        /// Apply a custom resizable style.
        /// </summary>
        CustomResizable = 10,
        /// <summary>
        /// Apply a custom unresizable style.
        /// </summary>
        CustomUnresizabe = 11,

        /// <summary>
        /// Primary color.
        /// </summary>
        Primary = 20,
        /// <summary>
        /// Secondary color.
        /// </summary>
        Secondary = 21,
        /// <summary>
        /// Tertiary color.
        /// </summary>
        Tertiary = 22,

        /// <summary>
        /// Apply a title style.
        /// </summary>
        Title = 30,
        /// <summary>
        /// Apply a subtitle style.
        /// </summary>
        Subtitle = 31,
        /// <summary>
        /// Apply a body style.
        /// </summary>
        Body = 32,
        /// <summary>
        /// Apply a foot-note style.
        /// </summary>
        FootNote = 33,
    }

    public static class CustomStyleKeywordMethods
    {
        //public static bool IsDefaultOrUndefined(this CustomStyleKeyword keyword)
        //{
        //    return keyword == CustomStyleKeyword.Default || keyword == CustomStyleKeyword.Undefined;
        //}
        public static bool IsCustom(this CustomStyleKeyword keyword)
            => keyword == CustomStyleKeyword.Custom || keyword == CustomStyleKeyword.CustomResizable || keyword == CustomStyleKeyword.CustomUnresizabe;
    }

    public enum CustomStyleExtraSimpleKeyword
    {
        /// <summary>
        /// No style will be applied.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Apply a custom style.
        /// </summary>
        Custom = 2,
    }

    public enum CustomStyleSimpleKeyword
    {
        /// <summary>
        /// No style will be applied.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Default style.
        /// </summary>
        Default = 1,
        /// <summary>
        /// Apply a custom style.
        /// </summary>
        Custom = 2,
    }

    public enum CustomStyleSizeKeyword
    {
        /// <summary>
        /// No style will be applied.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Default style.
        /// </summary>
        Default = 1,
        /// <summary>
        /// Apply a custom resizable style.
        /// </summary>
        CustomResizable = 10,
        /// <summary>
        /// Apply a custom unresizable style.
        /// </summary>
        CustomUnresizabe = 11,
    }

    public enum CustomStyleColorKeyword
    {
        /// <summary>
        /// No style will be applied.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Default style.
        /// </summary>
        Default = 1,
        /// <summary>
        /// Apply a custom style.
        /// </summary>
        Custom = 2,
        /// <summary>
        /// Primary color.
        /// </summary>
        Primary = 20,
        /// <summary>
        /// Secondary color.
        /// </summary>
        Secondary = 21,
        /// <summary>
        /// Tertiary color.
        /// </summary>
        Tertiary = 22,
    }

    public enum CustomStyleTextKeyword
    {
        /// <summary>
        /// No style will be applied.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Default style.
        /// </summary>
        Default = 1,
        /// <summary>
        /// Apply a custom style.
        /// </summary>
        Custom = 2,
        /// <summary>
        /// Apply a title style.
        /// </summary>
        Title = 30,
        /// <summary>
        /// Apply a subtitle style.
        /// </summary>
        Subtitle = 31,
        /// <summary>
        /// Apply a body style.
        /// </summary>
        Body = 32,
        /// <summary>
        /// Apply a foot-note style.
        /// </summary>
        FootNote = 33
    }
    
    public enum CustomStyleSizeMode
    {
        Px = 0,
        Percent = 1
    }

    public enum CustomStyleTheme
    {
        All = 0,
        Default = 1
    }

    public enum MouseBehaviour
    {
        MouseOut,
        MouseOver,
        MousePressed,
    }
}
