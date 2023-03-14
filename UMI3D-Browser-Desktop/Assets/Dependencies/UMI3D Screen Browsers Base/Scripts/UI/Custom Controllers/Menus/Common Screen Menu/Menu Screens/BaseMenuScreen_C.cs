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
using umi3d.commonScreen;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseMenuScreen_C : BaseVisual_C
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlLocaliseAttributeDescription m_title = new UxmlLocaliseAttributeDescription
        {
            name = "title"
        };

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="ve"></param>
        /// <param name="bag"></param>
        /// <param name="cc"></param>
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as BaseMenuScreen_C;

            custom.Title = m_title.GetValueFromBag(bag, cc);
        }
    }

    public virtual LocalisationAttribute Title
    {
        get => TitleLabel.LocaliseText;
        set
        {
            IsSet = false;
            if (value.IsEmpty) TitleLabel.RemoveFromHierarchy();
            else Header_.Insert(1, TitleLabel);
            TitleLabel.LocaliseText = value;
            IsSet = true;
        }
    }

    public virtual LocalisationAttribute BackText
    {
        get => Button_Back.LocaliseText;
        set
        {
            IsSet = false;
            if (value.IsEmpty) Button_Back.RemoveFromHierarchy();
            else Header__Left.Add(Button_Back);
            Button_Back.LocaliseText = value;
            IsSet = true;
        }
    }

    public override string StyleSheetPath_MainTheme => $"USS/menu";
    public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetMenusFolderPath}/menuScreen";
    public virtual string StyleSheetPath => $"";

    public virtual string USSCustomClassScreen => "menu-screen";

    public virtual string USSCustomClassHeader_ => $"{UssCustomClass_Emc}__header";
    public virtual string USSCustomClassTitle => $"{UssCustomClass_Emc}__title";
    public virtual string USSCustomClassHeader__Left => $"{UssCustomClass_Emc}__header__left";
    public virtual string USSCustomClassHeader__Right => $"{UssCustomClass_Emc}__header__right";

    public virtual string USSCustomClassScreen__Header => $"{USSCustomClassScreen}__header";
    public virtual string USSCustomClassScreen__Title => $"{USSCustomClassScreen}__title";
    public virtual string USSCustomClassScreen__Header__Left => $"{USSCustomClassScreen}__header__left";
    public virtual string USSCustomClassScreen__Header__Right => $"{USSCustomClassScreen}__header__right";
    public virtual string USSCustomClassButton_Back => $"{USSCustomClassScreen}__button__back";
    public virtual string USSCustomClassButton_Back_Icon => $"{USSCustomClassScreen}__button__back__icon";

    /// <summary>
    /// Text display in the back button of the screen that replace [this]
    /// </summary>
    public virtual LocalisationAttribute ShortScreenTitle { get; set; }
    public System.Action BackButtonCkicked;
    public VisualElement Header_ = new VisualElement { name = "header" };
    public Text_C TitleLabel = new Text_C { name = "title" };
    public VisualElement Header__Left = new VisualElement { name = "header-left" };
    public VisualElement Header__Right = new VisualElement { name = "header-right" };
    public Button_C Button_Back = new Button_C { name = "back" };
    public VisualElement Button_Back_Icon = new VisualElement();

    protected override void AttachStyleSheet()
    {
        base.AttachStyleSheet();
        this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        this.AddStyleSheetFromPath(StyleSheetPath);
    }

    protected override void AttachUssClass()
    {
        base.AttachUssClass();
        AddToClassList(USSCustomClassScreen);
        Header_.AddToClassList(USSCustomClassScreen__Header);
        TitleLabel.AddToClassList(USSCustomClassScreen__Title);
        Header__Left.AddToClassList(USSCustomClassScreen__Header__Left);
        Header__Right.AddToClassList(USSCustomClassScreen__Header__Right);
        Header_.AddToClassList(USSCustomClassHeader_);
        TitleLabel.AddToClassList(USSCustomClassTitle);
        Header__Left.AddToClassList(USSCustomClassHeader__Left);
        Header__Right.AddToClassList(USSCustomClassHeader__Right);
        Button_Back.AddToClassList(USSCustomClassButton_Back);
        Button_Back_Icon.AddToClassList(USSCustomClassButton_Back_Icon);
    }

    protected override void InitElement()
    {
        base.InitElement();
        TitleLabel.TextStyle = TextStyle.LowTitle;

        Button_Back.Type = ButtonType.Navigation;
        Button_Back.IconAlignment = ElementAlignment.Leading;
        Button_Back.clicked += () => BackButtonCkicked?.Invoke();

        Add(Header_);
        Header_.Add(Header__Left);
        Header_.Add(Header__Right);

        Button_Back.Add(Button_Back_Icon);        
    }

    protected override void SetProperties()
    {
        base.SetProperties();
        Title = null;
    }
}

public abstract class BaseMenuScreen_C<MenuScreenEnum> : BaseMenuScreen_C
    where MenuScreenEnum : struct, System.Enum
{
    public new class UxmlTraits : BaseMenuScreen_C.UxmlTraits
    {
        protected UxmlEnumAttributeDescription<MenuScreenEnum> m_currentScreen = new UxmlEnumAttributeDescription<MenuScreenEnum>
        {
            name = "current-screen"
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
            var custom = ve as BaseMenuScreen_C<MenuScreenEnum>;

            custom.CurrentScreen = m_currentScreen.GetValueFromBag(bag, cc);
        }
    }

    public virtual string USSCustomClassSelected_Icon => $"{UssCustomClass_Emc}__selected__icon";

    public Stack<MenuScreenEnum> ScreenStack = new Stack<MenuScreenEnum>();
    protected MenuScreenEnum m_currentScreen;

    /// <summary>
    /// Set: Clear stack and display the new screen.
    /// </summary>
    /// <value>Current screen displayed.</value>
    public virtual MenuScreenEnum CurrentScreen
    {
        get => m_currentScreen;
        set
        {
            m_currentScreen = value;
            ScreenStack.Clear();
            ScreenStack.Push(value);
            RemoveAllScreen();
            ResetButton();

            BaseMenuScreen_C screen;
            GetScreen(value, out screen);
            Add(screen);
        }
    }
    /// <summary>
    /// Add Screen to stack
    /// </summary>
    public virtual MenuScreenEnum AddScreenToStack
    {
        set
        {
            if (ScreenStack.TryPeek(out var lastScreen) && lastScreen.Equals(value)) return;
            ScreenStack.Push(value);

            GetScreen(m_currentScreen, out BaseMenuScreen_C backgroundScreen);

            GetScreen(value, out BaseMenuScreen_C foregroundScreen);
            Add(foregroundScreen);
            foregroundScreen.BackText = backgroundScreen.ShortScreenTitle;
            foregroundScreen.style.visibility = Visibility.Hidden;

            foregroundScreen.schedule.Execute(() =>
            {
                backgroundScreen.AddAnimation
                (
                    this,
                    () => backgroundScreen.style.left = 0f,
                    () => backgroundScreen.style.left = -50f,
                    "left",
                    AnimatorManager.NavigationScreenDuration,
                    callback: backgroundScreen.RemoveFromHierarchy
                );

                foregroundScreen.style.visibility = Visibility.Visible;
                foregroundScreen.AddAnimation
                (
                    this,
                    () => foregroundScreen.style.left = foregroundScreen.resolvedStyle.width,
                    () => foregroundScreen.style.left = 0f,
                    "left",
                    AnimatorManager.NavigationScreenDuration
                );
            });

            m_currentScreen = value;
        }
    }
    /// <summary>
    /// Remove current screen from stack and display previous Screen.
    /// </summary>
    /// <returns></returns>
    public virtual MenuScreenEnum? RemoveScreenFromStack()
    {
        if (!ScreenStack.TryPop(out var menuScreen)) return null;
        if (!ScreenStack.TryPop(out m_currentScreen)) return null;

        GetScreen(m_currentScreen, out BaseMenuScreen_C backgroungScreen);
        Add(backgroungScreen);
        if (ScreenStack.TryPeek(out var previousMenuScreen))
        {
            GetScreen(previousMenuScreen, out BaseMenuScreen_C previousScreen);
            backgroungScreen.BackText = previousScreen.ShortScreenTitle;
        }
        ScreenStack.Push(m_currentScreen);

        GetScreen(menuScreen, out BaseMenuScreen_C foregroundScreen);
        backgroungScreen.PlaceBehind(foregroundScreen);

        backgroungScreen.schedule.Execute(() =>
        {
            backgroungScreen.AddAnimation
            (
                this,
                () => backgroungScreen.style.left = -50f,
                () => backgroungScreen.style.left = 0f,
                "left",
                AnimatorManager.NavigationScreenDuration
            );

            foregroundScreen.AddAnimation
            (
                this,
                () => foregroundScreen.style.left = 0,
                () => foregroundScreen.style.left = foregroundScreen.resolvedStyle.width,
                "left",
                AnimatorManager.NavigationScreenDuration,
                callback: () =>
                {
                    foregroundScreen.BackText = null;
                    foregroundScreen.RemoveFromHierarchy();
                    ResetButton();
                }
            );
        });

        return menuScreen;
    }

    protected abstract void RemoveAllScreen();
    protected abstract void GetScreen(MenuScreenEnum screenEnum, out BaseMenuScreen_C screen);
    protected abstract void ResetButton();
}