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

namespace umi3d.cdk.volumes
{
    public class Cylinder : AbstractPrimitive
    {
        public float radius;
        public float height;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public override void Delete() { }

        public override bool IsInside(Vector3 point)
        {
            /*
             * algorithm :
             * 1- convert point into cylinder local coordinate
             * 2- check if distance from center is < radius
             * 3- check height
             */

            Vector3 localCoordinate = 
                Vector3.Scale(
                    new Vector3(
                        1f / scale.x,
                        1f / scale.y,
                        1f / scale.z),
                    Quaternion.Inverse(rotation) * (point - position));


            if (Vector3.ProjectOnPlane(localCoordinate, Vector3.up).magnitude > radius)
                return false;
            else if ((localCoordinate.y < 0) || (localCoordinate.y > height))
                return false;
            else
                return true;
        }
    }
}