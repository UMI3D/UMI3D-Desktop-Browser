using System;
using System.Collections;
using System.Collections.Generic;
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
    public BaseFPSData data;
    public IConcreteFPSNavigation concreteFPSNavigation;

    #endregion

    public event Action<Vector3> playerWillMoveDelegate;
    public event Action<Vector3> playerMovedDelegate;

    public void ComputeMovement()
    {
        if (data.continuousDestination.HasValue)
        {
            var delta = data.continuousDestination.Value - playerTransform.position;
            data.Movement = delta.normalized;
        }
        else
        {
            data.Movement = concreteFPSNavigation.HandleUserInput();
        }

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
        playerWillMoveDelegate?.Invoke(data.Movement);
        
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

        // Update the skeleton position to reflect the squatting or stand up position.
        skeleton.transform.localPosition = new Vector3
        (
            0,
            Mathf.Lerp
            (
                skeleton.transform.localPosition.y,
                (data.IsCrouching) ? data.squatHeight : data.standHeight,
                data.squatSpeed == 0 ? 1000000 : Time.deltaTime / data.squatSpeed
            ),
            0
        );

        ComputeVerticalMovement();

        playerMovedDelegate?.Invoke(data.Movement);
    }

    void Walk()
    {
        if (data.Movement == Vector3.zero)
        {
            return;
        }

        data.IsCrouching = collisionManager.ShouldSquat;

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

        data.Movement.z *= (data.Movement.z > 0) ? forwardSpeed() : backwardSpeed();
        data.Movement.x *= lateralSpeed();

        // Get a world desire direction.
        data.Movement *= Time.deltaTime;

        // Get a desire direction relative to the player rotation.
        data.Movement = playerTransform.rotation * new Vector3(
            data.Movement.x,
            data.Movement.y,
            data.Movement.z
        );

        // Get a direction relative to the player that is possible (avoid collision).
        data.Movement = collisionManager.GetPossibleDirection(data.Movement);

        playerTransform.position += data.Movement;
    }

    void Teleport()
    {
        UnityEngine.Debug.LogError($"Not implemented.");
    }

    void Fly()
    {
        data.Movement.z *= data.flyingSpeed;
        data.Movement.x *= data.flyingSpeed;
        //height += data.flyingSpeed * ((data.IsCrouching ? -1 : 0) + (data.WantToJump ? 1 : 0));
    }

    void ComputeVerticalMovement()
    {
        //var height = playerTransform.position.y;

        if (data.WantToJump && collisionManager.CanJump())
        {
            data.velocity = data.maxJumpVelocity;
        }
        data.velocity += data.gravity * Time.deltaTime;

        data.Movement.y = data.velocity * Time.deltaTime;




        playerTransform.Translate(0, height - playerTransform.position.y, 0);
    }




    /// <summary>
    /// Applies gravity to player and makes it jump.
    /// </summary>
    /// <param name="jumping"></param>
    /// <param name="height"></param>
    public void ComputeGravity(bool jumping, ref float height)
    {
        
        if (height >= data.groundYAxis)
        {
            heightDelta = (height - lastHeight) * Time.deltaTime;
            lastHeight = height;
            return;
        }

        float offset = Mathf.Abs(height - groundHeight);
        if
        (
            offset < data.maxStepHeight + stepEpsilon
            && offset > data.stepEpsilon
            && hasGroundHeightChangedLastFrame
        ) height = Mathf.Lerp(height, groundHeight, .5f);
        else
        {
            data.velocity = 0;
            height = groundHeight;
        }


        heightDelta = (height - lastHeight) * Time.deltaTime;
        lastHeight = height;
    }
}
