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
using Newtonsoft.Json;

namespace umi3d.cdk.interaction.selection.intent
{
    /// <summary>
    /// Implementation of a selection intent detector using the refined version of Kinematic Endpoint Prediction extanded in 3D, 
    /// from Lank et al. 2007 and Ruiz et al. 2009. The 3D expansion is an original work.
    /// </summary>
    [CreateAssetMenu(fileName = "RefinedKEPDetector", menuName = "UMI3D/Selection/Intent Detector/Refined KEP")]
    public class RefinedKEPDetector : AbstractSelectionIntentDetector
    {
        /// <summary>
        /// Store angular movements amplitude, speed and associated rotation
        /// </summary>
        protected class RotationDataSample
        {
            public double amplitude;
            public double speed;
            public Quaternion rotation;
        }

        /// <summary>
        /// Store accumulated angular data used to predict by regression
        /// </summary>
        protected LinkedList<RotationDataSample> rotationData;

        /// <summary>
        /// under this value, the movement is considered as stable
        /// </summary>
        [SerializeField]
        protected double stabilityThreshold = 0.02;

        /// <summary>
        /// Precision of roots computation in the quadratic polynomial
        /// </summary>
        [SerializeField]
        protected double amplitudeMinimum = 0.1;

        /// <summary>
        /// Precentage of the movement that should be completed before making a prediction
        /// </summary>
        [SerializeField]
        protected float targetDistanceThreshold = 0.80f;

        /// <summary>
        /// Minimal number of sample before an extrapolation could be made
        /// </summary>
        [SerializeField]
        protected int minimalNumberOfSamples = 20;

        /// <summary>
        /// Cone angle in degrees, correspond to the half of the full angle at its apex
        /// </summary>
        [SerializeField]
        protected float coneAngle = 15;

        /// <summary>
        /// Last predicted object. Makes it possible to select an object during more than one frame 
        /// </summary>
        protected InteractableContainer lastPredicted = null;

        /// <summary>
        /// Last rotation state
        /// </summary>
        protected Quaternion lastRotation;

        /// <summary>
        /// Total amplitude of the current movement
        /// </summary>
        protected double totalAmplitude;

        public override void InitDetector(AbstractController controller)
        {
            rotationData = new LinkedList<RotationDataSample>();

            pointerTransform = Camera.main.transform;

            lastRotation = pointerTransform.rotation;
            totalAmplitude = 0;
        }

        public override void ResetDetector()
        {
            rotationData.Clear();
            totalAmplitude = 0;
        }

        public override InteractableContainer PredictTarget()
        {
            var estimationReady = false;

            var newRotation = pointerTransform.rotation;
            var deltaAngle = Quaternion.Angle(newRotation, lastRotation);
            totalAmplitude += deltaAngle;

            if (deltaAngle == 0 && rotationData.Count > 1 && rotationData.Last.Value.amplitude == 0) // case where the movement is stopped for at least two frames, stops the prediction
            {
                InteractableContainer predictedInteractable = GetClosestToRay(pointerTransform.forward);
                lastPredicted = predictedInteractable;
                ResetDetector();
                return lastPredicted;
            }

            if (rotationData.Count >= minimalNumberOfSamples)
            {
                var lastAngleMovement = rotationData.Last.Value.amplitude;
                var stability = deltaAngle / lastAngleMovement;
                if (stability < stabilityThreshold) //if the mouvement is stable enough, can compute the prediction (Refined KEP, Ruiz 2009)
                    estimationReady = true;
            }

            var angularSample = new RotationDataSample() //KEP data of the current state
            {
                amplitude = totalAmplitude,
                speed = deltaAngle / Time.deltaTime,
                rotation = newRotation
            };

            rotationData.AddLast(angularSample); //accumulating data for computation
            lastRotation = new Quaternion(pointerTransform.rotation.x, pointerTransform.rotation.y, pointerTransform.rotation.z, pointerTransform.rotation.w);

            if (estimationReady)
            {
                var estimatedAmplitude = EstimateKinematicEndpointAmplitude(rotationData.Select(x=>x.amplitude), rotationData.Select(x=>x.speed));
                if (estimatedAmplitude == 0) // no strictly positive root found
                    return lastPredicted;

                var currentMovementAmplitude = rotationData.Select(x => x.amplitude).Sum();
                if (currentMovementAmplitude / estimatedAmplitude < targetDistanceThreshold) //the precentage of the completed movement should be above the threshold, otherwise the prediction is inaccurate
                    return lastPredicted;

                //Makes the hypothesis that it will be along the average rotation, with a linear increasing weigth
                var rotationDirection = GetWeightedAverageDirection(rotationData.Select(x => x.rotation.eulerAngles).ToList());
 
                Quaternion predictedRotation = rotationData.First.Value.rotation * Quaternion.Euler((float)estimatedAmplitude * rotationDirection);

                Vector3 predictedDirection = (predictedRotation * new Vector3(0,0,1)).normalized; //should be initialPosition
                var predictedInteractable = GetClosestToRay(predictedDirection);
                lastPredicted = predictedInteractable;

                //save data
                //ExportDataAsJSON(estimatedAmplitude, rotationDirection, predictedRotation);

                ResetDetector();

                //Draw debug rays
                //Debug.DrawRay(pointerTransform.position, pointerTransform.forward, Color.gray, 3.0f, false);
                //Debug.DrawRay(pointerTransform.position, predictedDirection, Color.red, 3.0f, false);
                //if (predictedInteractable != null)
                //    Debug.DrawLine(pointerTransform.position, predictedInteractable.transform.position, Color.blue, 3.0f, true);

                return predictedInteractable;
            }
            else
            {
                return lastPredicted;
            }

        }

        protected InteractableContainer GetClosestToRay(Vector3 estimatedDirection)
        {
            var estimatedConicZone = new ConicZoneSelection(pointerTransform.position, estimatedDirection, coneAngle);
            var objsInZone = estimatedConicZone.GetObjectsInZone();

            if (objsInZone.Count == 0)
            {
                return null;
            }
            else
            {
                var estimatedObject = estimatedConicZone.GetClosestObjectToRay(objsInZone);
                return estimatedObject;
            }
        }

        /// <summary>
        /// Compute an average vector with a linearily increasing ponderation
        /// </summary>
        /// <returns></returns>
        protected Vector3 GetWeightedAverageDirection(List<Vector3> vector3s)
        {
            var rotationDirection = new Vector3();
            var last = new Vector3();

            float dataNumber = vector3s.Count;
            float weightStep = 2f / ((dataNumber - 1f) * dataNumber);
            float weight = 0f; //linearily increasing weight
            foreach (var d in vector3s.Skip(1))
            {
                weight += weightStep;
                rotationDirection += (d - last).normalized * weight;
                last = d;
            }
            return rotationDirection.normalized;
        }

        /// <summary>
        /// Estimate the endpoint using kinematics.
        /// Estimate the total angular distance that will be achieved
        /// </summary>
        /// <returns></returns>
        protected double EstimateKinematicEndpointAmplitude(IEnumerable<double> amplitudeList, IEnumerable<double> speedList)
        {
            var order = 4;
            try
            {
                double[] polynomialCoeffs = Fit.Polynomial(amplitudeList.ToArray(), speedList.ToArray(), order); //least squares fitting
                var roots = FindRoots.Polynomial(polynomialCoeffs);

                double estimatedAmplitude = (from r in roots
                                             where r.IsReal() && r.Real > amplitudeMinimum
                                             orderby r.Real
                                             descending
                                             select r.Real).FirstOrDefault(); //finding the 2nd real root which is strictly positive

                return estimatedAmplitude;
            }
            catch (NonConvergenceException)
            {
                return default;
            }
            
        }

        /// <summary>
        /// Debug function that exports data in JSON files
        /// </summary>
        /// <param name="estimatedAmplitude"></param>
        /// <param name="rotationDirection"></param>
        /// <param name="predictedRotation"></param>
        protected virtual void ExportDataAsJSON(double estimatedAmplitude, Vector3 rotationDirection, Quaternion predictedRotation)
        {
            var path = @"D:\rotationDataKEP\";

            var fileNameRotationData = "datarotation";
            var number = System.IO.Directory.GetFiles(path).Where(f => f.StartsWith(path + fileNameRotationData)).Count();
            using (System.IO.StreamWriter file = System.IO.File.CreateText(path + fileNameRotationData + number.ToString() + ".json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, rotationData.Select(x => new {
                    x.amplitude,
                    x.speed,
                    r_x = x.rotation.eulerAngles.x,
                    r_y = x.rotation.eulerAngles.y
                }).ToList());
            }

            var fileNameRotationEstimation = "estimation";
            number = System.IO.Directory.GetFiles(path).Where(f => f.StartsWith(path + fileNameRotationEstimation)).Count();
            using (System.IO.StreamWriter file = System.IO.File.CreateText(path + fileNameRotationEstimation + number.ToString() + ".json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, new
                {
                    estimatedAmplitude,
                    rotDir = rotationDirection.ToString(),
                    rpred_x = predictedRotation.eulerAngles.x,
                    rpred_y = predictedRotation.eulerAngles.y
                });
            }
        }


    }
}