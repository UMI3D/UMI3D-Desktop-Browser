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

namespace umi3d.cdk.interaction.selection.intent
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
        public abstract void InitDetector(AbstractController controller);

        /// <summary>
        /// Reset parameters of the detector.
        /// To be called after an object has been selected.
        /// </summary>
        public abstract void ResetDetector();

        /// <summary>
        /// Predict the target of the user selection intention
        /// </summary>
        /// <returns>An interactable object or null</returns>
        public abstract InteractableContainer PredictTarget();
    }
}

