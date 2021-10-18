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

namespace BrowserDesktop.Selection
{
    /// <summary>
    /// A conic zone selector
    /// </summary>
    public class ConicZoneSelection : RayZoneSelection
    {
        /// <summary>
        /// Cone angle in degrees, correspond to the half of the full angle at its apex
        /// </summary>
        [SerializeField]
        float coneAngle = 15;

        public override bool IsObjectInZone(InteractableContainer obj)
        {
            var vectorToObject = obj.transform.position - originTransform.position;

            return Vector3.Dot(vectorToObject.normalized, originTransform.forward) > Mathf.Cos(coneAngle * Mathf.PI / 180);
        }

        public override List<InteractableContainer> GetObjectsInZone()
        {
            var objectsInZone = GetInteractableObjectInScene();
            return objectsInZone.Where(IsObjectInZone).ToList();
        }

        public ConicZoneSelection(Transform originTransform)
        {
            this.originTransform = originTransform;
        }

        public ConicZoneSelection(Transform originTransform, float angle)
        {
            this.originTransform = originTransform;
            this.coneAngle = angle;
        }

    }
}