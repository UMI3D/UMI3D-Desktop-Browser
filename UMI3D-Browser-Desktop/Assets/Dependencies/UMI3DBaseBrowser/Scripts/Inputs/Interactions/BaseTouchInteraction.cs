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
        protected HoldableButtonMenuItem menuItem;

        public override void Dissociate()
        {
            associatedInteraction = null;
            Menu?.Remove(menuItem);
            menuItem.UnSubscribe(Pressed);
            menuItem = null;
        }

        public override void Associate(common.interaction.AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
        {
            if (associatedInteraction != null) throw new System.Exception("This input is already binded to a interaction ! (" + associatedInteraction + ")");
            
            if (!IsCompatibleWith(interaction)) throw new System.Exception("Trying to associate an uncompatible interaction !");

            this.hoveredObjectId = hoveredObjectId;
            this.toolId = toolId;
            associatedInteraction = interaction as InteractionType;
            CreateMenuItem();
            menuItem.Subscribe(Pressed);
            Menu?.Add(menuItem);
        }
        
        private void Pressed(bool down)
        {
            if (down) PressedDown();
            else PressedUp();
        }

        protected abstract void CreateMenuItem();
        protected abstract void PressedDown();
        protected abstract void PressedUp();
    }
}
