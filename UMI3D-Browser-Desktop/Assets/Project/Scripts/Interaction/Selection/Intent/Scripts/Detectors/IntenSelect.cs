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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace umi3d.cdk.interaction.selection.intent
{
    /// <summary>
    /// Implementation of the IntenSelect detector of intention, from de Haan et al. 2005
    /// </summary>
    [CreateAssetMenu(fileName = "IntenSelectDetector", menuName = "UMI3D/Selection/Intent Detector/IntenSelect")]
    public class IntenSelect : AbstractSelectionIntentDetector
    {
        /// <summary>
        /// Corrective term according to de Haan et al. 2005
        /// </summary>
        [Header("Parameters")]
        [SerializeField]
        private float corrective_k = 4 / 5;

        /// <summary>
        /// Cone angle in degrees, correspond to the half of the full angle at its apex
        /// </summary>
        [SerializeField]
        private float coneAngle = 15;

        [SerializeField]
        private float stickinessCoeff = 0.5f;

        [SerializeField]
        private float snappinessCoeff = 0.5f;

        /// <summary>
        /// Tolerance at the boundaries (epsilon)
        /// </summary>
        [SerializeField]
        private float tolerance = 0.001f;

        /// <summary>
        /// Maximum value for the score
        /// </summary>
        private float boundary;

        /// <summary>
        /// If true, the algorithm is scaled to real time based on [Ortega 2013] suggestion
        /// </summary>
        [SerializeField]
        private bool scaledTimeEnhancement = false;

        /// <summary>
        /// Selection mode
        /// </summary>
        private enum IntenSelectMode
        { CONE_FROM_HAND, CONE_FROM_HEAD };

        [SerializeField]
        private IntenSelectMode currentMode = IntenSelectMode.CONE_FROM_HEAD;

        /// <summary>
        /// Dictionnary where objects considered by the Intenselect algorithm are stored
        /// The key is the id of the object, the value is its score
        /// </summary>
        private Dictionary<InteractableContainer, float> objectsToConsiderScoresDict;

        public override void InitDetector(AbstractController controller)
        {
            objectsToConsiderScoresDict = new Dictionary<InteractableContainer, float>();

            if (currentMode == IntenSelectMode.CONE_FROM_HEAD)
                pointerTransform = Camera.main.transform; // could get the Mouse and Keyboard controller viewPoint ?
            else
                pointerTransform = controller.transform;
            boundary = snappinessCoeff / (1 - stickinessCoeff);
        }

        public override void ResetDetector()
        {
            objectsToConsiderScoresDict.Clear();
        }

        /// <summary>
        /// Clear the detector except for the passed object
        /// </summary>
        /// <param name="obj"></param>
        private void ResetExceptOne(InteractableContainer obj)
        {
            ResetDetector();
            objectsToConsiderScoresDict.Add(obj, 0);
        }

        /// <summary>
        /// Find the target of the use intention according to the IntentSelect algorithm
        /// </summary>
        /// <returns>The intended object or null</returns>
        public override InteractableContainer PredictTarget()
        {
            var coneSelector = new ConicZoneSelection(pointerTransform.position, pointerTransform.forward, coneAngle);

            var interactableObjectsInScene = coneSelector.GetInteractableObjectInScene();

            foreach (var obj in interactableObjectsInScene)
            {
                if (!objectsToConsiderScoresDict.ContainsKey(obj))
                {
                    if (coneSelector.IsObjectInZone(obj))
                    {
                        objectsToConsiderScoresDict.Add(obj, 0);
                        objectsToConsiderScoresDict[obj] = ComputeScore(obj);
                    }
                }
                else
                {
                    if (objectsToConsiderScoresDict[obj] <= -(boundary - tolerance)) //remove useless objects that are too far
                        objectsToConsiderScoresDict.Remove(obj);
                    else
                        objectsToConsiderScoresDict[obj] = ComputeScore(obj);
                }
            }

            var maxScore = objectsToConsiderScoresDict.Values.Max();
            var estimatedTargetPair = objectsToConsiderScoresDict.FirstOrDefault(o => o.Value == maxScore); //find the object with the highest score

            return estimatedTargetPair.Key;
        }

        /// <summary>
        /// Compute the cumulative score according to the formula of IntentSelect
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Object's score</returns>
        private float ComputeScore(in InteractableContainer obj)
        {
            var vectorToObject = obj.transform.position - pointerTransform.position;

            float dperp = Mathf.Abs(Vector3.Dot(vectorToObject, pointerTransform.forward));
            float dproj = Vector3.Cross(vectorToObject, pointerTransform.forward).magnitude;

            if (dproj != 0)
            {
                var correctedAngle = Mathf.Atan2(Mathf.Pow(dproj, corrective_k), dperp) * 180 / Mathf.PI;
                var variation = 1 - (correctedAngle / coneAngle);
                if (scaledTimeEnhancement)
                    variation *= Time.deltaTime;
                var newScore = objectsToConsiderScoresDict[obj] * stickinessCoeff + variation * snappinessCoeff;
                if (newScore > (boundary)) //Avoid float overflow and give reactivity to the detector
                {
                    ResetExceptOne(obj);
                    return 1;
                }
                else
                    return newScore;
            }
            else
                return 0;
        }
    }
}