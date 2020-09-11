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
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbstractNotification : MonoBehaviour
{
    float TimeLeft;
    public Image Image;


    public Slider Slider;
    [SerializeField]
    TMP_Text title = null;
    [SerializeField]
    TMP_Text content = null;

    public void SetNotificationTime(float time)
    {
        StartCoroutine(TimeBeforeDesapearing(time));
    }

    protected virtual IEnumerator TimeBeforeDesapearing(float value)
    {
        var wait = new WaitForFixedUpdate();
        Slider.maxValue = TimeLeft = value;
        Slider.minValue = 0;
        while (TimeLeft > 0)
        {
            yield return wait;
            TimeLeft -= Time.fixedDeltaTime;
            Slider.value = TimeLeft;
        }
        Destroy(gameObject);
    }

    public Sprite Icon2D { get => Image.sprite; set => Image.sprite = value; }

    public string Content { get => content.text; set => content.text = value; }

    public string Title { get => title.text; set => title.text = value; }
}
