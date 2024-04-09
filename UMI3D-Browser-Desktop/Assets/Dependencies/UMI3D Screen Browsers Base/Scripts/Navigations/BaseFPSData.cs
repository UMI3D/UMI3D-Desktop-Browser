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
using UnityEngine;
using static umi3d.baseBrowser.Navigation.BaseFPSNavigation;

namespace umi3d.baseBrowser.Navigation
{
    [CreateAssetMenu(fileName = "FPSData", menuName = "UMI3D/FPS Data", order = 1)]
    public class BaseFPSData : ScriptableObject
    {
        [Header("View")]
        [Tooltip("Range of the viewpoint x angle (down to up)")]
        public Vector2 XAngleRange;
        [Tooltip("Range of the head x angle (down to up). \n" +
            "The head will follow the viewpoint bounded by this range")]
        public Vector2 XDisplayAngleRange;
        [Tooltip("Range of the viewpoint/head y angle (left to right)")]
        public Vector2 YAngleRange;
        [Tooltip("Angular speed of the viewpoint (up/down, left/right)")]
        public Vector2 AngularViewSpeed;

        [Header("Walk")]
        [Tooltip("speed when moving forward (normal, squatting, running)")]
        public Vector3 forwardSpeed;
        [Tooltip("speed when moving sideway (normal, squatting, running)")]
        public Vector3 lateralSpeed;
        [Tooltip("speed when moving backward (normal, squatting, running)")]
        public Vector3 backwardSpeed;

        [Header("Jump")]
        [Tooltip("max jump height when long pressing jump action")]
        public float MaxJumpHeight;
        [Tooltip("min jump height when short pressing jump action")]
        public float MinJumpHeight;

        [Header("Vertical Movement")]
        [Tooltip("gravity force")]
        public float gravity;
        [Tooltip("Current Vertical Velocity")]
        public float velocity;
        [Tooltip("Max jump velocity")]
        public float maxJumpVelocity => Mathf.Sqrt(2 * Mathf.Abs(gravity) * MaxJumpHeight);

        [Header("Squat")]
        [Tooltip("player height while squatting")]
        public float squatHeight;
        [Tooltip("player height while standing")]
        public float standHeight;
        [Tooltip("time to switch between standing and squatting (both ways)")]
        public float squatSpeed;
        [Tooltip("Torso angle when squatting")]
        public float squatTorsoAngle;

        [Header("Fly")]
        [Tooltip("speed when moving in every direction while flying")]
        public float flyingSpeed;

        [Header("Collision")]
        [Tooltip("Radius used from player center to raycast")]
        public float playerRadius = .3f;
        [Tooltip("Maximum angle for slope.")]
        public float maxSlopeAngle = 45f;
        [Tooltip("Maximum height for step.")]
        public float maxStepHeight = .2f;
        public float stepEpsilon = 0.05f;

        [Header("Collision")]
        [Tooltip("Current ground height.")]
        public float groundYAxis = 0f;

        [Header("Input")]
        [Tooltip("Whether or not the player want to jump.")]
        public bool WantToJump;
        [Tooltip("Whether or not the player is jumping.")]
        public bool IsJumping;
        [Tooltip("Whether or not the player want to squat.")]
        public bool WantToCrouch;
        [Tooltip("Whether or not the player is squatting.")]
        public bool IsCrouching;
        [Tooltip("Whether or not the player want to Sprint.")]
        public bool WantToSprint;
        [Tooltip("Whether or not the player is Sprinting.")]
        public bool IsSprinting;

        [Header("Movement")]
        [Tooltip("Navigation mode")]
        public E_NavigationMode navigationMode;
        /// <summary>
        /// Movement of the player.
        /// 
        /// <list type="bullet">
        /// <item>x: Left to right (positive: right)</item>
        /// <item>y: Down to up (positive: up)</item>
        /// <item>z: back to front (positive: front)</item>
        /// </list>
        /// </summary>
        [HideInInspector]
        public Vector3 Movement;
        /// <summary>
        /// Destination for continuous navigation.<br/>
        /// If the value is null then there is no server navigation.<br/>
        /// If the value is not null then a navigation asked by the server is performing.
        /// </summary>
        [HideInInspector]
        public Vector3? continuousDestination;
    }
}
