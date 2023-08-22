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
using ccs = umi3d.cdk.collaboration.UMI3DCollaborationClientServer;

namespace umi3d.baseBrowser.cursor
{
    public class BaseCursor : inetum.unityUtils.SingleBehaviour<BaseCursor>
    {
        public enum CursorState { Default, Hover, Clicked, FollowCursor }
        public enum CursorMovement { Free, Center, Confined, FreeHidden }

        public event System.Action<CursorState> UpdateCursor;

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
        private static event System.Action s_stateUpdated;
        private static CursorMovement s_movement;
        private static event System.Action s_movementUpdated;

        protected System.Collections.Generic.Dictionary<object, CursorMovement> m_movementMap = new System.Collections.Generic.Dictionary<object, CursorMovement>();
        private bool m_isMovementCenterOrFreeHidden => Movement == CursorMovement.Center || Movement == CursorMovement.FreeHidden;

        protected override void Awake()
        {
            base.Awake();
            s_stateUpdated += UpdateEnvironmentCursor;
            s_movementUpdated += UpdateUnityCursor;
            s_movementUpdated += UpdateEnvironmentCursor;
        }

        protected virtual void Start()
        {
            if (ccs.Exists) ccs.Instance.OnLeavingEnvironment.AddListener(Clear);
        }

        protected virtual void Update()
        {
            if (UnityEngine.Camera.main != null)
            {
                transform.position = UnityEngine.Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition); 
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (ccs.Exists) ccs.Instance.OnLeavingEnvironment.RemoveListener(Clear);
            s_stateUpdated = null;
            s_movementUpdated = null;
            Destroy(gameObject);
        }

        /// <summary>
        /// Add the Object to the map of movement.
        /// </summary>
        /// <param name="Object"></param>
        /// <param name="movement"></param>
        public static void SetMovement(object Object, CursorMovement movement)
        {
            if (!Exists) return;
            Instance.m_movementMap[Object] = movement;
            Instance.MapToMovement();
        }
        /// <summary>
        /// Remove the Object from the map of movement.
        /// </summary>
        /// <param name="Object"></param>
        public static void UnSetMovement(object Object)
        {
            if (!Exists) return;
            Instance.m_movementMap.Remove(Object);
            Instance.MapToMovement();
        }

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
            else movement = CursorMovement.Free;

            Movement = movement;
        }

        /// <summary>
        /// Clear Movement map
        /// </summary>
        public void Clear() => m_movementMap.Clear();

        private void UpdateUnityCursor()
        {
            switch (s_movement)
            {
                case CursorMovement.Center:
                    UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.Locked;
                    UnityEngine.Cursor.visible = false;
                    break;
                case CursorMovement.Free:
                    UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.None;
                    UnityEngine.Cursor.visible = true;
                    break;
                case CursorMovement.FreeHidden:
                    UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.None;
                    UnityEngine.Cursor.visible = false;
                    break;
                case CursorMovement.Confined:
                    UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.Confined;
                    UnityEngine.Cursor.visible = true;
                    break;
            }
        }

        private void UpdateEnvironmentCursor()
        {
            if (!m_isMovementCenterOrFreeHidden) return;
            if (Movement == CursorMovement.Center) UpdateCursor?.Invoke(State);
            else if (Movement == CursorMovement.FreeHidden) UpdateCursor?.Invoke(CursorState.FollowCursor);
        }
    }


}
