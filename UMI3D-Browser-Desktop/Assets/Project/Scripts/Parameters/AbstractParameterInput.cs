/*
Copyright 2019 Gfi Informatique

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
using umi3d.cdk.collaboration;
using umi3d.cdk.interaction;
using umi3d.cdk.menu;
using umi3d.common.interaction;
using UnityEngine.Events;

namespace BrowserDesktop.Parameters
{
    public abstract class AbstractParameterInput<InputMenuItem, ParameterType, ValueType> : AbstractUMI3DInput
        where InputMenuItem : AbstractInputMenuItem<ValueType>, new()
        where ParameterType : AbstractParameterDto<ValueType>

    {
        /// <summary>
        /// Associated menu item.
        /// </summary>
        public InputMenuItem menuItem;


        /// <summary>
        /// Interaction currently associated to this input.
        /// </summary>
        protected AbstractInteractionDto currentInteraction;

        /// <summary>
        /// Associated callback
        /// </summary>
        /// <see cref="Associate(AbstractInteractionDto)"/>
        protected UnityAction<ValueType> callback;

        protected string hoveredObjectId { get; private set; }

        protected string GetCurrentHoveredObjectID() { return hoveredObjectId; }


        public override void Associate(AbstractInteractionDto interaction, string toolId, string hoveredObjectId)
        {
            if (currentInteraction != null)
            {
                throw new System.Exception("This input is already associated to another interaction (" + currentInteraction + ")");
            }

            if (interaction is ParameterType)
            {
                this.hoveredObjectId = hoveredObjectId;
                menuItem = new InputMenuItem()
                {
                    dto = interaction as ParameterType,
                    Name = interaction.name
                };
                if (CircleMenu.Exists)
                    CircleMenu.Instance.MenuDisplayManager.menu.Add(menuItem);

                menuItem.NotifyValueChange((interaction as ParameterType).value);
                callback = x =>
                {
                    var dto = menuItem.dto;
                    dto.value = x;
                    var pararmeterDto = new ParameterSettingRequestDto()
                    {
                        id = currentInteraction.id,
                        toolId = toolId,
                        parameter = dto,
                        hoveredObjectId = GetCurrentHoveredObjectID()
                    };
                    UMI3DCollaborationClientServer.Send(pararmeterDto, true);
                };

                menuItem.Subscribe(callback);
                currentInteraction = interaction;
            }
            else
            {
                throw new System.Exception("Incompatible interaction");
            }
        }


        public override void Associate(ManipulationDto manipulation, DofGroupEnum dofs, string toolId, string hoveredObjectId)
        {
            throw new System.Exception("Incompatible interaction");
        }

        public override AbstractInteractionDto CurrentInteraction()
        {
            return currentInteraction;
        }

        public override void Dissociate()
        {
            currentInteraction = null;
            menuItem.UnSubscribe(callback);
            if (CircleMenu.Exists)
                CircleMenu.Instance?.MenuDisplayManager?.menu?.Remove(menuItem);
        }

        public override bool IsCompatibleWith(AbstractInteractionDto interaction)
        {
            return interaction is ParameterType;
        }

        public override bool IsAvailable()
        {
            return currentInteraction == null;
        }

        private void OnDestroy()
        {
            Dissociate();
        }

        public override void UpdateHoveredObjectId(string hoveredObjectId)
        {
            this.hoveredObjectId = hoveredObjectId;
        }
    }
}