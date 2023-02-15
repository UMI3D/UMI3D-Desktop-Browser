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
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.menu
{
    public class GameMenu_C : BaseMenuContainer_C<GameMenuScreens>, IGameView
    {
        public new class UxmlFactory : UxmlFactory<GameMenu_C, UxmlTraits> { }

        public virtual string StyleSheetGameMenuPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/gameMenu";
        public virtual string USSCustomClassGameMenu => "game-menu";

        public GameData_C GameData = new GameData_C { name = "game-data" };
        public LibraryScreen_C Libraries = new LibraryScreen_C { name = "libraries" };
        public SettingsContainer_C Settings = new SettingsContainer_C { name = "settings" };

        public Button_C Resume = new Button_C { name = "resume" };
        public Button_C Leave = new Button_C { name = "leave" };
        public ButtonGroup_C<GameMenuScreens> NavigationButtons = new ButtonGroup_C<GameMenuScreens> { name = "navigation-buttons" };

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetGameMenuPath);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            AddToClassList(USSCustomClassGameMenu);
        }

        protected override void InitElement()
        {
            base.InitElement();
            Leave.Type = ButtonType.Danger;

            Resume.LocaliseText = new LocalisationAttribute("Close menu", "LauncherScreen", "CloseMenu");
            Leave.LocaliseText = new LocalisationAttribute("Leave environmnent", "LauncherScreen", "LeaveEnvironment");

            NavigationButtons.LocalisedOptions = new List<LocalisationAttribute>
            {
                new LocalisationAttribute("Settings", "LauncherScreen", "Settings"),
                new LocalisationAttribute("Data", "LauncherScreen", "Data"),
                new LocalisationAttribute("Libraries", "LauncherScreen", "Libraries")
            };
            NavigationButtons.ValueEnumChanged += value => CurrentScreen = value;
            NavigationButtons.EnumValue = GameMenuScreens.Settings;

            Libraries.AllowDeletion = false;

            Navigation_ScrollView.Add(Resume);
            Navigation_ScrollView.Add(NavigationButtons);
            Navigation_ScrollView.Add(Leave);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            CurrentScreen = GameMenuScreens.Settings;
        }

        #region Implementation

        protected override void GetScreen(GameMenuScreens screenEnum, out BaseMenuScreen_C screen)
        {
            switch (screenEnum)
            {
                case GameMenuScreens.Data:
                    screen = GameData;
                    break;
                case GameMenuScreens.Libraries:
                    screen = Libraries;
                    break;
                case GameMenuScreens.Settings:
                    screen = Settings;
                    break;
                default:
                    screen = null;
                    break;
            }
        }

        protected override void GetScreenAndButton(GameMenuScreens screenEnum, out BaseMenuScreen_C screen, out Button_C button)
        {
            GetScreen(screenEnum, out screen);
            button = null;
        }

        protected override void RemoveAllScreen()
        {
            Libraries.RemoveFromHierarchy();
            Settings.RemoveFromHierarchy();
        }

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
            this.AddAnimation
            (
                persistentVisual,
                () => style.opacity = 0,
                () => style.opacity = 1,
                "opacity",
                0.5f,
                revert: revert
            );

            this.AddAnimation
            (
                persistentVisual,
                () => style.scale = new Scale(new Vector3(0.1f, 0.1f, 1)),
                () => style.scale = new Scale(Vector3.one),
                "scale",
                0.5f,
                revert: revert
            );

            this.AddAnimation
            (
                persistentVisual,
                () => style.translate = new Translate(Length.Percent(-50), Length.Percent(-50), 0),
                () => style.translate = new Translate(Length.Percent(0), Length.Percent(0), 0),
                "translate",
                0.5f,
                revert: revert,
                callback: revert ? RemoveFromHierarchy : null
            );
        }

        #endregion
    }
}
