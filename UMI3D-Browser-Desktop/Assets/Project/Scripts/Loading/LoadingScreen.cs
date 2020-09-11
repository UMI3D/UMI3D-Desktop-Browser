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

using BrowserDesktop.Controller;
using umi3d.cdk;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Scrollbar Scrollbar;
    public GameObject pannel;
    public GameObject ConnectionPannel;

    private void Start()
    {
        UMI3DEnvironmentLoader.Instance.onProgressChange.AddListener(OnProgressChange);
        UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(Hide);
        UMI3DResourcesManager.Instance.onProgressChange.AddListener(OnProgressChange);
    }

    public void OnProgressChange(float val)
    {
        if(!pannel.activeSelf && val < 1f)
        {
            pannel.SetActive(true);
            MouseAndKeyboardController.CanProcess = false;
        }

        Scrollbar.size = val;
    }

    void Hide()
    {
        pannel.SetActive(false);
        ConnectionPannel.SetActive(false);
        MouseAndKeyboardController.CanProcess = true;
    }


}
