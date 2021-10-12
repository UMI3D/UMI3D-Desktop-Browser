using System.Collections;
using System.Collections.Generic;
using umi3d.cdk;
using UnityEngine;
using MathNet.Numerics.RootFinding;
using MathNet.Numerics;
using System.Linq;


namespace BrowserDesktop.Intent
{
    [CreateAssetMenu(fileName = "RefinedKEPDetector", menuName = "UMI3D/Selection Intent Detector/Refined KEP ")]
    public class RefinedKEP : AbstractSelectionIntentDetector
    {
        private LinkedList<double> angularDistanceData;
        private LinkedList<double> angularSpeedData;
        private LinkedList<Vector3> angularVectors;

        [SerializeField]
        private double stabilityThreshold = 0.02;

        [SerializeField]
        private double rootsPrecision = 0.0001;

        [SerializeField]
        private int minimalNumberOfSamples = 20;

        private bool predictionReady = false;

        private Vector3 lastRotation;

        private ConeZoneSelector coneSelector;

        public override void InitDetector()
        {
            angularDistanceData = new LinkedList<double>();
            angularSpeedData = new LinkedList<double>();

            angularVectors = new LinkedList<Vector3>();

            pointerTransform = Camera.main.transform;

            coneSelector = new ConeZoneSelector(pointerTransform);

            lastRotation = pointerTransform.rotation.eulerAngles;

            predictionReady = false;
        }

        public override void ResetDetector()
        {
            angularDistanceData.Clear();
            angularSpeedData.Clear();
            angularVectors.Clear();
            predictionReady = false;
        }

        public override UMI3DNodeInstance PredictTarget()
        {

            var angleVector = (pointerTransform.rotation.eulerAngles - lastRotation);
            angleVector = new Vector3(angleVector.x % 360, angleVector.y % 360, angleVector.z % 360); //To change to center in [-180;180]
            var angleDelta = angleVector.magnitude;

            if (angularDistanceData.Count >= minimalNumberOfSamples)
            {
                var stability = angleDelta / angularDistanceData.Last.Value;
                if (stability < stabilityThreshold) //if the mouvement is stable enough, can compute the prediction (Refined KEP, Ruiz 2009)
                    predictionReady = true;
            }

            angularDistanceData.AddLast(angleDelta);
            angularSpeedData.AddLast(angleDelta / Time.deltaTime);
            angularVectors.AddLast(angleVector);

            lastRotation = pointerTransform.rotation.eulerAngles;
            lastRotation = new Vector3(lastRotation.x % 360, lastRotation.y % 360, lastRotation.z % 360);

            if (predictionReady)
            {
                Vector3 estimatedRay = estimateKinematicEndpoint();
                coneSelector.attachedPoint.forward = estimatedRay.normalized;
                var estimatedObject = coneSelector.GetClosestObjectToRay(coneSelector.GetObjectsInZone());
                ResetDetector();
                return estimatedObject;
            }
            else
            {
                return null;
            }
        

        }


        private double estimateAngularDistance()
        {
            var order = 4;
            double[] polynomialCoeffs = Fit.Polynomial(angularDistanceData.ToArray(), angularSpeedData.ToArray(), order); //least squares fitting
            var roots = FindRoots.Polynomial(polynomialCoeffs);

            double distanceEstimated = (from r in roots
                                       where r.IsReal() && (r.Norm() > rootsPrecision)
                                       orderby r.Real
                                       ascending
                                       select r.Real).First(); //finding the 2nd real root

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
            var direction = angularVectors.Aggregate(new Vector3(0,0,0), (sum,next)=> sum + next).normalized;
            var estimatedRotation = (float)distance * direction;
            return pointerTransform.rotation.eulerAngles; //?
        }


    }
}
