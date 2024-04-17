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
