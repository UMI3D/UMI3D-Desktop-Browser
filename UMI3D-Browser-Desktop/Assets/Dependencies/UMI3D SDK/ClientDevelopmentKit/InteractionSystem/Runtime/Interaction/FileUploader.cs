/*
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

using inetum.unityUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using umi3d.common;
using umi3d.common.interaction;


namespace umi3d.cdk.interaction
{
    /// <summary>
    /// Class to manage uploaded files
    /// </summary>
    public static class FileUploader
    {
        private const DebugScope scope = DebugScope.CDK | DebugScope.Interaction;

        private static readonly Dictionary<string, string> filesToUpload = new Dictionary<string, string>(); // key:fileId  value:path

        /// <summary>
        /// Checks if the fileId match with a file to upload then return the file in bytes and remove the file from filesToUpload, else return null
        /// </summary>
        /// <param name="fileId">The id file generated by AddFileToUpload </param>
        /// <returns>return the file in bytes.</returns>
        public static byte[] GetFileToUpload(string fileId)
        {
            if (!filesToUpload.ContainsKey(fileId))
            {
                UMI3DLogger.LogWarning("Server asked client to upload a file without its request, or the client already upload the file", scope);
                return null;
            }
            string path = filesToUpload[fileId];
            if (File.Exists(path))
            {
                filesToUpload.Remove(fileId);
                return File.ReadAllBytes(path);
            }
            else
            {
                UMI3DLogger.LogWarning("File not found : " + path + ". Local file cannot be uploaded.", scope);
                return null;
            }
        }

        /// <summary>
        /// Add a path to filesToUpload and return the fileId for this upload if the path is valid. Return null if the file doesn't exist.
        /// </summary>
        /// <param name="path">Path of the file on disk.</param>
        /// <returns></returns>
        public static string AddFileToUpload(string path)
        {
            if (!File.Exists(path))
            {
                UMI3DLogger.LogWarning("Warning, this file doesn't exist in your device : " + path, scope);
                return null;
            }
            string res = Guid.NewGuid().ToString();
            filesToUpload.Add(res, path);
            return res;
        }

        /// <summary>
        /// Get the name of the file from its fileId if it is in filesToUpload.
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public static string GetFileName(string fileId)
        {
            if (!filesToUpload.ContainsKey(fileId))
            {
                UMI3DLogger.LogWarning("Server asked client to upload a file without its request, or the client already upload the file", scope);
                return null;
            }
            return System.IO.Path.GetFileName(filesToUpload[fileId]);
        }

        /// <summary>
        /// Send upload request when a form contains an Upload File Parameter.
        /// </summary>
        /// <param name="form"></param>
        public static void CheckFormToUploadFile(ulong environmentId,FormDto form)
        {
            form.fields
                .Select(async id => (await UMI3DEnvironmentLoader.WaitForAnEntityToBeLoaded(environmentId, id, null)).dto)
                .ForEach(async p =>
            {
                var param = await p;
                if (param is UploadFileParameterDto ParameterDto)
                {
                    UMI3DLogger.Log(param.ToJson(), scope);
                    string pathValue = (param as UploadFileParameterDto).value;//the path of the file to upload
                    // -> create request with AddFileToUpload

                    string fileId = AddFileToUpload(pathValue);
                    if (fileId != null)
                    {

                        var req = new UploadFileRequestDto()
                        {
                            parameter = param,
                            fileId = fileId,
                            toolId = 0,
                            id = ParameterDto.id,
                            hoveredObjectId = 0
                        };
                        UMI3DClientServer.SendRequest(req, true);
                    }
                }
            });
        }
    }
}