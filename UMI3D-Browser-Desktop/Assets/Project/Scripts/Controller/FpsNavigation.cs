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
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.common;
using UnityEngine;
using UnityEngine.Events;

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

    [SerializeField]
    private Transform skeleton;

    [SerializeField]
    [Tooltip("List of point which from rays will be created to check is there is a navmesh under player's feet")]
    private List<Transform> feetRaycastOrigin;

    [SerializeField]
    [Tooltip("List of point which from rays will be created to check is there is an obstacle in front of the player")]
    private List<Transform> obstacleRaycastOrigins;

    [Header("Parameters")]
    [SerializeField]
    private FpsScriptableAsset data;

    [SerializeField]
    private float maxNeckAngle;

    [SerializeField]
    private float maxStepHeight = .2f;

    [SerializeField]
    private float maxSlopeAngle = 45f;

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
    public bool IsGrounded { get => Mathf.Abs(transform.position.y - groundHeight) < maxStepHeight; }

    /// <summary>
    /// Current ground height.
    /// </summary>
    private float groundHeight = 0;

    /// <summary>
    /// Is player active ?
    /// </summary>
    bool isActive = false;

    Vector3 destination;

    public static UnityEvent PlayerMoved = new UnityEvent();
    public enum State { Default, FreeHead, FreeMousse }
    public enum Navigation { Walking, Flying }

    public State state;

    bool changeToDefault = false;

    Vector3 lastAngleView;

    /// <summary>
    /// Is navigation currently performed ?
    /// </summary>
    bool navigateTo;

    Vector3 navigationDestination;

    /// <summary>
    /// Last frame player position .
    /// </summary>
    Vector3 lastPosition;

    #endregion

    #region Computed parameters

    private float maxJumpVelocity;

    /// <summary>
    /// Stores the last player positions delta of the last frames.
    /// </summary>
    FixedQueue<float> velocities = new FixedQueue<float>(3);

    /// <summary>
    /// Stores all data about player jumps.
    /// </summary>
    JumpData jumpData;

    #endregion

    #endregion

    #region Methods

    #region Abstract Navigation

    public override void Activate()
    {
        isActive = true;
        state = State.Default;
        jumpData = new JumpData();

        UpdateBaseHeight();

        maxJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(data.gravity) * data.MaxJumpHeight);
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
        navigateTo = true;
        navigationDestination = data.position;
    }

    public override void Teleport(TeleportDto data)
    {
        transform.position = data.position;
        transform.rotation = data.rotation;

        UpdateBaseHeight();
    }

    #endregion

    /// <summary>
    /// Applies gravity to player and makes it jump.
    /// </summary>
    /// <param name="jumping"></param>
    /// <param name="height"></param>
    void ComputeGravity(bool jumping, ref float height)
    {
        if (jumpData.jumping != jumping)
        {
            jumpData.jumping = jumping;

            if (jumpData.jumping && IsGrounded)
            {
                jumpData.velocity = maxJumpVelocity *Mathf.Clamp((1 + ComputeVelocity() * 6), 1, 1.5f);
                jumpData.lastTimeJumped = Time.time;
            }
        }

        jumpData.velocity += data.gravity * Time.deltaTime;
        height += jumpData.velocity * Time.deltaTime;

        if (height < groundHeight)
        {
            float offset = Mathf.Abs(height - groundHeight);
            if ((offset < maxStepHeight + stepEpsilon) && (offset > stepEpsilon))
            {
                height = Mathf.Lerp(height, groundHeight, .5f);
            } else
            {
                jumpData.velocity = 0;
                height = groundHeight;
            }
        }
    }

    private void Update()
    {
        if (!isActive)
            return;

        float height = transform.position.y;

        if ((CursorHandler.Movement == CursorHandler.CursorMovement.Free || CursorHandler.Movement == CursorHandler.CursorMovement.FreeHidden))
        {
            if (navigation != Navigation.Flying)
            {
                ComputeGravity(false, ref height);
                transform.Translate(0, height - transform.position.y, 0);
            }

            return;
        }

        if (TextInputDisplayerElement.isTyping)
            return;

        if (state == State.Default && Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.FreeView))) { state = State.FreeHead; }
        else if (state == State.FreeHead && !Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.FreeView))) { state = State.Default; changeToDefault = true; }

        HandleMovement();

        HandleView();
    }

    #region Movement

    /// <summary>
    /// Moves player.
    /// </summary>
    private void HandleMovement()
    {
        float height = transform.position.y;

        Vector2 move = Vector2.zero;

        if (navigateTo)
        {
            var delta = navigationDestination - transform.position;
            move = delta.normalized;

            if (Vector3.Distance(transform.position, navigationDestination) < .5f)
                navigateTo = false;
        }
        else
        {
            if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Forward))) { move.x += 1; }
            if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Backward))) { move.x -= 1; }
            if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Right))) { move.y += 1; }
            if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Left))) { move.y -= 1; }
            if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Forward))
                || Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Backward))
                || Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Right))
                || Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Left)))
            {
                PlayerMoved.Invoke();
            }
        }

        switch (navigation)
        {
            case Navigation.Walking:
                Walk(ref move, ref height);
                break;
            case Navigation.Flying:
                Fly(ref move, ref height);
                break;
        }

        move *= Time.deltaTime;

        Vector3 pos = transform.rotation * new Vector3(move.y, 0, move.x);

        if (CanMove(pos))
        {
            pos += transform.position;
        }
        else
        {
            pos = transform.position;
        }

        pos.y = height;

        transform.position = pos;

        velocities.Push((pos - lastPosition).magnitude);
        lastPosition = transform.localPosition;
    }

    /// <summary>
    /// Computes <paramref name="move"/> vector to perform a walk movement and applies gravitu.
    /// </summary>
    /// <param name="move"></param>
    private void Walk(ref Vector2 move, ref float height)
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

        bool squatting = Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Squat));
        skeleton.transform.localPosition = new Vector3(0, Mathf.Lerp(skeleton.transform.localPosition.y, (squatting) ? data.squatHeight : data.standHeight, data.squatSpeed == 0 ? 1000000 : Time.deltaTime / data.squatSpeed), 0);
        viewpoint.transform.localPosition = skeleton.transform.localPosition;

        ComputeGravity(Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Jump)), ref height);
    }

    private void Fly(ref Vector2 Move, ref float height)
    {
        Move.x *= data.flyingSpeed;
        Move.y *= data.flyingSpeed;
        height += data.flyingSpeed * 0.01f * ((Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Squat)) ? -1 : 0) + (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Jump)) ? 1 : 0));
    }

    /// <summary>
    /// Computes player velocity.
    /// </summary>
    /// <returns></returns>
    private float ComputeVelocity()
    {
        float sum = 0;
        foreach (var vel in velocities.data)
        {
            sum += vel;
        }

        return sum / velocities.data.Count;
    }

    #endregion

    /// <summary>
    /// Rotates camera.
    /// </summary>
    private void HandleView()
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

    private Vector3 NormalizeAngle(Vector3 angle)
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
        return CheckNavmesh(direction) && CheckCollision(direction) || navigation == Navigation.Flying;
    }

    /// <summary>
    /// Checks if a translation along <paramref name="direction"/> would move the player on a navmesh surface.
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private bool CheckNavmesh(Vector3 direction)
    {
        RaycastHit hit;

        RaycastHit foundHit = new RaycastHit { distance = Mathf.Infinity };

        direction = direction / 2f;

        foreach (Transform foot in feetRaycastOrigin)
        {
            if (UnityEngine.Physics.Raycast(foot.position + Vector3.up * (.05f + maxStepHeight) + direction, Vector3.down, out hit, 100, navmeshLayer)) {
                if ((foundHit.distance > hit.distance) && (Vector3.Angle(transform.up, hit.normal) <= maxSlopeAngle))
                    foundHit = hit;
            }
        }

        if (foundHit.distance < Mathf.Infinity)
        {
            groundHeight = foundHit.point.y;
            return true;
        }

        return false;
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

        foreach (var t in obstacleRaycastOrigins)
        {
            if (UnityEngine.Physics.Raycast(t.position, direction.normalized, .2f, obstacleLayer))
            {
                return false;
            }
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

    /// <summary>
    /// Checks under player's feet to update <see cref="groundHeight"/>.
    /// </summary>
    private void UpdateBaseHeight()
    {
        RaycastHit hit;

        RaycastHit foundHit = new RaycastHit { distance = Mathf.Infinity };


        foreach (Transform foot in feetRaycastOrigin)
        {
            if (UnityEngine.Physics.Raycast(foot.position + Vector3.up * (.05f + maxStepHeight), Vector3.down, out hit, 100, navmeshLayer))
            {

                if ((foundHit.distance > hit.distance))
                    foundHit = hit;
            }
        }

        if ((foundHit.distance < Mathf.Infinity) && (Vector3.Angle(transform.up, foundHit.normal) <= maxSlopeAngle))
        {
            groundHeight = foundHit.point.y;
        }
    }

    #endregion

    #endregion

    #region Struct Definition 

    struct JumpData
    {
        public bool jumping;
        public float velocity;
        public float lastTimeJumped;

        public JumpData(bool jumping, float lastTimeJumped, float velocity, float deltaHeight) : this()
        {
            this.jumping = jumping;
            this.velocity = velocity;
            this.lastTimeJumped = lastTimeJumped;
        }
    }


    class FixedQueue<T>
    {
        public List<T> data;

        public int capacity;

        public FixedQueue(int capacity)
        {
            Debug.Assert(capacity >= 1);
            this.capacity = capacity;

            data = new List<T>();
        }

        public void Push(T elt)
        {
            if (data.Count >= capacity)
                data.RemoveAt(0);

            data.Add(elt);
        }
    }

    #endregion
}
