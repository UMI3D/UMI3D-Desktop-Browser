using System.Collections;
using umi3d.cdk;
using umi3d.cdk.interaction;
using UnityEngine;

namespace BrowserDesktop.Intent
{
    /// <summary>
    /// Abstract parent class for all intention of selection detectors
    /// </summary>
    public abstract class AbstractSelectionIntentDetector : ScriptableObject
    {
        /// <summary>
        /// Transform associated with the pointing device
        /// </summary>
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

        /// <summary>
        /// Predict the target of the user selection intention
        /// </summary>
        /// <returns>An interactable object or null</returns>
        public abstract InteractableContainer PredictTarget();
    }
}

