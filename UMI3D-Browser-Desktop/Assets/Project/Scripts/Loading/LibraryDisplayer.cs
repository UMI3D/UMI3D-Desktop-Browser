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

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LibraryDisplayer : MonoBehaviour
{
    public TMP_Text label;
    public TMP_Text date;
    public Button delete;

    public void set(string label, string date, Action Ondelete)
    {
        this.label.text = label;
        this.date.text = date;
        this.delete.onClick.RemoveAllListeners();
        this.delete.onClick.AddListener(Ondelete.Invoke);
    }
}
