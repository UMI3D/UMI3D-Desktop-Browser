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
    public bool IsGrounded => Mathf.Abs(playerTransform.position.y - data.groundYAxis) < data.stepEpsilon;
    /// <summary>
    /// Is player close to ground ?
    /// </summary>
    public bool IsCloseToGround => Mathf.Abs(playerTransform.position.y - data.groundYAxis) <= data.maxStepHeight + data.stepEpsilon + .1f;
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
            data.obstacleLayer
        );

        if (isColliding)
        {
            data.groundYAxis = hit.point.y;
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

        desiredTranslation = GetPossibleHorizontalTranslation(desiredTranslation);
        desiredTranslation = GetPossibleVerticalTranslation(desiredTranslation);
        bool willEndUpAboveNavMesh = WillTranslationEndUpAboveNavMesh(desiredTranslation);
        if (!willEndUpAboveNavMesh)
        {
            // No movement allowed that can ends up in the vacuum.
            float delta = GetVerticalTranslationToGround(data.groundYAxis);
            desiredTranslation = Vector3.up * delta;
        }

        return desiredTranslation;
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
           data.obstacleLayer,
           false
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
           data.obstacleLayer,
           false
       );

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
            desiredTranslation.y + .1f,
            data.obstacleLayer,
            true
        );
        if (!willCollide)
        {
            return desiredTranslation;
        }

        float delta = GetVerticalTranslationToGround(hit.point.y + data.maxStepHeight + data.stepEpsilon);
        return new()
        {
            x = desiredTranslation.x,
            y = delta,
            z = desiredTranslation.z,
        };
    }

    float GetVerticalTranslationToGround(float hitYAxis)
    {
        float delta = playerTransform.position.y - hitYAxis;
        return -delta;
    }

    /// <summary>
    /// Return true if a translation along <paramref name="direction"/> would move the player on a navmesh surface.
    /// </summary>
    /// <param name="desiredTranslation"></param>
    /// <returns></returns>
    public bool WillTranslationEndUpAboveNavMesh(Vector3 desiredTranslation)
    {
        return colliderDelegate.WillCollide(
            new()
            {
                x = desiredTranslation.x,
                y = 0f,
                z = desiredTranslation.z,
            },
            Vector3.down,
            out RaycastHit hit,
            100f,
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
                data.obstacleLayer,
                false
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
                data.obstacleLayer,
                false
            );
        };
        return !data.IsCrouching || isCrouchingAndWillNotCollideIfStandUp();
    }
}
