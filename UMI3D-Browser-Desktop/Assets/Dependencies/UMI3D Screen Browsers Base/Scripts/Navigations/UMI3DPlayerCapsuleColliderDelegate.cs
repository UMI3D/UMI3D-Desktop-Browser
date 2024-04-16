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

        /// <summary>
        /// Project a collider toward <paramref name="direction"/>.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public CapsuleCollider ProjectCollider(Vector3 direction)
        {
            return new()
            {
                topSphereCenter = topSphereCenter + direction,
                bottomSphereCenter = bottomSphereCenter + direction,
                radius = radius
            };
        }
    }

    CapsuleCollider capsule;
    CapsuleCollider worldPositionCapsule;
#if UNITY_EDITOR
    CapsuleCollider debugCapsule;
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
        worldPositionCapsule = capsule.ProjectCollider(playerTransform.position);
    }

    public bool WillCollide(
        Vector3 direction,
        out RaycastHit hit,
        float maxDistance,
        LayerMask layer,
        bool drawGizmo = false
    )
    {
        UpdateDebugCapsule(worldPositionCapsule, direction * maxDistance);
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

    public bool WillCollide(
        Vector3 offset,
        Vector3 direction,
        out RaycastHit hit,
        float maxDistance,
        LayerMask layer,
        bool drawGizmo = false
    )
    {
        CapsuleCollider worldPositionCapsuleWithOffset = worldPositionCapsule.ProjectCollider(offset);
        if (drawGizmo)
        {
            UpdateDebugCapsule(worldPositionCapsuleWithOffset, direction);
        }
        return Physics.CapsuleCast(
                worldPositionCapsuleWithOffset.bottomSphereCenter,
                worldPositionCapsuleWithOffset.topSphereCenter,
                worldPositionCapsuleWithOffset.radius,
                direction,
                out hit,
                maxDistance,
                layer
            );
    }

    public void UpdateDebugCapsule(CapsuleCollider capsule, Vector3 direction)
    {
#if UNITY_EDITOR
        debugCapsule = capsule.ProjectCollider(direction);
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
        Gizmos.DrawWireSphere(debugCapsule.bottomSphereCenter, debugCapsule.radius);
        Gizmos.DrawWireSphere(debugCapsule.topSphereCenter, debugCapsule.radius);
#endif
    }
}
