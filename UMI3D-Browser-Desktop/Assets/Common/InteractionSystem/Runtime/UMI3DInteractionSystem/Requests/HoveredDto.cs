﻿/*
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

using System;

namespace umi3d.common.interaction
{
    /// <summary>
    /// Dto for hover event request from browser.
    /// </summary>
    public class HoveredDto : InteractionRequestDto
    {

        /// <summary>
        /// Hovered point position in the Interactable associated object's local frame.
        /// </summary>
        public SerializableVector3 position;

        /// <summary>
        /// Normal to the object's surface at the hovered point in the Interactable associated object's local frame.
        /// </summary>
        public SerializableVector3 normal;

        /// <summary>
        /// The direction of the browser's selection tool in the Interactable associated object's local frame.
        /// </summary>
        public SerializableVector3 direction;

        protected override uint GetOperationId() { return UMI3DOperationKeys.Hoverred; }

        public override (int, Func<byte[], int, int, (int, int)>) ToByteArray(int baseSize, params object[] parameters)
        {
            var fb = base.ToByteArray(baseSize, parameters);

            int size = UMI3DNetworkingHelper.GetSize(position) + UMI3DNetworkingHelper.GetSize(normal) + UMI3DNetworkingHelper.GetSize(direction) + fb.Item1;
            Func<byte[], int, int, (int, int)> func = (b, i, bs) =>
            {
                (i,bs) = fb.Item2(b, i, bs);
                bs += UMI3DNetworkingHelper.Write(position, b,ref i);
                bs += UMI3DNetworkingHelper.Write(normal, b, ref i);
                bs += UMI3DNetworkingHelper.Write(direction, b, ref i);
                return (i,bs);
            };
            return (size, func);
        }
    }
}