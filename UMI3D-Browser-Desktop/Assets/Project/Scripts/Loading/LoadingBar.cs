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
using UnityEngine.UIElements;

public class LoadingBar
{
    VisualElement root;
    VisualElement loadingBarContainer;
    VisualElement loadingBarProgress;
    VisualElement loadingScreen;

    public LoadingBar(VisualElement root)
    {
        loadingBarProgress = root.Q<VisualElement>("loading-bar-progress");
        loadingBarContainer = root.Q<VisualElement>("loading-bar-container");
        loadingScreen = root.Q<VisualElement>("loading-screen");

        UMI3DEnvironmentLoader.Instance.onProgressChange.AddListener(OnProgressChange);
        UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(Hide);
        UMI3DResourcesManager.Instance.onProgressChange.AddListener(OnProgressChange);
    }

    public void OnProgressChange(float val)
    {
        if ((loadingScreen.style.display == DisplayStyle.None) && val < 1f)
        {
            loadingScreen.style.display = DisplayStyle.Flex;
            MouseAndKeyboardController.CanProcess = false;
        }

        loadingBarProgress.style.width = val * loadingBarContainer.resolvedStyle.width;
    }

    void Hide()
    {
        Debug.Log("<color=green>Loading finished.</color>");

        loadingScreen.style.display = DisplayStyle.None;
        MouseAndKeyboardController.CanProcess = true;
    }

}
