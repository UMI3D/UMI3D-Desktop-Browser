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
using umi3d.baseBrowser.cursor;
using umi3d.baseBrowser.Navigation;
using UnityEngine;

public sealed class UMI3DMovementManager
{
    #region Dependencies

    public Transform playerTransform;
    public Transform skeleton;
    public UMI3DCollisionManager collisionManager;
    public BaseFPSData data;
    public IConcreteFPSNavigation concreteFPSNavigation;

    #endregion

    public event Action<Vector3> playerWillMoveDelegate;
    public event Action<Vector3> playerMovedDelegate;

    public void ComputeMovement()
    {
        data.playerTranslationSpeed = Vector3.zero;
        data.playerTranslation = Vector3.zero;
        collisionManager.ComputeGround();
        if (
                (BaseCursor.Movement == BaseCursor.CursorMovement.Free
                || BaseCursor.Movement == BaseCursor.CursorMovement.FreeHidden)
            )
        {
            ComputeVerticalSpeed();
            ComputeHorizontalAndVerticalTranslation();
            UpdatePlayerPosition();
            return;
        }

        ComputeHorizontalSpeed();
        ComputeVerticalSpeed();
        ComputeHorizontalAndVerticalTranslation();
        UpdatePlayerPosition();

        Func<bool> isNearDestination 
            = () => Vector3.Distance(
                playerTransform.position,
                data.continuousDestination.Value
            ) < .5f;
        if (data.continuousDestination.HasValue && isNearDestination())
        {
            data.continuousDestination = null;
        }
    }

    /// <summary>
    /// Update the skeleton position to reflect the crouching or standing up position.
    /// </summary>
    void UpdateSkeletonHeight()
    {
        skeleton.localPosition = new Vector3
        (
            0,
            Mathf.Lerp
            (
                skeleton.localPosition.y,
                (data.IsCrouching) ? data.crouchYAxis : 0f,
                data.crouchSpeed == 0 ? 1000000 : Time.deltaTime / data.crouchSpeed
            ),
            0
        );
    }

    void Walk()
    {
        Func<float> forwardSpeed = () =>
        {
            if (data.IsCrouching)
            {
                return data.forwardSpeed.y;
            }
            else if (data.IsSprinting)
            {
                return data.forwardSpeed.z;
            }
            else
            {
                return data.forwardSpeed.x;
            }
        };

        Func<float> backwardSpeed = () =>
        {
            if (data.IsCrouching)
            {
                return data.backwardSpeed.y;
            }
            else if (data.IsSprinting)
            {
                return data.backwardSpeed.z;
            }
            else
            {
                return data.backwardSpeed.x;
            }
        };

        Func<float> lateralSpeed = () =>
        {
            if (data.IsCrouching)
            {
                return data.lateralSpeed.y;
            }
            else if (data.IsSprinting)
            {
                return data.lateralSpeed.z;
            }
            else
            {
                return data.lateralSpeed.x;
            }
        };

        data.playerTranslationSpeed.z *= (data.playerTranslationSpeed.z > 0) ? forwardSpeed() : backwardSpeed();
        data.playerTranslationSpeed.x *= lateralSpeed();
    }

    void Teleport()
    {
        UnityEngine.Debug.LogError($"Not implemented.");
    }

    void Fly()
    {
        data.playerTranslationSpeed.z *= data.flyingSpeed;
        data.playerTranslationSpeed.x *= data.flyingSpeed;
    }

    void ComputeHorizontalSpeed()
    {
        if (data.continuousDestination.HasValue)
        {
            var delta = data.continuousDestination.Value - playerTransform.position;
            data.playerTranslationSpeed = delta.normalized; // TODO: add speed here.
        }
        else
        {
            concreteFPSNavigation.HandleUserInput();
        }

        if (data.playerTranslationSpeed == Vector3.zero)
        {
            return;
        }

        data.IsSprinting = data.WantToSprint;

        switch (data.navigationMode)
        {
            case E_NavigationMode.Default:
                Walk();
                break;
            case E_NavigationMode.Continuous:
                Walk();
                break;
            case E_NavigationMode.Teleportation:
                Teleport();
                break;
            case E_NavigationMode.Debug:
                Fly();
                break;
            default:
                break;
        }
    }

    void ComputeVerticalSpeed()
    {
        if (data.continuousDestination.HasValue)
        {
            return;
        }

        if (data.navigationMode == E_NavigationMode.Debug)
        {
            data.playerTranslationSpeed.y = data.flyingSpeed * ((data.WantToCrouch ? -1 : 0) + (data.WantToJump ? 1 : 0));
            return;
        }

        data.IsCrouching = data.WantToCrouch || !collisionManager.CanStandUp();
        if (collisionManager.IsGrounded)
        {
            if (data.WantToJump && collisionManager.CanJump())
            {
                data.IsJumping = true;
                data.verticalVelocity = data.MaxJumpVelocity;
            }
            else
            {
                data.IsJumping = false;
                data.verticalVelocity = 0f;
            }
        }
        else
        {
            if (collisionManager.IsAboveGround)
            {
                data.verticalVelocity += data.GravityVelocity;
            }
        }

        data.playerTranslationSpeed.y = data.verticalVelocity;
    }

    void ComputeHorizontalAndVerticalTranslation()
    {
        // Get a world desire direction and distance.
        data.playerTranslation = data.playerTranslationSpeed * Time.deltaTime;

        // Don't add too much gravity
        if ((data.playerTranslation.y != 0) && playerTransform.transform.position.y + data.playerTranslation.y < data.groundYAxis)
            data.playerTranslation.y = data.groundYAxis - playerTransform.transform.position.y;

        // Get a desire direction and distance relative to the player rotation.
        data.playerTranslation = playerTransform.rotation * data.playerTranslation;

        // Get a direction and distance relative to the player that is possible (avoid collision).
        data.playerTranslation = collisionManager.GetPossibleTranslation(data.playerTranslation);
        if (data.playerTranslation.y == 0 && collisionManager.IsBelowGround)
        {
            float delta = playerTransform.position.y - data.groundYAxis;
            data.playerTranslation.y = Mathf.Lerp(0, -delta, 0.4f);
        }
    }

    void UpdatePlayerPosition()
    {
        playerWillMoveDelegate?.Invoke(data.playerTranslation);
        playerTransform.position += data.playerTranslation;
        UpdateSkeletonHeight();
        playerMovedDelegate?.Invoke(data.playerTranslation);
    }
}
