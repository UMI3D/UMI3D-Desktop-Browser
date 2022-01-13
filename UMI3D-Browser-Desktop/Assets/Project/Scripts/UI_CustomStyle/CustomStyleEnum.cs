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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Browser.UICustomStyle
{
    public enum CustomStyleKeyword
    {
        VariableUndefined = 0,
        //Null = 1,
        //Auto = 2,
        //None = 3,
        //Initial = 4,
        ConstUndefined = 5,
        Variable = 6,
        Const = 7,
    }

    public enum CustomStyleSimpleKeyword
    {
        Undefined = 0,
        Variable = 6
    }
    
    public enum CustomStyleValueMode
    {
        Px = 0,
        Percent = 1
    }

    public enum CustomStyleTheme
    {
        All = 0,
        Default = 1
    }

    public enum CustomStyleBackgroundMode
    {
        MouseOut,
        MouseOver,
        MousePressed,

    }
}
