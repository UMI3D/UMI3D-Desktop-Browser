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
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    /// <summary>
    /// Helper class that manages the loading of <see cref="AbstractTool"/> entities.
    /// </summary>
    public class UMI3DAbstractToolLoader : AbstractLoader
    {

        public override bool CanReadUMI3DExtension(ReadUMI3DExtensionData data)
        {
            return false;
        }

        public override Task ReadUMI3DExtension(ReadUMI3DExtensionData value)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> SetUMI3DProperty(SetUMI3DPropertyData value)
        {
            var dto = value.entity.dto as AbstractToolDto;
            if (dto == null) return false;
            var tool = value.entity.Object as AbstractTool;
            switch (value.property.property)
            {
                case UMI3DPropertyKeys.AbstractToolName:
                    dto.name = (string)value.property.value;
                    break;
                case UMI3DPropertyKeys.AbstractToolDescription:
                    dto.description = (string)value.property.value;
                    break;
                case UMI3DPropertyKeys.AbstractToolIcon2D:
                    dto.icon2D = (ResourceDto)value.property.value;
                    break;
                case UMI3DPropertyKeys.AbstractToolIcon3D:
                    dto.icon3D = (ResourceDto)value.property.value;
                    break;
                case UMI3DPropertyKeys.AbstractToolInteractions:
                    return SetInteractions(dto, tool, value.property);
                case UMI3DPropertyKeys.AbstractToolActive:
                    dto.active = (bool)value.property.value;
                    break;
                case UMI3DPropertyKeys.EventTriggerAnimation:
                    (dto.interactions.Find(evt => evt.id.Equals(value.property.entityId)) as EventDto).TriggerAnimationId = (ulong)value.property.value;
                    break;
                case UMI3DPropertyKeys.EventReleaseAnimation:
                    (dto.interactions.Find(evt => evt.id.Equals(value.property.entityId)) as EventDto).ReleaseAnimationId = (ulong)value.property.value;
                    break;
                default:
                    return false;
            }
            return true;
        }

        public override async Task<bool> SetUMI3DProperty(SetUMI3DPropertyContainerData value)
        {
            var dto = value.entity.dto as AbstractToolDto;
            if (dto == null) return false;
            var tool = value.entity.Object as AbstractTool;
            switch (value.propertyKey)
            {
                case UMI3DPropertyKeys.AbstractToolName:
                    dto.name = UMI3DNetworkingHelper.Read<string>(value.container);
                    break;
                case UMI3DPropertyKeys.AbstractToolDescription:
                    dto.description = UMI3DNetworkingHelper.Read<string>(value.container);
                    break;
                case UMI3DPropertyKeys.AbstractToolIcon2D:
                    dto.icon2D = UMI3DNetworkingHelper.Read<ResourceDto>(value.container);
                    break;
                case UMI3DPropertyKeys.AbstractToolIcon3D:
                    dto.icon3D = UMI3DNetworkingHelper.Read<ResourceDto>(value.container);
                    break;
                case UMI3DPropertyKeys.AbstractToolInteractions:
                    return SetInteractions(dto, tool, value.operationId, value.propertyKey, value.container);
                case UMI3DPropertyKeys.AbstractToolActive:
                    dto.active = UMI3DNetworkingHelper.Read<bool>(value.container);
                    break;
                default:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Reads the value of an unknown <see cref="object"/> based on a received <see cref="ByteContainer"/> and updates it.
        /// <br/> Part of the bytes networking workflow.
        /// </summary>
        /// <param name="value">Unknown object</param>
        /// <param name="propertyKey">Property to update key in <see cref="UMI3DPropertyKeys"/></param>
        /// <param name="container">Received byte container</param>
        /// <returns>True if property setting was successful</returns>
        public override async Task<bool> ReadUMI3DProperty(ReadUMI3DPropertyData data)
        {
            switch (data.propertyKey)
            {
                case UMI3DPropertyKeys.AbstractToolName:
                    data.result = UMI3DNetworkingHelper.Read<string>(data.container);
                    break;
                case UMI3DPropertyKeys.AbstractToolDescription:
                    data.result = UMI3DNetworkingHelper.Read<string>(data.container);
                    break;
                case UMI3DPropertyKeys.AbstractToolIcon2D:
                    data.result = UMI3DNetworkingHelper.Read<ResourceDto>(data.container);
                    break;
                case UMI3DPropertyKeys.AbstractToolIcon3D:
                    data.result = UMI3DNetworkingHelper.Read<ResourceDto>(data.container);
                    break;
                case UMI3DPropertyKeys.AbstractToolInteractions:
                    return ReadInteractions(data);
                case UMI3DPropertyKeys.AbstractToolActive:
                    data.result = UMI3DNetworkingHelper.Read<bool>(data.container);
                    break;
                default:
                    return false;
            }
            return true;
        }


        /// <summary>
        /// Set the value of an <see cref="AbstractTool"/> based on a received <see cref="ByteContainer"/> and update it.
        /// <br/> Part of the bytes networking workflow.
        /// </summary>
        /// <param name="dto">Tool dto</param>
        /// <param name="tool">Tool</param>
        /// <param name="operationId">Operation id in <see cref="UMI3DOperationKeys"/></param>
        /// <param name="propertyKey">Property to update key in <see cref="UMI3DPropertyKeys"/></param>
        /// <param name="container">Received byte container</param>
        /// <returns>True if property setting was successful</returns>
        private static bool SetInteractions(AbstractToolDto dto, AbstractTool tool, uint operationId, uint propertyKey, ByteContainer container)
        {
            int index;
            AbstractInteractionDto value;

            switch (operationId)
            {
                case UMI3DOperationKeys.SetEntityListAddProperty:
                    index = UMI3DNetworkingHelper.Read<int>(container);
                    value = UMI3DNetworkingHelper.Read<AbstractInteractionDto>(container);
                    if (index == dto.interactions.Count)
                        dto.interactions.Add(value);
                    else if (index < dto.interactions.Count)
                        dto.interactions.Insert(index, value);
                    else return false;
                    tool.Added(value); break;
                case UMI3DOperationKeys.SetEntityListRemoveProperty:
                    index = UMI3DNetworkingHelper.Read<int>(container);
                    if (index < dto.interactions.Count)
                    {
                        AbstractInteractionDto removed = dto.interactions[index];
                        dto.interactions.RemoveAt(index);
                        tool.Removed(removed);
                    }
                    else return false;
                    break;
                case UMI3DOperationKeys.SetEntityListProperty:
                    index = UMI3DNetworkingHelper.Read<int>(container);
                    value = UMI3DNetworkingHelper.Read<AbstractInteractionDto>(container);
                    if (index < dto.interactions.Count)
                        dto.interactions[index] = value;
                    else return false;
                    tool.Updated();
                    break;
                default:
                    dto.interactions = UMI3DNetworkingHelper.ReadList<AbstractInteractionDto>(container);
                    tool.Updated();
                    break;
            }
            return true;
        }

        /// <summary>
        /// Set the value of an <see cref="AbstractTool"/> based on a received <see cref="SetEntityPropertyDto"/> and update it.
        /// </summary>
        /// <param name="dto">Tool dto</param>
        /// <param name="tool">Tool</param>
        /// <param name="property">Received dto</param>
        /// <returns>True if property setting was successful</returns>
        private static bool SetInteractions(AbstractToolDto dto, AbstractTool tool, SetEntityPropertyDto property)
        {
            switch (property)
            {
                case SetEntityListAddPropertyDto add:
                    dto.interactions.Add((AbstractInteractionDto)add.value);
                    break;
                case SetEntityListRemovePropertyDto rem:
                    if ((int)(Int64)rem.index < dto.interactions.Count)
                        dto.interactions.RemoveAt((int)(Int64)rem.index);
                    else return false;
                    break;
                case SetEntityListPropertyDto set:
                    if ((int)(Int64)set.index < dto.interactions.Count)
                        dto.interactions[(int)(Int64)set.index] = (AbstractInteractionDto)set.value;
                    else return false;
                    break;
                default:
                    dto.interactions = (List<AbstractInteractionDto>)property.value;
                    break;
            }
            tool.Updated();
            return true;
        }

        /// <summary>
        /// Read a received <see cref="ByteContainer"/>
        /// <br/> Part of the bytes networking workflow.
        /// </summary>
        /// <param name="value">Object</param>
        /// <param name="propertyKey">Property to update key in <see cref="UMI3DPropertyKeys"/></param>
        /// <param name="container">Received container</param>
        /// <returns>Always true</returns>
        private static bool ReadInteractions(ReadUMI3DPropertyData data)
        {
            data.result = UMI3DNetworkingHelper.ReadList<AbstractInteractionDto>(data.container);
            return true;
        }

        /// <summary>
        /// Read an <see cref="AbstractInteractionDto"/> from a received <see cref="ByteContainer"/>.
        /// <br/> Part of the bytes networking workflow.
        /// </summary>
        /// <param name="container">Byte container</param>
        /// <param name="readable">True if the byte container has been correctly read.</param>
        /// <returns></returns>
        public static AbstractInteractionDto ReadAbstractInteractionDto(ByteContainer container, out bool readable)
        {
            AbstractInteractionDto interaction;
            byte interactionCase = UMI3DNetworkingHelper.Read<byte>(container);
            switch (interactionCase)
            {
                case UMI3DInteractionKeys.Event:
                    var Event = new EventDto();
                    ReadAbstractInteractionDto(Event, container);
                    Event.hold = UMI3DNetworkingHelper.Read<bool>(container);
                    Event.TriggerAnimationId = UMI3DNetworkingHelper.Read<ulong>(container);
                    Event.ReleaseAnimationId = UMI3DNetworkingHelper.Read<ulong>(container);
                    interaction = Event;
                    break;
                case UMI3DInteractionKeys.Manipulation:
                    var Manipulation = new ManipulationDto();
                    ReadAbstractInteractionDto(Manipulation, container);
                    Manipulation.frameOfReference = UMI3DNetworkingHelper.Read<ulong>(container);
                    Manipulation.dofSeparationOptions = UMI3DNetworkingHelper.ReadList<DofGroupOptionDto>(container);
                    interaction = Manipulation;
                    break;
                case UMI3DInteractionKeys.Form:
                    var Form = new FormDto();
                    ReadAbstractInteractionDto(Form, container);
                    Form.fields = UMI3DNetworkingHelper.ReadList<AbstractParameterDto>(container);
                    interaction = Form;
                    break;
                case UMI3DInteractionKeys.Link:
                    var Link = new LinkDto();
                    ReadAbstractInteractionDto(Link, container);
                    Link.url = UMI3DNetworkingHelper.Read<string>(container);
                    interaction = Link;
                    break;
                case UMI3DInteractionKeys.BooleanParameter:
                    var Bool = new BooleanParameterDto();
                    ReadAbstractParameterDto(Bool, container);
                    Bool.value = UMI3DNetworkingHelper.Read<bool>(container);
                    interaction = Bool;
                    break;
                case UMI3DInteractionKeys.ColorParameter:
                    var Color = new ColorParameterDto();
                    ReadAbstractParameterDto(Color, container);
                    Color.value = UMI3DNetworkingHelper.Read<Color>(container);
                    interaction = Color;
                    break;
                case UMI3DInteractionKeys.Vector2Parameter:
                    var Vector2 = new Vector2ParameterDto();
                    ReadAbstractParameterDto(Vector2, container);
                    Vector2.value = UMI3DNetworkingHelper.Read<Vector2>(container);
                    interaction = Vector2;
                    break;
                case UMI3DInteractionKeys.Vector3Parameter:
                    var Vector3 = new Vector3ParameterDto();
                    ReadAbstractParameterDto(Vector3, container);
                    Vector3.value = UMI3DNetworkingHelper.Read<Vector3>(container);
                    interaction = Vector3;
                    break;
                case UMI3DInteractionKeys.Vector4Parameter:
                    var Vector4 = new Vector4ParameterDto();
                    ReadAbstractParameterDto(Vector4, container);
                    Vector4.value = UMI3DNetworkingHelper.Read<Vector4>(container);
                    interaction = Vector4;
                    break;
                case UMI3DInteractionKeys.UploadParameter:
                    var Upload = new UploadFileParameterDto();
                    ReadAbstractParameterDto(Upload, container);
                    Upload.value = UMI3DNetworkingHelper.Read<string>(container);
                    Upload.authorizedExtensions = UMI3DNetworkingHelper.Read<List<string>>(container);
                    interaction = Upload;
                    break;
                case UMI3DInteractionKeys.StringParameter:
                    var String = new StringParameterDto();
                    ReadAbstractParameterDto(String, container);
                    String.value = UMI3DNetworkingHelper.Read<string>(container);
                    interaction = String;
                    break;
                case UMI3DInteractionKeys.LocalInfoParameter:
                    var LocalInfo = new LocalInfoRequestParameterDto();
                    ReadAbstractParameterDto(LocalInfo, container);
                    LocalInfo.app_id = UMI3DNetworkingHelper.Read<string>(container);
                    LocalInfo.serverName = UMI3DNetworkingHelper.Read<string>(container);
                    LocalInfo.reason = UMI3DNetworkingHelper.Read<string>(container);
                    LocalInfo.key = UMI3DNetworkingHelper.Read<string>(container);
                    LocalInfo.value = new LocalInfoRequestParameterValue(UMI3DNetworkingHelper.Read<bool>(container), UMI3DNetworkingHelper.Read<bool>(container));
                    interaction = LocalInfo;
                    break;
                case UMI3DInteractionKeys.StringEnumParameter:
                    var EString = new EnumParameterDto<string>();
                    ReadAbstractParameterDto(EString, container);
                    EString.possibleValues = UMI3DNetworkingHelper.ReadList<string>(container);
                    EString.value = UMI3DNetworkingHelper.Read<string>(container);
                    interaction = EString;
                    break;
                case UMI3DInteractionKeys.FloatRangeParameter:
                    var RFloat = new FloatRangeParameterDto();
                    ReadAbstractParameterDto(RFloat, container);
                    RFloat.min = UMI3DNetworkingHelper.Read<float>(container);
                    RFloat.max = UMI3DNetworkingHelper.Read<float>(container);
                    RFloat.increment = UMI3DNetworkingHelper.Read<float>(container);
                    RFloat.value = UMI3DNetworkingHelper.Read<float>(container);
                    interaction = RFloat;
                    break;
                default:
                    interaction = null;
                    readable = false;
                    return null;
            }
            readable = true;
            return interaction;
        }

        /// <summary>
        /// Reads an <see cref="AbstractInteractionDto"/> from a received <see cref="ByteContainer"/> and updates it.
        /// <br/> Part of the bytes networking workflow.
        /// </summary>
        ///  <param name="interactionDto">interaction dto to update.</param>
        /// <param name="container">Byte container</param>
        /// <returns></returns>
        private static void ReadAbstractInteractionDto(AbstractInteractionDto interactionDto, ByteContainer container)
        {
            interactionDto.id = UMI3DNetworkingHelper.Read<ulong>(container);
            interactionDto.name = UMI3DNetworkingHelper.Read<string>(container);
            interactionDto.icon2D = UMI3DNetworkingHelper.Read<ResourceDto>(container);
            interactionDto.icon3D = UMI3DNetworkingHelper.Read<ResourceDto>(container);
            interactionDto.description = UMI3DNetworkingHelper.Read<string>(container);
            
        }

        /// <summary>
        /// Reads an <see cref="AbstractParameterDto"/> from a received <see cref="ByteContainer"/> and updates it.
        /// <br/> Part of the bytes networking workflow.
        /// </summary>
        ///  <param name="ParameterDto">interaction dto to update.</param>
        /// <param name="container">Byte container</param>
        /// <returns></returns>
        private static void ReadAbstractParameterDto(AbstractParameterDto ParameterDto, ByteContainer container)
        {
            ReadAbstractParameterDto(ParameterDto, container);
            ParameterDto.privateParameter = UMI3DNetworkingHelper.Read<bool>(container);
            ParameterDto.isDisplayer = UMI3DNetworkingHelper.Read<bool>(container);
        }
    }
}