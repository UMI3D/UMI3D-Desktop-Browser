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
    /// <summary>
    /// Group of manipulations.
    /// </summary>
    public class BaseManipulationGroup : BaseInteraction<ManipulationDto>
    {
        /// <summary>
        /// List of inputs that will be used to manipulate an interactable.
        /// </summary>
        [SerializeField]
        protected List<BaseInteraction<EventDto>> m_inputs = new List<BaseInteraction<EventDto>>();
        [SerializeField]
        public static List<DofGroupEnum> DofGroups = new List<DofGroupEnum>
        {
            DofGroupEnum.XY,
            DofGroupEnum.XZ,
            DofGroupEnum.YZ,

            DofGroupEnum.X,
            DofGroupEnum.Y,
            DofGroupEnum.Z,

            DofGroupEnum.RX,
            DofGroupEnum.RY,
            DofGroupEnum.RZ
        };

        #region Static methods and properties

        #region Instances

        protected static List<BaseManipulationGroup> s_instances = new List<BaseManipulationGroup>();
        protected static Dictionary<BaseManipulationGroup, List<BaseManipulation>> s_manipulationsByGroup = new Dictionary<BaseManipulationGroup, List<BaseManipulation>>();
        protected static int s_currentIndex;

        /// <summary>
        /// Current manipulation group.
        /// </summary>
        public static BaseManipulationGroup CurrentGroup
            => s_instances.Count > 0 ? s_instances[s_currentIndex] : null;

        public static BaseManipulationGroup Instanciate
        (
            AbstractController controller,
            List<BaseInteraction<EventDto>> Inputs,
            GameObject gO
        )
        {
            BaseManipulationGroup group = gO.AddComponent<BaseManipulationGroup>();
            group.Init(controller);
            group.m_inputs = Inputs;
            return group;
        }

        /// <summary>
        /// Current list of manipulations associated with the <see cref="CurrentGroup"/>
        /// </summary>
        public static List<BaseManipulation> CurrentManipulations
            => CurrentGroup == null
                || s_manipulationsByGroup == null
                || !s_manipulationsByGroup.ContainsKey(CurrentGroup)
            ? null
            : s_manipulationsByGroup[CurrentGroup];

        #endregion

        #region Incrementation and decrementation

        /// <summary>
        /// Deactivate current group and activate next one.
        /// </summary>
        public static void NextGroup() => SwicthGroup(s_currentIndex + 1);

        /// <summary>
        /// Deactivate current group and activate previous one.
        /// </summary>
        public static void PreviousGroup() => SwicthGroup(s_currentIndex - 1);

        protected static void SwicthGroup(int i)
        {
            if (s_instances.Count == 0) return;

            if (s_currentIndex < s_instances.Count && s_currentIndex >= 0) s_instances[s_currentIndex].Deactivate();

            if (s_instances.Count == 1 && i != 1)
            {
                s_currentIndex = -1;
                return;
            }

            if (i < 0) s_currentIndex = s_instances.Count - 1;
            else if (i >= s_instances.Count) s_currentIndex = 0;

            s_instances[s_currentIndex].Activate();
        }

        #endregion

        #endregion

        #region Activation, Deactivation, Select

        /// <summary>
        /// Whether or not this group is active.
        /// </summary>
        public bool IsActive { get => m_isActive; protected set => m_isActive = value; }
        [SerializeField]
        [Header("Do not update this value in the inspector.")]
        private bool m_isActive;

        protected void Activate()
        {
            IsActive = true;
            BaseManipulation.SelectFirst();
        }
        protected void Deactivate()
        {
            IsActive = false;
            
            foreach (BaseManipulation input in s_manipulationsByGroup[this]) input.Deactivate();
        }

        protected void Select() => SwicthGroup(s_instances.FindIndex(a => a == this));

        #endregion

        #region Associate

        public override void Associate(AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
        {
            UnityEngine.Debug.Log($"pommmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmme");
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
                    if (!DofGroups.Contains(sep.dofs))
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
            UnityEngine.Debug.Log("<color=blue>TODO: </color>" + $"associate groupe");
            if (!IsAvailableFor(manipulation)) throw new System.Exception($"This input is not available for {manipulation}");

            if (!IsCompatibleWith(manipulation)) throw new System.Exception("Trying to associate an uncompatible interaction !");

            AddGroup();

            this.hoveredObjectId = hoveredObjectId;
            this.toolId = toolId;
            associatedInteraction = manipulation;

            BaseManipulation input = ManipulationInputGenerator.Instanciate(controller, m_inputs.Find(a => a.IsAvailable()), dofs);
            input.Menu = Menu;
            input.bone = bone;
            input.Associate(manipulation, dofs, toolId, hoveredObjectId);
            input.Deactivate();
            AddManipulation(input);

            if (s_instances.Count == 1)
            {
                s_currentIndex = 0;
                Activate();
            }
            else Deactivate();
        }

        protected void AddGroup()
        {
            if (s_instances.Contains(this)) return;

            s_instances.Add(this);
            s_manipulationsByGroup.Add(this, new List<BaseManipulation>());
        }

        protected void AddManipulation(BaseManipulation input)
        {
            if (s_manipulationsByGroup[this].Contains(input)) return;

            s_manipulationsByGroup[this].Add(input);
        }

        #endregion

        #region Dissociate

        public override void Dissociate()
        {
            UnityEngine.Debug.Log("<color=orange>TODO: </color>" + $"dissociate groupe, {associatedInteraction == null}");
            if (IsActive) PreviousGroup();

            var manipulations = s_manipulationsByGroup[this];
            for (int i = manipulations.Count - 1; i >= 0; i--)
            {
                var input = manipulations[i];
                input.Dissociate();
                RemoveManipulation(input);
                Destroy(input);
            }

            RemoveGroup();
            associatedInteraction = null;
        }

        protected void RemoveGroup()
        {
            if (!s_instances.Contains(this)) return;

            s_instances.Remove(this);
            s_manipulationsByGroup.Remove(this);
        }

        protected void RemoveManipulation(BaseManipulation input)
        {
            if (!s_manipulationsByGroup[this].Contains(input)) return;

            s_manipulationsByGroup[this].Remove(input);
        }

        #endregion



        public override bool IsAvailable()
            => base.IsAvailable() && m_inputs.Exists(activationButton => activationButton.IsAvailable());

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
                        if (!DofGroups.Contains(dof.dofs)) return false;
                    }
                    return true;
                }
            );
        }

        public override void UpdateHoveredObjectId(ulong hoveredObjectId)
        {
            base.UpdateHoveredObjectId(hoveredObjectId);
            foreach (var input in s_manipulationsByGroup[this]) input.UpdateHoveredObjectId(hoveredObjectId);
        }
    }
}
