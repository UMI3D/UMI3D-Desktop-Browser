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
using UnityEngine;

namespace umi3d.baseBrowser.Navigation
{
    public partial class BaseFPSNavigation
    {
        protected bool vehicleFreeHead = false;
        protected cdk.UMI3DNodeInstance globalFrame;


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="data"></param>
        public override void UpdateFrame(common.FrameRequestDto data)
        {
            //vehicleFreeHead = data.StopNavigation;

            if (data.FrameId == 0)
            {
                this.transform.SetParent(cdk.UMI3DEnvironmentLoader.Instance.transform, true);
                //this.transform.localPosition = data.position;
                //this.transform.localRotation = data.rotation;
                DontDestroyOnLoad(cdk.UMI3DNavigation.Instance);
                globalFrame.Delete -= GlobalFrameDeleted;
                globalFrame = null;
            }
            else
            {
                cdk.UMI3DNodeInstance Frame = cdk.UMI3DEnvironmentLoader.GetNode(data.FrameId);
                if (Frame != null)
                {
                    globalFrame = Frame;
                    this.transform.SetParent(Frame.transform, true);
                    //this.transform.localPosition = data.position;
                    //this.transform.localRotation = data.rotation;
                    globalFrame.Delete += GlobalFrameDeleted;
                }
            }
        }

        void GlobalFrameDeleted()
        {
            cdk.UMI3DNavigation.Instance.transform.SetParent(cdk.UMI3DEnvironmentLoader.Instance.transform, true);
            DontDestroyOnLoad(cdk.UMI3DNavigation.Instance);
        }

    }
}
