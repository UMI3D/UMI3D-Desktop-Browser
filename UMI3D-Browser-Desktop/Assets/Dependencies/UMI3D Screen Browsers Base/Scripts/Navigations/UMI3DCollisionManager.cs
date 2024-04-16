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
using inetum.unityUtils;
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

    public BaseFPSData data;

    #endregion

    /// <summary>
    /// Is player currently grounded ?
    /// </summary>
    public bool IsGrounded => Mathf.Abs(playerTransform.position.y - data.groundYAxis) < data.maxStepHeight;
    /// <summary>
    /// Whether or not the player is below the ground.
    /// </summary>
    public bool IsBelowGround => playerTransform.position.y < data.groundYAxis;

    /// <summary>
    /// Get the ground height position.
    /// </summary>
    public void ComputeGround()
    {
        var isColliding = colliderDelegate.WillCollide(
            Vector3.down,
            out RaycastHit hit,
            100,
            data.navmeshLayer
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
        UnityEngine.Debug.Log($"message");
        desiredTranslation = GetPossibleHorizontalTranslation(desiredTranslation);
        desiredTranslation = GetPossibleVerticalTranslation(desiredTranslation);

        bool willCollide = colliderDelegate.WillCollide(
            desiredTranslation,
            out RaycastHit hit,
            .2f,
            data.obstacleLayer
        );
        return willCollide ? Vector3.zero : desiredTranslation;
    }

    public Vector3 GetPossibleHorizontalTranslation(Vector3 desiredTranslation)
    {
        Vector3 horizontalDesiredTranslation = new()
        {
            x = desiredTranslation.x,
            y = 0f,
            z = desiredTranslation.z,
        };
        bool willCollide = colliderDelegate.WillCollide(
           horizontalDesiredTranslation,
           out RaycastHit hit,
           horizontalDesiredTranslation.magnitude + .1f,
           data.obstacleLayer
       );
        if (!willCollide)
        {
            return desiredTranslation;
        }

        Vector3 projection = Vector3.ProjectOnPlane(
            horizontalDesiredTranslation,
            hit.normal
        );

        willCollide = colliderDelegate.WillCollide(
           projection,
           out hit,
           projection.magnitude + .1f,
           data.obstacleLayer
       );

        UnityEngine.Debug.Log($"hozi hit = {hit.transform.name}, {hit.transform.position.y}");

        return new()
        {
            x = willCollide ? 0f : projection.x,
            y = desiredTranslation.y,
            z = willCollide ? 0f : projection.z,
        };
    }

    public Vector3 GetPossibleVerticalTranslation(Vector3 desiredTranslation)
    {
        Vector3 horizontalDesiredTranslation = new()
        {
            x = desiredTranslation.x,
            y = 0f,
            z = desiredTranslation.z,
        };
        Vector3 verticalDesiredTranslation = Vector3.up * desiredTranslation.y;

        bool willCollide = colliderDelegate.WillCollide(
            horizontalDesiredTranslation,
            verticalDesiredTranslation,
            out RaycastHit hit,
            data.maxStepHeight + data.stepEpsilon,
            data.obstacleLayer
        );
        if (!willCollide)
        {
            return desiredTranslation;
        }

        Vector3 projection = Vector3.ProjectOnPlane(
            verticalDesiredTranslation,
            hit.normal
        );

        willCollide = colliderDelegate.WillCollide(
            horizontalDesiredTranslation,
            projection,
            out hit,
            data.maxStepHeight + data.stepEpsilon,
            data.obstacleLayer
        );

        if (!willCollide)
        {
            return new()
            {
                x = desiredTranslation.x,
                y = projection.y,
                z = desiredTranslation.z,
            };
        }

        float delta = playerTransform.position.y - hit.transform.position.y;
        UnityEngine.Debug.Log($"verti hit = {hit.transform.name}, {hit.transform.position.y}, {delta}");
        //var y = Mathf.Lerp(0f, delta, 2);
        return new()
        {
            x = desiredTranslation.x,
            y = - delta,
            z = desiredTranslation.z,
        };
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
            data.navmeshLayer,
            true
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
        return IsGrounded
            && !colliderDelegate.WillCollide(
                Vector3.up,
                out var hit,
                data.maxJumpAltitude,
                data.obstacleLayer
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
                data.obstacleLayer
            );
        };
        return !data.IsCrouching || isCrouchingAndWillNotCollideIfStandUp();
    }
}
