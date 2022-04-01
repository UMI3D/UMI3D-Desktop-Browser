/*
Copyright 2019 Gfi Informatique

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
using umi3d.cdk.collaboration;
using umi3d.common.interaction;
using BeardedManStudios.Forge.Networking;
using UnityEngine;
using umi3d.common.collaboration;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "ClientPCIdentifier", menuName = "UMI3D/Client PC Identifier")]
public class ClientPCIdentifier : ClientIdentifierApi
{
    public Action<List<string>, Action<bool>> ShouldDownloadLib;
    public Action<FormDto, Action<FormAnswerDto>> GetParameters;

    public override async Task<FormAnswerDto> GetParameterDtos(FormDto parameter)
    {
        bool b = true;
        FormAnswerDto form = null;
        Action<FormAnswerDto> callback = (f) => { form = f; b = false; };

        GetParameters.Invoke(parameter, callback);
        while (b)
            await Task.Yield();
        return form;
    }

    public override async Task<bool> ShouldDownloadLibraries(List<string> LibrariesId)
    {
        bool b = true;
        bool form = false;
        Action<bool> callback = (f) => { form = f; b = false; };

        ShouldDownloadLib.Invoke(LibrariesId, callback);
        while (b)
            await Task.Yield();
        return form;
    }
}