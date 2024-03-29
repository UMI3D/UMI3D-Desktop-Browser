/*
Copyright 2019 - 2023 Inetum

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

namespace umi3d.common.userCapture.binding
{
    /// <summary>
    /// Rig-Bone binding data, required to load a binding between a bone and a model rig.
    /// </summary>
    [System.Serializable]
    public class RigBoneBindingDataDto : BoneBindingDataDto
    {
        /// <summary>
        /// Name of the rig
        /// </summary>
        public string rigName { get; set; }

        /// <summary>
        /// Specifying the need to use the initial rotation in the binding.
        /// </summary>
        public bool applyOriginalRotation { get; set; }
    }
}