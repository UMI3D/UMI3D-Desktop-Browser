using System.Collections;
using System.Collections.Generic;
using umi3d.common.volume;
using UnityEngine;
using System.Linq;

namespace umi3d.cdk.volumes
{
    public class OBJVolumeCell : AbstractVolumeCell
    {
        public List<Mesh> meshes;

        public ulong id;

        public override Mesh GetBase()
        {
            Debug.Log("WARNING ! you are using a never tested before feature (wild!)"); //still some bugs here ...
            List<Mesh> bases = meshes.ConvertAll(m => GeometryTools.GetBase(m, 10, 0.01f));
            return GeometryTools.ForceNormalUp(GeometryTools.Merge(bases));
        }

        public override ulong Id() => id;

        public override bool IsInside(Vector3 point) => meshes.Exists(mesh => GeometryTools.IsInside(mesh, point));
    }
}