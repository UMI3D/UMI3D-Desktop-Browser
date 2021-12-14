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

using UnityEngine;

namespace umi3d.cdk.interaction.selection.intent
{
    /// <summary>
    /// Selector that uses an intent detector to predict targets
    /// </summary>
    public class IntentSelector : AbstractSelector
    {
        /// <summary>
        /// UMI3D controller
        /// </summary>
        [SerializeField]
        public AbstractController controller;

        /// <summary>
        /// Selection intent detector
        /// </summary>
        [SerializeField]
        private AbstractSelectionIntentDetector detector;

        /// <summary>
        /// Visual cue handler
        /// </summary>
        [SerializeField]
        private AbstractVisualCueHandler selectionVisualCueHandler;

        #region selectionCache

        private InteractableContainer lastSelectedInteractable;

        public SelectionData selectionData;

        #endregion selectionCache

        public override void Activate(int id)
        {
            base.Activate(id);
            detector.InitDetector(controller);
        }

        public override void Deactivate(int id)
        {
            base.Deactivate(id);
            detector.ResetDetector();
        }

        public override void Select()
        {
            if (controller.Interacting)
                return;

            InteractableContainer interactableToSelect = detector.PredictTarget();

            if (interactableToSelect != null
                && interactableToSelect != lastSelectedInteractable
                && interactableToSelect.Interactable.dto.interactions != null
                && interactableToSelect.Interactable.dto.interactions.Count > 0
                && !InteractionMapper.Instance.IsToolSelected(interactableToSelect.Interactable.dto.id))
            {
                if (lastSelectedInteractable != null)
                    UnselectLastSelected();

                selectionVisualCueHandler.ActivateSelectedVisualCue(interactableToSelect);
                controller.Project(AbstractInteractionMapper.Instance.GetTool(interactableToSelect.Interactable.dto.id), true,
                    new RequestedUsingSelector<IntentSelector>() { controller = this.controller }, interactableToSelect.Interactable.id);

                lastSelectedInteractable = interactableToSelect;
            }
            else if (interactableToSelect == null && interactableToSelect != lastSelectedInteractable)
            {
                UnselectLastSelected();
                lastSelectedInteractable = null;
            }
        }

        private void UnselectLastSelected()
        {
            selectionVisualCueHandler.DeactivateSelectedVisualCue(lastSelectedInteractable);

            controller.Release(AbstractInteractionMapper.Instance.GetTool(lastSelectedInteractable.Interactable.dto.id),
                                new RequestedUsingSelector<IntentSelector>());
            selectionData.clear();
        }
    }

    /// <summary>
    /// Data container for selection information at each frame
    /// </summary>
    public struct SelectionData
    {
        public bool ForceProjection;
        public bool ForceProjectionReleasable;

        public HoldableButtonMenuItem ForceProjectionMenuItem;

        public Interactable Selected;
        public ulong SelectedId;

        //public Interactable CurrentHovered;
        //public Transform CurrentHoveredTransform;
        //public ulong CurrentHoveredId;

        //public Vector3 point;
        //public Vector3 worldPoint;
        //public Vector3 centeredWorldPoint;
        //public Vector3 normal;

        //public Vector3 worldNormal;
        //public Vector3 direction;
        //public Vector3 worlDirection;
        //public Vector3 cursorOffset;

        //public Vector3 lastPoint, lastNormal, lastDirection;

        public SelectionState SelectionState;

        public void clear()
        {
            ForceProjection = false;
            ForceProjectionReleasable = false;
            ForceProjectionMenuItem = null;
            Selected = null;
            SelectedId = default;
            SelectionState = SelectionState.None;
        }

        //selectionData.CurrentHoveredId = UMI3DEnvironmentLoader.GetNodeID(hit.collider);

        //            selectionData.CurrentHovered = interactable;
        //            selectionData.CurrentHoveredTransform = interactableContainer.transform;

        //            selectionData.point = interactableContainer.transform.InverseTransformPoint(hit.point);
        //            selectionData.worldPoint = hit.point;
        //            if (Vector3.Distance(selectionData.worldPoint, hit.transform.position) < 0.1f) selectionData.centeredWorldPoint = hit.transform.position;
        //            else selectionData.centeredWorldPoint = selectionData.worldPoint;

        //            selectionData.normal = interactableContainer.transform.InverseTransformDirection(hit.normal);
        //            selectionData.worldNormal = hit.normal;

        //            selectionData.direction = interactableContainer.transform.InverseTransformDirection(ray.direction);
        //            selectionData.worlDirection = ray.direction;
    }

    public enum SelectionState
    { None, Selected, Manipulated }
}