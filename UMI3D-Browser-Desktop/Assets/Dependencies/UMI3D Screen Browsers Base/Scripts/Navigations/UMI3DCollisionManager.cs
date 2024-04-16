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
    public IPlayerColliderDelegate colliderDelegate;

    public LayerMask obstacleLayer;
    public LayerMask navmeshLayer;

    public BaseFPSData data;

    #endregion

    /// <summary>
    /// Is player currently grounded ?
    /// </summary>
    public bool IsGrounded => Mathf.Abs(playerTransform.position.y - data.groundYAxis) < data.maxStepHeight;

    /// <summary>
    /// Whether the user want to crouch or is crouching and cannot stand up.
    /// </summary>
    public bool IsCrouched => data.WantToCrouch || !CanStandUp();

    public void ComputeGround()
    {
        var isColliding = colliderDelegate.WillCollide(
            Vector3.down,
            out RaycastHit hit,
            100,
            navmeshLayer
        );

        if (isColliding)
        {
            data.groundYAxis = hit.transform.position.y;
        }
    }

    /// <summary>
    /// Returns a translation close to the desired translation in which the player 
    /// <list type="bullet">
    /// <item>will not collide with obstacle.</item>
    /// <item>will not ends up in the vacuum.</item>
    /// </list>
    /// </summary>
    /// <param name="desiredTranslation"></param>
    /// <returns></returns>
    public Vector3 GetPossibleTranslation(Vector3 desiredTranslation)
    {
        if (data.navigationMode == E_NavigationMode.Debug)
        {
            return desiredTranslation;
        }

        bool willEndUpAboveNavMesh = WillTranslationEndUpAboveNavMesh(desiredTranslation);
        if (!willEndUpAboveNavMesh)
        {
            // No movement allowed that can ends up in the vacuum.
            return Vector3.zero;
        }

        bool willCollide = colliderDelegate.WillCollide(
            desiredTranslation,
            out var hit,
            IsGrounded ? desiredTranslation.magnitude * 1.01f : 1f,
            obstacleLayer
        );
        if (!willCollide)
        {
            return desiredTranslation;
        }
        UnityEngine.Debug.Log($"collide = {hit.transform?.name}");


        Vector3 normal = Vector3.ProjectOnPlane(
            hit.normal,
            Vector3.up
        );
        Vector3 projectedDirection = Vector3.Project(
            desiredTranslation,
            Quaternion.Euler(0, 90, 0) * normal
        );
        willCollide = colliderDelegate.WillCollide(
            projectedDirection,
            out hit,
            .2f,
            obstacleLayer
        );
        return willCollide ? Vector3.zero : projectedDirection;
    }

    /// <summary>
    /// Return true if a translation along <paramref name="direction"/> would move the player on a navmesh surface.
    /// </summary>
    /// <param name="desiredTranslation"></param>
    /// <returns></returns>
    public bool WillTranslationEndUpAboveNavMesh(Vector3 desiredTranslation)
    {
        return colliderDelegate.WillCollide(
            desiredTranslation,
            Vector3.down,
            out RaycastHit hit,
            100,
            navmeshLayer
        );
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
            && !colliderDelegate.WillCollide(
                Vector3.up,
                out var hit,
                data.MaxJumpHeight,
                obstacleLayer
            );
    }

    /// <summary>
    /// Check if the player can stand up.
    /// </summary>
    /// <returns></returns>
    public bool CanStandUp()
    {
        Func<bool> isCrouchingAndWillNotCollideIfStandUp = () =>
        {
            return !colliderDelegate.WillCollide(
                Vector3.zero,
                out RaycastHit hit,
                0f,
                obstacleLayer
            );
        };
        return !data.IsCrouching || isCrouchingAndWillNotCollideIfStandUp();
    }
}
