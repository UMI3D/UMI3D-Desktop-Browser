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
using System.Collections;
using System.Collections.Generic;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.baseBrowser.Controller
{
    public interface IConcreteController
    {
        List<AbstractUMI3DInput> Inputs { get; }
        List<BaseInteraction<EventDto>> Manipulations { get; }
        BaseManipulationGroup ManipulationGroup { get; set; }

        /// <summary>
        /// Method call when Controller awakes.
        /// </summary>
        void Awake();
        /// <summary>
        /// Method call when Controller starts.
        /// </summary>
        void Start();
        /// <summary>
        /// Method call when Controller updates. Only if this ConcreteController is the one in command.
        /// </summary>
        void Update();
        /// <summary>
        /// Method call when Controller needs to find Event input.
        /// </summary>
        AbstractUMI3DInput FindInput(EventDto evt, bool unused = true, bool tryToFindInputForHoldableEvent = false);
        /// <summary>
        /// Clear inputs
        /// </summary>
        void ClearInputs();
        /// <summary>
        /// Reset inputs when environment launch.
        /// </summary>
        void ResetInputsWhenEnvironmentLaunch();
    }
}
