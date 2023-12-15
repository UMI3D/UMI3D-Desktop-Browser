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

using System;
using System.Collections.Generic;
using umi3d.baseBrowser.connection;
using umi3d.cdk.interaction;
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine;

namespace BrowserDesktop
{
    public class BatchmodeConnection : MonoBehaviour
    {
        #region Fields

        private const string urlArg = "-url";

        private Dictionary<string, string> formDataDictionary = new();

        #endregion

        #region Methods

        async void Start()
        {
            if (!Application.isBatchMode)
                return;

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 0)
            {
                InitWithArs(args);
            }
            BaseConnectionProcess.Instance.currentServer.serverUrl = "localhost:50043";

            await UMI3DAsyncManager.Yield();

            await BaseConnectionProcess.Instance.InitConnect();

            BaseConnectionProcess.Instance.GetParameterDtos += GetParametersDto;

            BaseConnectionProcess.Instance.ConnectionSucces += (MediaDto dto) => Debug.Log("Connection success to " + dto.url);
            BaseConnectionProcess.Instance.ConnectionFail += (error) => Debug.Log("Connection fail : " + error);
            BaseConnectionProcess.Instance.AskForDownloadingLibraries += (count, callback) => callback?.Invoke(true);
            BaseConnectionProcess.Instance.EnvironmentLoaded += EnvironmentLoaded;
            BaseConnectionProcess.Instance.EnvironmentLeave += EnvironmentLeave;
        }

        private void GetParametersDto(ConnectionFormDto form, Action<FormAnswerDto> callback)
        {
            if (form == null) callback.Invoke(null);

            FormAnswerDto answer = new FormAnswerDto()
            {
                boneType = 0,
                hoveredObjectId = 0,
                id = form.id,
                toolId = 0,
                answers = new List<ParameterSettingRequestDto>()
            };

            string paramName;

            foreach (AbstractParameterDto param in form.fields)
            {
                Debug.Log("-" + param.name + " " + param.GetType());

                (_, ParameterSettingRequestDto paramDto) = GlobalToolMenuManager.GetInteractionItem(param);

                paramName = param.name.ToLower().Trim();

                if (formDataDictionary.ContainsKey(paramName))
                {
                    try
                    {
                        switch (param)
                        {
                            case StringParameterDto:
                                paramDto.parameter = formDataDictionary[paramName];
                                break;
                            case EnumParameterDto<string> enumStr:
                                paramDto.parameter = enumStr.possibleValues[UnityEngine.Random.Range(0, enumStr.possibleValues.Count)];
                                break;
                            default:
                                break;
                        }
                    } catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }

                answer.answers.Add(paramDto);
            }

            callback.Invoke(answer);
        }

        private void EnvironmentLoaded()
        {
            Debug.Log("Environment loaded");
            Console.WriteLine("Environment loaded");
        }

        private void EnvironmentLeave()
        {
            Debug.Log("Environment leave");
        }

        private void InitWithArs(string[] args)
        {
            formDataDictionary.Clear();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == urlArg && i + 1 < args.Length)
                {
                    i++;
                    BaseConnectionProcess.Instance.currentServer.serverUrl = args[i];
                    Debug.Log("url set " + BaseConnectionProcess.Instance.currentServer.serverUrl);
                } else
                {
                    string[] arg = args[i].Split(":");
                    if (arg.Length == 2)
                    {
                        formDataDictionary[arg[0].ToLower().Trim()] = arg[1];
                        Debug.Log("Add " + arg[0] + " -> " + arg[1]);
                    }
                }
            }
        }

        #endregion
    }

    [Serializable]
    public class FormEntry
    {
        public string name = string.Empty;
        public string value = string.Empty;
    }
}