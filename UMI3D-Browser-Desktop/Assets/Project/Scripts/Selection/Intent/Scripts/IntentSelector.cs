/*
Copyright 2019 - 2021 Inetum
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
using umi3d.cdk;
using umi3d.cdk.interaction;
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine;

namespace BrowserDesktop.Intent
{
    public class IntentSelector : AbstractSelector
    {
        [SerializeField]
        private AbstractController controller;

        [SerializeField]
        private AbstractSelectionIntentDetector detector;

        [SerializeField]
        private SelectionHighlighter selectionHighlighter;

        private InteractableContainer lastSelectedInteractable;

        public override void Activate(int id)
        {
            base.Activate(id);
            detector.InitDetector();
        }

        public override void Deactivate(int id)
        {
            base.Deactivate(id);
            detector.ResetDetector();
        }

        public override void Select()
        {
            InteractableContainer interactableToSelect = detector.PredictTarget();

            if( interactableToSelect != null
                && interactableToSelect != lastSelectedInteractable
                && interactableToSelect.Interactable.dto.interactions != null 
                && interactableToSelect.Interactable.dto.interactions.Count > 0
                && !InteractionMapper.Instance.IsToolSelected(interactableToSelect.Interactable.dto.id))
            {
                if (lastSelectedInteractable != null)
                    selectionHighlighter.DeactivateSelectedHighlight(lastSelectedInteractable);

                selectionHighlighter.ActivateSelectedHighlight(interactableToSelect);
                lastSelectedInteractable = interactableToSelect;

                AbstractInteractionMapper.Instance.SelectTool(interactableToSelect.Interactable.dto.id, true, interactableToSelect.Interactable.id,
                                                                new RequestedUsingSelector<IntentSelector>() { controller = this.controller });
            } 
        }

    }
}

