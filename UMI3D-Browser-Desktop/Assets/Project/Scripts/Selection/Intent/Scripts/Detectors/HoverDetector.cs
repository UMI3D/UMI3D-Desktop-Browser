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
using System.Linq;
using umi3d.cdk.interaction;
using UnityEngine;


namespace BrowserDesktop.Selection.Intent
{
    [CreateAssetMenu(fileName = "IntenSelectDetector", menuName = "UMI3D/Selection/Intent Detector/Hover")]
    public class HoverDetector : AbstractSelectionIntentDetector
    {

        public override void InitDetector(AbstractController controller)
        {
            pointerTransform = Camera.main.transform; //?
        }

        public override InteractableContainer PredictTarget()
        {
            var raySelection = new RayZoneSelection(pointerTransform.position, pointerTransform.forward);

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
