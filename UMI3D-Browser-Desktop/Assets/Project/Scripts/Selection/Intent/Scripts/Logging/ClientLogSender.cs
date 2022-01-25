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

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.cdk.interaction.selection.intent.log
{
    internal class ClientLogSender : MonoBehaviour
    {
        [SerializeField]
        private int cacheSizeMax = 20;

        private int cacheSize => clientDataCache.tracking.Count;

        private string savePath;

        private ClientData clientDataCache = new ClientData();
        private ClientData clientDataSendingCache = new ClientData();

        private IntentSelectorManager manager;
        private GameObject logSupervisor;
        private Text logParameterContainer;

        private bool ready = false;

        private void Awake()
        {
            savePath = Application.persistentDataPath + "/sendData";
            if (System.IO.Directory.Exists(savePath))
                System.IO.Directory.Delete(savePath, true);

            System.IO.Directory.CreateDirectory(savePath);

            manager = FindObjectOfType<IntentSelectorManager>();
            StartCoroutine(waitForEnvironment());
        }

        public IEnumerator waitForEnvironment()
        {
            yield return new WaitUntil(() => { return UMI3DEnvironmentLoader.Exists; });

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() =>
            {
                ready = true;
                logSupervisor = GameObject.Find("Log Supervisor"); // SCENE SPECIFIC
                logParameterContainer = logSupervisor.GetComponentInChildren<Text>();
            });
        }

        private void LateUpdate()
        {
            if (!ready || logParameterContainer.text != "1")
                return;
            clientDataCache.tracking.Add(FetchTrackingData());
            clientDataCache.scene.Add(FetchSceneData());
            if (cacheSize > cacheSizeMax)
            {
                var fileName = "clientData_" + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss.fff") + "_" + (Time.frameCount % 1000).ToString();
                clientDataSendingCache = clientDataCache;
                clientDataCache = new ClientData();
                SendDataAsync(clientDataSendingCache, fileName);
            }
        }

        private TrackingData FetchTrackingData()
        {
            var pointerPosition = manager.intentSelector.controller.gameObject.transform.position;
            var pointerRotation = manager.intentSelector.controller.gameObject.transform.rotation.eulerAngles;

            var head = Camera.main.transform;
            var headPosition = head.position;
            var headRotation = head.rotation.eulerAngles;

            double Round(float f)
            {
                return f >= 1e-4 ? Math.Round(f, 4) : 0;
            }
            var trackingData = new TrackingData()
            {
                t = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds(),

                p_x = Round(pointerPosition.x),
                p_y = Round(pointerPosition.y),
                p_z = Round(pointerPosition.z),
                p_r_x = Round(pointerRotation.x),
                p_r_y = Round(pointerRotation.y),
                p_r_z = Round(pointerRotation.z),

                h_x = Round(headPosition.x),
                h_y = Round(headPosition.y),
                h_z = Round(headPosition.z),
                h_r_x = Round(headRotation.x),
                h_r_y = Round(headRotation.y),
                h_r_z = Round(headRotation.z)
            };

            return trackingData;
        }

        private List<TargetData> FetchSceneData()
        {
            // SPECIAL FOR THIS SCENE
            var objectParent = GameObject.Find("ToreAnchor");
            var dynamicTargetsData = new List<TargetData>();
            for (int i = 0; i < objectParent.transform.childCount; i++)
            {
                var child = objectParent.transform.GetChild(i);
                if (child.name.EndsWith("d"))
                {
                    dynamicTargetsData.Add(new TargetData()
                    {
                        n = int.Parse(child.name.Replace("d","")),
                        x = child.transform.position.x,
                        y = child.transform.position.y,
                        z = child.transform.position.z
                    });
                }
            }
            return dynamicTargetsData;
        }

        private async void SendDataAsync(ClientData data, string fileName)
        {
            if (InteractionMapper.uploadFileParameterDto is null)
            {
                Debug.Log("Not sending logs. DTM not started in log mode");
                return;
            }
            var filePath = savePath + "/" + fileName + ".json";
            await WriteDataTask(data, filePath);

            data.Clear();

            var id = FileUploader.AddFileToUpload(filePath);
            InteractionMapper.uploadFileParameterDto.value = filePath;
            var req = new UploadFileRequestDto()
            {
                fileId = id,
                parameter = InteractionMapper.uploadFileParameterDto,
                id = InteractionMapper.uploadFileParameterDto.id
            };
            UMI3DClientServer.SendData(req, true);
        }

        private Task WriteDataTask(ClientData data, string filePath)
        {
            return Task.Run(
                delegate
                {
                    using System.IO.StreamWriter file = System.IO.File.CreateText(filePath);
                    JsonSerializer serializer = new JsonSerializer();
                    //serialize object directly into file stream
                    serializer.Serialize(file, data);
                });
        }
    }
}