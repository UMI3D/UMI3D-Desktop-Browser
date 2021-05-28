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
using BrowserDesktop.Controller;
using BrowserDesktop.Menu;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using umi3d.common.userCapture;

[System.Serializable]
public class KeyMenuInput : AbstractUMI3DInput
{

    /// <summary>
    /// Associtated interaction (if any).
    /// </summary>
    public EventDto associatedInteraction { get; protected set; }
    /// <summary>
    /// Avatar bone linked to this input.
    /// </summary>
    public string bone = BoneType.RightHand;

    ulong toolId;
    ulong hoveredObjectId;

    bool risingEdgeEventSent;

    HoldableButtonMenuItem menuItem;

    public override void Associate(AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
    {
        if (associatedInteraction != null)
        {
            throw new System.Exception("This input is already binded to a interaction ! (" + associatedInteraction + ")");
        }

        if (IsCompatibleWith(interaction))
        {
            this.hoveredObjectId = hoveredObjectId;
            this.toolId = toolId;
            associatedInteraction = interaction as EventDto;
            menuItem = new HoldableButtonMenuItem
            {
                Name = associatedInteraction.name,
                Holdable = associatedInteraction.hold
            };
            menuItem.Subscribe(Pressed);
            if (CircularMenu.Exists)
            {
                CircularMenu.Instance.menuDisplayManager.menu.Add(menuItem);
            }
        }
        else
        {
            throw new System.Exception("Trying to associate an uncompatible interaction !");
        }
    }

    public override void Associate(ManipulationDto manipulation, DofGroupEnum dofs, ulong toolId, ulong hoveredObjectId)
    {
        throw new System.Exception("This input is can not be associated with a manipulation");
    }

    public override AbstractInteractionDto CurrentInteraction()
    {
        return associatedInteraction;
    }

    public override void Dissociate()
    {
        associatedInteraction = null;
        if (CircularMenu.Exists && menuItem != null)
        {
            CircularMenu.Instance.menuDisplayManager.menu.Remove(menuItem);
        }
        menuItem.UnSubscribe(Pressed);
        menuItem = null;
    }

    public override bool IsAvailable()
    {
        return associatedInteraction == null;
    }

    public override bool IsCompatibleWith(AbstractInteractionDto interaction)
    {
        return interaction is EventDto;
    }

    void Pressed(bool down)
    {
        if (down)
        {
            onInputDown.Invoke();
            if ((associatedInteraction).hold)
            {
                var eventdto = new EventStateChangedDto
                {
                    active = true,
                    boneType = bone,
                    id = associatedInteraction.id,
                    toolId = this.toolId,
                    hoveredObjectId = hoveredObjectId
                };
                UMI3DClientServer.SendData(eventdto, true);
                risingEdgeEventSent = true;
                MouseAndKeyboardController.isInputHold = true;
            }
            else
            {
                var eventdto = new EventTriggeredDto
                {
                    boneType = bone,
                    id = associatedInteraction.id,
                    toolId = this.toolId,
                    hoveredObjectId = hoveredObjectId
                };
                UMI3DClientServer.SendData(eventdto, true);
            }
        }
        else
        {
            onInputUp.Invoke();
            if ((associatedInteraction).hold)
            {
                if (risingEdgeEventSent)
                {
                    var eventdto = new EventStateChangedDto
                    {
                        active = false,
                        boneType = bone,
                        id = associatedInteraction.id,
                        toolId = this.toolId,
                        hoveredObjectId = hoveredObjectId
                    };
                    UMI3DClientServer.SendData(eventdto, true);
                    risingEdgeEventSent = false;
                    MouseAndKeyboardController.isInputHold = false;
                }
            }
        }
    }

    public override void UpdateHoveredObjectId(ulong hoveredObjectId)
    {
        this.hoveredObjectId = hoveredObjectId;
    }
}