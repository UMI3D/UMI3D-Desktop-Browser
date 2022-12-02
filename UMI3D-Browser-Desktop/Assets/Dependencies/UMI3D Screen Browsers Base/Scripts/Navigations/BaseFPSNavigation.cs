/*
Copyright 2019 - 2021 Inetum
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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace umi3d.baseBrowser.Navigation
{
    public abstract class BaseFPSNavigation : cdk.AbstractNavigation
    {
        #region Struct Definition 
        public enum State { Default, FreeHead, FreeMousse }
        public enum Navigation { Walking, Flying }
        protected struct JumpData
        {
            public bool IsJumping;
            public float velocity, lastTimeJumped;
            public JumpData(bool jumping, float lastTimeJumped, float velocity)
            {
                this.IsJumping = jumping;
                this.velocity = velocity;
                this.lastTimeJumped = lastTimeJumped;
            }
        }
        #endregion

        #region Fields
        public static BaseFPSNavigation Instance => s_instance;
        protected static BaseFPSNavigation s_instance;

        [HideInInspector]
        public bool IsJumping;
        [HideInInspector]
        public bool IsCrouching;
        [HideInInspector]
        public bool WantToCrouch;
        [HideInInspector]
        public Vector2 Movement;
        [Header("Player Body")]
        [SerializeField]
        protected Transform viewpoint;

        [SerializeField]
        protected Transform neckPivot;

        [SerializeField]
        protected Transform head;


        [SerializeField]
        protected Transform topHead;
        [SerializeField]
        protected Transform skeleton;

        [SerializeField]
        [Tooltip("Radius used from player center to raycast")]
        protected float playerRadius = .3f;
        [SerializeField]
        [Tooltip("List of point which from rays will be created to check is there is a navmesh under player's feet")]
        protected List<Transform> feetRaycastOrigin;

        [Header("Parameters")]
        [SerializeField]
        protected BaseFPSData data;

        [SerializeField]
        protected float maxNeckAngle;

        [SerializeField]
        protected float maxStepHeight = .2f;

        [SerializeField]
        protected float maxSlopeAngle = 45f;

        protected float stepEpsilon = 0.05f;

        [SerializeField]
        [Tooltip("Navigation mode")]
        protected Navigation navigation;

        [Header("Navmesh")]
        [SerializeField]
        public LayerMask obstacleLayer;

        [SerializeField]
        public LayerMask navmeshLayer;

        #region Player state

        /// <summary>
        /// Is player currently grounded ?
        /// </summary>
        public bool IsGrounded => Mathf.Abs(transform.position.y - groundHeight) < maxStepHeight;

        /// <summary>
        /// Current ground height.
        /// </summary>
        protected float groundHeight = 0;

        /// <summary>
        /// Has <see cref="groundHeight"/> changed last frame ?
        /// </summary>
        protected bool hasGroundHeightChangedLastFrame = false;

        /// <summary>
        /// Is player active ?
        /// </summary>
        protected bool isActive = false;
        protected bool vehicleFreeHead = false;

        protected Vector3 destination;


        public static UnityEvent PlayerMoved = new UnityEvent();

        public State state;

        protected bool changeToDefault = false;

        protected Vector3 lastAngleView;

        /// <summary>
        /// Is navigation currently performed ?
        /// </summary>
        protected bool navigateTo;

        protected Vector3 navigationDestination;

        protected float maxJumpVelocity;

        /// <summary>
        /// Stores all data about player jumps.
        /// </summary>
        protected JumpData jumpData;
        protected Vector3 currentCapsuleBase, currentCapsuleEnd;

        #endregion

#if UNITY_EDITOR

        private Vector3 collisionHitPoint;

#endif

        protected cdk.UMI3DNodeInstance globalVehicle;

        protected float lastObstacleHeight = .5f;

        #endregion

        #region Methods
        #region Abstract Navigation
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Activate()
        {
            isActive = true;
            state = State.Default;
            jumpData = new JumpData();
            UpdateBaseHeight();
            maxJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(data.gravity) * data.MaxJumpHeight);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Disable()
        {
            isActive = false;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="data"></param>
        public override void Navigate(common.NavigateDto data)
        {
            navigateTo = true;
            navigationDestination = data.position;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="data"></param>
        public override void Teleport(common.TeleportDto data)
        {
            transform.position = data.position;
            groundHeight = data.position.Y;
            transform.rotation = data.rotation;
            UpdateBaseHeight();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="data"></param>
        public override void Embark(common.VehicleDto data)
        {
            vehicleFreeHead = data.StopNavigation;

            if (data.VehicleId == 0)
            {
                this.transform.SetParent(cdk.UMI3DEnvironmentLoader.Instance.transform, true);
                this.transform.localPosition = data.position;
                this.transform.localRotation = data.rotation;
                DontDestroyOnLoad(cdk.UMI3DNavigation.Instance);
                globalVehicle.Delete -= new System.Action(() => {
                    cdk.UMI3DNavigation.Instance.transform.SetParent(cdk.UMI3DEnvironmentLoader.Instance.transform, true);
                    DontDestroyOnLoad(cdk.UMI3DNavigation.Instance);
                });
                globalVehicle = null;
            }
            else
            {
                cdk.UMI3DNodeInstance vehicle = cdk.UMI3DEnvironmentLoader.GetNode(data.VehicleId);
                if (vehicle != null)
                {
                    globalVehicle = vehicle;
                    this.transform.SetParent(vehicle.transform, true);
                    this.transform.localPosition = data.position;
                    this.transform.localRotation = data.rotation;
                    globalVehicle.Delete += new System.Action(() => {
                        cdk.UMI3DNavigation.Instance.transform.SetParent(cdk.UMI3DEnvironmentLoader.Instance.transform, true);
                        DontDestroyOnLoad(cdk.UMI3DNavigation.Instance);
                    });
                }
            }
        }

        #endregion

        void Start()
        {
            if (s_instance == null) s_instance = this;
            else Destroy(this.gameObject);
        }

        private void Update()
        {
            if (!isActive) return;

            OnUpdate();
        }

        /// <summary>
        /// return True if the override method can execute, false otherwithe.
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnUpdate()
        {
            if (vehicleFreeHead)
            {
                state = State.FreeHead;

                if
                (
                    !(Controller.BaseCursor.Movement == Controller.BaseCursor.CursorMovement.Free
                    || Controller.BaseCursor.Movement == Controller.BaseCursor.CursorMovement.FreeHidden)
                ) HandleView();

                return false;
            }

            //Mandatory for colision.
            (currentCapsuleBase, currentCapsuleEnd) = GetCapsuleSphereCenters();

            if
            (
                (Controller.BaseCursor.Movement == Controller.BaseCursor.CursorMovement.Free
                || Controller.BaseCursor.Movement == Controller.BaseCursor.CursorMovement.FreeHidden)
                && navigation != Navigation.Flying
            )
            {
                float height = transform.position.y;
                ComputeGravity(false, ref height);
                transform.Translate(0, height - transform.position.y, 0);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Applies gravity to player and makes it jump.
        /// </summary>
        /// <param name="jumping"></param>
        /// <param name="height"></param>
        protected void ComputeGravity(bool jumping, ref float height)
        {
            if (jumpData.IsJumping != jumping)
            {
                jumpData.IsJumping = jumping;

                if (jumpData.IsJumping && CanJump())
                {
                    jumpData.velocity = maxJumpVelocity;
                    jumpData.lastTimeJumped = Time.time;
                }
            }
            jumpData.velocity += data.gravity * Time.deltaTime;
            height += jumpData.velocity * Time.deltaTime;
            if (height < groundHeight)
            {
                float offset = Mathf.Abs(height - groundHeight);

                if ((offset < maxStepHeight + stepEpsilon) && (offset > stepEpsilon) && hasGroundHeightChangedLastFrame)
                    height = Mathf.Lerp(height, groundHeight, .5f);
                else
                {
                    jumpData.velocity = 0;
                    height = groundHeight;
                }
            }
        }
        /// <summary>
        /// Checks if player can jump.
        /// </summary>
        /// <returns></returns>
        protected bool CanJump()
        {
            if (!IsGrounded)
                return false;

            return !Physics.CapsuleCast(currentCapsuleBase, currentCapsuleEnd, playerRadius, transform.up, .5f, obstacleLayer);
        }

        /// <summary>
        /// Return a movement allowed for the player from a given <paramref name="direction"/>.
        /// If no movement is allowed, return Vector.zero.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected Vector3 ComputeMovement(Vector3 direction)
        {
            if (navigation == Navigation.Flying)
                return direction;

            if (CheckNavmesh(direction))
            {
                return CheckCollision(direction);
            }
            else
            {
                return Vector3.zero;
            }
        }
        /// <summary>
        /// Checks if a translation along <paramref name="direction"/> would move the player on a navmesh surface.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected bool CheckNavmesh(Vector3 direction)
        {
            RaycastHit hit;
            RaycastHit foundHit = new RaycastHit { distance = Mathf.Infinity };
            direction /= 2f;
            foreach (Transform foot in feetRaycastOrigin)
            {
                if (
                    UnityEngine.Physics.Raycast(foot.position + Vector3.up * maxStepHeight + direction, Vector3.down, out hit, 100, navmeshLayer)
                    && foundHit.distance > hit.distance
                    && Vector3.Angle(transform.up, hit.normal) <= maxSlopeAngle
                    )
                    foundHit = hit;
            }
            if (foundHit.distance < Mathf.Infinity)
            {
                float newHeight = foundHit.point.y;
                if (Mathf.Abs(newHeight - groundHeight) > .001f)
                {
                    groundHeight = newHeight;
                    hasGroundHeightChangedLastFrame = true;
                }
                else hasGroundHeightChangedLastFrame = false;
                return true;
            }
            hasGroundHeightChangedLastFrame = false;
            return false;
        }
        /// <summary>
        /// Checks if there is an obstacle for the player and returns an allowed movement for the player.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected Vector3 CheckCollision(Vector3 direction)
        {
            if (Physics.CapsuleCast(currentCapsuleBase, currentCapsuleEnd, playerRadius, direction, out var hit, IsGrounded ? direction.magnitude * 1.01f : 1f, obstacleLayer))
            {

#if UNITY_EDITOR
                collisionHitPoint = hit.point;
#endif

                Vector3 normal = Vector3.ProjectOnPlane(hit.normal, Vector3.up);
                Vector3 projectedDirection = Vector3.Project(direction, Quaternion.Euler(0, 90, 0) * normal);

                if (Physics.CapsuleCast(currentCapsuleBase, currentCapsuleEnd, playerRadius, projectedDirection, .2f, obstacleLayer))
                {
                    return Vector3.zero;
                }
                else
                {
                    return projectedDirection;
                }
            }

            return direction;
        }
        /// <summary>
        /// Checks under player's feet to update <see cref="groundHeight"/>.
        /// </summary>
        protected void UpdateBaseHeight()
        {
            RaycastHit foundHit = new RaycastHit { distance = Mathf.Infinity };
            RaycastHit[] hits = Physics.CapsuleCastAll(currentCapsuleBase, currentCapsuleEnd, playerRadius, Vector3.down, 100, navmeshLayer);
            foreach (RaycastHit hit in hits)
            {
                if ((foundHit.distance > hit.distance) && (Vector3.Angle(transform.up, hit.normal) <= maxSlopeAngle))
                {
                    foundHit = hit;
                }
            }
            if ((foundHit.distance < Mathf.Infinity) && (Vector3.Angle(transform.up, foundHit.normal) <= maxSlopeAngle)) groundHeight = foundHit.point.y;
        }
        protected (Vector3, Vector3) GetCapsuleSphereCenters()
        {
            return 
            (
                transform.position + transform.up * (playerRadius + maxStepHeight + stepEpsilon),
                topHead.position - transform.up * playerRadius
            );
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(currentCapsuleBase, playerRadius);
            Gizmos.DrawWireSphere(currentCapsuleEnd, playerRadius);
            Gizmos.DrawWireSphere(collisionHitPoint, .1f);
        }
#endif

        #region Movement

        /// <summary>
        /// Moves player.
        /// </summary>
        protected void HandleMovement()
        {
            float height = transform.position.y;

            Movement = Vector2.zero;

            if (navigateTo)
            {
                var delta = navigationDestination - transform.position;
                Movement = delta.normalized;

                if (Vector3.Distance(transform.position, navigationDestination) < .5f) navigateTo = false;
            }
            else UpdateMovement();

            switch (navigation)
            {
                case Navigation.Walking:
                    Walk(ref Movement, ref height);
                    break;
                case Navigation.Flying:
                    Fly(ref Movement, ref height);
                    break;
            }

            Movement *= Time.deltaTime;

            Vector3 pos = transform.rotation * new Vector3(Movement.y, 0, Movement.x);
            pos = ComputeMovement(pos);
            pos += transform.position;
            pos.y = height;
            transform.position = pos;
        }

        /// <summary>
        /// Update <see cref="Movement"/> field.
        /// </summary>
        protected abstract void UpdateMovement();
        
        /// <summary>
        /// Computes <paramref name="move"/> vector to perform a walk movement and applies gravity.
        /// </summary>
        /// <param name="move"></param>
        /// <param name="height"></param>
        protected abstract void Walk(ref Vector2 move, ref float height);

        /// <summary>
        /// Update speed when flying.
        /// </summary>
        /// <param name="Move"></param>
        /// <param name="height"></param>
        protected void Fly(ref Vector2 Move, ref float height)
        {
            Move.x *= data.flyingSpeed;
            Move.y *= data.flyingSpeed;
            height += data.flyingSpeed * ((IsCrouching ? -1 : 0) + (IsJumping ? 1 : 0));
        }

        #endregion

        #region Camera
        /// <summary>
        /// Rotates camera.
        /// </summary>
        protected abstract void HandleView();

        /// <summary>
        /// Common part of the HandleView method.
        /// </summary>
        /// <param name="angleView"></param>
        /// <param name="angularSpeed"></param>
        protected void BaseHandleView(Vector3 angleView, Vector2 angularSpeed)
        {
            Vector3 result = angleView + ((Vector3)angularSpeed).NormalizeAngle();
            if (changeToDefault)
            {
                result = lastAngleView;
                changeToDefault = false;
            }
            Vector3 displayResult;

            result.x = Mathf.Clamp(result.x, data.XAngleRange.x, data.XAngleRange.y);
            displayResult = result;
            displayResult.x = Mathf.Clamp(displayResult.x, data.XDisplayAngleRange.x, data.XDisplayAngleRange.y);

            if (state == State.Default)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, result.y, 0));
                lastAngleView = result;
            }
            else
            {
                Vector3 angleNeck = transform.rotation.eulerAngles.NormalizeAngle();
                float delta = Mathf.DeltaAngle(result.y, angleNeck.y);

                if (delta < data.YAngleRange.x) result.y = -data.YAngleRange.x + angleNeck.y;
                if (delta > data.YAngleRange.y) result.y = -data.YAngleRange.y + angleNeck.y;
            }
            viewpoint.transform.rotation = Quaternion.Euler(result);
            neckPivot.transform.rotation = Quaternion.Euler(new Vector3(Mathf.Clamp(result.x, -maxNeckAngle, maxNeckAngle), result.y, result.z));
            head.transform.rotation = Quaternion.Euler(displayResult);
        }
        #endregion

        #endregion
    }
}
public static class Vector3Extension
{
    /// <summary>
    /// For each component (x, y, z) Mathf.DeltaAngle(0, component)
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 NormalizeAngle(this Vector3 angle)
    {
        angle.x = Mathf.DeltaAngle(0, angle.x);
        angle.y = Mathf.DeltaAngle(0, angle.y);
        angle.z = Mathf.DeltaAngle(0, angle.z);

        return angle;
    }
}