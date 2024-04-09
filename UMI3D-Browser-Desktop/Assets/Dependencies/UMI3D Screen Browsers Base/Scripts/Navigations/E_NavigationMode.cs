using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_NavigationMode
{
    /// <summary>
    /// The default mode for the platform.
    /// </summary>
    Default, 
    /// <summary>
    /// Moving like a PC FPS
    /// </summary>
    Continuous,
    /// <summary>
    /// Moving like a VR FPS
    /// </summary>
    Teleportation,
    /// <summary>
    /// No collision, can fly.
    /// </summary>
    Debug
}
