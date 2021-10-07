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
using System.Collections;
using System.Collections.Generic;

namespace umi3d.cdk.volumes
{
    /// <summary>
    /// Box shaped primitive.
    /// </summary>
    public class Box : AbstractPrimitive
    {
        public Bounds bounds;

        public override void Delete() { }

        public override Mesh GetBase()
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();

            verts.Add(bounds.min);
            verts.Add(bounds.min + bounds.size.x * Vector3.right);
            verts.Add(bounds.min + bounds.size.x * Vector3.right + bounds.size.z * Vector3.forward);
            verts.Add(bounds.min + bounds.size.z * Vector3.forward);

            tris.Add(0);    tris.Add(2);    tris.Add(1);
            tris.Add(0);    tris.Add(3);    tris.Add(2);           

            Mesh base_ = new Mesh();
            base_.vertices = verts.ToArray();
            base_.triangles = tris.ToArray();
            base_.RecalculateNormals();
            return base_;
        }

        public override bool IsInside(Vector3 point)
        {
            return bounds.Contains(point);
        }
    }
}