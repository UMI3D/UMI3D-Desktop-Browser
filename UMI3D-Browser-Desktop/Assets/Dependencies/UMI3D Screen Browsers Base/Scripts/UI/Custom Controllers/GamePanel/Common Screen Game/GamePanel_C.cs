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
using System.Collections.Generic;
using umi3d.commonScreen.menu;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public class GamePanel_C : BaseVisual_C
    {
        public enum GameViews
        {
            Loader,
            GameMenu,
            Game
        }

        public new class UxmlFactory : UxmlFactory<GamePanel_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlEnumAttributeDescription<GameViews> m_currentView = new UxmlEnumAttributeDescription<GameViews>
            {
                name = "current-view",
                defaultValue = GameViews.Game
            };
            protected UxmlBoolAttributeDescription m_displayHeader = new UxmlBoolAttributeDescription
            {
                name = "display-header",
                defaultValue = false,
            };

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="ve"></param>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var custom = ve as GamePanel_C;

                custom.CurrentView = m_currentView.GetValueFromBag(bag, cc);
                custom.DisplayHeader = m_displayHeader.GetValueFromBag(bag, cc);
            }
        }

        /// <summary>
        /// Set: Clear stack and display the new screen.
        /// </summary>
        /// <value>Current screen displayed.</value>
        public virtual GameViews CurrentView
        {
            get => m_currentGameView;
            set
            {
                m_currentGameView = value;
                ViewStack.Clear();
                ViewStack.Push(value);
                RemoveAllView();

                GetView(value, out VisualElement view, true);
                Add(view);
            }
        }
        /// <summary>
        /// Add Screen to stack
        /// </summary>
        public virtual GameViews AddScreenToStack
        {
            set
            {
                if (ViewStack.TryPeek(out var lastScreen) && lastScreen.Equals(value)) return;
                if (lastScreen == GameViews.Loader && value == GameViews.GameMenu) return;
                ViewStack.Push(value);

                GetView(m_currentGameView, out VisualElement backgroundView, false);

                GetView(value, out VisualElement foregroundView, true);
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
        /// <summary>
        /// Remove current screen from stack and display previous Screen.
        /// </summary>
        /// <returns></returns>
        public virtual GameViews? RemoveScreenFromStack()
        {
            if (!ViewStack.TryPop(out var menuScreen)) return null;
            if (!ViewStack.TryPeek(out m_currentGameView)) return null;

            GetView(m_currentGameView, out VisualElement backgroundView, true);

            GetView(menuScreen, out VisualElement foregroundView, false);
            Add(backgroundView);
            backgroundView.PlaceBehind(foregroundView);

            backgroundView.schedule.Execute(() =>
            {
                ((IGameView)backgroundView).TransitionIn(this);

                ((IGameView)foregroundView).TransitionOut(this);
            });

            return menuScreen;
        }
        /// <summary>
        /// Whether or not the header should be displayed.
        /// </summary>
        public virtual bool DisplayHeader
        {
            get => m_displayHeader;
            set
            {
                Loader.DisplayHeader = value;
                Menu.DisplayHeader = value;
                Game.TopArea.DisplayHeader = value;
            }
        }

        public override string StyleSheetPath_MainTheme => $"USS/game";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetGamesFolderPath}/gamePanel";

        public override string UssCustomClass_Emc => "game-panel";

        public Loader_C Loader = new Loader_C { name = "loader" };
        public GameMenu_C Menu = new GameMenu_C { name = "game-menu" };
        public Game_C Game = new Game_C { name = "game" };

        public Stack<GameViews> ViewStack = new Stack<GameViews>();
        protected GameViews m_currentGameView;
        protected bool m_displayHeader;

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void InitElement()
        {
            base.InitElement();
            Game.TopArea.Menu.clicked += () => AddScreenToStack = GameViews.GameMenu;
            Menu.Resume.clicked += () => AddScreenToStack = GameViews.Game;
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            CurrentView = GameViews.Game;
            DisplayHeader = false;
        }

        #region Implementation

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
                    break;
                case GameViews.Game:
                    gameView = Game;
                    break;
                default:
                    gameView = null;
                    break;
            }
        }

        #endregion
    }
}
