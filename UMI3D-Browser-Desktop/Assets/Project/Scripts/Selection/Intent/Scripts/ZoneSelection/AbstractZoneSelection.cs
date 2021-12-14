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
using System.Linq;
using UnityEngine;

namespace umi3d.cdk.interaction.selection
{
    /// <summary>
    /// A Zone Selector to define a zone and select objects within
    /// </summary>
    public abstract class AbstractZoneSelection
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