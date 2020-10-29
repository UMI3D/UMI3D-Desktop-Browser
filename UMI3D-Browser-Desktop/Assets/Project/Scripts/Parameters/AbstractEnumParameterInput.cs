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
using umi3d.cdk.menu;
using umi3d.common.interaction;

namespace BrowserDesktop.Parameters
{
    public abstract class AbstractEnumParameterInput<InputMenuItem, ValueType> : AbstractParameterInput<InputMenuItem, EnumParameterDto<ValueType>, ValueType>
        where InputMenuItem : AbstractEnumInputMenuItem<ValueType>, new()
    {
        public override void Associate(AbstractInteractionDto interaction, string toolId, string hoveredObjectId)
        {
            if (currentInteraction != null)
            {
                throw new System.Exception("This input is already associated to another interaction (" + currentInteraction + ")");
            }

            if (interaction is EnumParameterDto<ValueType>)
            {
                EnumParameterDto<ValueType> stringEnum = interaction as EnumParameterDto<ValueType>;

                callback = newValue =>
                {
                    var dto = interaction as EnumParameterDto<ValueType>;
                    dto.value = newValue;
                    var pararmeterDto = new ParameterSettingRequestDto()
                    {
                        id = currentInteraction.id,
                        toolId = toolId,
                        parameter = dto,
                        hoveredObjectId = hoveredObjectId
                    };
                    UMI3DCollaborationClientServer.Send(pararmeterDto, true);
                };

                menuItem = new InputMenuItem()
                {
                    dto = stringEnum,
                    Name = interaction.name,
                    options = stringEnum.possibleValues
                };

                menuItem.NotifyValueChange(stringEnum.value);
                menuItem.Subscribe(callback);
                if (CircularMenu.Exists)
                    CircularMenu.Instance.menuDisplayManager.menu.Add(menuItem);
                currentInteraction = interaction;
            }
            else
            {
                throw new System.Exception("Incompatible interaction");
            }
        }
    }
}