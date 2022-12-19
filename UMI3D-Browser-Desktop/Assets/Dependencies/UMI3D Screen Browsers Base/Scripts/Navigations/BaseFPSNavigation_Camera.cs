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
        public enum State { Default, FreeHead, FreeMousse }

        #region Fields

        [Header("Camera")]
        [SerializeField]
        protected Transform viewpoint;
        [SerializeField]
        protected Transform neckPivot;
        [SerializeField]
        protected Transform head;
        public State state;

        protected Vector3 lastAngleView;

        #endregion

        /// <summary>
        /// Rotates camera.
        /// </summary>
        protected abstract void HandleView();

        /// <summary>
        /// Common part of the HandleView method.
        /// </summary>
        /// <param name="angleView"></param>
        /// <param name="angularSpeed"></param>
        protected void BaseHandleView(Vector3 angleView, Vector2 angularSpeed)
        {
            Vector3 result = angleView + ((Vector3)angularSpeed).NormalizeAngle();
            if (changeToDefault)
            {
                result = lastAngleView;
                changeToDefault = false;
            }
            Vector3 displayResult;

            result.x = Mathf.Clamp(result.x, data.XAngleRange.x, data.XAngleRange.y);
            displayResult = result;
            displayResult.x = Mathf.Clamp(displayResult.x, data.XDisplayAngleRange.x, data.XDisplayAngleRange.y);

            if (state == State.Default)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, result.y, 0));
                lastAngleView = result;
            }
            else
            {
                Vector3 angleNeck = transform.rotation.eulerAngles.NormalizeAngle();
                float delta = Mathf.DeltaAngle(result.y, angleNeck.y);

                if (delta < data.YAngleRange.x) result.y = -data.YAngleRange.x + angleNeck.y;
                if (delta > data.YAngleRange.y) result.y = -data.YAngleRange.y + angleNeck.y;
            }
            viewpoint.transform.rotation = Quaternion.Euler(result);
            neckPivot.transform.rotation = Quaternion.Euler(new Vector3(Mathf.Clamp(result.x, -maxNeckAngle, maxNeckAngle), result.y, result.z));
            head.transform.rotation = Quaternion.Euler(displayResult);
        }
    }
}

public static class Vector3Extension
{
    /// <summary>
    /// For each component (x, y, z) Mathf.DeltaAngle(0, component)
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 NormalizeAngle(this Vector3 angle)
    {
        angle.x = Mathf.DeltaAngle(0, angle.x);
        angle.y = Mathf.DeltaAngle(0, angle.y);
        angle.z = Mathf.DeltaAngle(0, angle.z);

        return angle;
    }
}
