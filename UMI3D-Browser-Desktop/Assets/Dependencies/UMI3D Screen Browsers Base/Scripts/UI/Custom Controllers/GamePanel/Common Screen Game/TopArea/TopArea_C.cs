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
using umi3d.commonDesktop.menu;
using umi3d.commonScreen.Displayer;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public class TopArea_C : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TopArea_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            protected UxmlEnumAttributeDescription<ControllerEnum> m_controller = new UxmlEnumAttributeDescription<ControllerEnum>
            {
                name = "controller",
                defaultValue = ControllerEnum.MouseAndKeyboard
            };
            protected UxmlBoolAttributeDescription m_displayHeader = new UxmlBoolAttributeDescription
            {
                name = "display-header",
                defaultValue = false,
            };
            UxmlBoolAttributeDescription m_isExpanded = new UxmlBoolAttributeDescription
            {
                name = "is-expanded",
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
                var custom = ve as TopArea_C;

                custom.Controller = m_controller.GetValueFromBag(bag, cc);
                custom.DisplayHeader = m_displayHeader.GetValueFromBag(bag, cc);
                custom.IsExpanded = m_isExpanded.GetValueFromBag(bag, cc);
            }
        }

        public ControllerEnum Controller
        {
            get => m_controller;
            set
            {
                m_controller = value;
                InformationArea.Controller = value;
            }
        }

        public virtual bool DisplayHeader
        {
            get => m_displayHeader;
            set
            {
                m_displayHeader = value;
                if (value)
                {
                    Insert(0, AppHeader);
                    AppHeader.Add(Main);
                }
                else
                {
                    AppHeader.RemoveFromHierarchy();
                    Add(Main);
                }
            }
        }

        public virtual bool IsExpanded
        {
            get => InformationArea.IsExpanded;
            set => InformationArea.IsExpanded = value;
        }

        public virtual string StyleSheetGamePath => $"USS/game";
        public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/topArea";
        public virtual string USSCustomClassName => "top__area";
        public virtual string USSCustomClassMain => $"{USSCustomClassName}-main";
        public virtual string USSCustomClassMenu => $"{USSCustomClassName}-menu";
        public virtual string USSCustomClassButton => $"{USSCustomClassName}-button";

        public AppHeader_C AppHeader = new AppHeader_C { name = "app-header" };
        public VisualElement Main = new VisualElement { name = "main" };
        public InformationArea_C InformationArea = new InformationArea_C { name = "information-area" };
        public Button_C Menu = new Button_C { name = "menu" };

        protected ControllerEnum m_controller;
        protected bool m_displayHeader;
        protected bool m_isExplanded;

        public TopArea_C() => InitElement();

        /// <summary>
        /// Initialize this element.
        /// </summary>
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
            Main.AddToClassList(USSCustomClassMain);
            Menu.AddToClassList(USSCustomClassMenu);
            Menu.AddToClassList(USSCustomClassButton);

            Menu.name = "menu";
            Menu.Category = ElementCategory.Game;

            Main.Add(InformationArea);
            Main.Add(Menu);

            Controller = ControllerEnum.MouseAndKeyboard;
            DisplayHeader = false;
            IsExpanded = false;
        }
    }
}
