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
using System.Collections.Generic;
using umi3d.baseBrowser.Controller;
using umi3d.baseBrowser.inputs.interactions;
using UnityEngine;

namespace BrowserDesktop.Cursor
{
    public class CursorKeyInput : KeyboardManipulation
    {
        protected void Update()
        {
            distCursor += Input.mouseScrollDelta.y * Time.deltaTime * ScrollToDistSpeed;
            if (distCursor < MinimumCursorDistance) distCursor = MinimumCursorDistance;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = umi3d.common.Physics.RaycastAll(ray, constrainDistanceChange ? distCursor + MaxDistCursorDelta : 100);

            ignore = new List<Transform>();
            ignore.AddRange(AvatarParent.gameObject.GetComponentsInChildren<Transform>());
            lastObject = (controller as BaseController)?.mouseData.CurrentHoveredTransform;
            if (lastObject != null)
                ignore.AddRange(lastObject.gameObject.GetComponentsInChildren<Transform>());
            bool ok = false;
            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    if (!ignore.Contains(hit.transform))
                    {
                        Cursor.position = hit.point;
                        distCursor = hit.distance;
                        ok = true;
                        break;
                    }
                }
            }
            if (!ok)
            {
                Cursor.position = ray.GetPoint(distCursor);
            }
            Cursor.LookAt(Head);
        }
    }
}