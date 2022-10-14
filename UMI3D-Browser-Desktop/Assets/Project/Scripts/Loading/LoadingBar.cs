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
using umi3d.cdk;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.connection
{
    public class LoadingBar : inetum.unityUtils.Singleton<LoadingBar>
    {
        public string Text
        {
            get => loadingBar.Text;
            set => loadingBar.Text = value;
        }

        LoadingBarElement loadingBar; 

        public void Setup(VisualElement root)
        {
            loadingBar = new LoadingBarElement(root.Q<VisualElement>("loading-screen"),OnProgressChange,OnHide,OnDisplay);

            cdk.collaboration.UMI3DCollaborationClientServer.onProgress.AddListener(NewProgress);

            cdk.UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(loadingBar.Hide);
        }

        Progress _progress = null;
        void NewProgress(Progress progress)
        {

            if (_progress != null)
            {
                _progress.OnCompleteUpdated.RemoveListener(OnCompleteUpdated);
                _progress.OnFailedUpdated.RemoveListener(OnFailedUpdated);
                _progress.OnStatusUpdated.RemoveListener(OnStatusUpdated);
            }
            _progress = progress;

            void OnCompleteUpdated(float i) { loadingBar.OnProgressChange(_progress.progressPercent / 100f); }
            void OnFailedUpdated(float i) { loadingBar.OnProgressChange(_progress.progressPercent / 100f); }
            void OnStatusUpdated(string i) { Text = _progress.currentState; }

            _progress.OnCompleteUpdated.AddListener(OnCompleteUpdated);
            _progress.OnFailedUpdated.AddListener(OnFailedUpdated);
            _progress.OnStatusUpdated.AddListener(OnStatusUpdated);

            loadingBar.OnProgressChange(_progress.progressPercent/100f);
            Text = _progress.currentState;

            if (!loadingBar.isDisplayed && _progress.started)
            {
                loadingBar.Display();
            }
            if (loadingBar.isDisplayed && !_progress.started)
            {
                loadingBar.Hide();
            }

        }

        public void OnProgressChange(float val)
        {
            if (val < 0) val = 0;
            if (!loadingBar.isDisplayed && val < 1f)
            {
                loadingBar.Display();
            }
            if (loadingBar.isDisplayed && val >= 1)
            {
                loadingBar.Hide();
            }
        }

        public void OnProgressOver()
        {
            OnProgressChange(3f);
        }

        void OnHide()
        {
            Controller.BaseCursor.UnSetMovement(this);
            Controller.BaseController.CanProcess = true;
        }

        void OnDisplay()
        {
            Controller.BaseCursor.SetMovement(this, Controller.BaseCursor.CursorMovement.Free);
            Controller.BaseController.CanProcess = false;
        }
    }

    public class LoadingBarElement
    {
        public string Text
        {
            get => loaderTxt.text;
            set => loaderTxt.text = value;
        }

        public bool isDisplayed => loadingScreen.style.display != DisplayStyle.None;

        VisualElement loadingBarContainer;
        VisualElement loadingBarProgress;
        VisualElement loadingScreen;
        Label loaderTxt;
        Label loaderValue;

        public Action<float> OnProgressChanges;
        public Action OnHide;
        public Action OnDisplay;

        float value = 0;

        public LoadingBarElement(VisualElement root, Action<float> OnProgressChanges = null, Action OnHide = null, Action OnDisplay = null)
        {
            loadingBarProgress = root.Q<VisualElement>("loading-bar-progress");
            loadingBarContainer = root.Q<VisualElement>("loading-bar-container");
            loadingScreen = root;
            loaderTxt = root.Q<Label>("loader-txt");
            loaderValue = root.Q<Label>("loader-value");

            Debug.Assert(loadingBarProgress != null);
            Debug.Assert(loadingBarContainer != null);
            Debug.Assert(loadingScreen != null);
            Debug.Assert(loaderTxt != null);

            loadingBarProgress.style.width = 0;
            loadingScreen.style.display = DisplayStyle.Flex;
            loadingScreen.style.display = DisplayStyle.None;

            value = 0;
            SetValueNextFrame();

            this.OnProgressChanges = OnProgressChanges;
            this.OnHide = OnHide;
            this.OnDisplay = OnDisplay;
        }

        public void OnProgressChange(float val)
        {
            OnProgressChanges?.Invoke(val);

            if (val > 1)
                val = 1;
            value = val;
            loaderValue.text = (val*100).ToString("N2")+" %";
            loadingBarProgress.style.width = val * loadingBarContainer.resolvedStyle.width;

        }

        async void SetValueNextFrame()
        {
            await UMI3DAsyncManager.Yield();
            OnProgressChange(value);
        }

        public void Display()
        {
            loadingScreen.style.display = DisplayStyle.Flex;
            OnDisplay?.Invoke();
        }

        public void Hide()
        {
            loadingScreen.style.display = DisplayStyle.None;
            OnHide?.Invoke();
        }
    }

}
