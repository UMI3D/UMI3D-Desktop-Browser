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
using UnityEngine;

public class Notification : AbstractNotification
{
    public RectTransform Rect;
    public float rightMarge;
    float xDelta;
    public float MovingTime = 0.5f;

    protected override IEnumerator TimeBeforeDesapearing(float value)
    {
        yield return base.TimeBeforeDesapearing(value);
        yield return StartCoroutine(ToXValue(2 * Rect.sizeDelta.x + rightMarge));
        Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(ToXValue(-Rect.sizeDelta.x + Rect.localPosition.x - rightMarge));
    }

    IEnumerator ToXValue(float value)
    {
        xDelta = Rect.localPosition.x;
        var wait = new WaitForFixedUpdate();
        if (MovingTime <= 0)
        {
            Rect.localPosition = new Vector3(value, 0, 0);
        }
        else
        {
            float DTime = MovingTime;
            while (Mathf.Abs(xDelta - value) > 0)
            {
                yield return wait;
                DTime -= Time.fixedDeltaTime;
                xDelta = Mathf.Lerp(xDelta, value, 1 - DTime / MovingTime);
                Rect.localPosition = new Vector3(xDelta, 0, 0);
            }
        }
    }
}
