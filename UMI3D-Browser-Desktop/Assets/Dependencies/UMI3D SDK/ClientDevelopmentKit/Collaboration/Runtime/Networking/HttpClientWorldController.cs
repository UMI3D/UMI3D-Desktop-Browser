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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using umi3d.common;
using umi3d.common.collaboration.dto.networking;
using umi3d.common.collaboration.dto.signaling;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.Networking;
using static umi3d.cdk.collaboration.HttpClient;

namespace umi3d.cdk.collaboration
{
    /// <summary>
    /// Send HTTP requests to the environment server.
    /// </summary>
    /// Usually used before connection or to retrieve DTOs.
    public class HttpClientWorldController
    {
        private const DebugScope k_Scope = DebugScope.CDK | DebugScope.Collaboration | DebugScope.Networking;

        private class FakePrivateIdentityDto : IdentityDto
        {
            public string GlobalToken;
            public string connectionDto;
            public List<LibrariesDto> libraries;

            public PrivateIdentityDto ToPrivateIdentity()
            {
                return new PrivateIdentityDto()
                {
                    globalToken = GlobalToken,
                    connectionDto = UMI3DDtoSerializer.FromJson<EnvironmentConnectionDto>(connectionDto, Newtonsoft.Json.TypeNameHandling.None),
                    libraries = libraries,
                    localToken = localToken,
                    headerToken = headerToken,
                    guid = guid,
                    displayName = displayName,
                    key = key,
                    login = login,
                    userId = userId
                };
            }
        }
        public class ParameterConverter : Newtonsoft.Json.JsonConverter
        {
            public override bool CanRead => true;

            public override bool CanWrite => false;

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(AbstractParameterDto);
            }


            public AbstractParameterDto ReadObjectArray(JObject obj, JToken tokenA)
            {
                switch (ReadObjectValue(obj))
                {
                    case Color col:
                        return new EnumParameterDto<Color>()
                        {
                            possibleValues = tokenA.Values<object>().Select(objA => (Color)ReadObjectValue(objA as JObject)).ToList(),
                            value = col
                        };
                    case Vector4 v4:
                        return new EnumParameterDto<Vector4>()
                        {
                            possibleValues = tokenA.Values<object>().Select(objA => (Vector4)ReadObjectValue(objA as JObject)).ToList(),
                            value = v4
                        };
                    case Vector3 v3:
                        return new EnumParameterDto<Vector3>()
                        {
                            possibleValues = tokenA.Values<object>().Select(objA => (Vector3)ReadObjectValue(objA as JObject)).ToList(),
                            value = v3
                        };
                    case Vector2 v2:
                        return new EnumParameterDto<Vector2>()
                        {
                            possibleValues = tokenA.Values<object>().Select(objA => (Vector2)ReadObjectValue(objA as JObject)).ToList(),
                            value = v2
                        };
                }
                UnityEngine.Debug.LogError($"Missing case. {obj}");
                return null;
            }

            public AbstractParameterDto ReadObject(JObject obj)
            {
                switch (ReadObjectValue(obj))
                {
                    case Color col:
                        return new ColorParameterDto
                        {
                            value = col.Dto()
                        };
                    case Vector4 v4:
                        return new Vector4ParameterDto
                        {
                            value = v4.Dto()
                        };
                    case Vector3 v3:
                        return new Vector3ParameterDto
                        {
                            value = v3.Dto()
                        };
                    case Vector2 v2:
                        return new Vector2ParameterDto
                        {
                            value = v2.Dto()
                        };
                }
                UnityEngine.Debug.LogError($"Missing case. {obj}");
                return null;
            }

            public object ReadObjectValue(JObject obj)
            {
                if (obj.TryGetValue("R", out JToken tokenR)
                    && obj.TryGetValue("G", out JToken tokenG)
                    && obj.TryGetValue("B", out JToken tokenB)
                    && obj.TryGetValue("A", out JToken tokenA))
                {
                    return new Color(tokenR.ToObject<float>(), tokenG.ToObject<float>(), tokenB.ToObject<float>(), tokenA.ToObject<float>());
                }

                if (obj.TryGetValue("X", out JToken tokenX)
                    && obj.TryGetValue("Y", out JToken tokenY))
                {
                    if (obj.TryGetValue("Z", out JToken tokenZ))
                    {
                        if (obj.TryGetValue("W", out JToken tokenW))
                            return new Vector4(tokenX.ToObject<float>(), tokenY.ToObject<float>(), tokenZ.ToObject<float>(), tokenW.ToObject<float>());
                        return new Vector3(tokenX.ToObject<float>(), tokenY.ToObject<float>(), tokenZ.ToObject<float>());
                    }
                    return new Vector2(tokenX.ToObject<float>(), tokenY.ToObject<float>());
                }
                UnityEngine.Debug.LogError($"Missing case. {obj}");
                return null;
            }


            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var jo = JObject.Load(reader);
                AbstractParameterDto dto = null;
                bool isArray = false;
                isArray = jo.TryGetValue("possibleValues", out JToken tokenA);

                if (jo.TryGetValue("value", out JToken token))
                {
                    switch (token.Type)
                    {
                        case JTokenType.String:
                            if (isArray)
                                dto = new EnumParameterDto<string>()
                                {
                                    possibleValues = tokenA.Values<string>().ToList(),
                                    value = token.ToObject<string>()
                                };
                            else
                                dto = new StringParameterDto()
                                {
                                    value = token.ToObject<string>()
                                };
                            break;
                        case JTokenType.Boolean:
                            dto = new BooleanParameterDto()
                            {
                                value = token.ToObject<bool>()
                            };
                            break;
                        case JTokenType.Float:
                            if (isArray)
                                dto = new EnumParameterDto<float>()
                                {
                                    possibleValues = tokenA.Values<float>().ToList(),
                                    value = token.ToObject<float>()
                                };
                            else
                                dto = new FloatParameterDto()
                                {
                                    value = token.ToObject<float>()
                                };
                            break;
                        case JTokenType.Integer:
                            if (isArray)
                                dto = new EnumParameterDto<int>()
                                {
                                    possibleValues = tokenA.Values<int>().ToList(),
                                    value = token.ToObject<int>()
                                };
                            else
                                dto = new IntegerParameterDto()
                                {
                                    value = token.ToObject<int>()
                                };
                            break;
                        case JTokenType.Object:
                            var obj = token.ToObject<object>() as JObject;
                            if (isArray)
                                dto = ReadObjectArray(obj, tokenA);
                            else
                                dto = ReadObject(obj);
                            break;
                        default:
                            UnityEngine.Debug.LogError($"TODO Add Case for Color, Range or Vector 2 3 4. {token.Type}");
                            break;
                    }
                }
                if (dto == null)
                    return null;

                if (jo.TryGetValue("privateParameter", out JToken tokenp))
                    dto.privateParameter = tokenp.ToObject<bool>();
                if (jo.TryGetValue("isDisplayer", out JToken tokendisp))
                    dto.isDisplayer = tokendisp.ToObject<bool>();
                if (jo.TryGetValue("description", out JToken tokend))
                    dto.description = tokend.ToObject<string>();
                if (jo.TryGetValue("id", out JToken tokeni))
                    dto.id = (ulong)tokeni.ToObject<int>();
                if (jo.TryGetValue("name", out JToken tokenn))
                    dto.name = tokenn.ToObject<string>();
                if (jo.TryGetValue("icon2D", out JToken tokenI2))
                    dto.icon2D = tokenI2.ToObject<ResourceDto>();
                if (jo.TryGetValue("icon3D", out JToken tokenI3))
                    dto.icon3D = tokenI3.ToObject<ResourceDto>();

                return dto;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        private readonly UMI3DWorldControllerClient m_WorldControllerClient;

        /// <summary>
        /// Init HttpClientWorldController.
        /// </summary>
        /// <param name="UMI3DClientServer"></param>
        public HttpClientWorldController(UMI3DWorldControllerClient pWorldControllerClient)
        {
            m_WorldControllerClient = pWorldControllerClient;
            UMI3DLogger.Log($"Init HttpClient(WorldController)", k_Scope | DebugScope.Connection);
        }

        private static bool DefaultShouldTryAgain(RequestFailedArgument argument)
        {
            return argument.count < 3;
        }

        /// <summary>
        /// Connect to a media
        /// </summary>
        /// <param name="connectionDto"></param>
        public static async Task<UMI3DDto> Connect(ConnectionDto connectionDto, string MasterUrl, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(connectionDto.ToJson(Newtonsoft.Json.TypeNameHandling.None));

            using (UnityWebRequest uwr = await PostRequest(null, MasterUrl + UMI3DNetworkingKeys.connect, "application/json", bytes, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), false))
            {
                UMI3DLogger.Log($"Received answer to Connect : \n " + uwr?.downloadHandler?.text,k_Scope | DebugScope.Connection);

                UMI3DDto dto = uwr?.downloadHandler.data != null ? ReadConnectAnswer(System.Text.Encoding.UTF8.GetString(uwr?.downloadHandler.data)) : null;
                return dto;
            }
        }

        private static UMI3DDto ReadConnectAnswer(string text)
        {
            PrivateIdentityDto dto1 = null;
            FakePrivateIdentityDto dto2 = null;

            try
            {
                dto1 = UMI3DDtoSerializer.FromJson<PrivateIdentityDto>(text, Newtonsoft.Json.TypeNameHandling.None);
            }
            catch (Exception)
            {
                dto2 = UMI3DDtoSerializer.FromJson<FakePrivateIdentityDto>(text, Newtonsoft.Json.TypeNameHandling.None);
            }

            ConnectionFormDto dto3 = UMI3DDtoSerializer.FromJson<ConnectionFormDto>(text, Newtonsoft.Json.TypeNameHandling.None, new List<JsonConverter>() { new ParameterConverter() });

            LibrariesToDownloadDto dto4 = UMI3DDtoSerializer.FromJson<LibrariesToDownloadDto>(text, Newtonsoft.Json.TypeNameHandling.None);
            if (dto1 != null && dto1?.globalToken != null && dto1?.connectionDto != null)
                return dto1;
            else if (dto2 != null && dto2?.GlobalToken != null && dto2?.connectionDto != null)
                return dto2.ToPrivateIdentity();
            else if (dto4 != null && dto4.Libraries != null)
                return dto4;
            else
                return dto3;

        }

        public static async Task<byte[]> SendGetWithoutAuth(string url, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send GetWithoutAuth {url}",k_Scope | DebugScope.Connection);

            int i = 0;
            while (i < 10)
            {
                i++;
                using (UnityWebRequest uwr = await GetRequest(null, url, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), false))
                {
                    UMI3DLogger.Log($"Received GetWithoutAuth {url}\n{uwr?.responseCode}\n{uwr?.url}",k_Scope | DebugScope.Connection);
                    if (uwr?.responseCode != 204)
                        return uwr?.downloadHandler.data;
                    UMI3DLogger.Log($"Resend GetPrivate Because responce code was 204 {url}",k_Scope | DebugScope.Connection);
                    await UMI3DAsyncManager.Delay(1000);
                }
            }
            return null;
        }

        /// <summary>
        /// Send request using GET method to get a Media at a specified url.
        /// </summary>
        /// <param name="url">Url to send the resquest to. For a vanilla server add '/media' at the end of the server url.</param>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public static async Task<MediaDto> SendGetMedia(string url, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send GetMedia",k_Scope | DebugScope.Connection);

            using (UnityWebRequest uwr = await GetRequest(null, url, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e)))
            {
                UMI3DLogger.Log($"Received GetMedia",k_Scope | DebugScope.Connection);
                if (uwr?.downloadHandler.data == null) return null;
                string json = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data);
                return UMI3DDtoSerializer.FromJson<MediaDto>(json, Newtonsoft.Json.TypeNameHandling.None);
            }
        }
    }
}