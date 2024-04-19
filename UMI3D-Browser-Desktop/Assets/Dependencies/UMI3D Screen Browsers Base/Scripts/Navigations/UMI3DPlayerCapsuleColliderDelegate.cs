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

    /// <summary>
    /// Capsule that excludes feet to avoid step.
    /// </summary>
    CapsuleCollider capsule;
    CapsuleCollider worldPositionCapsule;
#if UNITY_EDITOR
    CapsuleCollider debugCapsule;
    Vector3 collisionPoint;

    Func<CapsuleCollider> capsuleRef;
    Vector3 projection;
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
        UpdateDebugCapsule();
    }

    public void UpdateWorldPositionCapsule()
    {
        worldPositionCapsule = capsule.ProjectCollider(playerTransform.position);
    }

    public void UpdateDebugCapsule()
    {
#if UNITY_EDITOR
        var capsule = capsuleRef?.Invoke() ?? worldPositionCapsule;
        debugCapsule = capsule.ProjectCollider(projection);
#endif
    }

    public bool WillCollide(
        Vector3 direction,
        out RaycastHit hit,
        float maxDistance,
        LayerMask layer,
        bool drawGizmo = false
    )
    {
        var capsule = worldPositionCapsule;
        var hasCollided = Physics.CapsuleCast(
            capsule.bottomSphereCenter,
            capsule.topSphereCenter,
            capsule.radius,
            direction,
            out hit,
            maxDistance,
            layer
        );
        if (drawGizmo)
        {
            if (hasCollided)
            {
                SetDebugCapsule(
                    () =>
                    {
                        return worldPositionCapsule;
                    },
                    direction * maxDistance
                );
                collisionPoint = hit.point;
            }
        }
        return hasCollided;
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
        CapsuleCollider capsule = worldPositionCapsule.ProjectCollider(offset);
        var hasCollided = Physics.CapsuleCast(
            capsule.bottomSphereCenter,
            capsule.topSphereCenter,
            capsule.radius,
            direction,
            out hit,
            maxDistance,
            layer
        );
        if (drawGizmo)
        {
            if (hasCollided)
            {
                SetDebugCapsule(
                    () =>
                    {
                        return worldPositionCapsule.ProjectCollider(offset);
                    }, 
                    direction * maxDistance
                );
                collisionPoint = hit.point;
            }
        }
        return hasCollided;
    }

    public void SetDebugCapsule(Func<CapsuleCollider> capsule, Vector3 direction)
    {
#if UNITY_EDITOR
        capsuleRef = capsule;
        projection = direction;
#endif
    }

    public void DrawGizmos()
    {
#if UNITY_EDITOR
        // World position capsule.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(worldPositionCapsule.bottomSphereCenter, worldPositionCapsule.radius);
        Gizmos.DrawWireSphere(worldPositionCapsule.topSphereCenter, worldPositionCapsule.radius);

        // Projected capsule.
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(debugCapsule.bottomSphereCenter, debugCapsule.radius);
        Gizmos.DrawWireSphere(debugCapsule.topSphereCenter, debugCapsule.radius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(collisionPoint, .05f);
#endif
    }
}
