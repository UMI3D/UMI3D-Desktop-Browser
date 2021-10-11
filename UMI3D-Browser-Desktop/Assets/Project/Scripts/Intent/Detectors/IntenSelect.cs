using System.Collections.Generic;
using umi3d.cdk;
using UnityEngine;
using System.Linq;

namespace BrowserDesktop.Intent
{
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

        [Header("Score boundaries")]
        [SerializeField]
        private float scoreMax = 70;

        [SerializeField]
        private float scoreMin = -10;

        /// <summary>
        /// Cone angle in degrees, correspond to the half of the full angle at its apex
        /// </summary>
        private enum IntentSelectMode { CONE_FROM_HAND, CONE_FROM_HEAD };
        [SerializeField]
        private IntentSelectMode currentMode = IntentSelectMode.CONE_FROM_HAND;

        /// <summary>
        /// Dictionnary where objects considered by the Intenselect algorithm are stored
        /// The key is the id of the object, the value is its score
        /// </summary>
        private Dictionary<UMI3DNodeInstance, float> objectsToConsiderScoresDict;


        override public void InitDetector()
        {
            objectsToConsiderScoresDict = new Dictionary<UMI3DNodeInstance, float>();
            pointerTransform = Camera.main.transform;
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
            var interactableObjectsInScene = new List<UMI3DNodeInstance>();
            interactableObjectsInScene = (from e in UMI3DEnvironmentLoader.Entities()
                                          where e is UMI3DNodeInstance && (e as UMI3DNodeInstance).renderers.Count>0
                                          select (UMI3DNodeInstance)e).ToList(); //find interactable objects in scene


            foreach (var obj in interactableObjectsInScene)
            {
                var vectorToObject = obj.transform.position - pointerTransform.position;

                if (!objectsToConsiderScoresDict.ContainsKey(obj))
                {
                    if (Vector3.Dot(vectorToObject.normalized, pointerTransform.forward) > Mathf.Cos(coneAngle * Mathf.PI / 180)) // check whether the object is in the cone or not
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