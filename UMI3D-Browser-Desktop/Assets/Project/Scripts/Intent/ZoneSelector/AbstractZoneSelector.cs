using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3d.cdk;
using umi3d.cdk.interaction;
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
        public List<InteractableContainer> GetInteractableObjectInScene()
        {
            var interactableObjectsInScene = Object.FindObjectsOfType<InteractableContainer>().ToList(); //find interactable objects in scene
            return interactableObjectsInScene;
        }

        /// <summary>
        /// Checks wheter an object is in the defined zone or not
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract bool IsObjectInZone(InteractableContainer obj);

        /// <summary>
        /// Get all objects within the zone defined by the zone selector
        /// </summary>
        /// <returns></returns>
        public abstract List<InteractableContainer> GetObjectsInZone();
    }
}
