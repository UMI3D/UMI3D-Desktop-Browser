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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomGamePanel : VisualElement, ICustomElement
{
    public enum GameViews
    {
        Loader,
        GameMenu,
        Game
    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlEnumAttributeDescription<GameViews> m_currentView = new UxmlEnumAttributeDescription<GameViews>
        {
            name = "current-view",
            defaultValue = GameViews.Game
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomGamePanel;

            custom.Set
                (
                    m_currentView.GetValueFromBag(bag, cc)
                );
        }
    }

    public virtual string StyleSheetGamePath => $"USS/game";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/gamePanel";
    public virtual string USSCustomClassName => "game-panel";

    public virtual GameViews CurrentView
    {
        get => m_currentGameView;
        set
        {
            UnSetMovement?.Invoke(this);
            m_currentGameView = value;
            ViewStack.Clear();
            ViewStack.Push(value);
            RemoveAllView();

            VisualElement view;
            GetView(value, out view, true);
            Add(view);
        }
    }
    public virtual GameViews AddScreenToStack
    {
        set
        {
            if (ViewStack.TryPeek(out var lastScreen) && lastScreen.Equals(value)) return;
            ViewStack.Push(value);

            VisualElement backgroundView;
            GetView(m_currentGameView, out backgroundView, false);

            VisualElement foregroundView;
            GetView(value, out foregroundView, true);
            Add(foregroundView);
            foregroundView.style.visibility = Visibility.Hidden;

            backgroundView.schedule.Execute(() =>
            {
                foregroundView.WaitUntil
                        (
                            () => !float.IsNaN(foregroundView.layout.width) && !float.IsNaN(foregroundView.layout.height),
                            () =>
                            {
                                ((IGameView)backgroundView).TransitionOut(this);

                                foregroundView.style.visibility = StyleKeyword.Null;
                                ((IGameView)foregroundView).TransitionIn(this);
                            }
                        );
            });

            m_currentGameView = value;
        }
    }
    public virtual GameViews? RemoveScreenFromStack()
    {
        if (!ViewStack.TryPop(out var menuScreen)) return null;
        if (!ViewStack.TryPeek(out m_currentGameView)) return null;

        VisualElement backgroundView;
        GetView(m_currentGameView, out backgroundView, true);

        VisualElement foregroundView;
        GetView(menuScreen, out foregroundView, false);
        Add(backgroundView);
        backgroundView.PlaceBehind(foregroundView);

        backgroundView.schedule.Execute(() =>
        {
            ((IGameView)backgroundView).TransitionIn(this);

            ((IGameView)foregroundView).TransitionOut(this);
        });

        return menuScreen;
    }

    public System.Action<object> UnSetMovement;
    public CustomLoader Loader;
    public CustomGameMenu Menu;
    public CustomGame Game;

    public Stack<GameViews> ViewStack = new Stack<GameViews>();
    protected GameViews m_currentGameView;
    protected bool m_hasBeenInitialized;

    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetGamePath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);

        Game.TopArea.Menu.clicked += () => AddScreenToStack = GameViews.GameMenu;
        Menu.Resume.clicked += () => AddScreenToStack = GameViews.Game;
    }

    public virtual void Set() => Set(GameViews.Game);

    public virtual void Set(GameViews view)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        CurrentView = view;
    }

    protected void RemoveAllView()
    {
        Loader.RemoveFromHierarchy();
        Menu.RemoveFromHierarchy();
        Game.RemoveFromHierarchy();
    }

    protected void GetView(GameViews view, out VisualElement gameView, bool setMovement)
    {
        switch (view)
        {
            case GameViews.Loader:
                gameView = Loader;
                if (setMovement) Loader.SetMovement?.Invoke(this);
                if (!setMovement) Loader.UnSetMovement?.Invoke(this);
                break;
            case GameViews.GameMenu:
                gameView = Menu;
                if (setMovement) Menu.SetMovement?.Invoke(this);
                if (!setMovement) Menu.UnSetMovement?.Invoke(this);
                break;
            case GameViews.Game:
                gameView = Game;
                if (setMovement) Game.SetMovement?.Invoke(this);
                if (!setMovement) Game.UnSetMovement?.Invoke(this);
                break;
            default:
                gameView = null;
                break;
        }
    }
}
