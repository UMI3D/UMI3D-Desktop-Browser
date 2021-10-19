using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3d.cdk.interaction;
using UnityEngine;

namespace BrowserDesktop.Selection
{
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
            var objectsOnRay = (from hit in rayCastHits
                                where (hit.transform.GetComponent<InteractableContainer>() != null
                                || hit.transform.GetComponentInParent<InteractableContainer>() != null)
                                select hit.transform.GetComponent<InteractableContainer>() 
                                        ?? hit.transform.GetComponentInParent<InteractableContainer>()).ToList();
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

            return objList.Where(o => distToRay(o) == minDistance).FirstOrDefault();
        }

        /// <summary>
        /// Returns objects that are along the ray with their RayCastHit object
        /// </summary>
        /// <returns></returns>
        public Dictionary<InteractableContainer, RaycastHit> GetObjectsAlongRayWithRayCastHits()
        {
            var rayCastHits = GetRayCastHits();
            var objectsOnRay = (from hit in rayCastHits
                                where (hit.transform.GetComponent<InteractableContainer>() != null
                                || hit.transform.GetComponentInParent<InteractableContainer>() != null)
                                select (hit.transform.GetComponent<InteractableContainer>()
                                        ?? hit.transform.GetComponentInParent<InteractableContainer>(), hit)).ToDictionary(x=>x.Item1, x=>x.Item2);
            return objectsOnRay;
        }
    }
}
