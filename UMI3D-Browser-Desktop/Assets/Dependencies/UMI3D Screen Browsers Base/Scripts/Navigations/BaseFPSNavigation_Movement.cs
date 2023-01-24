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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace umi3d.baseBrowser.Navigation
{
    public partial class BaseFPSNavigation
    {
        public enum Navigation { Walking, Flying }

        public static UnityEvent PlayerMoved = new UnityEvent();

        [HideInInspector]
        public bool WantToJump;
        [HideInInspector]
        public bool IsCrouching;
        [HideInInspector]
        public bool WantToCrouch;
        [HideInInspector]
        public Vector2 Movement;

        [Header("Movement")]
        [SerializeField]
        protected BaseFPSData data;
        [SerializeField]
        [Tooltip("Navigation mode")]
        protected Navigation navigation;

        /// <summary>
        /// Stores all data about player jumps.
        /// </summary>
        protected JumpData jumpData;

        public void OnPlayerMoved() => PlayerMoved?.Invoke();

        /// <summary>
        /// Applies gravity to player and makes it jump.
        /// </summary>
        /// <param name="jumping"></param>
        /// <param name="height"></param>
        public void ComputeGravity(bool jumping, ref float height)
        {
            jumpData.ComputeVelocity(Time.time, Time.deltaTime, jumping && CanJump());

            height += jumpData.velocity * Time.deltaTime;
            if (height >= groundHeight) return;

            float offset = Mathf.Abs(height - groundHeight);
            if
            (
                offset < maxStepHeight + stepEpsilon
                && offset > stepEpsilon
                && hasGroundHeightChangedLastFrame
            ) height = Mathf.Lerp(height, groundHeight, .5f);
            else
            {
                jumpData.velocity = 0;
                height = groundHeight;
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
            if (navigation == Navigation.Flying) return direction;
            else if (CheckNavmesh(direction)) return CheckCollision(direction);
            else return Vector3.zero;
        }

        /// <summary>
        /// Moves player.
        /// </summary>
        public void HandleMovement()
        {
            float height = transform.position.y;

            Movement = Vector2.zero;

            if (navigateTo)
            {
                var delta = navigationDestination - transform.position;
                Movement = delta.normalized;

                if (Vector3.Distance(transform.position, navigationDestination) < .5f) navigateTo = false;
            }
            else UpdateMovement(ref Movement);

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
        /// Update <paramref name="move"/> field according to player input.
        /// </summary>
        protected virtual void UpdateMovement(ref Vector2 move)
            => CurrentNavigation?.UpdateMovement(ref move);

        /// <summary>
        /// Computes <paramref name="move"/> vector to perform a walk movement and applies gravity.
        /// </summary>
        /// <param name="move"></param>
        /// <param name="height"></param>
        protected virtual void Walk(ref Vector2 move, ref float height)
            => CurrentNavigation?.Walk(ref move, ref height);

        /// <summary>
        /// Update speed when flying.
        /// </summary>
        /// <param name="Move"></param>
        /// <param name="height"></param>
        protected void Fly(ref Vector2 Move, ref float height)
        {
            Move.x *= data.flyingSpeed;
            Move.y *= data.flyingSpeed;
            height += data.flyingSpeed * ((IsCrouching ? -1 : 0) + (WantToJump ? 1 : 0));
        }
    }
}
