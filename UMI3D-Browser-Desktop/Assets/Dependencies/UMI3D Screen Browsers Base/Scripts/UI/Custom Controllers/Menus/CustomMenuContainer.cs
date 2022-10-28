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

public abstract class CustomMenuContainer : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlBoolAttributeDescription m_displayHeader = new UxmlBoolAttributeDescription
        {
            name = "display-header",
            defaultValue = false,
        };
        protected UxmlStringAttributeDescription m_version = new UxmlStringAttributeDescription
        {
            name = "version",
            defaultValue = null
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomMenuContainer;

            custom.Set
                (
                    m_displayHeader.GetValueFromBag(bag, cc),
                    m_version.GetValueFromBag(bag, cc)
                );
        }
    }

    public virtual string StyleSheetMenuPath => $"USS/menu";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/menuContainer";
    public virtual string USSCustomClassName => "menu";
    public virtual string USSCustomClassHeader => $"{USSCustomClassName}__header";
    public virtual string USSCustomClassWindowButton => $"{USSCustomClassName}__window-button";
    public virtual string USSCustomClassMinimize => $"{USSCustomClassName}__minimize-icon";
    public virtual string USSCustomClassMaximize => $"{USSCustomClassName}__maximize-icon";
    public virtual string USSCustomClassClose => $"{USSCustomClassName}__close-icon";
    public virtual string USSCustomClassMain => $"{USSCustomClassName}__main";
    public virtual string USSCustomClassLogo_Container => $"{USSCustomClassName}__logo__container";
    public virtual string USSCustomClassLogo => $"{USSCustomClassName}__logo";
    public virtual string USSCustomClassNavigation => $"{USSCustomClassName}__navigation__scroll-view";
    public virtual string USSCustomClassSeparator => $"{USSCustomClassName}__separator";
    public virtual string USSCustomClassContainer => $"{USSCustomClassName}__container";
    public virtual string USSCustomClassFooter => $"{USSCustomClassName}__footer";
    public virtual string USSCustomClassVersion => $"{USSCustomClassName}__version";

    public virtual bool DisplayHeader
    {
        get => m_displayHeader;
        set
        {
            m_isSet = false;
            m_displayHeader = value;
            if (value) Insert(0, Header);
            else Header.RemoveFromHierarchy();
            m_isSet = true;
        }
    }
    public virtual string Version
    {
        get => VersionLabel.text;
        set
        {
            m_isSet = false;
            if (string.IsNullOrEmpty(value)) VersionLabel.RemoveFromHierarchy();
            else Footer.Add(VersionLabel);
            VersionLabel.text = value;
            m_isSet = true;
        }
    }

    public VisualElement Header = new VisualElement();
    public CustomButton Minimize;
    public VisualElement Minimize_Icon = new VisualElement { name = "mimimize-icon" };
    public CustomButton Maximize;
    public VisualElement Maximize_Icon = new VisualElement { name = "maximize-icon" };
    public CustomButton Close;
    public VisualElement Close_Icon = new VisualElement { name = "close-icon" };
    public VisualElement Main = new VisualElement();
    public VisualElement LogoContainer = new VisualElement();
    public VisualElement Logo = new VisualElement();
    public CustomScrollView Navigation_ScrollView;
    public VisualElement Separator = new VisualElement();
    public VisualElement Container = new VisualElement();
    public VisualElement Footer = new VisualElement();
    public CustomText VersionLabel;

    protected bool m_isSet;
    protected bool m_hasBeenInitialized;
    protected bool m_displayHeader;

    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetMenuPath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);
        Header.AddToClassList(USSCustomClassHeader);
        Minimize.AddToClassList(USSCustomClassWindowButton);
        Maximize.AddToClassList(USSCustomClassWindowButton);
        Close.AddToClassList(USSCustomClassWindowButton);
        Minimize_Icon.AddToClassList(USSCustomClassMinimize);
        Maximize_Icon.AddToClassList(USSCustomClassMaximize);
        Close_Icon.AddToClassList(USSCustomClassClose);
        Main.AddToClassList(USSCustomClassMain);
        LogoContainer.AddToClassList(USSCustomClassLogo_Container);
        Logo.AddToClassList(USSCustomClassLogo);
        Navigation_ScrollView.AddToClassList(USSCustomClassNavigation);
        Separator.AddToClassList(USSCustomClassSeparator);
        Container.AddToClassList(USSCustomClassContainer);
        Footer.AddToClassList(USSCustomClassFooter);
        VersionLabel.AddToClassList(USSCustomClassVersion);

        Minimize.Size = ElementSize.Small;
        Maximize.Size = ElementSize.Small;
        Close.Size = ElementSize.Small;

        Header.Add(Minimize);
        Minimize.Add(Minimize_Icon);
        Header.Add(Maximize);
        Maximize.Add(Maximize_Icon);
        Header.Add(Close);
        Close.Add(Close_Icon);
        Add(Main);
        Main.Add(LogoContainer);
        LogoContainer.Add(Logo);
        LogoContainer.Add(Navigation_ScrollView);
        Main.Add(Separator);
        Main.Add(Container);
        Add(Footer);
        Footer.Add(VersionLabel);
    }

    public virtual void Set() => Set(false, null);

    public virtual void Set(bool displayeHeader, string version)
    {
        m_isSet = false;

        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        DisplayHeader = displayeHeader;
        Version = version;

        m_isSet = true;
    }

    public override VisualElement contentContainer => m_isSet ? Container : this;
}

public abstract class CustomMenuContainer<MenuScreenEnum> : CustomMenuContainer
    where MenuScreenEnum : struct, System.Enum
{
    public new class UxmlTraits : CustomMenuContainer.UxmlTraits
    {
        protected UxmlEnumAttributeDescription<MenuScreenEnum> m_currentScreen = new UxmlEnumAttributeDescription<MenuScreenEnum>
        {
            name = "current-screen"
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }
    }

    public virtual string USSCustomClassSelected_Icon => $"{USSCustomClassName}__selected__icon";

    public VisualElement SelectedScreenIcon = new VisualElement { name = "selected-icon" };
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

            CustomMenuScreen screen;
            CustomButton button;
            GetScreenAndButton(value, out screen, out button);
            Add(screen);
            button?.Add(SelectedScreenIcon);
        }
    }
    public virtual MenuScreenEnum AddScreenToStack
    {
        set
        {
            if (ScreenStack.TryPeek(out var lastScreen) && lastScreen.Equals(value)) return;
            ScreenStack.Push(value);

            CustomMenuScreen backgroundScreen;
            GetScreen(m_currentScreen, out backgroundScreen);

            CustomMenuScreen foregroundScreen;
            CustomButton button;
            GetScreenAndButton(value, out foregroundScreen, out button);
            Add(foregroundScreen);
            button.Add(SelectedScreenIcon);
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

        GetScreenAndButton(m_currentScreen, out CustomMenuScreen backgroungScreen, out CustomButton button);
        Add(backgroungScreen);
        button.Add(SelectedScreenIcon);
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
                }
            );
        });

        return menuScreen;
    }

    public override void InitElement()
    {
        base.InitElement();
        SelectedScreenIcon.AddToClassList(USSCustomClassSelected_Icon);
    }

    protected abstract void RemoveAllScreen();
    protected abstract void GetScreen(MenuScreenEnum screenEnum, out CustomMenuScreen screen);
    protected abstract void GetScreenAndButton(MenuScreenEnum screenEnum, out CustomMenuScreen screen, out CustomButton button);
}

public enum LauncherScreens
{
    Home,
    //ConnectionSettings,
    //Session,
    Libraries,
    Settings
}

public enum LoaderScreens
{
    Loading,
    Form
}

public enum GameMenuScreens
{
    Data,
    Libraries,
    Settings
}