/*
Copyright 2019 - 2022 Inetum

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
using System.Threading.Tasks;
using UnityEngine;

namespace umi3d.baseBrowser.connection
{
    [CreateAssetMenu(fileName = "BaseClientIdentifier", menuName = "UMI3D/Base Client Identifier")]
    public class BaseClientIdentifier : cdk.collaboration.ClientIdentifierApi
    {
        public Action<List<string>, Action<bool>> ShouldDownloadLib;
        public Action<common.interaction.FormDto, Action<common.interaction.FormAnswerDto>> GetParameters;

        public override async Task<common.interaction.FormAnswerDto> GetParameterDtos(common.interaction.FormDto parameter)
        {
            bool b = true;
            common.interaction.FormAnswerDto form = null;
            GetParameters.Invoke(parameter, (f) => { form = f; b = false; });
            while (b) await Task.Yield();
            return form;
        }

        public override async Task<bool> ShouldDownloadLibraries(List<string> LibrariesId)
        {
            bool b = true;
            bool form = false;
            ShouldDownloadLib.Invoke(LibrariesId, (f) => { form = f; b = false; });
            while (b) await Task.Yield();
            return form;
        }
    }
}