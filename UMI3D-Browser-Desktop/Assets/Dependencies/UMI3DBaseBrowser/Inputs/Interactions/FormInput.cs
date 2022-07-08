﻿/*
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

using System.Linq;

namespace umi3d.baseBrowser.inputs.interactions
{
    [System.Serializable]
    public class FormInput : BaseTouchInteraction<common.interaction.FormDto>
    {
        protected override void CreateMenuItem()
        {
            menuItem = new HoldableButtonMenuItem
            {
                Name = associatedInteraction.name,
                Holdable = false
            };
        }

        protected override void PressedDown()
        {
            onInputDown.Invoke();

            var formAnswerDto = new common.interaction.FormAnswerDto
            {
                boneType = bone,
                id = associatedInteraction.id,
                toolId = this.toolId,
                answers = associatedInteraction
                    .fields
                    .Select(a => new common.interaction.ParameterSettingRequestDto()
                    {
                        toolId = this.toolId, 
                        id = a.id, 
                        boneType = bone, 
                        hoveredObjectId = hoveredObjectId, 
                        parameter = a.GetValue() 
                    }).ToList(),
                hoveredObjectId = hoveredObjectId
            };
            cdk.collaboration.UMI3DCollaborationClientServer.SendData(formAnswerDto, true);
        }

        protected override void PressedUp()
        {
            throw new System.NotImplementedException();
        }
    }
}
