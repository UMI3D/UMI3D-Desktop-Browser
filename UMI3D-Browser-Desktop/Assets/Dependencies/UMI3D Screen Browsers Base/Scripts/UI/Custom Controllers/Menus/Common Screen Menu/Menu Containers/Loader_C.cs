/*
Copyright 2019 - 2022 Inetum

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
using umi3d.commonScreen.Displayer;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.menu
{
    public class Loader_C : BaseMenuContainer_C<LoaderScreens>, IGameView
    {
        public new class UxmlFactory : UxmlFactory<Loader_C, UxmlTraits> { }

        public virtual string StyleSheetLoaderPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/loader";
        public virtual string USSCustomClassLoader => "loader";

        public LoadingScreen_C Loading = new LoadingScreen_C { name = "loading-screen" };
        public FormScreen_C Form = new FormScreen_C { name = "form-screen" };

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetLoaderPath);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            AddToClassList(USSCustomClassLoader);
        }

        protected override void InitElement()
        {
            base.InitElement();
            Navigation_ScrollView.RemoveFromHierarchy();

            umi3d.cdk.collaboration.UMI3DCollaborationClientServer.onProgress.AddListener(OnProgress);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            CurrentScreen = LoaderScreens.Loading;
        }

        #region Implementation

        umi3d.cdk.Progress _progress = null;

        void OnProgress(umi3d.cdk.Progress progress)
        {
            if (_progress != null)
            {
                _progress.OnCompleteUpdated -= OnCompleteUpdated;
                _progress.OnFailedUpdated -= OnFailedUpdated;
                _progress.OnStatusUpdated -= OnStatusUpdated;
            }
            _progress = progress;
            void OnCompleteUpdated(float i)
            {
                CurrentScreen = LoaderScreens.Loading;
                Loading.Value = _progress.progressPercent / 100f;
                Loading.LoadingBar.LocalisedTitle = new LocalisationAttribute
                (
                    $"Total loading : {_progress.progressPercent.ToString("0.00")} %",
                    "Other",
                    "LoadingPercent",
                    new string[] { _progress.progressPercent.ToString("0.00") }
                );
            }
            void OnFailedUpdated(float i) { Loading.Value = _progress.progressPercent / 100f; }
            void OnStatusUpdated(string i)
            {
                CurrentScreen = LoaderScreens.Loading;
                Loading.Message = _progress.currentState;
            }
            _progress.OnCompleteUpdated += OnCompleteUpdated;
            _progress.OnFailedUpdated += OnFailedUpdated;
            _progress.OnStatusUpdated += OnStatusUpdated;
            Loading.Value = _progress.progressPercent / 100f;
            Loading.Message = _progress.currentState;
        }

        protected override void GetScreen(LoaderScreens screenEnum, out BaseMenuScreen_C screen)
        {
            switch (screenEnum)
            {
                case LoaderScreens.Loading:
                    screen = Loading;
                    break;
                case LoaderScreens.Form:
                    screen = Form;
                    break;
                default:
                    screen = null;
                    break;
            }
        }

        protected override void GetScreenAndButton(LoaderScreens screenEnum, out BaseMenuScreen_C screen, out Button_C button)
        {
            switch (screenEnum)
            {
                case LoaderScreens.Loading:
                    screen = Loading;
                    button = null;
                    break;
                case LoaderScreens.Form:
                    screen = Form;
                    button = null;
                    break;
                default:
                    screen = null;
                    button = null;
                    break;
            }
        }

        protected override void RemoveAllScreen()
        {
            Loading.RemoveFromHierarchy();
            Form.RemoveFromHierarchy();
        }

        public Action<object> SetMovement;
        public Action<object> UnSetMovement;
        public Action<bool> ControllerCanProcess;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="persistentVisual"></param>
        public void TransitionIn(VisualElement persistentVisual)
            => Transition(persistentVisual, false);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="persistentVisual"></param>
        public void TransitionOut(VisualElement persistentVisual)
            => Transition(persistentVisual, true);

        protected virtual void Transition(VisualElement persistentVisual, bool revert)
        {
            this
                .SetOpacity(!revert ? 1 : 0)
                .WithAnimation(.5f)
                .SetCallback(revert ? RemoveFromHierarchy : null);
        }

        #endregion
    }
}
