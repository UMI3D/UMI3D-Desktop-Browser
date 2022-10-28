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
using System.Collections.Generic;
using UnityEngine.UIElements;

public class CustomLoader : CustomMenuContainer<LoaderScreens>, IGameView
{
    public new class UxmlTraits : CustomMenuContainer<LoaderScreens>.UxmlTraits
    {
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomLoader;

            custom.Set
                (
                    m_currentScreen.GetValueFromBag(bag, cc),
                    m_displayHeader.GetValueFromBag(bag, cc),
                    m_version.GetValueFromBag(bag, cc)
                 );
        }
    }

    public virtual string StyleSheetLoaderPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/loader";
    public virtual string USSCustomClassLoader => "loader";

    public CustomLoadingScreen Loading;
    public CustomFormScreen Form;

    public override void InitElement()
    {
        base.InitElement();

        try
        {
            this.AddStyleSheetFromPath(StyleSheetLoaderPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassLoader);

        Navigation_ScrollView.RemoveFromHierarchy();

        umi3d.cdk.UMI3DEnvironmentLoader.Instance?.onEnvironmentLoaded?.AddListener(() => ControllerCanProcess?.Invoke(true));

        umi3d.cdk.collaboration.UMI3DCollaborationClientServer.onProgress.AddListener(OnProgress);
    }

    umi3d.cdk.Progress _progress = null;
    void OnProgress(umi3d.cdk.Progress progress)
    {
        if (_progress != null)
        {
            _progress.OnCompleteUpdated.RemoveListener(OnCompleteUpdated);
            _progress.OnFailedUpdated.RemoveListener(OnFailedUpdated);
            _progress.OnStatusUpdated.RemoveListener(OnStatusUpdated);
        }
        _progress = progress;
        void OnCompleteUpdated(float i) 
        {
            CurrentScreen = LoaderScreens.Loading;
            Loading.Value = _progress.progressPercent / 100f;
            Loading.LoadingBar.title = $"Total loading : {_progress.progressPercent.ToString("0.00")} %";
        }
        void OnFailedUpdated(float i) { Loading.Value = _progress.progressPercent / 100f; }
        void OnStatusUpdated(string i) 
        {
            CurrentScreen = LoaderScreens.Loading;
            Loading.Message = _progress.currentState;
        }
        _progress.OnCompleteUpdated.AddListener(OnCompleteUpdated);
        _progress.OnFailedUpdated.AddListener(OnFailedUpdated);
        _progress.OnStatusUpdated.AddListener(OnStatusUpdated);
        Loading.Value = _progress.progressPercent / 100f;
        Loading.Message = _progress.currentState;
    }

    public override void Set() => Set(LoaderScreens.Loading, false, null);

    public virtual void Set(LoaderScreens screen, bool displayHeader, string version)
    {
        Set(displayHeader, version);

        CurrentScreen = screen;
    }

    protected override void GetScreen(LoaderScreens screenEnum, out CustomMenuScreen screen)
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

    protected override void GetScreenAndButton(LoaderScreens screenEnum, out CustomMenuScreen screen, out CustomButton button)
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

    public void TransitionIn(VisualElement persistentVisual)
        => Transition(persistentVisual, false);

    public void TransitionOut(VisualElement persistentVisual)
        => Transition(persistentVisual, true);

    protected virtual void Transition(VisualElement persistentVisual, bool revert)
    {
        this.AddAnimation
        (
            persistentVisual,
            () => style.opacity = 0,
            () => style.opacity = 1,
            "opacity",
            0.5f,
            revert: revert,
            callback: revert ? RemoveFromHierarchy : null
        );
    }
}
