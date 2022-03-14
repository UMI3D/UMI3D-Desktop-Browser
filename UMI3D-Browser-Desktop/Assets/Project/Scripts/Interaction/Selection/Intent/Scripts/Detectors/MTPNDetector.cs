using Accord.Math.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace umi3d.cdk.interaction.selection.intent
{
    [CreateAssetMenu(fileName = "MTPNDetector", menuName = "UMI3D/Selection/Intent Detector/MTP (N)")]
    public class MTPNDetector : AbstractSelectionIntentDetector
    {
        private float coneAngle = 15;
        private Transform headTransform;

        public override void InitDetector(AbstractController controller)
        {
            pointerTransform = Camera.main.transform; //?
            headTransform = Camera.main.transform; //?
        }

        private struct Candidate
        {
            public int id;
            public int score;
        }

        public override InteractableContainer PredictTarget()
        {
            var coneSelector = new ConicZoneSelection(pointerTransform.position, pointerTransform.forward, coneAngle);

            var objInCone = coneSelector.GetObjectsInZone();
            var N = objInCone.Count;
            if (N == 0)
                return null;
            int[,] victoriesMatrix = new int[N, N]; ;

            Dictionary<InteractableContainer, int> objScores = new Dictionary<InteractableContainer, int>();
            foreach (var obj in objInCone)
                objScores.Add(obj, 0);

            var directionPointer = pointerTransform.rotation * new Vector3(0, 0, 1);
            var directionHead = headTransform.rotation * new Vector3(0, 0, 1);

            for (var i = 0; i < N; i++)
            {
                var directionObjPointer_i = objInCone[i].transform.position - pointerTransform.position;
                var directionObjHead_i = objInCone[i].transform.position - headTransform.position;

                var distObjPointer_i = Vector3.Distance(objInCone[i].transform.position, pointerTransform.position);
                var distObjHead_i = Vector3.Distance(objInCone[i].transform.position, headTransform.position);
                var dotPointer_i = Vector3.Dot(directionPointer, directionObjPointer_i);
                var dotHead_i = Vector3.Dot(directionHead, directionObjHead_i);

                for (var j = i + 1; j < N; j++)
                {
                    var directionObjPointer_j = objInCone[j].transform.position - pointerTransform.position;
                    var directionObjHead_j = objInCone[j].transform.position - headTransform.position;

                    var distObjPointer_j = Vector3.Distance(objInCone[j].transform.position, pointerTransform.position);
                    var distObjHead_j = Vector3.Distance(objInCone[j].transform.position, headTransform.position);
                    var dotPointer_j = Vector3.Dot(directionPointer, directionObjPointer_j);
                    var dotHead_j = Vector3.Dot(directionHead, directionObjHead_j);

                    float deltaDistPointer = (distObjPointer_i - distObjPointer_j) / (distObjPointer_i + distObjPointer_j);
                    float deltaDistHead = (distObjHead_i - distObjHead_j) / (distObjHead_i + distObjHead_j);
                    float deltaDotPointer = (dotPointer_i - dotPointer_j) / (dotPointer_i + dotPointer_j);
                    float deltaDotHead = (dotHead_i - dotHead_j) / (dotHead_i + dotHead_j);

                    float[] obj1VsObj2Metrics = new float[4] { deltaDistPointer, deltaDistHead, deltaDotPointer, deltaDotHead };

                    victoriesMatrix[i, j] = CompareObjects(obj1VsObj2Metrics);
                    victoriesMatrix[j, i] = -victoriesMatrix[i, j];
                    objScores[objInCone[i]] += victoriesMatrix[i, j];
                    objScores[objInCone[j]] += victoriesMatrix[j, i];
                }
            }

            var maxVictories = objScores.Values.Max();
            var candidates = objScores
                .Where(x => x.Value == maxVictories)
                .Select(x => new Candidate() { id = objInCone.IndexOf(x.Key), score = x.Value })
                .ToList();

            if (candidates.Count == 1) // no election needed
                return objInCone[candidates[0].id];
            else
                return objInCone[RandomisedCondorcet(candidates, victoriesMatrix).id];
        }

        private Candidate RandomisedCondorcet(List<Candidate> candidates, int[,] victoriesMatrix)
        {
            var n = candidates.Count;
            var arrayOfOnes = new double[n];
            for (var i = 0; i < n; i++)
                arrayOfOnes[i] = 1;

            var linearContraints = new List<LinearConstraint>();

            var probabilisticConstraint = new LinearConstraint(numberOfVariables: n)
            {
                CombinedAs = arrayOfOnes,
                ShouldBe = ConstraintType.EqualTo,
                Value = 1
            };
            linearContraints.Add(probabilisticConstraint);

            for (int k = 0; k < n; k++)
            {
                var condorcetConstraint = new LinearConstraint(numberOfVariables: n)
                {
                    CombinedAs = Array.ConvertAll<int, double>(victoriesMatrix.GetRow(k), x => x),
                    ShouldBe = ConstraintType.GreaterThanOrEqualTo,
                    Value = 0
                };
                linearContraints.Add(condorcetConstraint);
            }

            var constraints = new LinearConstraintCollection(linearContraints);
            var solver = new AugmentedLagrangian(n, constraints);

            bool success = solver.Minimize();

            double[] solution = solver.Solution;

            var random = new System.Random().NextDouble();

            double sum = 0;
            for (var i = 0; i < n; i++)
            {
                sum += solution[i];
                if (sum > random)
                    return candidates[i];
            }
            return candidates[0];
        }

        /// <summary>
        /// Return 1 if obj 1 is better than obj 2
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private int CompareObjects(float[] feature)
        {
            if (feature[3] <= -0.0025450674019402977)
            {
                if (feature[2] <= 7.408833018398592E-4)
                    return 1;
                else
                {
                    if (feature[3] <= -0.020021664396318098)
                    {
                        if (feature[2] <= 0.10092115841796978)
                            return 1;
                        else
                            if (feature[0] <= 0.15480301707930913)
                            return -1;
                        else
                            return 1;
                    }
                    else
                    {
                        if (feature[1] <= -0.01104516809311298)
                        {
                            if (feature[0] <= -0.2052921944952896)
                                return 1;
                            else
                                return -1;
                        }
                        else
                        {
                            if (feature[0] <= 0.13713481780255293)
                                return 1;
                            else
                                return -1;
                        }
                    }
                }
            }
            else
            {
                if (feature[2] <= 7.408833018398592E-4)
                {
                    if (feature[3] <= 0.01975389898082204)
                    {
                        if (feature[1] <= 0.05134989073560836)
                            if (feature[0] <= 0.048187132453235915)
                                return 1;
                            else
                                return -1;
                        else
                            return 1;
                    }
                    else
                    {
                        if (feature[2] <= -0.09848143369526666)
                        {
                            if (feature[0] <= -0.15357659194215778)
                                return -1;
                            else
                                return 1;
                        }
                        else
                            return -1;
                    }
                }
                else
                {
                    if (feature[3] <= 0.002477775456729103)
                    {
                        if (feature[0] <= -0.04112576167669935)
                        {
                            if (feature[1] <= -0.050005076025131726)
                                return -1;
                            else
                                return 1;
                        }
                        else
                            return -1;
                    }
                    else
                        return -1;
                }
            }
        }

        public override void ResetDetector()
        {
            return;
        }
    }

    public static class ArrayExtensions
    {
        public static T[,] GetMatrixRows<T>(this T[,] array, List<int> rowsIds)
        {
            var n = array.Length;

            var newArray = new T[array.GetLength(0), n];

            foreach (var i in rowsIds)
            {
                for (var j = 0; j < n; j++)
                {
                    newArray[i, j] = array[i, j];
                }
            }
            return newArray;
        }

        public static T[,] GetMatrixColumns<T>(this T[,] array, List<int> colIds)
        {
            var n = array.Length;

            var newArray = new T[n, array.GetLength(1)];

            foreach (var i in colIds)
            {
                for (var j = 0; j < n; j++)
                {
                    newArray[j, i] = array[j, i];
                }
            }
            return newArray;
        }

        public static T[] GetRow<T>(this T[,] array, int rowId)
        {
            var n = array.Length;
            var newArray = new T[n];

            for (var j = 0; j < n; j++)
            {
                newArray[j] = array[rowId, j];
            }
            return newArray;
        }
    }
}