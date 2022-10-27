/*
Copyright 2019 - 2021 Inetum

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
using UnityEngine;

namespace umi3d.baseBrowser.notification
{
    public class Notification3D : AbstractNotification
    {
        public Transform Parent
        {
            get => transform.parent; set
            {
                transform.SetParent(value);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

            }
        }

        GameObject _icon3d;

        public GameObject Icon3D
        {
            get => _icon3d; set
            {
                if (_icon3d != null) Destroy(_icon3d);
                _icon3d = value;
                _icon3d.transform.SetParent(transform);
                _icon3d.transform.localPosition = Vector3.zero;
                _icon3d.transform.localRotation = Quaternion.identity;
            }
        }
    }
}