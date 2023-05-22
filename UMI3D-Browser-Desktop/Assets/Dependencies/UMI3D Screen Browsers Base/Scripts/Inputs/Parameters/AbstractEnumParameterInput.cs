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

using umi3d.common;

namespace umi3d.baseBrowser.parameters
{
    public abstract class AbstractEnumParameterInput<InputMenuItem, ValueType> : BaseParameter<InputMenuItem, common.interaction.EnumParameterDto<ValueType>, ValueType>
        where InputMenuItem : cdk.menu.AbstractEnumInputMenuItem<ValueType>, new()
    {
        public override void Associate(common.interaction.AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
        {
            if (currentInteraction != null)
                throw new System.Exception("This input is already associated to another interaction (" + currentInteraction + ")");

            if (!IsCompatibleWith(interaction)) throw new System.Exception("Incompatible interaction");

            var stringEnum = interaction as common.interaction.EnumParameterDto<ValueType>;

            callback = newValue =>
            {
                var dto = stringEnum;
                dto.value = newValue;
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
                cdk.UMI3DClientServer.SendData(pararmeterDto, true);
            };

            menuItem = new InputMenuItem()
            {
                dto = stringEnum,
                Name = interaction.name,
                options = stringEnum.possibleValues
            };

            menuItem.NotifyValueChange(stringEnum.value);
            menuItem.Subscribe(callback);
            Menu?.Add(menuItem);
            currentInteraction = interaction;
        }
    }
}