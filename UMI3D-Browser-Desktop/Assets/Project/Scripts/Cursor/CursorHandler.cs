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
using BrowserDesktop.Menu;
using inetum.unityUtils;
using System;
using System.Collections.Generic;
using umi3d.cdk.collaboration;
using UnityEngine;

namespace BrowserDesktop.Cursor
{
    public partial class CursorHandler
    {
        public enum CursorState { Default, Hover, Clicked, FollowCursor }
        public enum CursorMovement { Free, Center, Confined, FreeHidden }

        [SerializeField]
        private bool LastMenuState = false;

        public static CursorState State 
        { 
            get => Exists ? s_state : CursorState.Default;
            set 
            { 
                if (Exists && s_state != value)
                {
                    s_state = value; 
                    s_stateUpdated?.Invoke();
                }
            } 
        }
        public static CursorMovement Movement
        { 
            get => Exists ? s_movement : CursorMovement.Free;
            private set
            {
                if (Exists && s_movement != value)
                {
                    s_movement = value;
                    s_movementUpdated?.Invoke();
                }
            }
        }

        private static CursorState s_state;
        private static event Action s_stateUpdated;
        private static CursorMovement s_movement;
        private static event Action s_movementUpdated;


        private Dictionary<object, CursorMovement> m_movementMap = new Dictionary<object, CursorMovement>();

        private bool m_isMovementCenterOrFreeHidden 
            => Movement == CursorMovement.Center || Movement == CursorMovement.FreeHidden;
    }


    public partial class CursorHandler : SingleBehaviour<CursorHandler>
    {
        #region Movement mapping and setup

        /// <summary>
        /// Add the Object to the map of movement.
        /// </summary>
        /// <param name="Object"></param>
        /// <param name="movement"></param>
        public static void SetMovement(object Object, CursorMovement movement)
        {
            if (!Exists)
                return;
            Instance.m_movementMap[Object] = movement;
            Instance.MapToMovement();
        }
        /// <summary>
        /// Remove the Object from the map of movement.
        /// </summary>
        /// <param name="Object"></param>
        public static void UnSetMovement(object Object)
        {
            if (!Exists)
                return;
            Instance.m_movementMap.Remove(Object);
            Instance.MapToMovement();
        }

        /// <summary>
        /// Clear Movement map
        /// </summary>
        public void Clear()
            => m_movementMap.Clear();

        private void MapToMovement()
        {
            CursorMovement movement = CursorMovement.FreeHidden;
            if (m_movementMap.Count > 0) 
            {
                foreach (var move in m_movementMap.Values)
                {
                    if (move >= movement) continue;
                    movement = move;
                    if (movement == CursorMovement.Free) break;
                }
            }
            else 
                movement = CursorMovement.Free;

            Movement = movement;
        }

        #endregion

        #region Monobehaviour

        protected override void Awake()
        {
            base.Awake();
            s_stateUpdated += UpdateEnvironmentCursor;
            s_movementUpdated += UpdateUnityCursor;
            s_movementUpdated += UpdateEnvironmentCursor;
        }

        private void Start()
        {
            LastMenuState = false;
            UMI3DCollaborationClientServer.LoggingOut += Clear;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UMI3DCollaborationClientServer.LoggingOut -= Clear;
            s_stateUpdated = null;
            s_movementUpdated = null;
            Destroy(gameObject);
        }

        private void Update()
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (CursorDisplayer.Exists && CursorDisplayer.Instance.IsSettingsCursorDisplayed != (LastMenuState))
                CursorDisplayer.Instance.DisplaySettingsCursor(LastMenuState);
        }

        #endregion

        private void UpdateUnityCursor()
        {
            switch (s_movement)
            {
                case CursorMovement.Center:
                    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                    UnityEngine.Cursor.visible = false;
                    break;
                case CursorMovement.Free:
                    UnityEngine.Cursor.lockState = CursorLockMode.None;
                    UnityEngine.Cursor.visible = true;
                    break;
                case CursorMovement.FreeHidden:
                    UnityEngine.Cursor.lockState = CursorLockMode.None;
                    UnityEngine.Cursor.visible = false;
                    break;
                case CursorMovement.Confined:
                    UnityEngine.Cursor.lockState = CursorLockMode.Confined;
                    UnityEngine.Cursor.visible = true;
                    break;
            }
        }

        private void UpdateEnvironmentCursor()
        {
            if (!CursorDisplayer.Exists || !m_isMovementCenterOrFreeHidden)
                return;

            //LastMenuState = m_isMovementCenterOrFreeHidden;

            //if (!LastMenuState)
            //    return;

            if (Movement == CursorMovement.Center)
                CursorDisplayer.Instance.DisplayCursor(true, State);
            else if (Movement == CursorMovement.FreeHidden)
                CursorDisplayer.Instance.DisplayCursor(true, CursorState.FollowCursor);
        }
    }
}