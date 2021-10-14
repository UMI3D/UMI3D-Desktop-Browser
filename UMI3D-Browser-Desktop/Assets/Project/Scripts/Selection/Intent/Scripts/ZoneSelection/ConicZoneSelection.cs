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
using UnityEngine;
using System.Linq;
using umi3d.cdk.interaction;

namespace BrowserDesktop.Intent
{
    /// <summary>
    /// A conic zone selector
    /// </summary>
    public class ConicZoneSelection : AbstractZoneSelection
    {
        /// <summary>
        /// Cone angle in degrees, correspond to the half of the full angle at its apex
        /// </summary>
        [SerializeField]
        float coneAngle = 15;

        /// <summary>
        /// Origin and orientation of the cone's apex
        /// </summary>
        [SerializeField]
        public Transform attachedPoint;

        public override bool IsObjectInZone(InteractableContainer obj)
        {
            var vectorToObject = obj.transform.position - attachedPoint.position;

            return Vector3.Dot(vectorToObject.normalized, attachedPoint.forward) > Mathf.Cos(coneAngle * Mathf.PI / 180);
        }

        public override List<InteractableContainer> GetObjectsInZone()
        {
            var objectsInZone = GetInteractableObjectInScene();
            return objectsInZone.Where(IsObjectInZone).ToList();
        }

        public InteractableContainer GetClosestObjectToRay(List<InteractableContainer> objList)
        {
            System.Func<InteractableContainer, float> distToRay = obj =>
            {
                var vectorToObject = obj.transform.position - attachedPoint.position;
                return Vector3.Cross(vectorToObject.normalized, attachedPoint.forward).magnitude;
            };

            var minDistance = objList.Select(distToRay).Min();

            return objList.Where(o => distToRay(o) == minDistance).FirstOrDefault();
        }

        public ConicZoneSelection(Transform attachedPoint)
        {
            this.attachedPoint = attachedPoint;
        }

        public ConicZoneSelection(Transform attachedPoint, float angle)
        {
            this.attachedPoint = attachedPoint;
            this.coneAngle = angle;
        }

    }
}