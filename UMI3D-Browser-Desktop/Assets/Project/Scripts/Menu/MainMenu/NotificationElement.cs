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

using MainThreadDispatcher;
using System;

using UnityEngine.UIElements;

public class NotificationElement : VisualElement
{
    // UxmlFactory and UxmlTraits allow UIBuilder to use CardElement as a building block
    public new class UxmlFactory : UxmlFactory<NotificationElement, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits { }

    VisualElement progressionBar;

    public void Setup(string title, string content, int displayTime, Action OnDelete = null)
    {
        this.Q<Label>("notification-title").text = title;
        this.Q<Label>("notification-content").text = content;
        progressionBar = this.Q<VisualElement>("notification-progression-bar");

        this.experimental.animation.Start(0, 100, displayTime, (elt, val) =>
        {
            progressionBar.style.width = (val / 100) * 250;
        }).OnCompleted(() => {
            OnDelete.Invoke();
            this.RemoveFromHierarchy();
        });
    }
}
