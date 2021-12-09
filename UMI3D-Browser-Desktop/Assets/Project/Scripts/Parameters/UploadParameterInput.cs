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
using umi3d.cdk.interaction;
using umi3d.cdk.menu;
using umi3d.common.interaction;

namespace BrowserDesktop.Parameters
{
    public class UploadParameterInput : AbstractParameterInput<UploadInputMenuItem, UploadFileParameterDto, string>
    {
        protected override ParameterSettingRequestDto CreateRequestDto()
        {
            return new UploadFileRequestDto();
        }

        protected override void WriteRequestDtoProperties(ParameterSettingRequestDto dto, AbstractParameterDto menuItemDto, ulong toolId)
        {
            base.WriteRequestDtoProperties(dto, menuItemDto, toolId);
            (dto as UploadFileRequestDto).fileId = FileUploader.AddFileToUpload((dto.parameter as UploadFileParameterDto).value);
        }
    }
}