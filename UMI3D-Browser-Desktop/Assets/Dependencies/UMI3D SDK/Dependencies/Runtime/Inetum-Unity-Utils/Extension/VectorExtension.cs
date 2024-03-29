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

using UnityEngine;

namespace inetum.unityUtils
{

    public static class VectorExtension
    {

        public static Vector2 Unscaled(this Vector2 v2, Vector2 scale)
        {
            if (scale.x == 0) scale.x = 0.00001f;
            if (scale.y == 0) scale.y = 0.00001f;

            return new Vector2(v2.x * 1 / scale.x, v2.y * 1 / scale.y);
        }

        public static Vector2 Scaled(this Vector2 v2, Vector2 scale)
        {
            return new Vector2(v2.x * scale.x, v2.y * scale.y);
        }

        public static Vector3 Unscaled(this Vector3 v3, Vector3 scale)
        {
            if (scale.x == 0) scale.x = 0.00001f;
            if (scale.y == 0) scale.y = 0.00001f;
            if (scale.z == 0) scale.z = 0.00001f;

            return new Vector3(v3.x * 1 / scale.x, v3.y * 1 / scale.y, v3.z * 1 / scale.z);
        }

        public static Vector3 Scaled(this Vector3 v3, Vector3 scale)
        {
            return new Vector3(v3.x * scale.x, v3.y * scale.y, v3.z * scale.z);
        }

        public static Vector4 Unscaled(this Vector4 v4, Vector4 scale)
        {
            if (scale.x == 0) scale.x = 0.00001f;
            if (scale.y == 0) scale.y = 0.00001f;
            if (scale.z == 0) scale.z = 0.00001f;
            if (scale.w == 0) scale.w = 0.00001f;

            return new Vector4(v4.x * 1 / scale.x, v4.y * 1 / scale.y, v4.z * 1 / scale.z, v4.w * 1 / scale.w);
        }

        public static Vector4 Scaled(this Vector4 v4, Vector4 scale)
        {
            return new Vector4(v4.x * scale.x, v4.y * scale.y, v4.z * scale.z, v4.w * scale.w);
        }

        public static Vector4 ToVector4(this Quaternion q)
        {
            return new Vector4(q.x, q.y, q.z, q.w);
        }

        public static Quaternion ToQuaternion(this Vector4 v4)
        {
            return new Quaternion(v4.x, v4.y, v4.z, v4.w);
        }
    }
}