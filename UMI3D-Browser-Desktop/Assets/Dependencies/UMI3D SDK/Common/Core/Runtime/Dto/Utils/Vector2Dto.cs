﻿/*
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

namespace umi3d.common
{
    /// <summary>
    /// Serializable implementation of a vector with 2 float coordinates.
    /// </summary>
    [Serializable]
    public class Vector2Dto : UMI3DDto
    {
        public float X { get; set; }
        public float Y { get; set; }

        public static Vector2Dto one { get => new Vector2Dto() { X = 1, Y = 1 }; } 
        public static Vector2Dto zero { get => new Vector2Dto() { X = 0, Y = 0 }; }

        public override string ToString()
        {
            return $"{base.ToString()}[{X},{Y}]";
        }

        public float this[int i]
        {
            get { if (i == 0) return X; else if (i == 1) return Y; else throw new ArgumentOutOfRangeException(); }
            set { if (i == 0) X = value; else if (i == 1) Y = value; else throw new ArgumentOutOfRangeException(); }
        }
    }
}