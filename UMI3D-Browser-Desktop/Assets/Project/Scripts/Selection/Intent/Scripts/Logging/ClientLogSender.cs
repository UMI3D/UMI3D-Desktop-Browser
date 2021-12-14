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
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.cdk.interaction.selection.intent.log
{
    internal class ClientLogSender : MonoBehaviour
    {
        [SerializeField]
        private int cacheSizeMax = 20;

        private int cacheSize = 0;

        private string savePath;

        private List<TrackingData> trackingDataCache = new List<TrackingData>();

        private List<List<TargetData>> sceneDataCache = new List<List<TargetData>>();

        private IntentSelectorManager manager;

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
            });
        }

        private void LateUpdate()
        {
            if (!ready)
                return;
            trackingDataCache.Add(FetchTrackingData());
            sceneDataCache.Add(FetchSceneData());
            cacheSize++;
            if (cacheSize > cacheSizeMax)
            {
                var fileName = "clientData_" + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss") + "_" + (Time.frameCount % 1000).ToString();
                SendData(new ClientData() { tracking = trackingDataCache, scene = sceneDataCache }, fileName);
                trackingDataCache.Clear();
                sceneDataCache.Clear();
                cacheSize = 0;
            }
        }

        private TrackingData FetchTrackingData()
        {
            var pointerPosition = manager.intentSelector.controller.gameObject.transform.position;
            var pointerRotation = manager.intentSelector.controller.gameObject.transform.rotation.eulerAngles;

            var head = Camera.main.transform;
            var headPosition = head.position;
            var headRotation = head.rotation.eulerAngles;

            var trackingData = new TrackingData()
            {
                t = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds(),

                p_x = pointerPosition.x,
                p_y = pointerPosition.y,
                p_z = pointerPosition.z,
                p_r_x = pointerRotation.x,
                p_r_y = pointerRotation.y,
                p_r_z = pointerRotation.z,

                h_x = headPosition.x,
                h_y = headPosition.y,
                h_z = headPosition.z,
                h_r_x = headRotation.x,
                h_r_y = headRotation.y,
                h_r_z = headRotation.z,
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
                if (child.name.EndsWith("(dynamic)"))
                {
                    dynamicTargetsData.Add(new TargetData()
                    {
                        n = child.name,
                        x = child.transform.position.x,
                        y = child.transform.position.y,
                        z = child.transform.position.z
                    });
                }
            }
            return dynamicTargetsData;
        }

        private void SendData(object data, string fileName)
        {
            var filePath = savePath + "/" + fileName + ".json";

            using (System.IO.StreamWriter file = System.IO.File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, data);
            }

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
    }
}