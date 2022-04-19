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
using System.Collections.Generic;
using umi3d.cdk.collaboration;
using UnityEngine;

namespace BrowserDesktop.Cursor
{
    public partial class CursorHandler
    {
        public enum CursorState { Default, Hover, Clicked, FollowCursor }
        public enum CursorMovement { Free, Center, Confined, FreeHidden }

        public Texture2D HintCursor;
        public RectTransform CrossCursor;
        public RectTransform CircleCursor;
        public RectTransform ClickedCursor;
        public RectTransform LeftClickOptionCursor;
        public RectTransform LeftClickExitCursor;
        public RectTransform FollowCursor;
        public bool MenuIndicator = false;
        public bool ExitIndicator = false;
        public CursorMode cursorMode = CursorMode.Auto;
        public Vector2 hotSpot = Vector2.zero;
        [SerializeField]
        private bool LastMenuState = false;

        public static CursorState State 
        { 
            get => Exists ? Instance.state : CursorState.Default;
            set 
            { 
                if (Exists && Instance.state != value)
                    Instance.state = value; Instance.stateUpdated = true;
            } 
        }
        public static CursorMovement Movement => Exists ? Instance.cursorMovement : CursorMovement.Free;

        private Dictionary<object, CursorMovement> MovementMap = new Dictionary<object, CursorMovement>();
        private CursorState state;
        private CursorMovement cursorMovement;
        private bool stateUpdated = true;
        private bool movementUpdated = true;

        private bool m_isMovementCenterOrFreeHidden => cursorMovement == CursorMovement.Center || cursorMovement == CursorMovement.FreeHidden;
    }


    public partial class CursorHandler : SingleBehaviour<CursorHandler>
    {
        /// <summary>
        /// Add the Object to the map of movement.
        /// </summary>
        /// <param name="Object"></param>
        /// <param name="movement"></param>
        public static void SetMovement(object Object, CursorMovement movement)
        {
            if (!Exists)
                return;
            Instance.MovementMap[Object] = movement;
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
            Instance.MovementMap.Remove(Object);
            Instance.MapToMovement();
        }

        public void Clear()
        {
            MovementMap.Clear();
        }

        private void MapToMovement()
        {
            CursorMovement movement = CursorMovement.FreeHidden;
            if (MovementMap.Count > 0) 
            {
                foreach (var move in MovementMap.Values)
                {
                    if (move >= movement) continue;
                    movement = move;
                    if (movement == CursorMovement.Free) break;
                }
            }
            else 
                movement = CursorMovement.Free;

            if (movement == cursorMovement) return;
            movementUpdated = true; 
            cursorMovement = movement;
        }

        private void Start()
        {
            LastMenuState = false;
            stateUpdated = true;
            movementUpdated = true;
            UMI3DCollaborationClientServer.LoggingOut += Clear;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UMI3DCollaborationClientServer.LoggingOut -= Clear;
            Destroy(gameObject);
        }

        private void Update()
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            UpdateUnityCursor();
            
            if ((stateUpdated || LastMenuState != m_isMovementCenterOrFreeHidden) && CursorDisplayer.Exists)
            {
                LastMenuState = m_isMovementCenterOrFreeHidden;
                if (LastMenuState)
                {
                    if (cursorMovement == CursorMovement.Center)
                        CursorDisplayer.Instance.DisplayCursor(true, state);
                    else if (cursorMovement == CursorMovement.FreeHidden)
                        CursorDisplayer.Instance.DisplayCursor(true, CursorState.FollowCursor);
                }
                stateUpdated = false;
            }

            if (CursorDisplayer.Exists && CursorDisplayer.Instance.IsSettingsCursorDisplayed() != (LastMenuState && MenuIndicator))
                CursorDisplayer.Instance.DisplaySettingsCursor(LastMenuState && MenuIndicator);
        }

        private void UpdateUnityCursor()
        {
            if (!movementUpdated)
                return;

            switch (cursorMovement)
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
            movementUpdated = false;
        }
    }
}