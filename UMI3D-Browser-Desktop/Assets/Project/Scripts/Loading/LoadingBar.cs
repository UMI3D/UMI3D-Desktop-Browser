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
using BrowserDesktop.Cursor;
using inetum.unityUtils;
using System.Collections;
using umi3d.cdk;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadingBar : Singleton<LoadingBar>
{
    VisualElement root;
    VisualElement loadingBarContainer;
    VisualElement loadingBarProgress;
    VisualElement loadingScreen;
    Label loaderTxt;

    float value = 0;

    public LoadingBar() : base()
    {}

    public void Setup(VisualElement root)
    {
        loadingBarProgress = root.Q<VisualElement>("loading-bar-progress");
        loadingBarContainer = root.Q<VisualElement>("loading-bar-container");
        loadingScreen = root.Q<VisualElement>("loading-screen");
        loaderTxt = root.Q<Label>("loader-txt");

        Debug.Assert(loadingBarProgress != null);
        Debug.Assert(loadingBarContainer != null);
        Debug.Assert(loadingScreen != null);
        Debug.Assert(loaderTxt != null);

        loadingBarProgress.style.width = 0;
        loadingScreen.style.display = DisplayStyle.Flex;
        loadingScreen.style.display = DisplayStyle.None;

        value = 0;
        MainThreadDispatcher.UnityMainThreadDispatcher.Instance().Enqueue(SetValueNextFrame());

        UMI3DEnvironmentLoader.Instance.onProgressChange.AddListener(OnProgressChange);
        UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(Hide);
        UMI3DResourcesManager.Instance.onProgressChange.AddListener(OnProgressChange);

    }

    public void OnProgressChange(float val)
    {
        if ((loadingScreen.style.display == DisplayStyle.None) && val < 1f)
        {
            loadingScreen.style.display = DisplayStyle.Flex;
            umi3d.baseBrowser.Controller.BaseCursor.SetMovement(this, umi3d.baseBrowser.Controller.BaseCursor.CursorMovement.Free);
            DesktopController.CanProcess = false;
            
        }
        if (val > 1)
        {
            val = 1;
            Hide();
        }
        value = val;
        loadingBarProgress.style.width = val * loadingBarContainer.resolvedStyle.width;
    }

    IEnumerator SetValueNextFrame()
    {
        yield return new WaitForEndOfFrame();
        OnProgressChange(value);
    }

    public void SetText(string text)
    {
        loaderTxt.text = text;
    }

    void Hide()
    {
        loadingScreen.style.display = DisplayStyle.None;
        umi3d.baseBrowser.Controller.BaseCursor.UnSetMovement(this);
        DesktopController.CanProcess = true;
    }

}
