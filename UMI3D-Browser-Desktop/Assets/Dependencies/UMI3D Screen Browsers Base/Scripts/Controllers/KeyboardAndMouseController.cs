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
using System.Collections.Generic;
using umi3d.baseBrowser.Controller;
using umi3d.baseBrowser.cursor;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.cdk.interaction;
using umi3d.cdk.menu;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.desktopBrowser.Controller
{
    public class KeyboardAndMouseController : IConcreteController
    {
        public BaseController Controller;
        public MenuAsset ObjectMenu;
        //[Header("Input Action")]
        //[SerializeField]
        //protected List<CursorKeyInput> ManipulationActionInput = new List<CursorKeyInput>();
        protected List<KeyboardInteraction> KeyboardInteractions = new List<KeyboardInteraction>();

        public List<AbstractUMI3DInput> Inputs
        {
            get
            {
                List<AbstractUMI3DInput> list = new List<AbstractUMI3DInput>();
                list.AddRange(KeyboardInteractions);
                //list.AddRange(ManipulationInputs);
                return list;
            }
        }

        #region Monobehaviour Life Cycle

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
            KeyboardInteraction.S_Interactions?.ForEach(interaction =>
            {
                KeyboardInteractions.Add(interaction);
                interaction.Init(Controller);
                interaction.bone = Controller.interactionBoneType;
                interaction.Menu = ObjectMenu.menu;
            });
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Update()
        {
            //if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ContextualMenuNavigationDirect)) || Input.mouseScrollDelta.y < 0)
            //    m_navigationDirect++;
            //else if (Input.mouseScrollDelta.y > 0) m_navigationDirect--;
        }

        #endregion

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="unused"></param>
        /// <param name="tryToFindInputForHoldableEvent"></param>
        /// <returns></returns>
        public AbstractUMI3DInput FindInput(EventDto evt, bool unused = true, bool tryToFindInputForHoldableEvent = false)
            => KeyboardInteractions.Find(i => i.IsAvailable() || !unused);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void ClearInputs()
        {
            foreach (KeyboardInteraction input in KeyboardInteractions) if (!input.IsAvailable()) input.Dissociate();
        }
    }
}