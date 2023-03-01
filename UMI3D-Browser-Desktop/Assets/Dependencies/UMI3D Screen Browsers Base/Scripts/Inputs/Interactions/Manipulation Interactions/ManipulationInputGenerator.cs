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
using inetum.unityUtils;
using System.Collections.Generic;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.baseBrowser.inputs.interactions
{
    public class ManipulationInputGenerator : SingleBehaviour<ManipulationInputGenerator>
    {
        /// <summary>
        /// <see cref="BaseManipulation.strength"/>
        /// </summary>
        public float strength;
        /// <summary>
        /// Reference to the <see cref="Cursor.FrameIndicator"/>
        /// </summary>
        public Cursor.FrameIndicator frameIndicator;
        /// <summary>
        /// TODO: not define
        /// </summary>
        public Transform manipulationCursor;

        /// <summary>
        /// instanciate and init Manipulation according to dofGroups and Inputs
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="manipulationInputs"></param>
        static public BaseManipulation Instanciate
        (
            AbstractController controller, 
            BaseInteraction<EventDto> Input, 
            DofGroupEnum dofGroup
        ) => (Exists) ? Instance._Instanciate(controller, Input, dofGroup) : null;

        protected virtual BaseManipulation _Instanciate
        (
            AbstractController controller, 
            BaseInteraction<EventDto> input, 
            DofGroupEnum dofGroup
        )
        {
            var inputInstance = gameObject.AddComponent<BaseManipulation>();

            inputInstance.activationButton = input;
            inputInstance.DofGroup = dofGroup;
            inputInstance.Init(controller);
            inputInstance.strength = strength;
            inputInstance.frameIndicator = frameIndicator;
            inputInstance.manipulationCursor = manipulationCursor;

            return inputInstance;
        }
    }
}