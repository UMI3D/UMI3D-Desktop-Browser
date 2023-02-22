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
using System.Collections.Generic;
using umi3d.baseBrowser.Controller;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.mobileBrowser.Controller
{
    public class MobileController : IConcreteController
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<AbstractUMI3DInput> Inputs
        {
            get {
                List<AbstractUMI3DInput> list = new List<AbstractUMI3DInput>();
                return list;
            }
        }

        public List<BaseInteraction<EventDto>> Manipulations => throw new System.NotImplementedException();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Awake()
        {

        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Start()
        {

        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Update()
        {

        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="unused"></param>
        /// <param name="tryToFindInputForHoldableEvent"></param>
        /// <returns></returns>
        public AbstractUMI3DInput FindInput(EventDto evt, bool unused = true, bool tryToFindInputForHoldableEvent = false)
            => null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void ClearInputs()
        {
        }
    }
}