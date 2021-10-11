using System.Collections;
using umi3d.cdk;
using UnityEngine;

namespace BrowserDesktop.Intent
{
    public abstract class AbstractSelectionIntentDetector : ScriptableObject
    {
        /// <summary>
        /// Transform associated with the pointing device
        /// </summary>
        [SerializeField]
        protected Transform pointerTransform;

        /// <summary>
        /// Initialize the detector
        /// </summary>
        public abstract void InitDetector();

        /// <summary>
        /// Reset parameters of the detector.
        /// To be called after an object has be selected.
        /// </summary>
        public abstract void ResetDetector();

        public abstract UMI3DNodeInstance PredictTarget();
    }
}

