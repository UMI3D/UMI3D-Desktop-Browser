﻿/*
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
    public class UpdateBindingsActivationSerializer : IUMI3DSerializerSubModule<UpdateBindingsActivationDto>
    {
        public bool Read(ByteContainer container, out UpdateBindingsActivationDto result)
        {
            bool readable = true;
            readable &= UMI3DSerializer.TryRead(container, out bool areBindingsActivated);

            if (!readable)
            {
                result = default;
                return false;
            }

            result = new UpdateBindingsActivationDto() { areBindingsActivated = areBindingsActivated };
            return readable;
        }

        public Bytable Write(UpdateBindingsActivationDto dto)
        {
            return UMI3DSerializer.Write(dto.areBindingsActivated);
        }
    }
}