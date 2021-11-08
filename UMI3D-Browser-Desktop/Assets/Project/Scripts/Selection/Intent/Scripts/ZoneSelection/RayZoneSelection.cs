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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace umi3d.cdk.interaction.selection
{
    /// <summary>
    /// A ray zone selection helper. The zone is the casted ray.
    /// </summary>
    public class RayZoneSelection : AbstractZoneSelection
    {
        /// <summary>
        /// Origin and orientation of the ray
        /// </summary>
        public Vector3 direction;
        public Vector3 origin;
   
        public RayZoneSelection(Vector3 origin, Vector3 direction) 
        {
            this.origin = origin;
            this.direction = direction;
        }

        public RayZoneSelection(Transform originTransform)
        {
            this.origin = originTransform.position;
            this.direction = originTransform.forward;
        }

        public override List<InteractableContainer> GetObjectsInZone()
        {
            var rayCastHits = GetRayCastHits();

            var objectsOnRay = new List<InteractableContainer>();
            foreach (var hit in rayCastHits)
            {
                var interContainer = hit.transform.GetComponent<InteractableContainer>();
                if (interContainer == null)
                    interContainer = hit.transform.GetComponentInParent<InteractableContainer>();
                if (interContainer != null)
                    objectsOnRay.Add(interContainer);
            }
            
            return objectsOnRay;
        }

        public override bool IsObjectInZone(InteractableContainer obj)
        {
            return GetObjectsInZone().Contains(obj);
        }

        /// <summary>
        /// Return all currently pointed objects (any type) as an array of RaycastHit.
        /// </summary>
        /// <returns></returns>
        public RaycastHit[] GetRayCastHits()
        {
            Debug.DrawRay(origin, direction.normalized * 100f, Color.red, 0, true);
            return umi3d.common.Physics.RaycastAll(origin, direction);
        }

        /// <summary>
        /// Returns the closest object to the ray (minimal orthogonal projection)
        /// </summary>
        /// <param name="objList"></param>
        /// <returns></returns>
        public InteractableContainer GetClosestObjectToRay(List<InteractableContainer> objList)
        {
            if (objList.Count == 0)
                return null;

            System.Func<InteractableContainer, float> distToRay = obj =>
            {
                var vectorToObject = obj.transform.position - origin;
                return Vector3.Cross(vectorToObject.normalized, direction).magnitude;
            };

            var minDistance = objList.Select(distToRay)?.Min();

            return objList.FirstOrDefault(o => distToRay(o) == minDistance);
        }

        public InteractableContainer GetClosestObjectOnRay()
        {
            var interactablesWithDistances = GetObjectsAlongRayWithRayCastHits();
            var interactables = interactablesWithDistances.Keys.ToList();
            if (interactables.Count == 0)
                return null;

            var activeInteractables = interactables.Where(obj => (obj != null && obj.Interactable.Active)).DefaultIfEmpty();
            if (activeInteractables == default)
                return null;

            //Sort them by hasPriority and distance from user
            var activeInteractablesWithPriority = (from obj in activeInteractables
                                                   where obj.Interactable.HasPriority
                                                   select obj).ToList();
            if (activeInteractablesWithPriority.Count > 0)
                interactables = activeInteractablesWithPriority;

            var minDist = (from obj in interactables
                           select interactablesWithDistances[obj].distance).Min();

            var closestActiveInteractable = interactables.FirstOrDefault(obj => interactablesWithDistances[obj].distance == minDist);
            return closestActiveInteractable;
        }

        /// <summary>
        /// Returns objects that are along the ray with their RayCastHit object
        /// </summary>
        /// <returns></returns>
        public Dictionary<InteractableContainer, RaycastHit> GetObjectsAlongRayWithRayCastHits()
        {
            var rayCastHits = GetRayCastHits();
            var objectsOnRay = new Dictionary<InteractableContainer, RaycastHit>();
            foreach (var hit in rayCastHits)
            {
                var interContainer = hit.transform.GetComponent<InteractableContainer>();
                if (interContainer == null)
                    interContainer = hit.transform.GetComponentInParent<InteractableContainer>();
                if (interContainer != null)
                    objectsOnRay.Add(interContainer, hit);
            }

            return objectsOnRay;
        }
    }
}
