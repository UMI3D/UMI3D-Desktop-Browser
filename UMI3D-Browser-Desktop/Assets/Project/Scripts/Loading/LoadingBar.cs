using BrowserDesktop.Controller;
using System.Collections;
using System.Collections.Generic;
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
