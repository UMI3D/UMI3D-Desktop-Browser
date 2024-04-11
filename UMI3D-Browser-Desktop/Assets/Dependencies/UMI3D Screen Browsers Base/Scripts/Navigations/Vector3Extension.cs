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
