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

namespace umi3d.baseBrowser.inputs.interactions
{
    public abstract class BaseTouchInteraction<InteractionType> : BaseInteraction<InteractionType>
        where InteractionType : common.interaction.AbstractInteractionDto
    {
        protected ButtonMenuItem menuItem;
        protected bool m_isDown;

        public override void Dissociate()
        {
            if (m_isDown) Pressed(false);
            associatedInteraction = null;
            environmentId = 0;
            Menu?.Remove(menuItem);
            menuItem?.UnSubscribe(Pressed);
            menuItem = null;
        }

        public override void Associate(ulong environmentId, common.interaction.AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
        {
            if (associatedInteraction != null) throw new System.Exception("This input is already binded to a interaction ! (" + associatedInteraction + ")");
            
            if (!IsCompatibleWith(interaction)) throw new System.Exception("Trying to associate an uncompatible interaction !");
            
            this.hoveredObjectId = hoveredObjectId;
            this.toolId = toolId;
            associatedInteraction = interaction as InteractionType;
            this.environmentId = environmentId;
            CreateMenuItem();
            menuItem.Subscribe(Pressed);
            Menu?.Add(menuItem);
        }

        /// <summary>
        /// Set the state of the touch to not pressed.
        /// </summary>
        public void ResetTouchInteraction() => m_isDown = false;

        protected void Pressed(bool down)
        {
            m_isDown = down;
            if (down) PressedDown();
            else PressedUp();
        }

        protected abstract void CreateMenuItem();
        protected abstract void PressedDown();
        protected abstract void PressedUp();
    }
}
