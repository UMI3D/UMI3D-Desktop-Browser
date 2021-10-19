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
using UnityEngine;
using MathNet.Numerics;
using System.Linq;
using umi3d.cdk.interaction;

namespace BrowserDesktop.Selection.Intent
{
    /// <summary>
    /// Implementation of a selection intent detector using the refined version of Kinematic Endpoint Prediction extanded in 3D, 
    /// from Lank et al. 2007 and Ruiz et al. 2009. The 3D extansion is an original work.
    /// </summary>
    [CreateAssetMenu(fileName = "RefinedKEPDetector", menuName = "UMI3D/Selection/Intent Detector/Refined KEP")]
    public class RefinedKEPDetector : AbstractSelectionIntentDetector
    {
        private LinkedList<double> angularDistanceData;
        private LinkedList<double> angularSpeedData;
        private LinkedList<Vector3> angularVectors;

        private Vector3 initialDirection;

        /// <summary>
        /// under this value, the movement is considered as stable
        /// </summary>
        [SerializeField]
        private double stabilityThreshold = 0.02;

        /// <summary>
        /// Precision of roots computation in the quadratic polynomial
        /// </summary>
        [SerializeField]
        private double rootsPrecision = 0.0001;

        /// <summary>
        /// Minimal number of sample before an extrapolation could be made
        /// </summary>
        [SerializeField]
        private int minimalNumberOfSamples = 20;

        /// <summary>
        /// Cone angle in degrees, correspond to the half of the full angle at its apex
        /// </summary>
        [SerializeField]
        private float coneAngle = 15;

        private bool predictionReady = false;

        private InteractableContainer lastEstimated = null;

        private Vector3 lastRotation;

        public override void InitDetector(AbstractController controller)
        {
            angularDistanceData = new LinkedList<double>();
            angularSpeedData = new LinkedList<double>();

            angularVectors = new LinkedList<Vector3>();

            pointerTransform = Camera.main.transform;

            lastRotation = pointerTransform.rotation.eulerAngles;

            initialDirection = pointerTransform.forward.normalized;

            predictionReady = false;
        }

        public override void ResetDetector()
        {
            angularDistanceData.Clear();
            angularSpeedData.Clear();
            angularVectors.Clear();
            initialDirection = pointerTransform.forward.normalized;
            predictionReady = false;
        }

        public override InteractableContainer PredictTarget()
        {
            var angleVector = pointerTransform.rotation.eulerAngles - lastRotation;
            var angleDelta = angleVector.magnitude;

            if (angularDistanceData.Count >= minimalNumberOfSamples)
            {
                var stability = angleDelta / angularDistanceData.Last.Value;
                if (stability < stabilityThreshold) //if the mouvement is stable enough, can compute the prediction (Refined KEP, Ruiz 2009)
                    predictionReady = true;
            }

            angularDistanceData.AddLast(angleDelta); //accumulating data for computation
            angularSpeedData.AddLast(angleDelta / Time.deltaTime);
            angularVectors.AddLast(angleVector);

            lastRotation = pointerTransform.rotation.eulerAngles;
            lastRotation = new Vector3(lastRotation.x, lastRotation.y, lastRotation.z);

            if (predictionReady)
            {
                Vector3 estimatedDirection = estimateKinematicEndpoint();
                var estimatedConicZone = new ConicZoneSelection(pointerTransform.position, estimatedDirection, coneAngle);
                var objsInZone = estimatedConicZone.GetObjectsInZone();

                if (objsInZone.Count == 0)
                {
                    lastEstimated = null;
                    return null;
                }
                else
                {
                    var estimatedObject = estimatedConicZone.GetClosestObjectToRay(objsInZone);
                    lastEstimated = estimatedObject;
                    ResetDetector();
                    return estimatedObject;
                }
            }
            else
            {
                return lastEstimated;
            }
        }

        /// <summary>
        /// Estimate the total angular distance that will be achieved
        /// </summary>
        /// <returns></returns>
        private double estimateAngularDistance()
        {
            var order = 4;
            double[] polynomialCoeffs = Fit.Polynomial(angularDistanceData.ToArray(), angularSpeedData.ToArray(), order); //least squares fitting
            var roots = FindRoots.Polynomial(polynomialCoeffs);

            double distanceEstimated = (from r in roots
                                        where r.IsReal() && (r.Norm() > rootsPrecision)
                                        orderby r.Real
                                        ascending
                                        select r.Real).FirstOrDefault(); //finding the 2nd real root

            return distanceEstimated;
        }


        /// <summary>
        /// Estimate the endpoint using kinematics.
        /// Makes the hypothesis that the endpoint would be along the average direction
        /// </summary>
        /// <returns></returns>
        private Vector3 estimateKinematicEndpoint()
        {
            var distance = estimateAngularDistance();
            var direction = angularVectors.Aggregate(new Vector3(0, 0, 0), (sum, next) => sum + next).normalized;
            var estimatedRotation = (float)distance * direction;
            
            return Quaternion.Euler(estimatedRotation) * initialDirection; //?
        }


    }
}