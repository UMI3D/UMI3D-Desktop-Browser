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
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.baseBrowser.inputs.interactions
{
    public class ManipulationGroupeForDesktop : BaseManipulationGroup
    {
        /// <summary>
        /// List of inputs that will be used to manipulate an interactable.
        /// </summary>
        [SerializeField]
        protected List<KeyboardManipulation> m_inputs = new List<KeyboardManipulation>();

        /// <summary>
        /// Bind this manipulation group.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="Inputs"></param>
        public void Bind(AbstractController controller, List<KeyboardManipulation> Inputs)
        {
            Init(controller);
            m_inputs = Inputs;
        }

        public override bool IsAvailable()
            => base.IsAvailable() && m_inputs.Exists(activationButton => activationButton.IsAvailable());
    }
}
