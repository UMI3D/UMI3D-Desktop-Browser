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

namespace umi3d.baseBrowser.Navigation
{
    [CreateAssetMenu(fileName = "FPSData", menuName = "UMI3D/FPS Data", order = 1)]
    public class BaseFPSData : ScriptableObject
    {
        [Header("View (Camera, Head, Neck)")]
        [Tooltip("Max rotation angle for the head around x axis (down to up).")]
        public Vector2 maxXHeadAngle = new Vector2(-60f, 70f);
        [Tooltip("Max rotation angle for the viewpoint(camera) around the x axis (down to up)")]
        public float maxXCameraAngle = 90f;
        [Tooltip("Max rotation angle for the viewpoint(camera)/head around the y axis (left to right)")]
        public float maxYCameraAngle = 90f;
        [Tooltip("Max rotation angle for the neck.")]
        public float maxNeckAngle = 50f;
        [Tooltip("Angular speed of the viewpoint(camera) (up/down, left/right)")]
        public Vector2 AngularViewSpeed = new Vector2(5f, 5f);

        [Header("Vertical Movement")]
        [Tooltip("gravity force")]
        public float gravity;
        [Tooltip("Max jump velocity")]
        public float maxJumpVelocity => Mathf.Sqrt(2 * Mathf.Abs(gravity) * MaxJumpHeight);

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

        [Header("Crouch")]
        [Tooltip("player height while squatting")]
        public float crouchYAxis = -.3f;
        [Tooltip("time to switch between standing and squatting (both ways)")]
        public float crouchSpeed = 0.2f;
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
        [Tooltip("Current ground height.")]
        public float groundYAxis = 0f;

        [Header("Input")]
        [Tooltip("Whether or not the player want to jump.")]
        public bool WantToJump;
        [Tooltip("Whether or not the player is jumping.")]
        public bool IsJumping;
        [Tooltip("Whether or not the player want to crouch.")]
        public bool WantToCrouch;
        [Tooltip("Whether or not the player is crouching.")]
        public bool IsCrouching;
        [Tooltip("Whether or not the player want to Sprint.")]
        public bool WantToSprint;
        [Tooltip("Whether or not the player is Sprinting.")]
        public bool IsSprinting;
        [Tooltip("Whether or not the player want to look around.")]
        public bool WantToLookAround;

        [Header("Movement")]
        [Tooltip("Navigation mode")]
        public E_NavigationMode navigationMode;
        [Tooltip("Camera mode")]
        public E_CameraMode cameraMode;
        [Tooltip("Cursor mode")]
        public E_CursorMode cursorMode;

        /// <summary>
        /// Current Vertical Velocity of the player.
        /// </summary>
        [HideInInspector]
        public float playerVerticalVelocity;
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
        public Vector3 playerMovement;
        /// <summary>
        /// Destination for continuous navigation.<br/>
        /// If the value is null then there is no server navigation.<br/>
        /// If the value is not null then a navigation asked by the server is performing.
        /// </summary>
        [HideInInspector]
        public Vector3? continuousDestination;
        /// <summary>
        /// Movement of the camera according to axis.
        /// 
        /// <list type="bullet">
        /// <item>x: Down to up (positive: up)</item>
        /// <item>y: Left to right (positive: right)</item>
        /// </list>
        /// </summary>
        [HideInInspector]
        public Vector2 cameraMovement;
    }
}
