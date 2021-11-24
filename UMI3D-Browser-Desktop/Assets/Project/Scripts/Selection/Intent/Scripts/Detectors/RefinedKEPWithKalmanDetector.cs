using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Linq;
using Newtonsoft.Json;
using KalmanFilter;

namespace umi3d.cdk.interaction.selection.intent
{
    /// <summary>
    /// Implementation of a selection intent detector using the refined version of Kinematic Endpoint Prediction extanded in 3D, 
    /// from Lank et al. 2007 and Ruiz et al. 2009. The 3D expansion is an original work.
    /// </summary>
    [CreateAssetMenu(fileName = "RefinedKEPWithKalmanDetector", menuName = "UMI3D/Selection/Intent Detector/Refined KEP + Kalman")]
    public class RefinedKEPWithKalmanDetector : RefinedKEPDetector
    {
        protected LinearKalmanFilter kalmanFilter;

        protected List<float> filteredData = new List<float>();

        protected List<float> predictedData = new List<float>();

        protected float predictionAmplitudeKalman = 0;

        protected class RotationDataSampleKalman : RotationDataSample
        {
            public double estimatedAmplitude;
            public double predictedAmplitude;
            public Vector3 directionRotation;
            public float t = 0;
        }

        [Header("Kalman filter parameters")]
        /// <summary>
        /// Standard deviation of the mouvement's noise
        /// </summary>
        [SerializeField]
        protected float movementNoiseStd;

        /// <summary>
        /// Standard deviation of the observation model
        /// </summary>
        [SerializeField]
        protected float observationNoiseStd;

        public override void InitDetector(AbstractController controller)
        {
            base.InitDetector(controller);

            var processModelInit = Matrix.Build.DenseIdentity(5, 5);
            var observationModel = Matrix.Build.Dense(1, 5, 0);
            observationModel[0, 0] = 1;

            kalmanFilter = new LinearKalmanFilter(movementNoiseStd, observationNoiseStd, processModelInit, observationModel);

            kalmanFilter.InitWithGuessed(new double[5] { 0, 0, 0, 0, 0 });

            var prediction = kalmanFilter.Predict();
            predictedData.Add((float)prediction[0]);
        }

        public override void ResetDetector()
        {
            base.ResetDetector();
            filteredData.Clear();
            predictedData.Clear();
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

            //UPDATE
            var measure = new double[] { totalAmplitude };
            var estimatedAmplitudeKalman = (float)kalmanFilter.Update(measure)[0];
            filteredData.Add(estimatedAmplitudeKalman);

            static Vector3 to180deg(Vector3 vec)
            {
                System.Func<float, float> to180 = x => x % 360 > 180 ? x - 360 : x;
                return new Vector3(to180(vec.x), to180(vec.y), to180(vec.z));
            }

            var rotationDirection = (rotationData.Last == null) ? new Vector3() : (to180deg(newRotation.eulerAngles) - to180deg(rotationData.Last.Value.rotation.eulerAngles)).normalized;
            var angularSample = new RotationDataSampleKalman() //KEP data of the current state
            {
                amplitude = totalAmplitude,
                speed = deltaAngle / Time.deltaTime,
                rotation = newRotation,
                estimatedAmplitude = estimatedAmplitudeKalman,
                directionRotation = rotationDirection,
                predictedAmplitude = predictionAmplitudeKalman,
                t = Time.deltaTime
            };

            rotationData.AddLast(angularSample); //accumulating data for computation
            lastRotation = new Quaternion(pointerTransform.rotation.x, pointerTransform.rotation.y, pointerTransform.rotation.z, pointerTransform.rotation.w);

            //PREDICT
            var processModel = GetProcessModel(Time.deltaTime); // assumption : next frame will be in duration dt
            predictionAmplitudeKalman = (float)kalmanFilter.Predict(processModel)[0];
            predictedData.Add(predictionAmplitudeKalman);


            if (estimationReady)
            {
                var estimatedAmplitude = EstimateKinematicEndpointAmplitude(rotationData.Select(x=>((RotationDataSampleKalman)x).estimatedAmplitude), rotationData.Select(x=>x.speed));
                if (estimatedAmplitude == 0) // no strictly positive root found
                    return lastPredicted;

                var currentMovementAmplitude = rotationData.Select(x => x.amplitude).Sum();
                if (currentMovementAmplitude / estimatedAmplitude < targetDistanceThreshold) //the precentage of the completed movement should be above the threshold, otherwise the prediction is inaccurate
                    return lastPredicted;

                //Makes the hypothesis that it will be along the average rotation, with a linear increasing weigth
                var rotationAverageDirection = GetWeightedAverageDirection(rotationData.Select(x => x.rotation.eulerAngles).ToList());

                Quaternion predictedRotation = rotationData.First.Value.rotation * Quaternion.Euler((float)estimatedAmplitude * rotationAverageDirection);

                //looking for pointed direction and interactable
                Vector3 predictedDirection = (predictedRotation * new Vector3(0, 0, 1)).normalized; //should be initialPosition
                var predictedInteractable = GetClosestToRay(predictedDirection);
                lastPredicted = predictedInteractable;

                //save data
                ExportDataAsJSON(estimatedAmplitude, rotationAverageDirection, predictedRotation);

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

        private double[][] GetProcessModel(float dt)
        {
            var dt2 = dt * dt;
            var dt3 = dt2 * dt;
            var dt4 = dt3 * dt;
            var a = 0.5 * dt2;
            var b = (1 / 6) * dt3;
            double[][] model = new double[5][]
            {
                new double [5] { 1, dt, a,  b,  (1/24) * dt4 },
                new double [5] { 0, 1,  dt, a,  b },
                new double [5] { 0, 0,  1,  dt, a },
                new double [5] { 0, 0,  0,  1,  dt },
                new double [5] { 0, 0,  0,  0,  1 }
            };
            return model;
        }

        /// <summary>
        /// Debug function that exports data in JSON files
        /// </summary>
        /// <param name="estimatedAmplitude"></param>
        /// <param name="rotationDirection"></param>
        /// <param name="predictedRotation"></param>
        protected override void ExportDataAsJSON(double estimatedAmplitude, Vector3 rotationDirection, Quaternion predictedRotation)
        {

            var path = @"D:\rotationDataKEP\kalman\";

            var fileNameRotationPrediction = "datarotationKalman";
            var number = System.IO.Directory.GetFiles(path).Where(f => f.StartsWith(path + fileNameRotationPrediction)).Count();
            using (System.IO.StreamWriter file = System.IO.File.CreateText(path + fileNameRotationPrediction + number.ToString() + ".json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, rotationData.Select(d => new
                {
                    d.amplitude,
                    d.speed,
                    r_x = d.rotation.eulerAngles.x,
                    r_y = d.rotation.eulerAngles.y,
                    estimated_amp = ((RotationDataSampleKalman)d).estimatedAmplitude,
                    predicted_amp = ((RotationDataSampleKalman)d).predictedAmplitude,
                    delta_r_x = ((RotationDataSampleKalman)d).directionRotation.x,
                    delta_r_y = ((RotationDataSampleKalman)d).directionRotation.y,
                    time = ((RotationDataSampleKalman)d).t
                }).ToList()); ;
            }

        }
    }

    
}
