using BrowserDesktop.Cursor;
using BrowserDesktop.Interaction;
using BrowserDesktop.Parameters;
using inetum.unityUtils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using umi3d.common.userCapture;
using UnityEngine;
using UnityEngine.Events;


namespace BrowserDesktop.Selection.Intent
{
    [CreateAssetMenu(fileName = "IntenSelectDetector", menuName = "UMI3D/Selection/Intent Detector/Hover")]
    public class HoverDetector : AbstractSelectionIntentDetector
    {
        private RayZoneSelection raySelection;

        public override void InitDetector(AbstractController controller)
        {
            pointerTransform = Camera.main.transform; //?
            raySelection = new RayZoneSelection();
            raySelection.originTransform = pointerTransform;
        }

        public override InteractableContainer PredictTarget()
        {
            //1. Cast a ray to find all interactables
            var interactablesWithDistances = raySelection.GetObjectsAlongRayWithRayCastHits();
            var interactables = interactablesWithDistances.Keys.ToList();
            if (interactables.Count == 0)
                return null;

            var activeInteractables = interactables.Where(obj => (obj != null && obj.Interactable.Active)).DefaultIfEmpty();
            if (activeInteractables == default)
                return null;

            //2. Sort them by hasPriority and distance from user
            var activeInteractablesWithPriority = (from obj in activeInteractables
                                                   where obj.Interactable.HasPriority
                                                  select obj).ToList();
            if (activeInteractablesWithPriority.Count > 0)
                interactables = activeInteractablesWithPriority;

            var minDist = (from obj in interactables
                           select interactablesWithDistances[obj].distance).Min();

            var closestActiveInteractable = (from obj in interactables
                                             where interactablesWithDistances[obj].distance == minDist
                                                select obj).FirstOrDefault();

            //3. Save the data about the closest one
            if (!closestActiveInteractable.Equals(default))
            {
                Interactable interactable = closestActiveInteractable.Interactable;
            }

            return closestActiveInteractable;
        }

        public override void ResetDetector()
        {
        }

    }
}
