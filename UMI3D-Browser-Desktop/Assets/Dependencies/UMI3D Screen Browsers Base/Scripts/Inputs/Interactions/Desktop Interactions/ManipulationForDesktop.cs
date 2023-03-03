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
using umi3d.baseBrowser.cursor;
using umi3d.cdk;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.baseBrowser.inputs.interactions
{
    public class ManipulationForDesktop : BaseManipulation
    {
        /// <summary>
        /// Button to activate this input.
        /// </summary>
        public KeyboardManipulation ManipulationInput;

        public override bool IsAvailable()
            => base.IsAvailable() && ManipulationInput.IsAvailable();

        protected override IEnumerator NetworkMessageSender()
        {
            yield return null;

            Vector3 StartPosition = new Vector3();

            while (true)
            {
                if
                (
                    IsActive
                    && associatedInteraction != null
                )
                {
                    if (ManipulationInput.Key.IsPressed())
                    {
                        if (manipulated)
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                            manipulationCursor.position = frameIndicator.Project(ray, DofGroup);

                            Vector3 distanceInWorld = manipulationCursor.position - StartPosition;
                            Vector3 distanceInFrame = frameOfReference.InverseTransformDirection(distanceInWorld);

                            var pararmeterDto = new ManipulationRequestDto()
                            {
                                boneType = bone,
                                id = associatedInteraction.id,
                                toolId = this.toolId,
                                hoveredObjectId = hoveredObjectId
                            };

                            MapDistanceWithDof(distanceInFrame, ref pararmeterDto);
                            UMI3DClientServer.SendData(pararmeterDto, true);
                        }
                        else
                        {
                            if (DoesPerformRotation()) BaseCursor.SetMovement(this, BaseCursor.CursorMovement.Free);

                            manipulated = true;

                            StartPosition = frameOfReference.position;
                            manipulationCursor.position = StartPosition;
                            frameIndicator.gameObject.SetActive(true);
                            frameIndicator.DofGroup = DofGroup;
                            frameIndicator.Frame = frameOfReference;
                            BaseCursor.State = BaseCursor.CursorState.Clicked;
                        }
                    }
                    else if (manipulated) LeaveManipulation();
                }

                yield return new WaitForSeconds(1f / networkFrameRate);
            }
        }

        protected override bool DoesPerformRotation()
            => DofGroup == DofGroupEnum.RX
            || DofGroup == DofGroupEnum.RY
            || DofGroup == DofGroupEnum.RZ;
    }
}
