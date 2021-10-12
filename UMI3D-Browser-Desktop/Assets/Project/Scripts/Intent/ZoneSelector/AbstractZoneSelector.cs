using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3d.cdk;
using UnityEngine;

namespace BrowserDesktop.Intent
{
    /// <summary>
    /// A Zone Selector to define a zone and select objects within
    /// </summary>
    public abstract class AbstractZoneSelector
    {
        /// <summary>
        /// Get all the interactable object in the scene
        /// </summary>
        /// <returns></returns>
        public List<UMI3DNodeInstance> GetInteractableObjectInScene()
        {
            var interactableObjectsInScene = new List<UMI3DNodeInstance>();
            interactableObjectsInScene = (from e in UMI3DEnvironmentLoader.Entities()
                                          where e is UMI3DNodeInstance && (e as UMI3DNodeInstance).renderers.Count > 0
                                          select (UMI3DNodeInstance)e).ToList(); //find interactable objects in scene
            return interactableObjectsInScene;
        }

        /// <summary>
        /// Checks wheter an object is in the defined zone or not
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract bool IsObjectInZone(UMI3DNodeInstance obj);

        /// <summary>
        /// Get all objects within the zone defined by the zone selector
        /// </summary>
        /// <returns></returns>
        public abstract List<UMI3DNodeInstance> GetObjectsInZone();
    }
}
