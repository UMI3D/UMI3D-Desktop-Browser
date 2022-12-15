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

using umi3d.cdk;
using umi3d.common;

namespace umi3d.baseBrowser.inputs.interactions
{
    [System.Serializable]
    public class KeyMenuInput : BaseTouchInteraction<common.interaction.EventDto>
    {
        /// <summary>
        /// True if an Abstract Input is currently hold by a user.
        /// </summary>
        public static bool IsInputHold;
        /// <summary>
        /// True if the rising edge event has been sent through network (to avoid sending falling edge only).
        /// </summary>
        protected bool risingEdgeEventSent;

        protected void Awake()
        {
            bone = common.userCapture.BoneType.RightHand;
        }

        protected override void CreateMenuItem()
        {
            menuItem = new ButtonMenuItem
            {
                Name = associatedInteraction.name,
                IsHoldable = associatedInteraction.hold
            };
        }

        protected override void PressedDown()
        {
            onInputDown.Invoke();
            if (associatedInteraction.hold)
            {
                var eventdto = new common.interaction.EventStateChangedDto
                {
                    active = true,
                    boneType = bone,
                    id = associatedInteraction.id,
                    toolId = this.toolId,
                    hoveredObjectId = hoveredObjectId
                };
                cdk.UMI3DClientServer.SendData(eventdto, true);
                risingEdgeEventSent = true;
                IsInputHold = true;
            }
            else
            {
                var eventdto = new common.interaction.EventTriggeredDto
                {
                    boneType = bone,
                    id = associatedInteraction.id,
                    toolId = this.toolId,
                    hoveredObjectId = hoveredObjectId
                };
                cdk.UMI3DClientServer.SendData(eventdto, true);
            }
            if (associatedInteraction.TriggerAnimationId != 0)
                StartAnim(associatedInteraction.TriggerAnimationId);
        }

        protected override void PressedUp()
        {
            onInputUp.Invoke();
            if (associatedInteraction.ReleaseAnimationId != 0) StartAnim(associatedInteraction.ReleaseAnimationId);
            if (!associatedInteraction.hold || !risingEdgeEventSent) return;
            
            var eventdto = new common.interaction.EventStateChangedDto
            {
                active = false,
                boneType = bone,
                id = associatedInteraction.id,
                toolId = this.toolId,
                hoveredObjectId = hoveredObjectId
            };
            cdk.UMI3DClientServer.SendData(eventdto, true);
            IsInputHold = false;
            risingEdgeEventSent = false;
        }

        protected void StartAnim(ulong id)
        {
            var anim = UMI3DAbstractAnimation.Get(id);
            if (anim != null)
            {
                anim.SetUMI3DProperty(UMI3DEnvironmentLoader.GetEntity(id), new SetEntityPropertyDto()
                {
                    entityId = id,
                    property = UMI3DPropertyKeys.AnimationPlaying,
                    value = true
                });
                anim.Start();
            }
        }
    }
}