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
using umi3d.baseBrowser.cursor;
using UnityEngine;

namespace umi3d.baseBrowser.Navigation
{
    public partial class BaseFPSNavigation : cdk.AbstractNavigation
    {
        #region Fields

        public static BaseFPSNavigation Instance => s_instance;
        protected static BaseFPSNavigation s_instance;

        public IConcreteFPSNavigation CurrentNavigation;

        protected List<IConcreteFPSNavigation> m_navigations = new List<IConcreteFPSNavigation>();

        /// <summary>
        /// Is player active ?
        /// </summary>
        protected bool isActive = false;

        [Header("Player Body")]
        public Transform skeleton;

        protected Vector3 destination;

        /// <summary>
        /// Is navigation currently performed ?
        /// </summary>
        protected bool navigateTo;
        protected Vector3 navigationDestination;

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
            jumpData = new JumpData()
            {
                data = data,
                maxJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(data.gravity) * data.MaxJumpHeight)
            };
            UpdateBaseHeight();
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
            navigationDestination = transform.parent.position + data.position.Struct();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="data"></param>
        public override void Teleport(common.TeleportDto data)
        {
            transform.localPosition = data.position.Struct();
            groundHeight = data.position.Y;
            transform.localRotation = data.rotation.Quaternion();
            UpdateBaseHeight();
        }

        #endregion

        private void Awake()
        {
            //TODO instantiate concrete navigations.
            m_navigations.Add
            (
                new KeyboardAndMouseFpsNavigation()
                {
                    FPSNavigation = this,
                    data = data,
                }
            );

            //TODO for now CurrentNavigation is the desktop one.
            CurrentNavigation = m_navigations.Find(navigation => navigation is KeyboardAndMouseFpsNavigation);
        }

        private void Start()
        {
            if (s_instance == null) s_instance = this;
            else Destroy(this.gameObject);
        }

        private void Update()
        {
            if (!isActive) return;

            CurrentNavigation?.Update();
        }

        /// <summary>
        /// return True if the override method can execute, false otherwithe.
        /// </summary>
        /// <returns></returns>
        public virtual bool OnUpdate()
        {
            if (vehicleFreeHead)
            {
                state = State.FreeHead;

                if
                (
                    !(BaseCursor.Movement == BaseCursor.CursorMovement.Free
                    || BaseCursor.Movement == BaseCursor.CursorMovement.FreeHidden)
                ) HandleView();

                return false;
            }

            //Mandatory for colision.
            (currentCapsuleBase, currentCapsuleEnd) = GetCapsuleSphereCenters();

            if
            (
                (BaseCursor.Movement == BaseCursor.CursorMovement.Free
                || BaseCursor.Movement == BaseCursor.CursorMovement.FreeHidden)
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

        #endregion
    }
}
