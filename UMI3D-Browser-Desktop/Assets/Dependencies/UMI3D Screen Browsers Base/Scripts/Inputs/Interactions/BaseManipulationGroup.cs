/*
Copyright 2019 - 2022 Inetum

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
using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.baseBrowser.inputs.interactions
{
    public class BaseManipulationGroup : BaseInteraction<ManipulationDto>
    {
        [SerializeField]
        protected List<BaseInteraction<EventDto>> Inputs = new List<BaseInteraction<EventDto>>();
        [SerializeField]
        protected List<DofGroupEnum> dofGroups = new List<DofGroupEnum>();

        protected ButtonMenuItem menuItem;
        protected bool Active;
        protected List<BaseManipulation> manipulationInputs = new List<BaseManipulation>();

        #region Instances List

        protected static List<BaseManipulationGroup> instances = new List<BaseManipulationGroup>();
        protected static Dictionary<BaseManipulationGroup, List<BaseManipulation>> InputInstances = new Dictionary<BaseManipulationGroup, List<BaseManipulation>>();
        protected static int currentInstance;

        public static BaseManipulationGroup CurrentManipulationGroup
            => instances.Count > 0 ? instances[currentInstance] : null;

        public static BaseManipulationGroup Instanciate(AbstractController controller, List<BaseInteraction<EventDto>> Inputs, List<DofGroupEnum> dofGroups, Transform parent)
        {
            BaseManipulationGroup group = parent.gameObject.AddComponent<BaseManipulationGroup>();
            group.Inputs = Inputs;
            group.dofGroups = dofGroups;
            group.controller = controller;
            return group;
        }

        public static List<BaseManipulation> GetManipulations()
        {
            if
            (
                CurrentManipulationGroup == null
                || InputInstances == null
                || !InputInstances.ContainsKey(CurrentManipulationGroup)
            )
                return null;

            return InputInstances[CurrentManipulationGroup];
        }

        #endregion

        public override void Associate(AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
        {
            if (associatedInteraction != null)
                throw new System.Exception("This input is already binded to a interaction ! (" + associatedInteraction + ")");

            if (!IsCompatibleWith(interaction))
                throw new System.Exception("Trying to associate an uncompatible interaction !");

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

        public override void Associate(ManipulationDto manipulation, DofGroupEnum dofs, ulong toolId, ulong hoveredObjectId)
        {
            if 
            (
                (associatedInteraction != null && associatedInteraction != manipulation) 
                || !dofGroups.Contains(dofs)
            )
                throw new System.Exception("Trying to associate an uncompatible interaction !");

            associatedInteraction = manipulation;
            this.hoveredObjectId = hoveredObjectId;
            Add();
            UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"instanciate with a.locked ?");
            BaseManipulation input = ManipulationInputGenerator.Instanciate(controller, Inputs.Find(a => a.IsAvailable()), dofs, ref manipulationInputs);
            input.bone = bone;
            input.Associate(manipulation, dofs, toolId, hoveredObjectId);
            Add(input);
        }

        public override void Dissociate()
        {
            foreach (BaseManipulation input in manipulationInputs)
            {
                input.Dissociate();
                Remove(input);
                Destroy(input);
            }
            Remove();
            manipulationInputs.Clear();
            associatedInteraction = null;
        }

        public override bool IsAvailable()
            => base.IsAvailable() && Inputs.Exists(activationButton => activationButton.IsAvailable());

        public bool IsAvailableFor(ManipulationDto manipulation)
            => manipulation == associatedInteraction || IsAvailable();

        public override bool IsCompatibleWith(AbstractInteractionDto interaction)
        {
            if (!(interaction is ManipulationDto manipulationDto)) return false;

            return manipulationDto.dofSeparationOptions.Exists
            (
                (sep) =>
                {
                    foreach (DofGroupDto dof in sep.separations)
                    {
                        if (!dofGroups.Contains(dof.dofs)) return false;
                    }
                    return true;
                }
            );
        }

        public override void UpdateHoveredObjectId(ulong hoveredObjectId)
        {
            base.UpdateHoveredObjectId(hoveredObjectId);
            foreach (var input in InputInstances[this]) input.UpdateHoveredObjectId(hoveredObjectId);
        }

        public static void NextManipulation() => SwicthManipulation(currentInstance + 1);

        public static void PreviousManipulation() => SwicthManipulation(currentInstance - 1);

        protected static void SwicthManipulation(int i)
        {
            if (instances.Count == 0) return;

            if (currentInstance < instances.Count) instances[currentInstance].Deactivate();

            currentInstance = i;
            if (currentInstance < 0) currentInstance = instances.Count - 1;
            else if (currentInstance >= instances.Count) currentInstance = 0;

            instances[currentInstance].Activate();
        }

        protected void Add()
        {
            if (instances.Contains(this)) return;

            instances.Add(this);
            InputInstances.Add(this, new List<BaseManipulation>());
            menuItem = new ButtonMenuItem()
            {
                Name = associatedInteraction.name
            };
            menuItem.Subscribe(Select);
            if (instances.Count == 1)
            {
                currentInstance = 0;
                Activate();
            }
            else Deactivate();
        }

        protected void Remove()
        {
            if (!instances.Contains(this)) return;

            if (Active) PreviousManipulation();
            instances.Remove(this);
            InputInstances.Remove(this);
            if (instances.Count == 0)
            {
                currentInstance = 0;
                //umi3d.baseBrowser.Controller.BaseCursor.State = umi3d.baseBrowser.Controller.BaseCursor.CursorState.Hover;
            } 
            if (menuItem != null)
            {
                Menu.Remove(menuItem);
                menuItem.UnSubscribe(Select);
                menuItem = null;
            }
        }

        protected void Activate()
        {
            Active = true;
            if (menuItem != null) Menu.Remove(menuItem);
            BaseManipulation.SelectFirst();
            UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"displayer");
            //foreach (BaseManipulation mainipulation in manipulationInputs)
            //    mainipulation.DisplayDisplayer(true);
        }
        protected void Deactivate()
        {
            Active = false;
            Menu.Add(menuItem);
            foreach (BaseManipulation input in manipulationInputs)
            {
                UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"displayer");
                //input.DisplayDisplayer(false);
                input.Deactivate();
            }
        }

        protected void Select() => SwicthManipulation(instances.FindIndex(a => a == this));

        protected void Add(BaseManipulation input)
        {
            if (InputInstances[this].Contains(input)) return;

            InputInstances[this].Add(input);
            if (Active && InputInstances[this].Count == 1)
            {
                currentInstance = 0;
                input.Activate();
            }
            else input.Deactivate();
            UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"displayer");
            //input.DisplayDisplayer(active);
        }

        protected void Remove(BaseManipulation input)
        {
            if (!InputInstances[this].Contains(input)) return;

            if (Active) PreviousManipulation();
            InputInstances[this].Remove(input);
            if (Active && InputInstances[this].Count == 0)
            {
                currentInstance = 0;
                //umi3d.baseBrowser.Controller.BaseCursor.State = umi3d.baseBrowser.Controller.BaseCursor.CursorState.Hover;
            }
        }
    }
}
