using System.Collections.Generic;

namespace umi3d.cdk.interaction.selection.intent.log
{
    internal class ClientData
    {
        public List<List<TargetData>> scene;
        public List<TrackingData> tracking;

        public ClientData()
        {
            scene = new List<List<TargetData>>();
            tracking = new List<TrackingData>();
        }

        public void Clear()
        {
            scene.Clear();
            tracking.Clear();
        }
    }

    internal class TargetData
    {
        public int n; //target number
        public double x;
        public double y;
        public double z;
    }

    internal class TrackingData
    {
        public long t;

        public double p_x;
        public double p_y;
        public double p_z;
        public double p_r_x;
        public double p_r_y;
        public double p_r_z;

        public double h_x;
        public double h_y;
        public double h_z;
        public double h_r_x;
        public double h_r_y;
        public double h_r_z;
    }

}