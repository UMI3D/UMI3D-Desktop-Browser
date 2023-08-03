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

using System;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.common;
using UnityEngine;

namespace umi3d.baseBrowser.parameters
{
    public abstract class BaseParameter<InputMenuItem, ParameterType, ValueType> : umi3d.cdk.interaction.AbstractUMI3DInput, IInteractionWithBone
        where InputMenuItem : umi3d.cdk.menu.AbstractInputMenuItem<ValueType>, new()
        where ParameterType : umi3d.common.interaction.AbstractParameterDto<ValueType>
    {
        /// <summary>
        /// Associated menu item.
        /// </summary>
        public InputMenuItem menuItem;
        /// <summary>
        /// Interaction currently associated to this input.
        /// </summary>
        protected common.interaction.AbstractInteractionDto currentInteraction;
        public override common.interaction.AbstractInteractionDto CurrentInteraction()
        {
            return currentInteraction;
        }
        /// <summary>
        /// Associated callback
        /// </summary>
        /// <see cref="Associate(AbstractInteractionDto)"/>
        protected Action<ValueType> callback;

        protected ulong hoveredObjectId { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Transform boneTransform { get; set; }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public uint bone { get; set; }

        protected ulong GetCurrentHoveredObjectID() => hoveredObjectId;

        private void OnDestroy() => Dissociate();
        public override void Dissociate()
        {
            currentInteraction = null;
            menuItem.UnSubscribe(callback);
            Menu?.Remove(menuItem);
        }
        public override void Associate(common.interaction.AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
        {
            if (currentInteraction != null)
                throw new System.Exception("This input is already associated to another interaction (" + currentInteraction + ")");

            if (!IsCompatibleWith(interaction)) throw new System.Exception("Incompatible interaction");

            this.hoveredObjectId = hoveredObjectId;
            menuItem = new InputMenuItem()
            {
                dto = interaction as ParameterType,
                Name = interaction.name
            };

            menuItem.NotifyValueChange((interaction as ParameterType).value);
            callback = x =>
            {
                var dto = menuItem.dto;
                dto.value = x;
                var pararmeterDto = new common.interaction.ParameterSettingRequestDto()
                {
                    id = currentInteraction.id,
                    toolId = toolId,
                    parameter = dto,
                    hoveredObjectId = GetCurrentHoveredObjectID(),
                    boneType = bone,
                    bonePosition = (Vector3Dto)boneTransform.position.Dto(),
                    boneRotation = (Vector4Dto)boneTransform.rotation.Dto()
                };
                umi3d.cdk.UMI3DClientServer.SendData(pararmeterDto, true);
            };

            menuItem.Subscribe(callback);
            currentInteraction = interaction;
            Menu?.Add(menuItem);
        }
        public override void Associate(common.interaction.ManipulationDto manipulation, common.interaction.DofGroupEnum dofs, ulong toolId, ulong hoveredObjectId)
            => throw new System.Exception("Incompatible interaction");
        public override bool IsCompatibleWith(common.interaction.AbstractInteractionDto interaction) => interaction is ParameterType;
        public override bool IsAvailable() => currentInteraction == null;
        public override void UpdateHoveredObjectId(ulong hoveredObjectId)
        {
            this.hoveredObjectId = hoveredObjectId;
        }
    }
}
