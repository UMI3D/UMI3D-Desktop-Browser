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

namespace umi3d.common
{
    /// <summary>
    /// Dto for an association linking together two objects in the 3D space
    /// </summary>
    [System.Serializable]
    public class BindingDto : AbstractOperationDto
    {
        public BindingDto()
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="boundNodeId">An identifier defined by the designer.</param>
        /// <param name="active">Is it activated</param>
        /// <param name="data">A ref to the Dto containing all the information about the said binding or bindings</param>
        public BindingDto(ulong boundNodeId, AbstractBindingDataDto data)
        {
            this.boundNodeId = boundNodeId;
            this.data = data;
        }

        /// <summary>
        /// An identifier defined by the designer.
        /// </summary>
        public ulong boundNodeId { get; private set; }

        /// <summary>
        /// A ref to the Dto containing all the information about the said binding or bindings
        /// </summary>
        public AbstractBindingDataDto data { get; private set; }
    }
}