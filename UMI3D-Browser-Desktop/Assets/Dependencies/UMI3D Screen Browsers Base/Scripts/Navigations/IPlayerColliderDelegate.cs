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
        LayerMask layer
    );

    /// <summary>
    /// Draw gizmos to debug collider.
    /// </summary>
    void DrawGizmos();
}
