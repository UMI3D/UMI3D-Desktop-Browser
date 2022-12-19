/*
Copyright 2019 - 2023 Inetum

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
    public abstract partial class BaseFPSNavigation : cdk.AbstractNavigation
    {
        #region Struct Definition 
        
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

        #region Player state

        /// <summary>
        /// Is player active ?
        /// </summary>
        protected bool isActive = false;
        
        protected Vector3 destination;

        public static UnityEvent PlayerMoved = new UnityEvent();

        protected bool changeToDefault = false;

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
        

        #endregion

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


        #endregion

        private void Start()
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
        /// Computes <paramref name="move"/> vector to perform a walk movement and applies gravitu.
        /// </summary>
        /// <param name="move"></param>
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

        #endregion
    }
}
