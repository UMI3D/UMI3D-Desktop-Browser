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
using umi3d.cdk;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.baseBrowser.inputs.interactions
{
    public class BaseManipulation : BaseInteraction<ManipulationDto>
    {
        public Sprite Icon;
        public Transform manipulationCursor;
        /// <summary>
        /// DofGroup of the Manipulation.
        /// </summary>
        public DofGroupEnum DofGroup;
        public umi3d.baseBrowser.Cursor.FrameIndicator frameIndicator;
        /// <summary>
        /// Button to activate this input.
        /// </summary>
        public BaseInteraction<EventDto> activationButton;
        protected bool Active;
        protected bool manipulated = false;
        /// <summary>
        /// Launched coroutine for network message sending (if any).
        /// </summary>
        /// <see cref="networkMessageSender"/>
        protected Coroutine messageSenderCoroutine;
        /// <summary>
        /// Frame of reference of the <see cref="associatedInteraction"/> (if any).
        /// </summary>
        protected Transform frameOfReference;
        /// <summary>
        /// Frame rate applied to message emission through network (high values can cause network flood).
        /// </summary>
        public float networkFrameRate = 30;
        /// <summary>
        /// Input multiplicative strength.
        /// </summary>
        public float strength = 1;

        #region Instances List

        protected static int currentInstance;

        #endregion

        public override void Associate(AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
        {
            if (associatedInteraction != null)
                throw new System.Exception("This input is already binded to a interaction ! (" + associatedInteraction + ")");

            if (!IsCompatibleWith(interaction))
                throw new System.Exception("Trying to associate an uncompatible interaction !");

            this.toolId = toolId;
            foreach (DofGroupOptionDto group in (interaction as ManipulationDto).dofSeparationOptions)
            {
                foreach (DofGroupDto sep in group.separations)
                {
                    if (sep.dofs == DofGroup)
                    {
                        Associate(interaction as ManipulationDto, sep.dofs, toolId, hoveredObjectId);
                        return;
                    }
                }
            }
        }

        public override void Associate(ManipulationDto manipulation, DofGroupEnum dofs, ulong toolId, ulong hoveredObjectId)
        {
            if (associatedInteraction != null)
                throw new System.Exception("This input is already binded to a interaction ! (" + associatedInteraction + ")");

            if (dofs != DofGroup)
                throw new System.Exception("Trying to associate an uncompatible interaction !");

            this.hoveredObjectId = hoveredObjectId;
            associatedInteraction = manipulation;

            UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"Create displayer");
            //if (manipulationDisplayer == null)
            //{
            //    manipulationDisplayer = ManipulationDisplayerManager.CreateDisplayer();
            //}
            //if (manipulationDisplayer != null)
            //{
            //    manipulationDisplayer.SetUp(associatedInteraction.name, Icon);
            //}

            StartCoroutine(SetFrameOFReference());
            messageSenderCoroutine = StartCoroutine(networkMessageSender());
        }

        public override void Dissociate()
        {
            if (messageSenderCoroutine != null)
            {
                StopCoroutine(messageSenderCoroutine);
                messageSenderCoroutine = null;
            }

            UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"remove Displayer");

            associatedInteraction = null;
        }

        public override bool IsCompatibleWith(AbstractInteractionDto interaction)
        {
            if (!(interaction is ManipulationDto manipulationDto)) return false;

            return manipulationDto.dofSeparationOptions.Exists
            (
                sep => sep.separations.Exists(dof => dof.dofs == DofGroup)
            );
        }

        public override bool IsAvailable()
            => base.IsAvailable() && activationButton.IsAvailable();

        public bool IsCompatibleWith(DofGroupEnum dofGroup) => dofGroup == DofGroup;

        public static void SelectFirst() => SwicthManipulation(0);

        public static void NextManipulation() => SwicthManipulation(currentInstance + 1);

        public static void PreviousManipulation() => SwicthManipulation(currentInstance - 1);

        public static void SwicthManipulation(int i)
        {
            List<BaseManipulation> instances = BaseManipulationGroup.GetManipulations();

            if (instances == null || instances.Count == 0) return;

            if (currentInstance < instances.Count) instances[currentInstance].Deactivate();

            currentInstance = i;
            if (currentInstance < 0) currentInstance = instances.Count - 1;
            else if (currentInstance >= instances.Count) currentInstance = 0;
            if (currentInstance < instances.Count) instances[currentInstance].Activate();
        }

        public void Activate()
        {
            Active = true;
        }
        public void Deactivate()
        {
            Active = false;
            manipulated = false;
            frameIndicator.gameObject.SetActive(false);
        }

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

        protected IEnumerator networkMessageSender()
        {
            UnityEngine.Debug.Log("TODO");
            yield return null;
            //while (true)
            //{
            //    if 
            //    (
            //        Active 
            //        && associatedInteraction != null 
            //    )
            //    {
            //        if (Input.GetKey(InputLayoutManager.GetInputCode(activationButton.activationButton)))
            //        {
            //            if (manipulated)
            //            {
            //                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //                manipulationCursor.position = frameIndicator.Project(ray, DofGroup);

            //                Vector3 distanceInWorld = manipulationCursor.position - StartPosition;
            //                Vector3 distanceInFrame = frameOfReference.InverseTransformDirection(distanceInWorld);

            //                var pararmeterDto = new ManipulationRequestDto()
            //                {
            //                    boneType = bone,
            //                    id = associatedInteraction.id,
            //                    toolId = this.toolId,
            //                    hoveredObjectId = GetCurrentHoveredObjectId()
            //                };
            //                MapDistanceWithDof(distanceInFrame, ref pararmeterDto);
            //                UMI3DClientServer.SendData(pararmeterDto, true);
            //            }
            //            else
            //            {
            //                if (DoesPerformRotation())
            //                {
            //                    umi3d.baseBrowser.Controller.BaseCursor.SetMovement(this, umi3d.baseBrowser.Controller.BaseCursor.CursorMovement.Free);
            //                }

            //                manipulated = true;

            //                StartPosition = frameOfReference.position;
            //                manipulationCursor.position = StartPosition;
            //                frameIndicator.gameObject.SetActive(true);
            //                frameIndicator.DofGroup = DofGroup;
            //                frameIndicator.Frame = frameOfReference;
            //                umi3d.baseBrowser.Controller.BaseCursor.State = umi3d.baseBrowser.Controller.BaseCursor.CursorState.Clicked;
            //            }
            //        }
            //        else if (manipulated)
            //        {
            //            manipulated = false;
            //            umi3d.baseBrowser.Controller.BaseCursor.SetMovement(this, umi3d.baseBrowser.Controller.BaseCursor.CursorMovement.Center);
            //            frameIndicator.gameObject.SetActive(false);
            //            umi3d.baseBrowser.Controller.BaseCursor.State = umi3d.baseBrowser.Controller.BaseCursor.CursorState.Hover;
            //        }
            //    }

            //    yield return new WaitForSeconds(1f / networkFrameRate);
            //}
        }

        void MapDistanceWithDof(Vector3 distance, ref ManipulationRequestDto Manipulation)
        {
            switch (DofGroup)
            {
                //dof 2
                case DofGroupEnum.XY:
                    Manipulation.translation = new Vector3(distance.x, distance.y, 0) * strength;
                    break;
                case DofGroupEnum.XZ:
                    Manipulation.translation = new Vector3(distance.x, 0, distance.z) * strength;
                    break;
                case DofGroupEnum.YZ:
                    Manipulation.translation = new Vector3(0, distance.y, distance.z) * strength;
                    break;
                //dof 1
                case DofGroupEnum.X:
                    Manipulation.translation = new Vector3(distance.x, 0, 0) * strength;
                    break;
                case DofGroupEnum.Y:
                    Manipulation.translation = new Vector3(0, distance.y, 0) * strength;
                    break;
                case DofGroupEnum.Z:
                    Manipulation.translation = new Vector3(0, 0, distance.z) * strength;
                    break;
                case DofGroupEnum.RX:
                    if (Radius(distance.y, distance.z) > 0.1f)
                        Manipulation.rotation = Quaternion.Euler(AngleMod(Mathf.Rad2Deg * Mathf.Atan2(distance.z, distance.y)) * strength, 0, 0);
                    break;
                case DofGroupEnum.RY:
                    if (Radius(distance.z, distance.x) > 0.1f)
                        Manipulation.rotation = Quaternion.Euler(0, AngleMod(Mathf.Rad2Deg * Mathf.Atan2(distance.x, distance.z)) * strength, 0);
                    break;
                case DofGroupEnum.RZ:
                    if (Radius(distance.x, distance.y) > 0.1f)
                        Manipulation.rotation = Quaternion.Euler(0, 0, AngleMod(Mathf.Rad2Deg * Mathf.Atan2(distance.y, distance.x) - 90) * strength);
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