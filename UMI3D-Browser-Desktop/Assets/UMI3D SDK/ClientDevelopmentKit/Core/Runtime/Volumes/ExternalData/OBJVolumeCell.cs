using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.cdk.volumes
{
    public class OBJVolumeCell : AbstractVolumeCell
    {
        public List<Mesh> meshes;

        public ulong id;
        public override ulong Id() => id;


        public override bool IsInside(Vector3 point)
        {
            throw new System.NotImplementedException();
        }
    }
}