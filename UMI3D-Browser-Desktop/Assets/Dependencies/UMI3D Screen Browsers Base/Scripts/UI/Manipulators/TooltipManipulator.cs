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
using umi3d.commonScreen;
using UnityEngine;
using UnityEngine.UIElements;

public class TooltipManipulator : MouseManipulator
{
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseEnterEvent>(MouseEntered);
        target.RegisterCallback<MouseLeaveEvent>(MouseLeft);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseEnterEvent>(MouseEntered);
        target.UnregisterCallback<MouseLeaveEvent>(MouseLeft);
    }

    protected virtual void MouseEntered(MouseEnterEvent evt)
    {
        evt.StopPropagation();
        if (!string.IsNullOrEmpty(target.tooltip))
        {
            TooltipsLayer_C.SetTextPosition(target);
            TooltipsLayer_C.Text = target.tooltip;
        }
    }

    protected virtual void MouseLeft(MouseLeaveEvent evt)
    {
        evt.StopPropagation();
        if (!string.IsNullOrEmpty(target.tooltip)) TooltipsLayer_C.Text = null;
    }
}
