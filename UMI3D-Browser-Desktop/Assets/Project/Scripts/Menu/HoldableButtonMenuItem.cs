/*
Copyright 2019 Gfi Informatique

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
using umi3d.common;
using umi3d.cdk.menu;
using UnityEngine;
using UnityEngine.Events;
using System;

/// <summary>
/// Button menu item.
/// </summary>
/// <see cref="BooleanMenuItem"/>
public class HoldableButtonMenuItem : MenuItem, umi3d.common.IObservable<bool>
{
    /// <summary>
    /// If true, the button will stay pressed on selection.
    /// </summary>
    public bool Holdable = false;

    /// <summary>
    /// Image to display (if any).
    /// </summary>
    public Sprite sprite;


    private bool buttonDown = false;

    /// <summary>
    /// Subscribers on value change
    /// </summary>
    private List<Action<bool>> subscribers = new List<Action<bool>>();



    /// <summary>
    /// Subscribe a callback for button press.
    /// </summary>
    /// <param name="callback">Callback to invoke on button press</param>
    public virtual bool Subscribe(Action<bool> callback)
    {
        if (!subscribers.Contains(callback))
        {
            subscribers.Add(callback);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Unsubscribe a callback from the value change.
    /// </summary>
    /// <param name="callback"></param>
    public virtual bool UnSubscribe(Action<bool> callback)
    {
        return subscribers.Remove(callback);
    }

    public override string ToString()
    {
        return Name;
    }

    public virtual bool GetValue()
    {
        return buttonDown;
    }

    public virtual void NotifyValueChange(bool down)
    {
        buttonDown = down;

        if (Holdable || buttonDown)
            foreach (Action<bool> sub in subscribers)
            {
                sub.Invoke(buttonDown);
            }
    }
}
