/*
Copyright 2019 Gfi Informatique

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

using BrowserDesktop.Controller;
using BrowserDesktop.Cursor;
using BrowserDesktop.Menu;
using System.Collections;
using umi3d.cdk;
using umi3d.common;
using UnityEngine;

public class FpsNavigation : AbstractNavigation
{
    #region Fields

    [Header("Player parts")]

    [SerializeField]
    private Transform viewpoint;

    [SerializeField]
    private Transform neckPivot;

    [SerializeField]
    private Transform head;

    [Header("Parameters")]
    [SerializeField]
    private FpsScriptableAsset data;

    [SerializeField]
    private float maxNeckAngle;

    [SerializeField]
    private float maxStepHeight = .2f;

    private float stepEpsilon = 0.05f;

    [SerializeField]
    [Tooltip("Navigation mode")]
    private Navigation navigation;

    [Header("Navmesh")]
    [SerializeField]
    public LayerMask obstacleLayer;

    [SerializeField]
    public LayerMask navmeshLayer;

    #region Player state

    /// <summary>
    /// Is player currently grounded ?
    /// </summary>
    public bool IsGrounded { get => Mathf.Abs(transform.position.y - groundHeight) < 0.01f; }

    /// <summary>
    /// Current ground height.
    /// </summary>
    private float groundHeight = 0;

    /// <summary>
    /// Is player active ?
    /// </summary>
    private bool isActive = false;

    JumpData jumpData;

    public State state;

    bool changeToDefault = false;

    Vector3 lastAngleView;

    #endregion

    #region Computed parameters

    private float maxJumpVelocity;

    private float minJumpVelocity;

    #endregion

    #endregion

    #region Methods

    #region Abstract Navigation

    public override void Activate()
    {
        isActive = true;
        state = State.Default;
        jumpData = new JumpData();
        Debug.LogError("IMPLEMENT COUNCH");
        Debug.LogError("IMPLEMENT START");

        maxJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(data.gravity) * data.MaxJumpHeight);
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(data.gravity) * data.MinJumpHeight);
    }

    public override void Disable()
    {
        isActive = false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="data"></param>
    public override void Navigate(NavigateDto data)
    {
        throw new System.NotImplementedException("NAVIGATE TO not implemented yet.");
        //navigateTo = true;
        //destination = data.position;
    }

    public override void Teleport(TeleportDto data)
    {
        transform.position = data.position;
        transform.rotation = data.rotation;

        UpdateBaseHeight();
    }

    #endregion

    void ApplyGravity(bool jumping, ref float height)
    {
        if (jumpData.jumping != jumping)
        {
            jumpData.jumping = jumping;

            if (jumpData.jumping && IsGrounded)
            {
                jumpData.velocity = maxJumpVelocity;
            }
        }

        jumpData.velocity += data.gravity * Time.deltaTime;
        height += jumpData.velocity * Time.deltaTime;

        if (height < groundHeight)
        {
            if (Mathf.Abs(height - groundHeight) < maxStepHeight + stepEpsilon)
            {
                height = Mathf.Lerp(height, groundHeight, .5f);
            } else
            {
                height = groundHeight;
            }

            jumpData.velocity = 0;
        }
    }

    private void Update()
    {
        if (!isActive)
            return;

        /*if (CursorHandler.Movement == CursorHandler.CursorMovement.Free || CursorHandler.Movement == CursorHandler.CursorMovement.FreeHidden)
        {
            Vector3 position = Node.transform.position;

            ComputeJump(false);

            height += jumpData.deltaHeight;
            position.y = height;
            if (position.y < groundHeight)
                position.y = groundHeight;

            SkeletonContainer.transform.position = position;
            return;
        }*/

        if (TextInputDisplayerElement.isTyping)
            return;

        if (state == State.Default && Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.FreeView))) { state = State.FreeHead; }
        else if (state == State.FreeHead && !Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.FreeView))) { state = State.Default; changeToDefault = true; }
        Vector2 Move = Vector2.zero;

        /*if (navigateTo)
        {
            var delta = destination - Node.transform.position;
            Move = delta.normalized;
        }
        else
        {*/
            if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Forward))) { Move.x += 1; }
            if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Backward))) { Move.x -= 1; }
            if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Right))) { Move.y += 1; }
            if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Left))) { Move.y -= 1; }
        //}

        float height = transform.position.y;

        switch (navigation)
        {
            case Navigation.Walking:
                Walk(ref Move, ref height);
                break;
            case Navigation.Flying:
                Fly(ref Move, ref height);
                break;
        }

        Move *= Time.deltaTime;

        HandleView();

        Vector3 pos = transform.rotation * new Vector3(Move.y, 0, Move.x);

        if (CanMove(pos))
        {
            pos += transform.position;
        } else
        {
            pos = transform.position;
        }

        pos.y = height;

        transform.position = pos;
    }

    /// <summary>
    /// Computes <paramref name="move"/> vector to perform a walk movement and applies gravitu.
    /// </summary>
    /// <param name="move"></param>
    void Walk(ref Vector2 move, ref float height)
    {
        if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Squat)))
        {
            move.x *= (move.x > 0) ? data.forwardSpeed.y : data.backwardSpeed.y;
            move.y *= data.lateralSpeed.y;
        }
        else if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Sprint)))
        {
            move.x *= (move.x > 0) ? data.forwardSpeed.z : data.backwardSpeed.z;
            move.y *= data.lateralSpeed.z;
        }
        else
        {
            move.x *= (move.x > 0) ? data.forwardSpeed.x : data.backwardSpeed.x;
            move.y *= data.lateralSpeed.x;
        }
        //bool Squatting = Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Squat));
        //height = Mathf.Lerp(height, (Squatting) ? data.squatHeight : data.standHeight, data.squatSpeed == 0 ? 1000000 : Time.deltaTime / data.squatSpeed);

        ApplyGravity(Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Jump)), ref height);
    }

    void Fly(ref Vector2 Move, ref float height)
    {
        Move.x *= data.flyingSpeed;
        Move.y *= data.flyingSpeed;
        height += data.flyingSpeed * ((Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Squat)) ? -1 : 0) + (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Jump)) ? 1 : 0));
    }

    void HandleView()
    {
        if (state == State.FreeMousse) return;
        Vector3 angleView = NormalizeAngle(viewpoint.rotation.eulerAngles);

        Vector2 angularSpeed = new Vector2(-1 * Input.GetAxis("Mouse Y") * data.AngularViewSpeed.x, Input.GetAxis("Mouse X") * data.AngularViewSpeed.y);
        Vector3 result = NormalizeAngle(angleView + (Vector3)angularSpeed);
        if (changeToDefault)
        {
            result = lastAngleView;
            changeToDefault = false;
        }
        Vector3 displayResult;

        if (result.x < data.XAngleRange.x) result.x = data.XAngleRange.x;
        if (result.x > data.XAngleRange.y) result.x = data.XAngleRange.y;
        displayResult = result;
        if (displayResult.x < data.XDisplayAngleRange.x) displayResult.x = data.XDisplayAngleRange.x;
        if (displayResult.x > data.XDisplayAngleRange.y) displayResult.x = data.XDisplayAngleRange.y;

        if (state == State.Default)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, result.y, 0));
            lastAngleView = result;
        }
        else
        {
            Vector3 angleNeck = NormalizeAngle(transform.rotation.eulerAngles);
            float delta = Mathf.DeltaAngle(result.y, angleNeck.y);

            if (delta < data.YAngleRange.x) result.y = -data.YAngleRange.x + angleNeck.y;
            if (delta > data.YAngleRange.y) result.y = -data.YAngleRange.y + angleNeck.y;
        }
        viewpoint.transform.rotation = Quaternion.Euler(result);
        neckPivot.transform.rotation = Quaternion.Euler(new Vector3(Mathf.Clamp(result.x, -maxNeckAngle, maxNeckAngle), result.y, result.z));
        head.transform.rotation = Quaternion.Euler(displayResult);
    }

    Vector3 NormalizeAngle(Vector3 angle)
    {
        angle.x = Mathf.DeltaAngle(0, angle.x);
        angle.y = Mathf.DeltaAngle(0, angle.y);
        angle.z = Mathf.DeltaAngle(0, angle.z);
        return angle;
    }

    #region Check Navmesh and Obstacles

    private float lastObstacleHeight = .5f;

    private bool CanMove(Vector3 direction)
    {
        return CheckNavmesh(direction) && CheckCollision(direction);
    }

    /// <summary>
    /// Checks if a translation along <paramref name="direction"/> would move the player on a navmesh surface.
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private bool CheckNavmesh(Vector3 direction)
    {
        RaycastHit hit;

        direction = direction / 2f;

        Vector3 origin = transform.position + Vector3.up * maxStepHeight + direction;

        if (UnityEngine.Physics.Raycast(origin + Vector3.up * (.05f + maxStepHeight), Vector3.down, out hit, 100, navmeshLayer))
        {
            groundHeight = hit.point.y;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if there is an obstacle for the player.
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private bool CheckCollision(Vector3 direction)
    {
        if (UnityEngine.Physics.Raycast(transform.position + Vector3.up * 1.75f, direction.normalized, .2f, obstacleLayer))
        {
            return false;
        }
        if (UnityEngine.Physics.Raycast(transform.position + Vector3.up * (maxStepHeight + stepEpsilon), direction.normalized, .2f, obstacleLayer))
        {
            return false;
        }

        if (UnityEngine.Physics.Raycast(transform.position + Vector3.up * lastObstacleHeight, direction.normalized, .2f, obstacleLayer))
        {
            return false;
        }

        for (int i = 0; i < 3; i++)
        {
            float random = Random.Range(maxStepHeight + stepEpsilon, 1.8f);

            if (UnityEngine.Physics.Raycast(transform.position + Vector3.up * random, direction.normalized, .2f, obstacleLayer))
            {
                lastObstacleHeight = random;
                return false;
            }
        }

        lastObstacleHeight = .5f;

        return true;
    }

    private void UpdateBaseHeight()
    {

    }

    #endregion

    #endregion

    #region Struct Definition 

    public enum State { Default, FreeHead, FreeMousse }
    public enum Navigation { Walking, Flying }

    struct JumpData
    {
        public bool jumping;
        public float velocity;

        public JumpData(bool jumping, float timeSinceJump, float velocity, float deltaHeight) : this()
        {
            this.jumping = jumping;
            this.velocity = velocity;
        }
    }

    #endregion
}
