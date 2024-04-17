/*
Copyright 2019 - 2024 Inetum

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

public static class Vector3Extension
{
    /// <summary>
    /// For each component (x, y, z) Mathf.DeltaAngle(0, component)
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 NormalizeAngle(this Vector3 angle)
    {
        angle.x = Mathf.DeltaAngle(0, angle.x);
        angle.y = Mathf.DeltaAngle(0, angle.y);
        angle.z = Mathf.DeltaAngle(0, angle.z);

        return angle;
    }
}
