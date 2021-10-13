using System.Collections;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.cdk.interaction;
using UnityEngine;

namespace BrowserDesktop.Intent
{
    [CreateAssetMenu(fileName = "RefinedKEPDetector", menuName = "UMI3D/Selection Intent Detector/Refined KEP ")]
    public class RefinedKEP : AbstractSelectionIntentDetector
    {
        public override void InitDetector()
        {
            throw new System.NotImplementedException();
        }

        public override InteractableContainer PredictTarget()
        {
            throw new System.NotImplementedException();
        }

        public override void ResetDetector()
        {
            throw new System.NotImplementedException();
        }

        private float approximateKinematicsPolynomial()
        {
            return default;
        }
    }
}
