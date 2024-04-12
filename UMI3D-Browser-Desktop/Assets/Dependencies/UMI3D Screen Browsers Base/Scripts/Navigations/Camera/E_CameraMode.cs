using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_CameraMode
{
    /// <summary>
    /// The camera is locked.<br/>
    /// The user cannot move the camera.
    /// </summary>
    Locked,
    /// <summary>
    /// The user can move the camera in the limit of the neck rotation.<br/>
    /// The movement of the camera is navigation independent. 
    /// That means that the direction of the camera is not necessary the direction where the player will move.
    /// </summary>
    NeckMovement,
    /// <summary>
    /// The user can move the camera.<br/> 
    /// The direction of the camera is the direction where the player will move.
    /// </summary>
    Navigation,
    /// <summary>
    /// The user can move the camera in every direction.
    /// </summary>
    Free
}
