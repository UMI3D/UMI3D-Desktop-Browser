using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3d.cdk;
using UnityEngine;

public abstract class AbstractZoneSelector
{
    public List<UMI3DNodeInstance> GetInteractableObjectInScene()
    {
        var interactableObjectsInScene = new List<UMI3DNodeInstance>();
        interactableObjectsInScene = (from e in UMI3DEnvironmentLoader.Entities()
                                      where e is UMI3DNodeInstance && (e as UMI3DNodeInstance).renderers.Count > 0
                                      select (UMI3DNodeInstance)e).ToList(); //find interactable objects in scene
        return interactableObjectsInScene;
    }

    public abstract bool IsObjectInZone(UMI3DNodeInstance obj);

    public abstract List<UMI3DNodeInstance> GetObjectsInZone();
}
