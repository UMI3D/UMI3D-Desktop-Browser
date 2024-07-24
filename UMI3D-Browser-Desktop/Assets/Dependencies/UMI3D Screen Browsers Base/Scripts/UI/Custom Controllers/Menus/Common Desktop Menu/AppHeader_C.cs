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
using inetum.unityUtils;
using umi3d.browserRuntime.notificationKeys;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonDesktop.menu
{
    public class AppHeader_C : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<AppHeader_C, UxmlTraits> { }

        public virtual string StyleSheetMenuPath => $"USS/menu";
        public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/appHeader";
        public virtual string USSCustomClassName => "app__header";

        public virtual string USSCustomClassButtonsBox => $"{USSCustomClassName}-buttons__box";
        public virtual string USSCustomClassWindowButton => $"{USSCustomClassName}-window__button";
        public virtual string USSCustomClassMinimize => $"{USSCustomClassName}-minimize__icon";
        public virtual string USSCustomClassMaximize => $"{USSCustomClassName}-maximize__icon";
        public virtual string USSCustomClassClose => $"{USSCustomClassName}-close__icon";
        public virtual string USSCustomClassContainer => $"{USSCustomClassName}-container";

        public VisualElement AppButtonsBox = new VisualElement { name = "app-button-box" };
        public Button_C Minimize = new Button_C { name = "minimize" };
        public VisualElement Minimize_Icon = new VisualElement { name = "mimimize-icon" };
        public Button_C Maximize = new Button_C { name = "maximize" };
        public VisualElement Maximize_Icon = new VisualElement { name = "maximize-icon" };
        public Button_C Close = new Button_C { name = "close" };
        public VisualElement Close_Icon = new VisualElement { name = "close-icon" };
        public VisualElement Container = new VisualElement { name = "container" };

        protected bool m_isSet = false;

        public AppHeader_C() => InitElement();

        protected virtual void InitElement()
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

            AppButtonsBox.AddToClassList(USSCustomClassButtonsBox);
            Minimize.AddToClassList(USSCustomClassWindowButton);
            Maximize.AddToClassList(USSCustomClassWindowButton);
            Close.AddToClassList(USSCustomClassWindowButton);
            Minimize_Icon.AddToClassList(USSCustomClassMinimize);
            Maximize_Icon.AddToClassList(USSCustomClassMaximize);
            Close_Icon.AddToClassList(USSCustomClassClose);
            Container.AddToClassList(USSCustomClassContainer);

            Minimize.Height = ElementSize.Small;
            Maximize.Height = ElementSize.Small;
            Close.Height = ElementSize.Small;
            Minimize.Width = ElementSize.Large;
            Maximize.Width = ElementSize.Large;
            Close.Width = ElementSize.Large;

            Close.Type = ButtonType.Danger;

            Add(Container);
            Add(AppButtonsBox);
            AppButtonsBox.Add(Minimize);
            Minimize.Add(Minimize_Icon);
            AppButtonsBox.Add(Maximize);
            Maximize.Add(Maximize_Icon);
            AppButtonsBox.Add(Close);
            Close.Add(Close_Icon);

            m_isSet = true;

            Minimize.clicked += Minimized;
            Maximize.clicked += Windowed;
            Close.clicked += Application.Quit;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override VisualElement contentContainer => m_isSet ? Container : this;

        /// <summary>
        /// Hide the window.
        /// </summary>
        void Minimized()
        {
            NotificationHub.Default.Notify(
                this,
                WindowsManagerNotificationKey.Minimize
            );
        }
         
        /// <summary>
        /// Leave full screen mode to window mode.
        /// </summary>
        void Windowed()
        {
            NotificationHub.Default.Notify(
                this,
                WindowsManagerNotificationKey.FullScreenModeWillChange,
                new()
                {
                    { WindowsManagerNotificationKey.FullScreenModeChangedInfo.Mode, FullScreenMode.Windowed }
                }
            );
        }
    }
}
