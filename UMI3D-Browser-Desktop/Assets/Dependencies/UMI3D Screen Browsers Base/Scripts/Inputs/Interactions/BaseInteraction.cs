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

using UnityEngine;

namespace umi3d.baseBrowser.inputs.interactions
{
    public abstract class BaseInteraction<InteractionType> : cdk.interaction.AbstractUMI3DInput, IInteractionWithBone
        where InteractionType : common.interaction.AbstractInteractionDto
    {
        /// <summary>
        /// Associtated interaction (if any).
        /// </summary>
        public InteractionType associatedInteraction { get; protected set; }
        public override common.interaction.AbstractInteractionDto CurrentInteraction() => associatedInteraction;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Transform boneTransform { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public uint bone { get; set; }


        protected ulong toolId, hoveredObjectId;
        public override void UpdateHoveredObjectId(ulong hoveredObjectId) => this.hoveredObjectId = hoveredObjectId;

        public override void Associate(common.interaction.ManipulationDto manipulation, common.interaction.DofGroupEnum dofs, ulong toolId, ulong hoveredObjectId)
            => throw new System.Exception("This input is can not be associated with a manipulation");
        public override bool IsAvailable() => associatedInteraction == null;
        public override bool IsCompatibleWith(common.interaction.AbstractInteractionDto interaction) => interaction is InteractionType;
    }
}
