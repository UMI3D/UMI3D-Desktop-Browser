using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.interaction;
using umi3d.cdk.interaction.selection;
using umi3d.cdk.interaction.selection.intent;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "MTPAggDetector", menuName = "UMI3D/Selection/Intent Detector/MTP (Agg)")]
public class MTPAggDetector : AbstractSelectionIntentDetector
{
    float coneAngle = 22.5f;
    Transform headTransform;

    public override void InitDetector(AbstractController controller)
    {
        pointerTransform = Camera.main.transform; //?
        headTransform = Camera.main.transform; //?
    }

    private class Candidate
    {
        public InteractableContainer objContainer;
        public float distPointer;
        public float distHead;
        public float dotPointer;
        public float dotHead;
        public float dotHeadPointer;

        public float[] ToArray()
        {
            return new float[5] { distPointer, distHead, dotPointer, dotHead, dotHeadPointer };
        }
    }


    public override InteractableContainer PredictTarget()
    {
        var coneSelector = new ConicZoneSelection(pointerTransform.position, pointerTransform.forward, coneAngle);

        var objInCone = coneSelector.GetObjectsInZone();

        var candidatesObjects = new List<Candidate>();

        
        foreach (var obj in objInCone)
        {
            var directionPointer = pointerTransform.rotation * new Vector3(0, 0, 1);
            var directionHead = headTransform.rotation * new Vector3(0, 0, 1);

            var directionObjPointer = obj.transform.position - pointerTransform.position;
            var directionObjHead = obj.transform.position - headTransform.position;

            var metricsObj = new Candidate()
            {
                objContainer = obj,
                distPointer = Vector3.Distance(obj.transform.position, pointerTransform.position),
                distHead = Vector3.Distance(obj.transform.position, headTransform.position),
                dotPointer = Vector3.Dot(directionPointer, directionObjPointer),
                dotHead = Vector3.Dot(directionHead, directionObjHead),
                dotHeadPointer = Vector3.Dot(directionPointer, directionHead),
            };

            candidatesObjects.Add(metricsObj);
        }
        var N = candidatesObjects.Count();
        float metricDistPointerAvg = candidatesObjects.Select(x=> x.distPointer).Sum() / N;
        float metricDistHeadAvg = candidatesObjects.Select(x => x.distHead).Sum() / N;
        float metricDotPointerAvg = candidatesObjects.Select(x => Mathf.Abs(x.dotPointer)).Sum() / N;
        float metricDotHeadAvg = candidatesObjects.Select(x => Mathf.Abs(x.dotHead)).Sum() / N;
        float metricDotHeadPointerAvg = candidatesObjects.Select(x => Mathf.Abs(x.dotHeadPointer)).Sum() / N;

        var objChosen = candidatesObjects.Select(x =>
        {
            x.distPointer /= metricDistPointerAvg;
            x.distHead /= metricDistHeadAvg;
            x.dotPointer /= metricDotPointerAvg;
            x.dotHead /= metricDotHeadAvg;
            x.dotHeadPointer /= metricDotHeadPointerAvg;
            return x;
        })
            .Where(x => IsCandidate(x.ToArray()))
            .OrderBy(x => x.dotPointer)
            .FirstOrDefault();

        if (objChosen == default)
            return null;
        return objChosen.objContainer;
    }

    /// <summary>
    /// Decision tree
    /// </summary>
    /// <param name="features"></param>
    /// <returns></returns>
    private bool IsCandidate(float[] features)
    {
        if (features[2] <= 1.0441326508748237)
        {
            if (features[1] <= 1.0841183871523317)
                return true;
            else
            {
                if (features[1] <= 1.1661081723049755)
                {
                    if (features[2] <= 0.9271953647518172)
                    {
                        if (features[3] <= 0.6825226617598784)
                            return false;
                        else
                            return true;
                    }
                    else
                    {
                        if (features[3] <= 1.057771575696317)
                            return false;
                        else
                            return true;
                    }
                }
                else
                {
                    if (features[0] <= 0.8583929183258362)
                        return false;
                    else
                    {
                        if (features[1] <= 1.2865988475267023)
                            return false;
                        else
                            return true;
                    }
                }
            }             
        }   
        else
        {
            if (features[0] <= 0.9816072968564543)
            {
                if (features[1] <= 1.0871566138053033)
                    return true;
                else
                {
                    if (features[1] <= 1.1661081723049755)
                        if (features[4] <= 0.9013849325608254)
                            return true;
                        else
                            return false;
                    else
                        return false;
                }    
            }
            else
            {
                if (features[3] <= 0.9951182740345799)
                {
                    if (features[1] <= 0.9704350397518207)
                        return true;
                    else
                        return false;         
                }
                else
                {
                    if (features[1] <= 1.068143892260501)
                        return true;
                    else
                        return false;
                }
            }    
        }
    }

    public override void ResetDetector()
    {
        return;
    }

 
}
