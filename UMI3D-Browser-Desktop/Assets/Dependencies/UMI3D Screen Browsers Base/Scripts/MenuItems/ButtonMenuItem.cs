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
using umi3d.cdk.menu;
using umi3d.common;
using UnityEngine.Events;

/// <summary>
/// Button menu item.
/// </summary>
/// <see cref="BooleanMenuItem"/>
public partial class ButtonMenuItem : MenuItem
{
    /// <summary>
    /// If true: notify when down and up.
    /// If false: notify only when up.
    /// </summary>
    public bool IsHoldable = false;

    /// <summary>
    /// State of the button (Down means active).
    /// </summary>
    private bool m_isDown = false;
}

public partial class ButtonMenuItem : IObservable<bool>
{
    /// <summary>
    /// Subscribers on value change
    /// </summary>
    private List<UnityAction<bool>> subscribers = new List<UnityAction<bool>>();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual bool GetValue() => m_isDown;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="newValue"></param>
    public virtual void NotifyValueChange(bool newValue)
    {
        if (m_isDown == newValue) return;
        m_isDown = newValue;

        foreach (UnityAction<bool> sub in subscribers) sub.Invoke(m_isDown);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="callback"></param>
    public virtual void Subscribe(UnityAction<bool> callback)
    {
        if (!subscribers.Contains(callback)) subscribers.Add(callback);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="callback"></param>
    public virtual void UnSubscribe(UnityAction<bool> callback) => subscribers.Remove(callback);
}
