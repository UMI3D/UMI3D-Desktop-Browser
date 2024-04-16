using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.baseBrowser.Navigation;
using UnityEngine;

/// <summary>
/// A capsule is composed of:
/// <list type="bullet">
/// <item>The position of the bottom sphere center.</item>
/// <item>The position of the top sphere center.</item>
/// <item>The radius of the spheres.</item>
/// </list>
/// </summary>
public class UMI3DPlayerCapsuleColliderDelegate : IPlayerColliderDelegate
{
    #region Dependencies

    public Transform playerTransform;
    public BaseFPSData data;

    #endregion

    [Serializable]
    public struct CapsuleCollider
    {
        public Vector3 topSphereCenter;
        public Vector3 bottomSphereCenter;
        public float radius;

        /// <summary>
        /// Create a Capsule collider with:
        /// <list type="bullet">
        /// <item>the position of the top sphere center,</item>
        /// <item>the maximum height of a step,</item>
        /// <item>the radius of the spheres.</item>
        /// </list>
        /// </summary>
        /// <param name="topSphereCenter"></param>
        /// <param name="step"></param>
        /// <param name="radius"></param>
        public CapsuleCollider(Vector3 topSphereCenter, float step, float radius)
        {
            this.topSphereCenter = topSphereCenter;
            this.bottomSphereCenter = Vector3.up * (step + radius);
            this.radius = radius;
        }

        public CapsuleCollider GetWorldPosition(Transform playerTransform)
        {
            return new()
            {
                topSphereCenter = topSphereCenter + playerTransform.position,
                bottomSphereCenter = bottomSphereCenter + playerTransform.position,
                radius = radius
            };
        }

        public CapsuleCollider ProjectCollider(Vector3 offset)
        {
            return new()
            {
                topSphereCenter = topSphereCenter + offset,
                bottomSphereCenter = bottomSphereCenter + offset,
                radius = radius
            };
        }
    }

    CapsuleCollider capsule;
    CapsuleCollider worldPositionCapsule;
#if UNITY_EDITOR
    CapsuleCollider projectedCapsule;
#endif

    public void Init()
    {
        capsule = new(
            data.topSphereCenter,
            data.maxStepHeight + data.stepEpsilon,
            data.capsuleRadius
        );
    }

    public void ComputeCollider()
    {
        UpdateWorldPositionCapsule();
    }

    public void UpdateWorldPositionCapsule()
    {
        worldPositionCapsule = capsule.GetWorldPosition(playerTransform);
    }

    /// <summary>
    /// Return true if the player will collide with something 
    /// that has a layer [<paramref name="layer"/>] 
    /// in the direction [<paramref name="direction"/>].<br/>
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="layer"></param>
    /// <param name="hit"></param>
    /// <returns></returns>
    public bool WillCollide(
        Vector3 direction,
        out RaycastHit hit,
        float maxDistance,
        LayerMask layer
    )
    {
        UpdateProjectedCapsule(direction * maxDistance);
        return Physics.CapsuleCast(
                worldPositionCapsule.bottomSphereCenter,
                worldPositionCapsule.topSphereCenter,
                worldPositionCapsule.radius,
                direction,
                out hit,
                maxDistance,
                layer
            );
    }

    
    public void UpdateProjectedCapsule(Vector3 offset)
    {
#if UNITY_EDITOR
        projectedCapsule = worldPositionCapsule.ProjectCollider(offset);
#endif
    }

    public void DrawGizmos()
    {
#if UNITY_EDITOR
        // World position capsule.
        //UpdateWorldPositionCapsule();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(worldPositionCapsule.bottomSphereCenter, worldPositionCapsule.radius);
        Gizmos.DrawWireSphere(worldPositionCapsule.topSphereCenter, worldPositionCapsule.radius);

        // Projected capsule.
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(projectedCapsule.bottomSphereCenter, projectedCapsule.radius);
        Gizmos.DrawWireSphere(projectedCapsule.topSphereCenter, projectedCapsule.radius);
#endif
    }
}
