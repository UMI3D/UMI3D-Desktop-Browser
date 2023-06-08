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
using System.Collections;
using System.Collections.Generic;
using umi3d.common;
using UnityEngine;

namespace umi3d.baseBrowser.Navigation
{
    public partial class BaseFPSNavigation
    {
        protected bool vehicleFreeHead = false;
        protected cdk.UMI3DNodeInstance globalVehicle;

        public override void UpdateFrame(FrameRequestDto data)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="data"></param>
        /*public override void Embark(common.VehicleDto data)
        {
            vehicleFreeHead = data.StopNavigation;

            if (data.VehicleId == 0)
            {
                this.transform.SetParent(cdk.UMI3DEnvironmentLoader.Instance.transform, true);
                this.transform.localPosition = data.position;
                this.transform.localRotation = data.rotation;
                DontDestroyOnLoad(cdk.UMI3DNavigation.Instance);
                globalVehicle.Delete -= new System.Action(() => {
                    cdk.UMI3DNavigation.Instance.transform.SetParent(cdk.UMI3DEnvironmentLoader.Instance.transform, true);
                    DontDestroyOnLoad(cdk.UMI3DNavigation.Instance);
                });
                globalVehicle = null;
            }
            else
            {
                cdk.UMI3DNodeInstance vehicle = cdk.UMI3DEnvironmentLoader.GetNode(data.VehicleId);
                if (vehicle != null)
                {
                    globalVehicle = vehicle;
                    this.transform.SetParent(vehicle.transform, true);
                    this.transform.localPosition = data.position;
                    this.transform.localRotation = data.rotation;
                    globalVehicle.Delete += new System.Action(() => {
                        cdk.UMI3DNavigation.Instance.transform.SetParent(cdk.UMI3DEnvironmentLoader.Instance.transform, true);
                        DontDestroyOnLoad(cdk.UMI3DNavigation.Instance);
                    });
                }
            }
        }*/
    }
}
