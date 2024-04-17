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

public interface IPlayerColliderDelegate
{
    /// <summary>
    /// Set collider to right positions.
    /// </summary>
    void ComputeCollider();

    /// <summary>
    /// Return true if the player will collide with something 
    /// that has a layer [<paramref name="layer"/>] 
    /// in the direction [<paramref name="direction"/>].<br/>
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="layer"></param>
    /// <param name="hit"></param>
    /// <returns></returns>
    bool WillCollide(
        Vector3 direction,
        out RaycastHit hit,
        float maxDistance,
        LayerMask layer,
        bool includeFeet,
        bool drawGizmo = false
    );

    /// <summary>
    /// Return true if the player will collide with something 
    /// that has a layer [<paramref name="layer"/>] 
    /// in the direction [<paramref name="direction"/>].<br/>
    /// <br/>
    /// The collider is first projected with an offset of <paramref name="offset"/> before computing collisiton.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="layer"></param>
    /// <param name="hit"></param>
    /// <returns></returns>
    public bool WillCollide(
        Vector3 offset,
        Vector3 direction,
        out RaycastHit hit,
        float maxDistance,
        LayerMask layer,
        bool includeFeet,
        bool drawGizmo = false
    );

    /// <summary>
    /// Draw gizmos to debug collider.
    /// </summary>
    void DrawGizmos();
}
