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
    /// A conic zone selection helper
    /// </summary>
    public class ConicZoneSelection : RayZoneSelection
    {
        /// <summary>
        /// Cone angle in degrees, correspond to the half of the full angle at its apex
        /// </summary>
        [SerializeField]
        private float coneAngle = 15;

        public override bool IsObjectInZone(InteractableContainer obj)
        {
            var vectorToObject = obj.transform.position - origin;

            return Vector3.Dot(vectorToObject.normalized, direction) > Mathf.Cos(coneAngle * Mathf.PI / 180);
        }

        public override List<InteractableContainer> GetObjectsInZone()
        {
            var objectsInZone = GetInteractableObjectInScene();
            return objectsInZone.Where(IsObjectInZone).ToList();
        }

        public ConicZoneSelection(Transform originTransform, float angle) : base(originTransform)
        {
            coneAngle = angle;
        }

        public ConicZoneSelection(Vector3 origin, Vector3 direction, float angle) : base(origin, direction)
        {
            coneAngle = angle;
        }
    }
}