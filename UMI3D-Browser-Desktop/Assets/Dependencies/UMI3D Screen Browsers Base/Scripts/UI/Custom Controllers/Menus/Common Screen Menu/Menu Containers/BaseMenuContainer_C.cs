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
using umi3d.commonDesktop.menu;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.menu
{
    public abstract class BaseMenuContainer_C : BaseVisual_C
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

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="ve"></param>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var custom = ve as BaseMenuContainer_C;

                custom.DisplayHeader = m_displayHeader.GetValueFromBag(bag, cc);
                custom.Version = m_version.GetValueFromBag(bag, cc);
            }
        }

        public virtual bool DisplayHeader
        {
            get => m_displayHeader;
            set
            {
                IsSet = false;
                m_displayHeader = value;
                if (value) Insert(0, Header);
                else Header.RemoveFromHierarchy();
                IsSet = true;
            }
        }
        public virtual LocalisationAttribute Version
        {
            get => VersionLabel.LocaliseText;
            set
            {
                IsSet = false;
                if (value.IsEmpty) VersionLabel.RemoveFromHierarchy();
                else Footer.Add(VersionLabel);
                VersionLabel.LocaliseText = value;
                IsSet = true;
            }
        }

        public override string StyleSheetPath_MainTheme => $"USS/menu";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetMenusFolderPath}/menuContainer";

        public override string UssCustomClass_Emc => "menu";
        public virtual string USSCustomClassHeader => $"{UssCustomClass_Emc}__header";
        public virtual string USSCustomClassMain => $"{UssCustomClass_Emc}__main";
        public virtual string USSCustomClassLogo_Container => $"{UssCustomClass_Emc}__logo__container";
        public virtual string USSCustomClassLogo => $"{UssCustomClass_Emc}__logo";
        public virtual string USSCustomClassNavigation => $"{UssCustomClass_Emc}__navigation__scroll-view";
        public virtual string USSCustomClassSeparator => $"{UssCustomClass_Emc}__separator";
        public virtual string USSCustomClassContainer => $"{UssCustomClass_Emc}__container";
        public virtual string USSCustomClassFooter => $"{UssCustomClass_Emc}__footer";
        public virtual string USSCustomClassVersion => $"{UssCustomClass_Emc}__version";

        public AppHeader_C Header = new AppHeader_C { name = "app-header" };
        public VisualElement Main = new VisualElement { name = "main" };
        public VisualElement LogoContainer = new VisualElement { name = "logo-container" };
        public VisualElement Logo = new VisualElement { name = "logo" };
        public ScrollView_C Navigation_ScrollView = new ScrollView_C { name = "scroll-view" };
        public VisualElement Separator = new VisualElement();
        public VisualElement Container = new VisualElement();
        public VisualElement Footer = new VisualElement { name = "footer" };
        public Text_C VersionLabel = new Text_C { name = "version" };

        protected bool m_displayHeader;

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            Header.AddToClassList(USSCustomClassHeader);
            Main.AddToClassList(USSCustomClassMain);
            LogoContainer.AddToClassList(USSCustomClassLogo_Container);
            Logo.AddToClassList(USSCustomClassLogo);
            Navigation_ScrollView.AddToClassList(USSCustomClassNavigation);
            Separator.AddToClassList(USSCustomClassSeparator);
            Container.AddToClassList(USSCustomClassContainer);
            Footer.AddToClassList(USSCustomClassFooter);
            VersionLabel.AddToClassList(USSCustomClassVersion);
        }

        protected override void InitElement()
        {
            base.InitElement();
            Add(Main);
            Main.Add(LogoContainer);
            LogoContainer.Add(Logo);
            LogoContainer.Add(Navigation_ScrollView);
            Main.Add(Separator);
            Main.Add(Container);
            Add(Footer);
            Footer.Add(VersionLabel);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            DisplayHeader = false;
            Version = null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override VisualElement contentContainer => IsSet ? Container : this;
    }

    public abstract class BaseMenuContainer_C<MenuScreenEnum> : BaseMenuContainer_C
        where MenuScreenEnum : struct, System.Enum
    {
        public new class UxmlTraits : BaseMenuContainer_C.UxmlTraits
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
                var custom = ve as BaseMenuContainer_C<MenuScreenEnum>;

                custom.CurrentScreen = m_currentScreen.GetValueFromBag(bag, cc);
            }
        }

        public virtual string USSCustomClassSelected_Icon => $"{UssCustomClass_Emc}__selected__icon";

        public VisualElement SelectedScreenIcon = new VisualElement { name = "selected-icon" };
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

                GetScreenAndButton(value, out BaseMenuScreen_C screen, out Button_C button);
                Add(screen);
                button?.Add(SelectedScreenIcon);
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

                GetScreenAndButton(value, out BaseMenuScreen_C foregroundScreen, out Button_C button);
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
        /// <summary>
        /// Remove current screen from stack and display previous Screen.
        /// </summary>
        /// <returns></returns>
        public virtual MenuScreenEnum? RemoveScreenFromStack()
        {
            if (!ScreenStack.TryPop(out var menuScreen)) return null;
            if (!ScreenStack.TryPop(out m_currentScreen)) return null;

            GetScreenAndButton(m_currentScreen, out BaseMenuScreen_C backgroungScreen, out Button_C button);
            Add(backgroungScreen);
            button.Add(SelectedScreenIcon);
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
                    }
                );
            });

            return menuScreen;
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            SelectedScreenIcon.AddToClassList(USSCustomClassSelected_Icon);
        }

        protected abstract void RemoveAllScreen();
        protected abstract void GetScreen(MenuScreenEnum screenEnum, out BaseMenuScreen_C screen);
        protected abstract void GetScreenAndButton(MenuScreenEnum screenEnum, out BaseMenuScreen_C screen, out Button_C button);
    }
}

public enum LauncherScreens
{
    Home,
    Settings,
    Libraries
}

public enum LoaderScreens
{
    Loading,
    Form
}

public enum GameMenuScreens
{
    Settings,
    Data,
    Libraries
}