using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.baseBrowser.cursor;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.baseBrowser.Navigation;
using umi3d.common;
using UnityEngine;

public sealed class UMI3DMovementManager
{
    #region Dependencies

    public Transform playerTransform;
    public Transform skeleton;
    public UMI3DCollisionManager collisionManager;
    public UMI3DCameraManager cameraManager;
    public BaseFPSData data;
    public IConcreteFPSNavigation concreteFPSNavigation;

    #endregion

    public event Action<Vector3> playerWillMoveDelegate;
    public event Action<Vector3> playerMovedDelegate;

    public void ComputeMovement()
    {
        data.playerMovement = Vector3.zero;
        if(
                (BaseCursor.Movement == BaseCursor.CursorMovement.Free
                || BaseCursor.Movement == BaseCursor.CursorMovement.FreeHidden)
            )
        {
            ComputeVerticalMovement();
            data.playerMovement *= Time.deltaTime;
            data.playerMovement = collisionManager.GetPossibleDirection(data.playerMovement);
            playerTransform.position += data.playerMovement;
            return;
        }

        cameraManager.HandleView();
        ComputeHorizontalAndVerticalMovement();

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

    void ComputeHorizontalAndVerticalMovement()
    {
        ComputeHorizontalMovement();
        ComputeVerticalMovement();
        // Get a world desire direction.
        data.playerMovement *= Time.deltaTime;

        // Get a desire direction relative to the player rotation.
        data.playerMovement = playerTransform.rotation * data.playerMovement;

        // Get a direction relative to the player that is possible (avoid collision).
        // TODO: collisionManager.GetPossibleDirection is wrong.
        data.playerMovement = collisionManager.GetPossibleDirection(data.playerMovement);
        if (data.playerMovement != Vector3.zero)
            UnityEngine.Debug.Log($"player movement = {data.playerMovement}");

        playerWillMoveDelegate?.Invoke(data.playerMovement);

        playerTransform.position += data.playerMovement;

        // Update the skeleton position to reflect the squatting or stand up position.
        //skeleton.localPosition = new Vector3
        //(
        //    0,
        //    Mathf.Lerp
        //    (
        //        skeleton.localPosition.y,
        //        (data.IsCrouching) ? data.crouchYAxis : 0f,
        //        data.crouchSpeed == 0 ? 1000000 : Time.deltaTime / data.crouchSpeed
        //    ),
        //    0
        //);

        skeleton.localPosition = new Vector3(0f, (data.IsCrouching) ? data.crouchYAxis : 0f, 0f);

        playerMovedDelegate?.Invoke(data.playerMovement);
    }

    void Walk()
    {
        if (data.playerMovement == Vector3.zero)
        {
            return;
        }

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

        data.playerMovement.z *= (data.playerMovement.z > 0) ? forwardSpeed() : backwardSpeed();
        data.playerMovement.x *= lateralSpeed();
    }

    void Teleport()
    {
        UnityEngine.Debug.LogError($"Not implemented.");
    }

    void Fly()
    {
        data.playerMovement.z *= data.flyingSpeed;
        data.playerMovement.x *= data.flyingSpeed;
    }

    void ComputeHorizontalMovement()
    {
        if (data.continuousDestination.HasValue)
        {
            var delta = data.continuousDestination.Value - playerTransform.position;
            data.playerMovement = delta.normalized;
        }
        else
        {
            concreteFPSNavigation.HandleUserInput();
        }

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

    void ComputeVerticalMovement()
    {
        if (data.continuousDestination.HasValue)
        {
            return;
        }

        if (data.navigationMode == E_NavigationMode.Debug)
        {
            data.playerMovement.y = data.flyingSpeed * ((data.WantToCrouch ? -1 : 0) + (data.WantToJump ? 1 : 0));

            return;
        }

        data.IsCrouching = collisionManager.ShouldSquat;

        data.playerVerticalVelocity = 0f;
        if (collisionManager.CanJump())
        {
            data.playerVerticalVelocity += data.maxJumpVelocity;
        }
        data.playerVerticalVelocity += data.gravity * Time.deltaTime;

        data.playerMovement.y = data.playerVerticalVelocity * Time.deltaTime;
    }
}
