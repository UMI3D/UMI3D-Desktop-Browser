/*
Copyright 2019 - 2023 Inetum

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

namespace umi3d.baseBrowser.Navigation
{
    public partial class BaseFPSNavigation
    {
        #region Field

        [Header("Collision")]
        [SerializeField]
        public LayerMask obstacleLayer;
        [SerializeField]
        public LayerMask navmeshLayer;
        [SerializeField]
        protected Transform topHead;
        [SerializeField]
        [Tooltip("List of point which from rays will be created to check if there is a navmesh under player's feet")]
        protected List<Transform> feetRaycastOrigin;
        [SerializeField]
        [Tooltip("Radius used from player center to raycast")]
        protected float playerRadius = .3f;
        [SerializeField]
        protected float maxSlopeAngle = 45f;
        [SerializeField]
        protected float maxStepHeight = .2f;

        /// <summary>
        /// Is player currently grounded ?
        /// </summary>
        public bool IsGrounded => Mathf.Abs(transform.position.y - groundHeight) < maxStepHeight;

        protected float lastObstacleHeight = .5f;
        /// <summary>
        /// Current ground height.
        /// </summary>
        protected float groundHeight = 0;
        /// <summary>
        /// Has <see cref="groundHeight"/> changed last frame ?
        /// </summary>
        protected bool hasGroundHeightChangedLastFrame = false;
        protected Vector3 currentCapsuleBase, currentCapsuleEnd;
        protected float stepEpsilon = 0.05f;

#if UNITY_EDITOR

        private Vector3 collisionHitPoint;

#endif

        #endregion

        /// <summary>
        /// Checks if a translation along <paramref name="direction"/> would move the player on a navmesh surface.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected bool CheckNavmesh(Vector3 direction)
        {
            RaycastHit hit;
            RaycastHit foundHit = new RaycastHit { distance = Mathf.Infinity };
            direction /= 2f;
            foreach (Transform foot in feetRaycastOrigin)
            {
                if (
                    UnityEngine.Physics.Raycast(foot.position + Vector3.up * maxStepHeight + direction, Vector3.down, out hit, 100, navmeshLayer)
                    && foundHit.distance > hit.distance
                    && Vector3.Angle(transform.up, hit.normal) <= maxSlopeAngle
                    )
                    foundHit = hit;
            }
            if (foundHit.distance < Mathf.Infinity)
            {
                float newHeight = foundHit.point.y;
                if (Mathf.Abs(newHeight - groundHeight) > .001f)
                {
                    groundHeight = newHeight;
                    hasGroundHeightChangedLastFrame = true;
                }
                else hasGroundHeightChangedLastFrame = false;
                return true;
            }
            hasGroundHeightChangedLastFrame = false;
            return false;
        }
        /// <summary>
        /// Checks if there is an obstacle for the player and returns an allowed movement for the player.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected Vector3 CheckCollision(Vector3 direction)
        {
            if (Physics.CapsuleCast(currentCapsuleBase, currentCapsuleEnd, playerRadius, direction, out var hit, IsGrounded ? direction.magnitude * 1.01f : 1f, obstacleLayer))
            {

#if UNITY_EDITOR
                collisionHitPoint = hit.point;
#endif

                Vector3 normal = Vector3.ProjectOnPlane(hit.normal, Vector3.up);
                Vector3 projectedDirection = Vector3.Project(direction, Quaternion.Euler(0, 90, 0) * normal);

                if (Physics.CapsuleCast(currentCapsuleBase, currentCapsuleEnd, playerRadius, projectedDirection, .2f, obstacleLayer))
                {
                    return Vector3.zero;
                }
                else
                {
                    return projectedDirection;
                }
            }

            return direction;
        }

        /// <summary>
        /// Checks under player's feet to update <see cref="groundHeight"/>.
        /// </summary>
        protected void UpdateBaseHeight()
        {
            RaycastHit foundHit = new RaycastHit { distance = Mathf.Infinity };
            RaycastHit[] hits = Physics.CapsuleCastAll(currentCapsuleBase, currentCapsuleEnd, playerRadius, Vector3.down, 100, navmeshLayer);
            foreach (RaycastHit hit in hits)
            {
                if ((foundHit.distance > hit.distance) && (Vector3.Angle(transform.up, hit.normal) <= maxSlopeAngle))
                {
                    foundHit = hit;
                }
            }
            if ((foundHit.distance < Mathf.Infinity) && (Vector3.Angle(transform.up, foundHit.normal) <= maxSlopeAngle)) groundHeight = foundHit.point.y;
        }

        /// <summary>
        /// Checks if player can jump.
        /// </summary>
        /// <returns></returns>
        public bool CanJump()
        {
            if (!IsGrounded)
                return false;

            return !Physics.CapsuleCast(currentCapsuleBase, currentCapsuleEnd, playerRadius, transform.up, .5f, obstacleLayer);
        }

        protected (Vector3, Vector3) GetCapsuleSphereCenters()
        {
            return
            (
                transform.position + transform.up * (playerRadius + maxStepHeight + stepEpsilon),
                topHead.position - transform.up * playerRadius
            );
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(currentCapsuleBase, playerRadius);
            Gizmos.DrawWireSphere(currentCapsuleEnd, playerRadius);
            Gizmos.DrawWireSphere(collisionHitPoint, .1f);
        }
#endif
    }
}
