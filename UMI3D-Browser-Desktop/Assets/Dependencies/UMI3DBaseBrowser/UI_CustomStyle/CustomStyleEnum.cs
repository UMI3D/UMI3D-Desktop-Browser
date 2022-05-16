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
        /// Menu Primary Light color.
        /// </summary>
        MenuPrimaryLight = 20,
        /// <summary>
        /// Menu Primary Dark color.
        /// </summary>
        MenuPrimaryDark = 21,
        /// <summary>
        /// Menu Secondary Light color.
        /// </summary>
        MenuSecondaryLight = 22,
        /// <summary>
        /// Menu Secondary Dark color.
        /// </summary>
        MenuSecondaryDark = 23,
        /// <summary>
        /// Menu Transparent Light color.
        /// </summary>
        MenuTransparentLight = 24,
        /// <summary>
        /// Menu Transparent Dark color.
        /// </summary>
        MenuTransparentDark = 25,

        /// <summary>
        /// Label Primary Light color
        /// </summary>
        LabelPrimaryLight = 26,
        /// <summary>
        /// Label Primary Dark color
        /// </summary>
        LabelPrimaryDark = 27,
        /// <summary>
        /// Label Secondary Light color
        /// </summary>
        LabelSecondaryLight = 28,
        /// <summary>
        /// Label Secondary Dark color
        /// </summary>
        LabelSecondaryDark = 29,

        /// <summary>
        /// Icon Primary Light color
        /// </summary>
        IconPrimaryLight = 30,
        /// <summary>
        /// Icon Primary Dark color
        /// </summary>
        IconPrimaryDark = 31,
        /// <summary>
        /// Icon Secondary Light color
        /// </summary>
        IconSecondaryLight = 32,
        /// <summary>
        /// Icon Secondary Dark color
        /// </summary>
        IconSecondaryDark = 33,
    }

    public static class CustomStyleKeywordMethods
    {
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
        /// Menu Primary Light color.
        /// </summary>
        MenuPrimaryLight = 20,
        /// <summary>
        /// Menu Primary Dark color.
        /// </summary>
        MenuPrimaryDark = 21,
        /// <summary>
        /// Menu Secondary Light color.
        /// </summary>
        MenuSecondaryLight = 22,
        /// <summary>
        /// Menu Secondary Dark color.
        /// </summary>
        MenuSecondaryDark = 23,
        /// <summary>
        /// Menu Transparent Light color.
        /// </summary>
        MenuTransparentLight = 24,
        /// <summary>
        /// Menu Transparent Dark color.
        /// </summary>
        MenuTransparentDark = 25,

        /// <summary>
        /// Label Primary Light color
        /// </summary>
        LabelPrimaryLight = 26,
        /// <summary>
        /// Label Primary Dark color
        /// </summary>
        LabelPrimaryDark = 27,
        /// <summary>
        /// Label Secondary Light color
        /// </summary>
        LabelSecondaryLight = 28,
        /// <summary>
        /// Label Secondary Dark color
        /// </summary>
        LabelSecondaryDark = 29,

        /// <summary>
        /// Icon Primary Light color
        /// </summary>
        IconPrimaryLight = 30,
        /// <summary>
        /// Icon Primary Dark color
        /// </summary>
        IconPrimaryDark = 31,
        /// <summary>
        /// Icon Secondary Light color
        /// </summary>
        IconSecondaryLight = 32,
        /// <summary>
        /// Icon Secondary Dark color
        /// </summary>
        IconSecondaryDark = 33,
    }
    
    public enum CustomStyleSizeMode
    {
        Px = 0,
        Percent = 1
    }

    public enum MouseBehaviour
    {
        MouseOut,
        MouseOver,
        MousePressed,
    }
}
