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

public abstract class CustomGameMenu : CustomMenuContainer<GameMenuScreens>, IGameView
{
    public new class UxmlTraits : CustomMenuContainer<GameMenuScreens>.UxmlTraits
    {
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomGameMenu;

            custom.Set
            (
                m_currentScreen.GetValueFromBag(bag, cc),
                m_displayHeader.GetValueFromBag(bag, cc),
                m_version.GetValueFromBag(bag, cc)
              );
        }
    }

    public virtual string StyleSheetGameMenuPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/gameMenu";
    public virtual string USSCustomClassGameMenu => "game-menu";

    public CustomGameData GameData;
    public CustomLibraryScreen Libraries;
    public CustomSettingsContainer Settings;

    public CustomButton Resume;
    public CustomButton Leave;
    public CustomButtonGroup<GameMenuScreens> NavigationButtons;

    public override void InitElement()
    {
        base.InitElement();

        try
        {
            this.AddStyleSheetFromPath(StyleSheetGameMenuPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassGameMenu);

        Leave.Type = ButtonType.Danger;

        Resume.text = "Close Menu";
        Leave.text = "Leave environmnent";

        NavigationButtons.ValueEnumChanged += value => CurrentScreen = value;

        Navigation_ScrollView.Add(Resume);
        Navigation_ScrollView.Add(NavigationButtons);
        Navigation_ScrollView.Add(Leave);
    }

    public override void Set() => Set(GameMenuScreens.Settings, false, null);

    public virtual void Set(GameMenuScreens screen, bool displayHeader, string version)
    {
        Set(displayHeader, version);

        NavigationButtons.Value = screen.ToString();
    }

    protected override void GetScreen(GameMenuScreens screenEnum, out CustomMenuScreen screen)
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

    protected override void GetScreenAndButton(GameMenuScreens screenEnum, out CustomMenuScreen screen, out CustomButton button)
    {
        GetScreen(screenEnum, out screen);
        button = null;
    }

    protected override void RemoveAllScreen()
    {
        Libraries.RemoveFromHierarchy();
        Settings.RemoveFromHierarchy();
    }

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
}
