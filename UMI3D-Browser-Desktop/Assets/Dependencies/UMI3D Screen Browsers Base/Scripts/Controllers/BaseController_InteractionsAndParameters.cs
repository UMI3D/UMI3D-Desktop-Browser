/*
Copyright 2019 - 2023 Inetum

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
using umi3d.baseBrowser.inputs.interactions;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.baseBrowser.Controller
{
    public partial class BaseController
    {
        public override List<AbstractUMI3DInput> inputs
        {
            get
            {
                List<AbstractUMI3DInput> list = new List<AbstractUMI3DInput>();
                if (CurrentController != null) list.AddRange(CurrentController.Inputs);
                list.AddRange(EventInputs);
                list.AddRange(floatParameterInputs);
                list.AddRange(floatRangeParameterInputs);
                list.AddRange(intParameterInputs);
                list.AddRange(boolParameterInputs);
                list.AddRange(stringParameterInputs);
                list.AddRange(stringEnumParameterInputs);
                return list;
            }
        }

        protected List<BaseManipulationGroup> ManipulationGroupInputs = new List<BaseManipulationGroup>();
        protected List<EventInteraction> EventInputs = new List<EventInteraction>();
        protected List<FormInteraction> FormInputs = new List<FormInteraction>();
        protected List<LinkInteraction> LinkInputs = new List<LinkInteraction>();
        /// <summary>
        /// Instantiated float parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<parameters.FloatParameterInput> floatParameterInputs = new List<parameters.FloatParameterInput>();
        /// <summary>
        /// Instantiated float range parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<parameters.FloatRangeParameterInput> floatRangeParameterInputs = new List<parameters.FloatRangeParameterInput>();
        /// <summary>
        /// Instantiated int parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<parameters.IntParameterInput> intParameterInputs = new List<parameters.IntParameterInput>();
        /// <summary>
        /// Instantiated bool parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<parameters.BooleanParameterInput> boolParameterInputs = new List<parameters.BooleanParameterInput>();
        /// <summary>
        /// Instantiated string parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<parameters.StringParameterInput> stringParameterInputs = new List<parameters.StringParameterInput>();
        /// <summary>
        /// Instantiated string enum parameter inputs.
        /// </summary>
        /// <see cref="FindInput(AbstractParameterDto, bool)"/>
        protected List<parameters.StringEnumParameterInput> stringEnumParameterInputs = new List<parameters.StringEnumParameterInput>();

        #region Clear

        /// <summary>
        /// Clear <paramref name="inputs"/> and apply <paramref name="action"/> on each element of <paramref name="inputs"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputs"></param>
        /// <param name="action"></param>
        public static void ClearInputs<T>(ref List<T> inputs, Action<T> action)
            where T : AbstractUMI3DInput
        {
            inputs.ForEach(action);
            inputs = new List<T>();
        }

        /// <summary>
        /// Clear Interactions and Parameters Inputs.
        /// </summary>
        public override void Clear()
        {
            Action<AbstractUMI3DInput> action = (input) =>
            {
                input.Dissociate();
                Destroy(input);
            };

            CurrentController?.ClearInputs();
            ClearInputs(ref ManipulationGroupInputs, input =>
            {
                if (!input.IsAvailable()) input.Dissociate();
            });
            ClearInputs(ref EventInputs, action);
            ClearParameters(action);
        }
        protected void ClearParameters(Action<AbstractUMI3DInput> action)
        {
            ClearInputs(ref floatParameterInputs, action);
            ClearInputs(ref floatRangeParameterInputs, action);
            ClearInputs(ref intParameterInputs, action);
            ClearInputs(ref boolParameterInputs, action);
            ClearInputs(ref stringParameterInputs, action);
            ClearInputs(ref stringEnumParameterInputs, action);
        }
        
        #endregion

        #region Find Inputs

        protected AbstractUMI3DInput FindInput<T>(List<T> inputs, System.Predicate<T> predicate, GameObject gO = null) where T : AbstractUMI3DInput, new()
        {
            T input = inputs.Find(predicate);
            if (input == null) AddInput(inputs, out input, gO);
            return input;
        }
        protected void AddInput<T>(List<T> inputs, out T input, GameObject gO) where T : AbstractUMI3DInput, new()
        {
            if (gO != null) input = gO.AddComponent<T>();
            else input = new T();

            if (input is EventInteraction keyMenuInput) keyMenuInput.bone = interactionBoneType;
            else if (input is FormInteraction formInput) formInput.bone = interactionBoneType;
            else if (input is LinkInteraction linkInput) linkInput.bone = interactionBoneType;
            input.Menu = ObjectMenu.menu;
            inputs.Add(input);
        }

        #endregion

        #region Find Interactions
        
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="form"></param>
        /// <param name="unused"></param>
        /// <returns></returns>
        public override AbstractUMI3DInput FindInput(FormDto form, bool unused = true)
            => FindInput(FormInputs, i => i.IsAvailable() || !unused, EventActions);
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="link"></param>
        /// <param name="unused"></param>
        /// <returns></returns>
        public override AbstractUMI3DInput FindInput(LinkDto link, bool unused = true)
            => FindInput(LinkInputs, i => i.IsAvailable() || !unused, EventActions);
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="unused"></param>
        /// <param name="tryToFindInputForHoldableEvent"></param>
        /// <returns></returns>
        public override AbstractUMI3DInput FindInput(EventDto evt, bool unused = true, bool tryToFindInputForHoldableEvent = false)
        {
            AbstractUMI3DInput input = null;
            if (CurrentController != null) input = CurrentController?.FindInput(evt, unused, tryToFindInputForHoldableEvent);
            if (input == null) input = FindInput(EventInputs, i => i.IsAvailable() || !unused, EventActions);
            return input;
        }

        #endregion

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="param"></param>
        /// <param name="unused"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override AbstractUMI3DInput FindInput(AbstractParameterDto param, bool unused = true)
        {
            if (param is FloatRangeParameterDto) return FindInput(floatRangeParameterInputs, i => i.IsAvailable(), ParameterActions);
            else if (param is FloatParameterDto) return FindInput(floatParameterInputs, i => i.IsAvailable(), ParameterActions);
            else if (param is IntegerParameterDto) return FindInput(intParameterInputs, i => i.IsAvailable());
            else if (param is IntegerRangeParameterDto) throw new System.NotImplementedException();
            else if (param is BooleanParameterDto) return FindInput(boolParameterInputs, i => i.IsAvailable(), ParameterActions);
            else if (param is StringParameterDto) return FindInput(stringParameterInputs, i => i.IsAvailable(), ParameterActions);
            else if (param is EnumParameterDto<string>) return FindInput(stringEnumParameterInputs, i => i.IsAvailable(), ParameterActions);
            else return null;
        }

        #region Find Manipulations

        public override DofGroupOptionDto FindBest(DofGroupOptionDto[] options)
        {
            foreach (var GroupOption in options)
            {
                bool ok = true;
                foreach (DofGroupDto dof in GroupOption.separations)
                {
                    if (!BaseManipulationGroup.DofGroups.Contains(dof.dofs))
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok) return GroupOption;
            }

            throw new System.NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="manip"></param>
        /// <param name="dof"></param>
        /// <param name="unused"></param>
        /// <returns></returns>
        public override AbstractUMI3DInput FindInput(ManipulationDto manip, DofGroupDto dof, bool unused = true)
        {
            BaseManipulationGroup input = CurrentController.ManipulationGroup;

            if (input == null) UnityEngine.Debug.LogError($"Couln't find a manipulation group.");

            return input;
        }

        #endregion
    }
}
