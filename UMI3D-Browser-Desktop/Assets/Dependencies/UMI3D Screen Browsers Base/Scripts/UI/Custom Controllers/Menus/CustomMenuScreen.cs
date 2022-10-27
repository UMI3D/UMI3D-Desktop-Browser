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
using UnityEngine.UIElements;

public abstract class CustomMenuScreen : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlStringAttributeDescription m_title = new UxmlStringAttributeDescription
        { 
            name = "title", 
            defaultValue = null 
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomMenuScreen;

            custom.Set
                (
                    m_title.GetValueFromBag(bag, cc)
                 );
        }
    }

    public virtual string StyleSheetMenuPath => $"USS/menu";
    public virtual string StyleSheetScreenPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/menuScreen";
    public virtual string StyleSheetPath => $"";
    public virtual string USSCustomClassScreen => "menu-screen";
    public virtual string USSCustomClassName => "";
    public virtual string USSCustomClassHeader_ => $"{USSCustomClassName}__header";
    public virtual string USSCustomClassTitle => $"{USSCustomClassName}__title";
    public virtual string USSCustomClassHeader__Left => $"{USSCustomClassName}__header__left";
    public virtual string USSCustomClassHeader__Right => $"{USSCustomClassName}__header__right";
    public virtual string USSCustomClassScreen__Header => $"{USSCustomClassScreen}__header";
    public virtual string USSCustomClassScreen__Title => $"{USSCustomClassScreen}__title";
    public virtual string USSCustomClassScreen__Header__Left => $"{USSCustomClassScreen}__header__left";
    public virtual string USSCustomClassScreen__Header__Right => $"{USSCustomClassScreen}__header__right";
    public virtual string USSCustomClassButton_Back => $"{USSCustomClassScreen}__button__back";
    public virtual string USSCustomClassButton_Back_Icon => $"{USSCustomClassScreen}__button__back__icon";

    public virtual string Title
    {
        get => TitleLabel.text;
        set
        {
            m_isSet = false;
            if (string.IsNullOrEmpty(value)) TitleLabel.RemoveFromHierarchy();
            else Header_.Insert(1, TitleLabel);
            TitleLabel.text = value;
            m_isSet = true;
        }
    }

    public virtual string BackText
    {
        get => Button_Back.text;
        set
        {
            m_isSet = false;
            if (string.IsNullOrEmpty(value)) Button_Back.RemoveFromHierarchy();
            else Header__Left.Add(Button_Back);
            Button_Back.text = value;
            m_isSet = true;
        }
    }

    /// <summary>
    /// Text display in the back button of the screen that replace [this]
    /// </summary>
    public virtual string ShortScreenTitle { get; set; }
    public System.Action BackButtonCkicked;
    public VisualElement Header_ = new VisualElement();
    public CustomText TitleLabel;
    public VisualElement Header__Left = new VisualElement();
    public VisualElement Header__Right = new VisualElement();
    public CustomButton Button_Back;
    public VisualElement Button_Back_Icon = new VisualElement();

    protected bool m_hasBeenInitialised;
    protected bool m_isSet = false;

    public virtual void InitElement()
    {
        TitleLabel.TextStyle = TextStyle.LowTitle;

        try
        {
            this.AddStyleSheetFromPath(StyleSheetMenuPath);
            this.AddStyleSheetFromPath(StyleSheetScreenPath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassScreen);
        AddToClassList(USSCustomClassName);
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

        Button_Back.Type = ButtonType.Navigation;
        Button_Back.IconAlignment = ElementAlignment.Leading;
        Button_Back.clicked += () => BackButtonCkicked?.Invoke();

        Add(Header_);
        Header_.Add(Header__Left);
        Header_.Add(Header__Right);

        Button_Back.Add(Button_Back_Icon);
    }

    public abstract void Set();

    public virtual void Set(string title)
    {
        m_isSet = false;
        if (!m_hasBeenInitialised)
        {
            InitElement();
            m_hasBeenInitialised = true;
        }

        Title = title;
        m_isSet = true;
    }
}

public abstract class CustomMenuScreen<MenuScreenEnum> : CustomMenuScreen
    where MenuScreenEnum : struct, System.Enum
{
    public new class UxmlTraits : CustomMenuScreen.UxmlTraits
    {
        protected UxmlEnumAttributeDescription<MenuScreenEnum> m_currentScreen = new UxmlEnumAttributeDescription<MenuScreenEnum>
        {
            name = "current-screen"
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomMenuScreen<MenuScreenEnum>;

            custom.Set
            (
                m_title.GetValueFromBag(bag, cc),
                m_currentScreen.GetValueFromBag(bag, cc)
            );
        }
    }

    public virtual string USSCustomClassSelected_Icon => $"{USSCustomClassName}__selected__icon";

    public Stack<MenuScreenEnum> ScreenStack = new Stack<MenuScreenEnum>();
    protected MenuScreenEnum m_currentScreen;

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

            CustomMenuScreen screen;
            GetScreen(value, out screen);
            Add(screen);
        }
    }
    public virtual MenuScreenEnum AddScreenToStack
    {
        set
        {
            if (ScreenStack.TryPeek(out var lastScreen) && lastScreen.Equals(value)) return;
            ScreenStack.Push(value);

            GetScreen(m_currentScreen, out CustomMenuScreen backgroundScreen);

            GetScreen(value, out CustomMenuScreen foregroundScreen);
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
    public virtual MenuScreenEnum? RemoveScreenFromStack()
    {
        if (!ScreenStack.TryPop(out var menuScreen)) return null;
        if (!ScreenStack.TryPop(out m_currentScreen)) return null;

        GetScreen(m_currentScreen, out CustomMenuScreen backgroungScreen);
        Add(backgroungScreen);
        if (ScreenStack.TryPeek(out var previousMenuScreen))
        {
            GetScreen(previousMenuScreen, out CustomMenuScreen previousScreen);
            backgroungScreen.BackText = previousScreen.ShortScreenTitle;
        }
        ScreenStack.Push(m_currentScreen);

        GetScreen(menuScreen, out CustomMenuScreen foregroundScreen);
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

    public virtual void Set(string title, MenuScreenEnum currentScreen)
    {
        Set(title);

        CurrentScreen = currentScreen;
    }

    protected abstract void RemoveAllScreen();
    protected abstract void GetScreen(MenuScreenEnum screenEnum, out CustomMenuScreen screen);
    protected abstract void ResetButton();
}