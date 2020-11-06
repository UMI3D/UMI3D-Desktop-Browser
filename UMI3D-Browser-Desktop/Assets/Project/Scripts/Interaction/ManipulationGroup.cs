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
using BrowserDesktop.Cursor;
using BrowserDesktop.Menu;
using System.Collections.Generic;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine;

namespace BrowserDesktop.Interaction
{
    public class ManipulationGroup : AbstractUMI3DInput
    {
        bool active = false;
        ButtonMenuItem menuItem;
        string hoveredObjectId;
        #region Instances List

        static List<ManipulationGroup> instances = new List<ManipulationGroup>();
        static internal Dictionary<ManipulationGroup, List<ManipulationInput>> InputInstances = new Dictionary<ManipulationGroup, List<ManipulationInput>>();
        static int currentInstance;


        static public void NextManipulation()
        {
            SwicthManipulation(currentInstance + 1);
        }

        static public void PreviousManipulation()
        {
            SwicthManipulation(currentInstance - 1);
        }

        static void SwicthManipulation(int i)
        {
            if (instances.Count > 0)
            {
                if (currentInstance < instances.Count)
                    instances[currentInstance].Deactivate();
                currentInstance = i;
                if (currentInstance < 0)
                    currentInstance = instances.Count - 1;
                else if (currentInstance >= instances.Count)
                    currentInstance = 0;
                if (currentInstance < instances.Count)
                    instances[currentInstance].Activate();
            }
        }

        static public ManipulationGroup CurrentManipulationGroup { get { if (instances.Count > 0) return instances[currentInstance]; else return null; } }

        bool Active { get => active; set { active = value; } }

        internal void Activate()
        {
            Active = true;
            if (CircularMenu.Exists && menuItem != null)
                CircularMenu.Instance.menuDisplayManager.menu.Remove(menuItem);
            ManipulationInput.SelectFirst();
            foreach (ManipulationInput input in manipulationInputs)
            {
                input.DisplayDisplayer(true);
            }
        }
        internal void Deactivate()
        {
            Active = false;
            if (CircularMenu.Exists)
            {
                CircularMenu.Instance.menuDisplayManager.menu.Add(menuItem);
            }
            foreach (ManipulationInput input in manipulationInputs)
            {
                input.DisplayDisplayer(false);
                input.Deactivate();
            }
        }

        internal void Add()
        {
            if (!instances.Contains(this))
            {
                instances.Add(this);
                InputInstances.Add(this, new List<ManipulationInput>());
                menuItem = new ButtonMenuItem()
                {
                    Name = associatedInteraction.name,
                    toggle = false,
                };
                menuItem.Subscribe(Select);
                if (instances.Count == 1)
                {
                    currentInstance = 0;
                    Activate();
                }
                else
                {
                    Deactivate();
                }
            }
        }
        internal void Remove()
        {
            if (instances.Contains(this))
            {
                if (Active) PreviousManipulation();
                instances.Remove(this);
                InputInstances.Remove(this);
                if (instances.Count == 0)
                {
                    currentInstance = 0;
                    CursorHandler.State = CursorHandler.CursorState.Hover;
                }
                if (CircularMenu.Exists && menuItem != null)
                    CircularMenu.Instance.menuDisplayManager.menu.Remove(menuItem);
                if (menuItem != null)
                {
                    menuItem.UnSubscribe(Select);
                    menuItem = null;
                }
            }
        }

        private void OnDestroy()
        {
            Remove();
        }


        internal void Add(ManipulationInput input)
        {
            if (!InputInstances[this].Contains(input))
            {
                InputInstances[this].Add(input);
                if (active && InputInstances[this].Count == 1)
                {
                    currentInstance = 0;
                    input.Activate();
                }
                else
                {
                    input.Deactivate();
                }
                input.DisplayDisplayer(active);
            }
        }
        internal void Remove(ManipulationInput input)
        {
            if (InputInstances[this].Contains(input))
            {
                if (Active) PreviousManipulation();
                InputInstances[this].Remove(input);
                if (active && InputInstances[this].Count == 0)
                {
                    currentInstance = 0;
                    CursorHandler.State = CursorHandler.CursorState.Hover;
                }
            }
        }

        void Select(bool state)
        {
            SwicthManipulation(instances.FindIndex(a => a == this));
            if (CircularMenu.Exists)
                CircularMenu.Collapse();
        }

        #endregion


        ManipulationInputButton button;
        List<ManipulationInput> manipulationInputs = new List<ManipulationInput>();
        protected ManipulationDto associatedInteraction;
        [SerializeField]
        List<CursorKeyInput> Inputs = new List<CursorKeyInput>();
        [SerializeField]
        List<DofGroupEnum> dofGroups = new List<DofGroupEnum>();

        static public ManipulationGroup Instanciate(AbstractController controller, List<CursorKeyInput> Inputs, List<DofGroupEnum> dofGroups, Transform parent)
        {
            ManipulationGroup group = parent.gameObject.AddComponent<ManipulationGroup>();
            group.Inputs = Inputs;
            group.dofGroups = dofGroups;
            group.controller = controller;
            return group;
        }

        public override bool IsCompatibleWith(AbstractInteractionDto interaction)
        {
            return (interaction is ManipulationDto) &&
            (interaction as ManipulationDto).dofSeparationOptions.Exists((sep) => { 
                foreach(DofGroupDto dof in sep.separations)
                {
                    if (!dofGroups.Contains(dof.dofs))
                        return false;
                }
                return true;
            });
        }

        public override bool IsAvailable()
        {
            return associatedInteraction == null && Inputs.Exists(activationButton => (activationButton.IsAvailable() || activationButton.Locked));
        }

        public bool IsAvailableFor(ManipulationDto manipulation)
        {
            return manipulation == associatedInteraction ||IsAvailable();
        }

        public override AbstractInteractionDto CurrentInteraction()
        {
            return associatedInteraction;
        }

        public override void Associate(AbstractInteractionDto interaction, string toolId, string hoveredObjectId)
        {
            if (associatedInteraction != null)
            {
                throw new System.Exception("This input is already binded to a interaction ! (" + associatedInteraction + ")");
            }
            if (IsCompatibleWith(interaction))
            {
                this.hoveredObjectId = hoveredObjectId;
                foreach (DofGroupOptionDto group in (interaction as ManipulationDto).dofSeparationOptions)
                {
                    bool ok = true;
                    foreach (DofGroupDto sep in group.separations)
                    {
                        if (!dofGroups.Contains(sep.dofs))
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (!ok) continue;
                    foreach (DofGroupDto sep in group.separations)
                    {
                        Associate(interaction as ManipulationDto, sep.dofs, toolId, hoveredObjectId);
                    }
                    return;
                }
            }
            else
            {
                throw new System.Exception("Trying to associate an uncompatible interaction !");
            }
        }

        public override void Associate(ManipulationDto manipulation, DofGroupEnum dofs, string toolId, string hoveredObjectId)
        {
            if ((associatedInteraction == null || associatedInteraction == manipulation) && dofGroups.Contains(dofs))
            {
                associatedInteraction = manipulation;
                this.hoveredObjectId = hoveredObjectId;
                Add();
                ManipulationInput input = ManipulationInputGenerator.Instanciate(controller, Inputs.Find((a) => (a.IsAvailable() || a.Locked)), dofs, ref manipulationInputs);
                input.Associate(manipulation, dofs, toolId, hoveredObjectId);
                Add(input);
            }
            else
            {
                throw new System.Exception("Trying to associate an uncompatible interaction !");
            }
        }

        public override void Dissociate()
        {
            foreach(ManipulationInput input in manipulationInputs)
            {
                input.Dissociate();
                Remove(input);
                Destroy(input);
            }
            Remove();
            manipulationInputs.Clear();
            associatedInteraction = null;
        }

        public override void UpdateHoveredObjectId(string hoveredObjectId)
        {
            this.hoveredObjectId = hoveredObjectId;
            foreach(var input in InputInstances[this])
            {
                input.UpdateHoveredObjectId(hoveredObjectId);
            }
        }
    }
}