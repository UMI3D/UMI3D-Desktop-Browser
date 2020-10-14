﻿/*
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
using UnityEngine;

[CreateAssetMenu(fileName = "ClientPCIdentifier", menuName = "UMI3D/Client PC Identifier")]
public class ClientPCIdentifier : ClientIdentifierApi
{

    public Action<Action<string,string>> GetIdentityAction;
    public Action<Action<string>> GetLoginAction;
    public Action<List<string>,Action<bool>> ShouldDownloadLib;
    public Action<FormDto,Action<FormDto>> GetParameters;

    public override void GetParameterDtos(FormDto parameter, Action<FormDto> callback)
    {
        GetParameters.Invoke(parameter, callback);
    }

    public override void ShouldDownloadLibraries(List<string> ids,Action<bool> callback)
    {
        ShouldDownloadLib.Invoke(ids,callback);
    }

    public override void GetIdentity(Action<string,string> callback)
    {
        GetIdentityAction.Invoke(callback);
    }

    public override void GetIdentity(Action<string> callback)
    {
        GetLoginAction.Invoke(callback);
    }
}