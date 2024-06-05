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

    public Func<CollisionDebugger> collisionDebugger = () => new();

    #endregion

    public struct CollisionDebugger
    {
        [Flags]
        public enum E_Collision
        {
            None = 0,
            Ground = 1,
            HorizontalFirstCollision = 1 << 1,
            HorizontalSecondCollision = 1 << 2,
            VerticalCollision = 1 << 3,
            JumpCollision = 1 << 4,
            StandUpCollision = 1 << 5,
        }

        public E_Collision collision;

        public bool DebugGround => collision.HasFlag(E_Collision.Ground);

        public bool DebugHorizontalFirstCollision => collision.HasFlag(E_Collision.HorizontalFirstCollision);

        public bool DebugHorizontalSecondCollision => collision.HasFlag(E_Collision.HorizontalSecondCollision);

        public bool DebugVerticalCollision => collision.HasFlag(E_Collision.VerticalCollision);

        public bool DebugJumpCollision => collision.HasFlag(E_Collision.JumpCollision);

        public bool DebugStandUpCollision => collision.HasFlag(E_Collision.StandUpCollision);
    }

    /// <summary>
    /// Feet altitude - ground altitude
    /// </summary>
    float feetGroundDelta => playerTransform.position.y - data.groundYAxis;
    /// <summary>
    /// Is player currently grounded ?
    /// </summary>
    public bool IsGrounded => Mathf.Abs(feetGroundDelta) < data.stepEpsilon;
    /// <summary>
    /// Is player close to ground ?
    /// </summary>
    public bool IsCloseToGround => Mathf.Abs(feetGroundDelta) <= data.maxStepHeight + data.stepEpsilon + .1f;
    /// <summary>
    /// Whether or not the player is below the ground.
    /// </summary>
    public bool IsBelowGround => feetGroundDelta < data.groundYAxis;
    /// <summary>
    /// Whether or not the player is above the ground.
    /// </summary>
    public bool IsAboveGround => feetGroundDelta > data.groundYAxis;

    /// <summary>
    /// Get the ground altitude.<br/>
    /// Ground can be navemash or obstacle.
    /// </summary>
    public void ComputeGround()
    {
        var isColliding = colliderDelegate.WillCollide(
            Vector3.down,
            out RaycastHit hit,
            data.maxAltitudeToCheckGround,
            data.navmeshLayer,
            collisionDebugger().DebugGround
        );

        if (isColliding)
        {
            data.previousGroundYAxis = data.groundYAxis;
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
        // Check if the horizontal translation (without the vertical translation) end up above nav mesh).
        bool willEndUpAboveNavMesh = WillTranslationEndUpAboveNavMesh(
            new()
            {
                x = desiredTranslation.x,
                y = 0f,
                z = desiredTranslation.z
            }
        );
        // No movement allowed that can ends up in the vacuum.
        desiredTranslation = willEndUpAboveNavMesh 
            ? desiredTranslation
            : Vector3.up * desiredTranslation.y;

        desiredTranslation = GetPossibleVerticalTranslation(desiredTranslation);
        willEndUpAboveNavMesh = WillTranslationEndUpAboveNavMesh(desiredTranslation);
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
           collisionDebugger().DebugHorizontalFirstCollision
       );
        if (!willCollide)
        {
            return desiredTranslation;
        }
        if (collisionDebugger().DebugHorizontalFirstCollision)
        {
            UnityEngine.Debug.Log($"Horizontal first collision: {hit.transform.name}, hitPoint= {hit.point}, horizontal translation= {horizontalDesiredTranslation}");
        }

        Vector3 projection = Vector3.ProjectOnPlane(
            horizontalDesiredTranslation,
            Vector3.ProjectOnPlane(hit.normal, Vector3.up)
        );

        willCollide = colliderDelegate.WillCollide(
           projection,
           out hit,
           projection.magnitude + .1f,
           data.obstacleLayer,
           collisionDebugger().DebugHorizontalSecondCollision
       );
        if (!willCollide)
        {
            return new()
            {
                x = projection.x,
                y = desiredTranslation.y,
                z = projection.z,
            };
        }
        if (collisionDebugger().DebugHorizontalSecondCollision)
        {
            UnityEngine.Debug.Log($"Horizontal second collision: {hit.transform.name}, hitPoint= {hit.point}, projection= {projection}");
        }
        return Vector3.up * desiredTranslation.y;
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
            collisionDebugger().DebugVerticalCollision
        );
        if (!willCollide)
        {
            return desiredTranslation;
        }
        //float delta = GetVerticalTranslationToGround(hit.point.y + data.stepEpsilon);
        Debug.Log("To discuss with Romain why previous was added");
        float delta = hit.point.y - (playerTransform.position.y + data.topSphereCenter.y + data.capsuleRadius);

        if (collisionDebugger().DebugVerticalCollision)
        {
            UnityEngine.Debug.Log($"Vertical collision: {hit.transform.name}, player = {playerTransform.position.y}, hitPoint= {hit.point.y}, delta= {delta}");
        }
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
            data.maxAltitudeToCheckGround,
            data.navmeshLayer,
            false
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
        if (!IsGrounded)
        {
            return false;
        }

        bool willCollide = colliderDelegate.WillCollide(
            Vector3.up,
            out var hit,
            data.maxJumpAltitude,
            data.obstacleLayer,
            collisionDebugger().DebugJumpCollision
        );
        if (willCollide && collisionDebugger().DebugJumpCollision)
        {
            UnityEngine.Debug.Log($"Jump collision: {hit.transform.name}, hitPoint= {hit.point}");
        }

        return !willCollide;
    }

    /// <summary>
    /// Check if the player can stand up.
    /// </summary>
    /// <returns></returns>
    public bool CanStandUp()
    {
        // If the player does not want to stand.
        if (data.WantToCrouch)
        {
            return false;
        }
        // If the player is not crouching.
        if (!data.IsCrouching)
        {
            return true;
        }

        // The player want to stand up and is crouching.
        bool willCollide = colliderDelegate.WillCollide(
            Vector3.zero,
            out RaycastHit hit,
            0f, // TODO: change this value
            data.obstacleLayer,
            collisionDebugger().DebugStandUpCollision
        );
        if (willCollide && collisionDebugger().DebugStandUpCollision)
        {
            UnityEngine.Debug.Log($"Stand up collision: {hit.transform.name}, hitPoint= {hit.point}");
        }

        return !willCollide;
    }
}
