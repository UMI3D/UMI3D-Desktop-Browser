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
using UnityEngine;


namespace umi3d.cdk.interaction.selection.intent
{
    /// <summary>
    /// Simple hover detector that performs a ray-cast
    /// </summary>
    [CreateAssetMenu(fileName = "IntenSelectDetector", menuName = "UMI3D/Selection/Intent Detector/Hover")]
    public class HoverDetector : AbstractSelectionIntentDetector
    {

        public override void InitDetector(AbstractController controller)
        {
            pointerTransform = Camera.main.transform; //?
        }

        public override InteractableContainer PredictTarget()
        {
            var raySelection = new RayZoneSelection(pointerTransform.position, pointerTransform.forward);

            var closestActiveInteractable = raySelection.GetClosestObjectOnRay();

            return closestActiveInteractable;
        }

        public override void ResetDetector()
        {
        }

    }
}
