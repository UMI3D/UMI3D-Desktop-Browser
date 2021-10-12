using System.Collections;
using System.Collections.Generic;
using umi3d.cdk;
using UnityEngine;
using System.Linq;

namespace BrowserDesktop.Intent
{
    /// <summary>
    /// A conic zone selector
    /// </summary>
    public class ConeZoneSelector : AbstractZoneSelector
    {
        /// <summary>
        /// Cone angle in degrees, correspond to the half of the full angle at its apex
        /// </summary>
        [SerializeField]
        private float coneAngle = 15;

        /// <summary>
        /// Origin and orientation of the cone's apex
        /// </summary>
        [SerializeField]
        public Transform attachedPoint;

        public override bool IsObjectInZone(UMI3DNodeInstance obj)
        {
            var vectorToObject = obj.transform.position - attachedPoint.position;

            return Vector3.Dot(vectorToObject.normalized, attachedPoint.forward) > Mathf.Cos(coneAngle * Mathf.PI / 180);
        }

        public override List<UMI3DNodeInstance> GetObjectsInZone()
        {
            var objectsInZone = GetInteractableObjectInScene();
            return objectsInZone.Where(IsObjectInZone).ToList();
        }

        public UMI3DNodeInstance GetClosestObjectToRay(List<UMI3DNodeInstance> objList)
        {
            System.Func<UMI3DNodeInstance, float> distToRay = obj =>
            {
                var vectorToObject = obj.transform.position - attachedPoint.position;
                return Vector3.Cross(vectorToObject.normalized, attachedPoint.forward).magnitude;
            };

            var minDistance = objList.Select(distToRay).Min();

            return objList.Where(o => distToRay(o) == minDistance).FirstOrDefault();
        }

        public ConeZoneSelector(Transform attachedPoint)
        {
            this.attachedPoint = attachedPoint;
        }

        public ConeZoneSelector(Transform attachedPoint, float angle)
        {
            this.attachedPoint = attachedPoint;
            this.coneAngle = angle;
        }

    }
}