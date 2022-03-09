using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.interaction;
using umi3d.cdk.interaction.selection;
using umi3d.cdk.interaction.selection.intent;
using UnityEngine;

[CreateAssetMenu(fileName = "MTPNDetector", menuName = "UMI3D/Selection/Intent Detector/MTP (N)")]
public class MTPNDetector : AbstractSelectionIntentDetector
{
    float coneAngle = 15;

    public override void InitDetector(AbstractController controller)
    {
        pointerTransform = Camera.main.transform; //?
    }

    public override InteractableContainer PredictTarget()
    {
        var coneSelector = new ConicZoneSelection(pointerTransform.position, pointerTransform.forward, coneAngle);

        var objInCone = coneSelector.GetObjectsInZone();

        return objInCone[0];
    }

    public override void ResetDetector()
    {
        return;
    }

 
}
