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
using MathNet.Numerics;
using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.baseBrowser.Navigation;
using UnityEngine;

public sealed class UMI3DCollisionManager
{
    #region Dependencies

    public Transform playerTransform;

    public Transform topHead;
    /// <summary>
    /// List of point which from rays will be created to check if there is a navmesh under player's feet
    /// </summary>
    public List<Transform> feetRaycastOrigin;

    public LayerMask obstacleLayer;
    public LayerMask navmeshLayer;

    public BaseFPSData data;

    #endregion

    #region Collision

    const int hitsLength = 10;
    RaycastHit[] hits = new RaycastHit[hitsLength];

    public (Vector3, Vector3) GetCapsuleSphereCenters()
    {
        return
        (
            playerTransform.position + playerTransform.up * (data.playerRadius + data.maxStepHeight + data.stepEpsilon),
            topHead.position - playerTransform.up * data.playerRadius
        );
    }

    /// <summary>
    /// Return true if a translation along <paramref name="direction"/> would move the player on a navmesh surface.
    /// </summary>
    /// <param name="desiredDirection"></param>
    /// <returns></returns>
    public bool WillTranslationEndUpAboveNavMesh(Vector3 desiredDirection)
    {
        desiredDirection /= 2f;
        // Try to find the nearest collision to the player. 
        RaycastHit? _foundHit = null;
        foreach (Transform foot in feetRaycastOrigin)
        {
            var hasHit = Physics.Raycast(
                foot.position + Vector3.up * data.maxStepHeight + desiredDirection,
                Vector3.down,
                out RaycastHit hit,
                100,
                navmeshLayer
            );
            Func<bool> hasFoundACloserHit = () => hit.distance < (_foundHit?.distance ?? Mathf.Infinity);
            Func<bool> isSlopeInfToMaxSlop = () => Vector3.Angle(playerTransform.up, hit.normal) <= data.maxSlopeAngle;

            if (hasHit && hasFoundACloserHit() && isSlopeInfToMaxSlop())
            {
                _foundHit = hit;
            }
        }

        return _foundHit.HasValue;

        //if (!_foundHit.HasValue)
        //{
        //    hasGroundHeightChangedLastFrame = false;
        //    return false;
        //}

        //// If a collision has been found update groundHeight. ?????????
        //RaycastHit foundHit = _foundHit.Value;
        //float newGroundHeight = foundHit.point.y;
        //groundYAxis = newGroundHeight;
        //hasGroundHeightChangedLastFrame = Mathf.Abs(newGroundHeight - groundYAxis) > .001f;
        //return true;
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
        var capsule = GetCapsuleSphereCenters();
        return !Physics.CapsuleCast(
                capsule.Item1,
                capsule.Item2,
                data.playerRadius,
                direction,
                out hit,
                maxDistance,
                layer
            );
    }

    public void ComputeGround()
    {
        RaycastHit? _foundHit = null;
        var capsule = GetCapsuleSphereCenters();
        int hitCount = Physics.CapsuleCastNonAlloc(
            capsule.Item1,
            capsule.Item2,
            data.playerRadius,
            Vector3.down,
            hits,
            100,
            navmeshLayer
        );
        hitCount = Math.Min(hitCount, hitsLength);
        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit hit = hits[i];
            var hasFoundACloserHit = hit.distance < (_foundHit?.distance ?? Mathf.Infinity);
            var isSlopeInfToMaxSlop = Vector3.Angle(playerTransform.up, hit.normal) <= data.maxSlopeAngle;
            if (hasFoundACloserHit && isSlopeInfToMaxSlop)
            {
                _foundHit = hit;
            }
        }
        if (_foundHit.HasValue)
        {
            data.groundYAxis = _foundHit.Value.point.y;
        }
    }

    /// <summary>
    /// Checks if player can jump.
    /// <list type="bullet">
    /// <item>
    /// <see cref="BaseFPSData.WantToJump"/> is true.
    /// </item>
    /// <item>
    /// <see cref="IsGrounded"/> is true.
    /// </item>
    /// <item>
    /// The player will not collide with an obstacle.
    /// </item>
    /// </list>
    /// </summary>
    /// <returns></returns>
    public bool CanJump()
    {
        return data.WantToJump
            && IsGrounded
            && !WillCollide(
                playerTransform.up,
                out var hit,
                data.MaxJumpHeight,
                obstacleLayer
            );
    }

    public bool CanStandUp()
    {
        Func<bool> isSquattingAndWillNotCollideIfStandUp 
            = () => 
            data.IsCrouching
            && !WillCollide(
                playerTransform.up,
                out var hit,
                data.MaxJumpHeight, // TODO: update this value.
                obstacleLayer
            );
        return !data.IsCrouching || isSquattingAndWillNotCollideIfStandUp();
    }

    #endregion

    /// <summary>
    /// Returns a direction close to the desired direction in which the player 
    /// <list type="bullet">
    /// <item>will not collide with obstacle.</item>
    /// <item>will not ends up in the vacuum.</item>
    /// </list>
    /// </summary>
    /// <param name="desiredDirection"></param>
    /// <returns></returns>
    public Vector3 GetPossibleDirection(Vector3 desiredDirection)
    {
        if (data.navigationMode == E_NavigationMode.Debug)
        {
            return desiredDirection;
        }

        bool willTranslationEndUpAboveNavMesh = WillTranslationEndUpAboveNavMesh(desiredDirection);
        if (!willTranslationEndUpAboveNavMesh)
        {
            // No movement allowed that can ends up in the vacuum.
            return Vector3.zero;
        }

        bool willCollide = WillCollide(
            desiredDirection,
            out var hit,
            IsGrounded ? desiredDirection.magnitude * 1.01f : 1f,
            obstacleLayer
        );
        if (!willCollide)
        {
            return desiredDirection;
        }


        Vector3 normal = Vector3.ProjectOnPlane(hit.normal, Vector3.up);
        Vector3 projectedDirection = Vector3.Project(
            desiredDirection,
            Quaternion.Euler(0, 90, 0) * normal
        );
        willCollide = WillCollide(
            projectedDirection,
            out hit,
            .2f,
            obstacleLayer
        );
        return willCollide ? Vector3.zero : projectedDirection;
    }

    /// <summary>
    /// Is player currently grounded ?
    /// </summary>
    public bool IsGrounded => Mathf.Abs(playerTransform.position.y - data.groundYAxis) < data.maxStepHeight;

    public bool ShouldSquat => data.WantToCrouch || !CanStandUp();
}
