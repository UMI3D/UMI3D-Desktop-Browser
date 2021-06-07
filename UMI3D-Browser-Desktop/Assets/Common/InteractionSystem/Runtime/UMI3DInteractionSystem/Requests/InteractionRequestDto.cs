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
    [System.Serializable]
    /// <summary>
    /// An abstract class any interaction request have to be derivated from.
    /// </summary>
    public abstract class InteractionRequestDto : AbstractBrowserRequestDto
    {

        /// <summary>
        /// Id of the interactable or tool (in the case of an interaction related to hoverring).
        /// </summary>
        public ulong toolId;

        /// <summary>
        /// Id of the interaction (in the case of an interaction related to hoverring).
        /// </summary>
        public ulong id;

        /// <summary>
        /// The id of the currently hoverred object.
        /// It will be always null for an Interaction inside a Tool.
        /// For an Interaction inside an Interactable, it could be the Id of the Interactable associated object, or the Id of a sub-object if Interaction.notifyHoverPosition == true.
        /// </summary>
        public ulong hoveredObjectId;

        /// <summary>
        /// The type of bone associated to the user's controller.
        /// </summary>
        public uint boneType;
        protected override uint GetOperationId() { return UMI3DOperationKeys.InteractionRequest; }

        public override (int, Func<byte[], int, int,(int,int)>) ToByteArray(int baseSize,params object[] parameters)
        {
            var fb = base.ToByteArray(baseSize,parameters);
            int size = fb.Item1 + 3 * sizeof(ulong) + UMI3DNetworkingHelper.GetSize(boneType);
            Func<byte[], int, int, (int, int)> func = (b, i, bs) =>
            {
                (i,bs)= fb.Item2(b, i,bs);
                bs += UMI3DNetworkingHelper.Write(toolId,b,ref i);
                bs += UMI3DNetworkingHelper.Write(id, b, ref i);
                bs += UMI3DNetworkingHelper.Write(hoveredObjectId, b, ref i);
                bs += UMI3DNetworkingHelper.Write(boneType, b, ref i);
                return (i, bs);
            };
            return (size, func);
        }
    }
}