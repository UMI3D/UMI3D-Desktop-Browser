using System.Collections.Generic;

namespace umi3d.cdk.interaction.selection.intent.log
{
    internal class ClientData
    {
        public List<List<TargetData>> scene;
        public List<TrackingData> tracking;
    }

    internal class TargetData
    {
        public int n; //target number
        public float x;
        public float y;
        public float z;
    }

    internal class TrackingData
    {
        public long t;

        public float p_x;
        public float p_y;
        public float p_z;
        public float p_r_x;
        public float p_r_y;
        public float p_r_z;

        public float h_x;
        public float h_y;
        public float h_z;
        public float h_r_x;
        public float h_r_y;
        public float h_r_z;
    }

}