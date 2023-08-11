/*
Copyright 2019 - 2022 Inetum

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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using umi3d.baseBrowser.cursor;
using umi3d.cdk;
using umi3d.cdk.menu.interaction;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.baseBrowser.inputs.interactions
{
    public abstract class BaseManipulation : BaseInteraction<ManipulationDto>
    {
        public ManipulationMenuItem menuItem;

        /// <summary>
        /// DofGroup of the Manipulation.
        /// </summary>
        public DofGroupEnum DofGroup;
        
        /// <summary>
        /// Input multiplicative strength.
        /// </summary>
        public float strength = 1;

        #region Instances

        protected static int s_currentIndex;

        #region Incrementation and decrementation

        /// <summary>
        /// Deactivate current manipulation and activate next one.
        /// </summary>
        public static void NextManipulation() => SwitchManipulation(s_currentIndex + 1);

        /// <summary>
        /// Deactivate current manipulation and activate previous one.
        /// </summary>
        public static void PreviousManipulation() => SwitchManipulation(s_currentIndex - 1);

        /// <summary>
        /// Update the current selected manipulation.
        /// </summary>
        /// <param name="i"></param>
        public static void SwitchManipulation(int i)
        {
            List<BaseManipulation> instances = BaseManipulationGroup.CurrentManipulations;

            if (instances == null || instances.Count == 0) return;

            if (s_currentIndex < instances.Count && s_currentIndex >= 0) instances[s_currentIndex].Deactivate();

            if (instances.Count == 0)
            {
                s_currentIndex = -1;
                UnityEngine.Debug.Log($"switch group {i}, {s_currentIndex}");
                return;
            }

            if (i < 0) s_currentIndex = instances.Count - 1;
            else if (i >= instances.Count) s_currentIndex = 0;
            else s_currentIndex = i;

            instances[s_currentIndex].Activate();
        }

        #endregion

        #endregion

        #region Activation, Deactivation, Select

        /// <summary>
        /// Whether or not this manipulation is active.
        /// </summary>
        public bool IsActive { get => m_isActive; protected set => m_isActive = value; }
        [SerializeField]
        [Header("Do not update this value in the inspector.")]
        private bool m_isActive;

        protected virtual void Activate()
        {
            IsActive = true;
            menuItem.UnSubscribe(Select);
            menuItem.Select();
            menuItem.Subscribe(Select);
        }
        /// <summary>
        /// Deactivate this manipulation.
        /// </summary>
        public virtual void Deactivate()
        {
            IsActive = false;
            manipulated = false;
            frameIndicator.gameObject.SetActive(false);
        }

        /// <summary>
        /// Select the first manipulation.
        /// </summary>
        public static void SelectFirst() => SwitchManipulation(0);

        /// <summary>
        /// Select this manipulation.
        /// </summary>
        public virtual void Select() => SwitchManipulation(BaseManipulationGroup.CurrentManipulations.IndexOf(this));

        /// <summary>
        /// Unselect all manipulations.
        /// </summary>
        public static void UnSelectAll()
        {
            List<BaseManipulation> instances = BaseManipulationGroup.CurrentManipulations;

            if (instances == null || instances.Count == 0) return;

            if (s_currentIndex < instances.Count && s_currentIndex >= 0) instances[s_currentIndex].Deactivate();
            s_currentIndex = -1;
        }

        #endregion

        #region Association and dissociation

        public override void Associate(AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
            => throw new System.NotImplementedException();

        public override void Associate(ManipulationDto manipulation, DofGroupEnum dofs, ulong toolId, ulong hoveredObjectId)
        {
            if (associatedInteraction != null)
                throw new System.Exception("This input is already binded to a interaction ! (" + associatedInteraction + ")");

            if (!IsCompatibleWith(dofs)) throw new System.Exception("Trying to associate an uncompatible interaction !");

            this.hoveredObjectId = hoveredObjectId;
            this.toolId = toolId;
            associatedInteraction = manipulation;

            menuItem = new ManipulationMenuItem
            {
                Name = associatedInteraction.name,
                dof = new DofGroupDto
                {
                    dofs = dofs,
                }
            };
            menuItem.Subscribe(Select);
            Menu?.Add(menuItem);

            StartCoroutine(SetFrameOFReference());
            messageSenderCoroutine = StartCoroutine(NetworkMessageSender());
        }

        public override void Dissociate()
        {
            if (messageSenderCoroutine != null)
            {
                StopCoroutine(messageSenderCoroutine);
                messageSenderCoroutine = null;
                LeaveManipulation();
            }

            associatedInteraction = null;
            Menu?.Remove(menuItem);
            menuItem?.UnSubscribe(Select);
            menuItem = null;
        }

        protected virtual void LeaveManipulation()
        {
            manipulated = false;
            BaseCursor.SetMovement(this, BaseCursor.CursorMovement.Center);
            frameIndicator.gameObject.SetActive(false);
            BaseCursor.State = BaseCursor.CursorState.Hover;
        }

        #endregion

        #region Is compatible or is availlable

        public override bool IsCompatibleWith(AbstractInteractionDto interaction)
        {
            if (!(interaction is ManipulationDto manipulationDto)) return false;

            return manipulationDto.dofSeparationOptions.Exists
            (
                sep => sep.separations.Exists(dof => dof.dofs == DofGroup)
            );
        }

        

        public bool IsCompatibleWith(DofGroupEnum dofGroup) => dofGroup == DofGroup;

        #endregion

        /// <summary>
        /// Frame of reference of the <see cref="associatedInteraction"/> (if any).
        /// </summary>
        protected Transform frameOfReference;
        
        protected IEnumerator SetFrameOFReference()
        {
            var wait = new WaitForFixedUpdate();
            while (true)
            {
                if (associatedInteraction?.frameOfReference != null)
                {
                    GameObject frame = cdk.UMI3DEnvironmentLoader.GetNode(associatedInteraction.frameOfReference).gameObject;
                    if (frame != null)
                    {
                        frameOfReference = frame.transform;
                        yield break;
                    }
                    else yield return wait;

                }
                else yield break;
            }
        }

        /// <summary>
        /// Launched coroutine for network message sending (if any).
        /// </summary>
        /// <see cref="NetworkMessageSender"/>
        protected Coroutine messageSenderCoroutine;
        protected bool manipulated = false;
        public Transform manipulationCursor;
        public Cursor.FrameIndicator frameIndicator;
        /// <summary>
        /// Frame rate applied to message emission through network (high values can cause network flood).
        /// </summary>
        public float networkFrameRate = 30;

        protected abstract IEnumerator NetworkMessageSender();

        protected abstract bool DoesPerformRotation();

        protected void MapDistanceWithDof(Vector3 distance, ref ManipulationRequestDto Manipulation)
        {
            switch (DofGroup)
            {
                //dof 2
                case DofGroupEnum.XY:
                    Manipulation.translation = (new Vector3(distance.x, distance.y, 0) * strength).Dto();
                    break;
                case DofGroupEnum.XZ:
                    Manipulation.translation = (new Vector3(distance.x, 0, distance.z) * strength).Dto();
                    break;
                case DofGroupEnum.YZ:
                    Manipulation.translation = (new Vector3(0, distance.y, distance.z) * strength).Dto();
                    break;
                //dof 1
                case DofGroupEnum.X:
                    Manipulation.translation = (new Vector3(distance.x, 0, 0) * strength).Dto();
                    break;
                case DofGroupEnum.Y:
                    Manipulation.translation = (new Vector3(0, distance.y, 0) * strength).Dto();
                    break;
                case DofGroupEnum.Z:
                    Manipulation.translation = (new Vector3(0, 0, distance.z) * strength).Dto();
                    break;
                case DofGroupEnum.RX:
                    if (Radius(distance.y, distance.z) > 0.1f)
                        Manipulation.rotation = Quaternion.Euler(AngleMod(Mathf.Rad2Deg * Mathf.Atan2(distance.z, distance.y)) * strength, 0, 0).Dto();
                    break;
                case DofGroupEnum.RY:
                    if (Radius(distance.z, distance.x) > 0.1f)
                        Manipulation.rotation = Quaternion.Euler(0, AngleMod(Mathf.Rad2Deg * Mathf.Atan2(distance.x, distance.z)) * strength, 0).Dto();
                    break;
                case DofGroupEnum.RZ:
                    if (Radius(distance.x, distance.y) > 0.1f)
                        Manipulation.rotation = Quaternion.Euler(0, 0, AngleMod(Mathf.Rad2Deg * Mathf.Atan2(distance.y, distance.x) - 90) * strength).Dto();
                    break;
            }
        }

        float Radius(float x, float y) => Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));

        float AngleMod(float angle)
        {
            while (true)
            {
                if (angle <= 180)
                {
                    if (angle > -180) return angle;
                    angle += 360;
                }
                else angle -= 360;
            }
        }
    }
}
