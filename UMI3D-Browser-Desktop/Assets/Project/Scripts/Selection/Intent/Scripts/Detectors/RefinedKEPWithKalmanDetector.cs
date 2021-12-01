using System.Collections.Generic;
using UnityEngine;
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

        protected float predictionAmplitudeKalman = 0;

        protected class RotationDataSampleKalman : RotationDataSample
        {
            public double estimatedAmplitudeKalman;
            public double estimatedSpeedKalman;
            public double predictedAmplitudeKalman;
            public Vector3 directionRotation;
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

            kalmanFilter = RenewFilter();
        }

        public override void ResetDetector()
        {
            base.ResetDetector();

            kalmanFilter = RenewFilter();
        }

        /// <summary>
        /// Generate a filter
        /// </summary>
        /// <returns></returns>
        public LinearKalmanFilter RenewFilter()
        {
            var processModelInit = Matrix.Build.DenseIdentity(5, 5);
            var observationModel = Matrix.Build.Dense(2, 5, 0);
            observationModel[0, 0] = 1;
            observationModel[1, 1] = 1;

            var kalmanFilter = new LinearKalmanFilter(movementNoiseStd, observationNoiseStd, processModelInit, observationModel);

            kalmanFilter.InitWithGuessed(new double[5] { 0, 0, 0, 0, 0.1 });

            kalmanFilter.Predict();

            return kalmanFilter;
        }


        public override InteractableContainer PredictTarget()
        {
            if (rotationData.Count == 0)
            {
                var initSample = new RotationDataSampleKalman() //KEP data of the starting state
                {
                    amplitudeTotal = 0,
                    speed = 0,
                    rotation = pointerTransform.rotation,
                    deltaTime = Time.deltaTime,
                    directionRotation = new Vector3(),
                    estimatedAmplitudeKalman = 0,
                    predictedAmplitudeKalman = (float)kalmanFilter.StateEstimationPredicted[0],
                    estimatedSpeedKalman = 0
                };
                rotationData.AddLast(initSample);
            }

            var newRotation = pointerTransform.rotation;
            var deltaAngle = Quaternion.Angle(newRotation, lastRotation);
            totalAmplitude += deltaAngle;

            // case where the movement is stopped for at least two frames, stops the prediction
            if (deltaAngle == 0 && rotationData.Count > 1 && rotationData.Last.Value.speed == 0) 
            {
                InteractableContainer predictedInteractable = GetClosestToRay(pointerTransform.forward);
                lastPredicted = predictedInteractable;
                ResetDetector();
                return lastPredicted;
            }

            //UPDATE FILTER
            var measure = new double[] { totalAmplitude, deltaAngle / Time.deltaTime };
            var estimation = kalmanFilter.Update(measure);
            var estimatedAmplitudeKalman = (float)estimation[0];
            var estimatedSpeedKalman = (float)estimation[1];

            var rotationDirection = (rotationData.Last == null) ? new Vector3() : (to180deg(newRotation.eulerAngles) - to180deg(rotationData.Last.Value.rotation.eulerAngles)).normalized;
            var angularSample = new RotationDataSampleKalman() //KEP data of the current state
            {
                amplitudeTotal = totalAmplitude,
                speed = deltaAngle / Time.deltaTime,
                rotation = newRotation,
                deltaTime = Time.deltaTime,
                directionRotation = rotationDirection,
                estimatedAmplitudeKalman = estimatedAmplitudeKalman,
                predictedAmplitudeKalman = (float)kalmanFilter.StateEstimationPredicted[0],
                estimatedSpeedKalman = estimatedSpeedKalman
            };

            rotationData.AddLast(angularSample); //accumulating data for computation
            lastRotation = new Quaternion(pointerTransform.rotation.x, pointerTransform.rotation.y, pointerTransform.rotation.z, pointerTransform.rotation.w);

            //PREDICT
            var processModel = GetProcessModel(Time.deltaTime); // assumption : next frame will be in duration dt
            var prediction = kalmanFilter.Predict(processModel);

            return KEPPredictor(rotationData.Select(x => ((RotationDataSampleKalman)x).estimatedAmplitudeKalman), 
                                rotationData.Select(x => ((RotationDataSampleKalman)x).estimatedSpeedKalman), 
                                rotationData.Count);
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

        protected Vector3 to180deg(Vector3 vec)
        {
            System.Func<float, float> to180 = x => x % 360 > 180 ? x - 360 : x;
            return new Vector3(to180(vec.x), to180(vec.y), to180(vec.z));
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
                    amplitude = d.amplitudeTotal,
                    d.speed,
                    r_x = d.rotation.eulerAngles.x,
                    r_y = d.rotation.eulerAngles.y,
                    estimated_amp = ((RotationDataSampleKalman)d).estimatedAmplitudeKalman,
                    estimated_speed = ((RotationDataSampleKalman)d).estimatedSpeedKalman,
                    predicted_amp = ((RotationDataSampleKalman)d).predictedAmplitudeKalman,
                    estimated = ((RotationDataSampleKalman)d).predictedAmplitudeKalman,
                    delta_r_x = ((RotationDataSampleKalman)d).directionRotation.x,
                    delta_r_y = ((RotationDataSampleKalman)d).directionRotation.y,
                    time = ((RotationDataSampleKalman)d).deltaTime
                }).ToList()); ;
            }

        }
    }

    
}
