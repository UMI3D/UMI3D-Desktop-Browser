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

using UnityEngine;

namespace umi3d.baseBrowser.inputs.interactions
{
    [System.Serializable]
    public class LinkInput : BaseTouchInteraction<common.interaction.LinkDto>
    {
        protected void Awake()
        {
            bone = common.userCapture.BoneType.RightHand;
        }

        protected override void CreateMenuItem()
        {
            menuItem = new ButtonMenuItem
            {
                Name = associatedInteraction.name,
                IsHoldable = false
            };
        }

        protected override void PressedDown()
        {
            onInputDown.Invoke();
            Application.OpenURL(associatedInteraction.url);
            var formAnswer = new common.interaction.LinkOpened()
            {
                boneType = bone,
                id = associatedInteraction.id,
                toolId = this.toolId,
                hoveredObjectId = hoveredObjectId,
            };
            cdk.UMI3DClientServer.SendData(formAnswer, true);
        }

        protected override void PressedUp()
        {
            throw new System.NotImplementedException();
        }
    }
}