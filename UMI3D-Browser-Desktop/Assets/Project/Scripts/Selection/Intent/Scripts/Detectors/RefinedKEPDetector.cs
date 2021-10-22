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
        /// <summary>
        /// Store angular movements amplitude, speed and associated rotation
        /// </summary>
        private class AngularDataSample
        {
            public double amplitude;
            public double speed;
            public Vector3 eulerRotation;
        }

        /// <summary>
        /// Store accumulated angular data used to predict by regression
        /// </summary>
        private LinkedList<AngularDataSample> angularData;

        /// <summary>
        /// Initial direction of the pointer vector at the start of the prediction
        /// </summary>
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
        private double amplitudeMinimum = 0.1;

        /// <summary>
        /// Precentage of the movement that should be completed before making a prediction
        /// </summary>
        [SerializeField]
        private float targetDistanceThreshold = 0.80f;

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

        private bool estimationReady = false;

        private InteractableContainer lastEstimated = null;

        private Vector3 lastRotation;

        public override void InitDetector(AbstractController controller)
        {
            angularData = new LinkedList<AngularDataSample>();

            pointerTransform = Camera.main.transform;

            lastRotation = pointerTransform.rotation.eulerAngles;

            initialDirection = pointerTransform.forward.normalized;

            estimationReady = false;
        }

        public override void ResetDetector()
        {
            angularData.Clear();
            initialDirection = pointerTransform.forward.normalized;
            estimationReady = false;
        }

        public override InteractableContainer PredictTarget()
        {
            var deltaRotation = pointerTransform.rotation.eulerAngles - lastRotation;
            var deltaAngle = deltaRotation.magnitude;

            if (deltaAngle == 0 && angularData.Count > 1)
            {
                return GetClosestToRay(pointerTransform.forward);
            }

            if (angularData.Count >= minimalNumberOfSamples)
            {
                var lastAngleMovement = angularData.Last.Value.amplitude;
                var stability = deltaAngle / lastAngleMovement;
                if (stability < stabilityThreshold) //if the mouvement is stable enough, can compute the prediction (Refined KEP, Ruiz 2009)
                    estimationReady = true;
            }

            var angularSample = new AngularDataSample();
            angularSample.amplitude = deltaAngle;
            angularSample.speed = deltaAngle / Time.deltaTime;
            angularSample.eulerRotation = deltaRotation;
            angularData.AddLast(angularSample); //accumulating data for computation

            lastRotation = pointerTransform.rotation.eulerAngles;
            lastRotation = new Vector3(lastRotation.x, lastRotation.y, lastRotation.z);

            if (estimationReady)
            {
                var estimatedAmplitude = estimateKinematicEndpointAmplitude();
                if (estimatedAmplitude == 0) // no strictly positive root found
                {
                    estimationReady = false;
                    return null;
                }
                    
                var currentMovementAmplitude = angularData.Select(x => x.amplitude).Sum();
                if (currentMovementAmplitude / estimatedAmplitude < targetDistanceThreshold) //the precentage of the completed movement should be above the threshold, otherwise the prediction is inaccurate
                {
                    estimationReady = false;
                    return null;
                }

                //Makes the hypothesis that it will be along the average rotation
                var direction = angularData.Select(x => x.eulerRotation).Aggregate(new Vector3(0, 0, 0), (sum, next) => sum + next).normalized; 
                var predictedRotation = (float)estimatedAmplitude * direction;

                var predictedDirection = (Quaternion.Euler(predictedRotation) * initialDirection).normalized;
                var predictedInteractable = GetClosestToRay(predictedDirection);
                return predictedInteractable;
            }
            else
            {
                return lastEstimated;
            }
        }

        private InteractableContainer GetClosestToRay(Vector3 estimatedDirection)
        {
            var estimatedConicZone = new ConicZoneSelection(pointerTransform.position, estimatedDirection, coneAngle);
            var objsInZone = estimatedConicZone.GetObjectsInZone();

            if (objsInZone.Count == 0)
            {
                lastEstimated = null;
                ResetDetector();
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

        /// <summary>
        /// Estimate the endpoint using kinematics.
        /// Estimate the total angular distance that will be achieved
        /// </summary>
        /// <returns></returns>
        private double estimateKinematicEndpointAmplitude()
        {
            var order = 4;
            double[] polynomialCoeffs = Fit.Polynomial(angularData.Select(x=>x.amplitude).ToArray(), angularData.Select(x => x.speed).ToArray(), order); //least squares fitting
            var roots = FindRoots.Polynomial(polynomialCoeffs);

            double estimatedAmplitude = (from r in roots
                                         where r.IsReal() && r.Real > amplitudeMinimum
                                         orderby r.Real
                                         ascending
                                         select r.Real).FirstOrDefault(); //finding the 2nd real root which is strictly positive

            return estimatedAmplitude;
        }


    }
}