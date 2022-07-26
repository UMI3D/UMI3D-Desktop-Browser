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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.connection
{
    public class LoadingBar : inetum.unityUtils.Singleton<LoadingBar>
    {
        public string Text
        {
            get => loaderTxt.text;
            set => loaderTxt.text = value;
        }

        VisualElement loadingBarContainer;
        VisualElement loadingBarProgress;
        VisualElement loadingScreen;
        Label loaderTxt;

        float value = 0;

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

            cdk.UMI3DEnvironmentLoader.Instance.onProgressChange.AddListener(OnProgressChange);
            cdk.UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(Hide);
            cdk.UMI3DResourcesManager.Instance.onProgressChange.AddListener(OnProgressChange);

        }

        public void OnProgressChange(float val)
        {
            if ((loadingScreen.style.display == DisplayStyle.None) && val < 1f)
            {
                loadingScreen.style.display = DisplayStyle.Flex;
                Controller.BaseCursor.SetMovement(this, Controller.BaseCursor.CursorMovement.Free);
                Controller.BaseController.CanProcess = false;
            }
            if (val > 1)
            {
                val = 1;
                Hide();
            }
           value = val; 
            loadingBarProgress.style.width = val * loadingBarContainer.resolvedStyle.width;
        }

        System.Collections.IEnumerator SetValueNextFrame()
        {
            yield return new WaitForEndOfFrame();
            OnProgressChange(value);
        }

        void Hide()
        {
            loadingScreen.style.display = DisplayStyle.None;
            Controller.BaseCursor.UnSetMovement(this);
            Controller.BaseController.CanProcess = true;
        }
    }
}
