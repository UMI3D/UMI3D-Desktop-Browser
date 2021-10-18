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
        [SerializeField]
        public Transform originTransform;

        public int maxDistance = 0;

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
            Ray ray = new Ray(originTransform.position, originTransform.forward);
            Debug.DrawRay(ray.origin, ray.direction.normalized * 100f, Color.red, 0, true);
            return umi3d.common.Physics.RaycastAll(ray.origin, ray.direction);
        }

        public InteractableContainer GetClosestObjectToRay(List<InteractableContainer> objList)
        {
            System.Func<InteractableContainer, float> distToRay = obj =>
            {
                var vectorToObject = obj.transform.position - originTransform.position;
                return Vector3.Cross(vectorToObject.normalized, originTransform.forward).magnitude;
            };

            var minDistance = objList.Select(distToRay).Min();

            return objList.Where(o => distToRay(o) == minDistance).FirstOrDefault();
        }

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
