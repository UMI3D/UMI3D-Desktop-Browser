using System.Collections.Generic;
using umi3d.cdk;
using UnityEngine;
using System.Linq;

namespace BrowserDesktop.Intent
{
    /// <summary>
    /// Implementation of the IntenSelect detector of intention, from de Haan et al. 2005
    /// </summary>
    [CreateAssetMenu(fileName = "IntenSelectDetector", menuName = "UMI3D/Selection Intent Detector/IntenSelect ")]
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

        /// <summary>
        /// Maximum score before provoking a reset of the detector
        /// </summary>
        [Header("Score boundaries")]
        [SerializeField]
        private float scoreMax = 70;

        /// <summary>
        /// Minimum score for an object to remain considered
        /// </summary>
        [SerializeField]
        private float scoreMin = -10;

        /// <summary>
        /// Conic zone selector
        /// </summary>
        private ConeZoneSelector coneSelector;

        /// <summary>
        /// Cone angle in degrees, correspond to the half of the full angle at its apex
        /// </summary>
        private enum IntenSelectMode { CONE_FROM_HAND, CONE_FROM_HEAD };
        [SerializeField]
        private IntenSelectMode currentMode = IntenSelectMode.CONE_FROM_HEAD;

        /// <summary>
        /// Dictionnary where objects considered by the Intenselect algorithm are stored
        /// The key is the id of the object, the value is its score
        /// </summary>
        private Dictionary<UMI3DNodeInstance, float> objectsToConsiderScoresDict;


        override public void InitDetector()
        {
            objectsToConsiderScoresDict = new Dictionary<UMI3DNodeInstance, float>();

            if (currentMode == IntenSelectMode.CONE_FROM_HEAD)
                pointerTransform = Camera.main.transform;
            else
                throw new System.NotImplementedException();

            coneSelector = new ConeZoneSelector(pointerTransform, coneAngle);
        }

        override public void ResetDetector()
        {
            objectsToConsiderScoresDict.Clear();
        }

        /// <summary>
        /// Clear the detector except for the passed object
        /// </summary>
        /// <param name="obj"></param>
        private void ResetExceptOne(UMI3DNodeInstance obj)
        {
            ResetDetector();
            objectsToConsiderScoresDict.Add(obj, 0);
        }

        /// <summary>
        /// Find the target of the use intention according to the IntentSelect algorithm 
        /// </summary>
        /// <returns>The intended object or null</returns>
        override public UMI3DNodeInstance PredictTarget()
        {
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
                    if (objectsToConsiderScoresDict[obj] <= scoreMin) //remove useless objects that are too far
                        objectsToConsiderScoresDict.Remove(obj);

                    else
                        objectsToConsiderScoresDict[obj] = ComputeScore(obj);
                }
            }

            var estimatedTargetPair = (from o in objectsToConsiderScoresDict
                                       where o.Value == objectsToConsiderScoresDict.Values.Max()
                                       select o).FirstOrDefault(); //find the object with the highest score

           return estimatedTargetPair.Key;
        }


        /// <summary>
        /// Compute the cumulative score according to the formula of IntentSelect
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Object's score</returns>
        private float ComputeScore(in UMI3DNodeInstance obj)
        {
            var vectorToObject = obj.transform.position - pointerTransform.position;

            float dperp = Mathf.Abs(Vector3.Dot(vectorToObject, pointerTransform.forward));
            float dproj = Vector3.Cross(vectorToObject, pointerTransform.forward).magnitude;

            if (dproj != 0)
            {
                var correctedAngle = Mathf.Atan2(Mathf.Pow(dproj, corrective_k), dperp) * 180/Mathf.PI;
                var variation = 1 - (correctedAngle / coneAngle);
                var newScore = objectsToConsiderScoresDict[obj] + variation;
                if (newScore>scoreMax) //Avoid float overflow and give reactivity to the detector
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