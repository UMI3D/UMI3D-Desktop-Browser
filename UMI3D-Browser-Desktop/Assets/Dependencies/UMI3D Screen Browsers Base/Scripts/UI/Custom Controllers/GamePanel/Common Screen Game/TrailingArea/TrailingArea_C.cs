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
using umi3d.baseBrowser.ui.viewController;
using umi3d.cdk.menu;
using umi3d.cdk.menu.interaction;
using umi3d.common.interaction;
using umi3d.commonMobile.game;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public class TrailingArea_C : BaseVisual_C
    {
        public enum WindowsEnum
        {
            None,
            ContextualMenu,
            EmoteWindow,
            ToolsWindow,
            ToolsItemsWindow
        }

        public new class UxmlFactory : UxmlFactory<TrailingArea_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            protected UxmlEnumAttributeDescription<ControllerEnum> m_controller = new UxmlEnumAttributeDescription<ControllerEnum>
            {
                name = "controller",
                defaultValue = ControllerEnum.MouseAndKeyboard
            };
            protected UxmlBoolAttributeDescription m_displayNotifAndUsersArea = new UxmlBoolAttributeDescription
            {
                name = "display-notif-users-area",
                defaultValue = false
            };
            protected UxmlEnumAttributeDescription<WindowsEnum> m_activeWindow = new UxmlEnumAttributeDescription<WindowsEnum>
            {
                name = "active-window",
            };
            protected UxmlBoolAttributeDescription m_leftHand = new UxmlBoolAttributeDescription
            {
                name = "left-hand",
                defaultValue = false
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
                var custom = ve as TrailingArea_C;

                custom.Controller = m_controller.GetValueFromBag(bag, cc);
                custom.DisplayNotifUsersArea = m_displayNotifAndUsersArea.GetValueFromBag(bag, cc);
                custom.ActiveWindow = m_activeWindow.GetValueFromBag(bag, cc);
                custom.LeftHand = m_leftHand.GetValueFromBag(bag, cc);
            }
        }

        public ControllerEnum Controller
        {
            get => m_controller;
            set
            {
                m_controller = value;
                switch (value)
                {
                    case ControllerEnum.MouseAndKeyboard:
                        CameraLayer.RemoveFromHierarchy();
                        ButtonsArea.RemoveFromHierarchy();
                        break;
                    case ControllerEnum.Touch:
                        Add(CameraLayer);
                        Add(ButtonsArea);
                        break;
                    case ControllerEnum.GameController:
                        CameraLayer.RemoveFromHierarchy();
                        ButtonsArea.RemoveFromHierarchy();
                        break;
                    default:
                        break;
                }
            }
        }

        public virtual bool DisplayNotifUsersArea
        {
            get
            {
                if (Application.isPlaying) return Game_C.S_displayNotifUserArea;
                else return m_displayNotifAndUserArea;
            }
            set
            {
                if (!Application.isPlaying) m_displayNotifAndUserArea = value;
                if (value)
                {
                    this.AddIfNotInHierarchy(NotifAndUserArea);
                    NotifAndUserArea.style.visibility = Visibility.Hidden;
                    NotifAndUserArea.notificationCenter.UpdateFilter();
                    NotifAndUserArea.style.opacity = StyleKeyword.Null;
                }
                else NotifAndUserArea.notificationCenter.ResetNewNotificationFilter();

                if
                (
                    !value
                    && NotifAndUserArea.FindRoot() == null
                ) return;

                NotifAndUserArea.schedule.Execute(() =>
                {
                    NotifAndUserArea.style.visibility = StyleKeyword.Null;
                    NotifAndUserArea
                        .SetWidth(value ? Length.Percent(60) : Length.Percent(0))
                        .WithAnimation(.5f)
                        .SetCallback(value ? null : NotifAndUserArea.RemoveIfIsInHierarchy);
                });
            }
        }

        public virtual WindowsEnum ActiveWindow
        {
            get => m_activeWindow;
            set
            {
                if (m_activeWindow == value) return;
                switch (m_activeWindow)
                {
                    case WindowsEnum.None:
                        break;
                    case WindowsEnum.ContextualMenu:
                        DisplayObjectMenu = false;
                        break;
                    case WindowsEnum.EmoteWindow:
                        DisplayEmoteWindow = false;
                        break;
                    case WindowsEnum.ToolsWindow:
                        DisplayToolsWindow = false;
                        break;
                    case WindowsEnum.ToolsItemsWindow:
                        DisplayToolsItemsWindow = false;
                        break;
                    default:
                        break;
                }

                switch (value)
                {
                    case WindowsEnum.None:
                        break;
                    case WindowsEnum.ContextualMenu:
                        DisplayObjectMenu = true;
                        break;
                    case WindowsEnum.EmoteWindow:
                        DisplayEmoteWindow = true;
                        break;
                    case WindowsEnum.ToolsWindow:
                        DisplayToolsWindow = true;
                        break;
                    case WindowsEnum.ToolsItemsWindow:
                        DisplayToolsItemsWindow = true;
                        break;
                    default:
                        break;
                }

                m_activeWindow = value;
            }
        }

        public bool LeftHand
        {
            get => m_leftHand;
            set
            {
                m_leftHand = value;
                ButtonsArea.LeftHand = value;
                if (value)
                {
                    RemoveFromClassList(UssCustomClass_Emc);
                    AddToClassList(USSCustomClassNameReverse);
                }
                else
                {
                    RemoveFromClassList(USSCustomClassNameReverse);
                    AddToClassList(UssCustomClass_Emc);
                }
            }
        }

        public override string StyleSheetPath_MainTheme => $"USS/game";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetGamesFolderPath}/trailingArea";

        public override string UssCustomClass_Emc => "trailing__area";
        public virtual string USSCustomClassNameReverse => "trailing__area-reverse";
        public virtual string USSCustomClassObjectMenu => $"{UssCustomClass_Emc}-object__menu";
        public virtual string USSCustomClassEmoteWindow => $"{UssCustomClass_Emc}-emote__window";
        public virtual string USSCustomClassToolsWindow => $"{UssCustomClass_Emc}-tools__window";
        public virtual string USSCustomClassManipulations => $"{UssCustomClass_Emc}-manipulations";
        public virtual string USSCustomClassCameraLayer => $"{UssCustomClass_Emc}-camera__layer";
        public virtual string USSCustomClassWindowContainer => $"{UssCustomClass_Emc}-window__container";

        public ToolsWindow_C ToolsWindow = new ToolsWindow_C { name = "tools-window" };
        public ToolsItemsWindow_C ToolsItemsWindow = new ToolsItemsWindow_C { name = "tools-items-window" };
        public Form_C ObjectMenu = new Form_C { name = "contextual-menu" };
        public ButtonArea_C ButtonsArea = new ButtonArea_C { name = "buttons-area" };
        public VisualElement CameraLayer = new VisualElement { name = "camera-layer" };
        public ExpandableDataCollection_C<VisualElement> WindowContainer = new ExpandableDataCollection_C<VisualElement> { name = "window-container" };
        public ScrollableExpandableDataCollection_C<ManipulationMenuItem> ManipulationContainer = new ScrollableExpandableDataCollection_C<ManipulationMenuItem> { name = "manipulation-container" };

        public NotifAndUsersArea_C NotifAndUserArea;
        public EmoteWindow_C EmoteWindow;

        public TouchManipulator2 CameraManipulator = new TouchManipulator2(null, 0, 0);
        
        public static System.Action LeftHandModeUpdated;
        protected bool m_leftHand;
        protected ControllerEnum m_controller;
        protected WindowsEnum m_activeWindow;
        
        protected bool m_displayNotifAndUserArea;
        protected Vector2 m_initialDownPosition;
        protected Vector2 m_localPosition;
        protected bool m_cameraMoved;
        protected Vector2 m_direction;

        protected override void InstanciateChildren()
        {
            base.InstanciateChildren();
            if (NotifAndUserArea == null)
            {
                if (Application.isPlaying) NotifAndUserArea = NotifAndUsersArea_C.Instance;
                else NotifAndUserArea = new NotifAndUsersArea_C();
            }
            if (EmoteWindow == null)
            {
                if (Application.isPlaying) EmoteWindow = EmoteWindow_C.Instance;
                else EmoteWindow = new EmoteWindow_C();
            }
        }

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            ObjectMenu.AddToClassList(USSCustomClassObjectMenu);
            EmoteWindow.AddToClassList(USSCustomClassEmoteWindow);
            ToolsWindow.AddToClassList(USSCustomClassToolsWindow);
            ManipulationContainer.AddToClassList(USSCustomClassManipulations);
            CameraLayer.AddToClassList(USSCustomClassCameraLayer);
            WindowContainer.AddToClassList(USSCustomClassWindowContainer);
        }

        protected override void InitElement()
        {
            base.InitElement();
            CameraLayer.AddManipulator(CameraManipulator);
            CameraManipulator.ClickedDownWithInfo += (evt, localposition) => m_initialDownPosition = localposition;
            CameraManipulator.MovedWithInfo += (evt, localposition) =>
            {
                m_localPosition = localposition;
                m_direction = localposition - m_initialDownPosition;
                m_direction.x /= worldBound.width;
                m_direction.y /= -worldBound.height;
                m_direction *= 50;
                m_cameraMoved = true;
            };

            ButtonsArea.ClickedDown = (evt, worldPosition) => CameraManipulator.OnClickedDownWithInf(evt, CameraLayer.WorldToLocal(worldPosition));
            ButtonsArea.Moved = (evt, worldPosition) => CameraManipulator.OnMovedWithInf(evt, CameraLayer.WorldToLocal(worldPosition));

            ObjectMenu.Category = ElementCategory.Game;
            ObjectMenu.Title = "Contextual Menu";

            GlobalToolsMenu = Resources.Load<MenuAsset>("Scriptables/GamePanel/GlobalToolsMenu");
            ToolsWindow.Title = "Toolbox";
            ToolsWindow.Category = ElementCategory.Game;
            ToolsWindow.AddRoot(GlobalToolsMenu.menu);

            ToolsItemsWindow.Category = ElementCategory.Game;

            WindowContainer.MakeItem = datum => datum;
            WindowContainer.BindItem = (datum, element) => { };
            WindowContainer.UnbindItem = (datum, element) => { };
            WindowContainer.FindItem = param => param.Item1.name == param.Item2.name;
            WindowContainer.AnimationTimeIn = 1f;
            WindowContainer.AnimationTimeOut = .5f;

            ManipulationMenu = Resources.Load<MenuAsset>("Scriptables/GamePanel/ManipulationMenu");
            ManipulationContainer.Mode = ScrollViewMode.Horizontal;
            ManipulationContainer.MakeItem = datum => new Manipulation_C();
            ManipulationContainer.BindItem = (datum, element) =>
            {
                var manipulation = element as Manipulation_C;

                manipulation.Dof = datum.dof.dofs;
            };
            ManipulationContainer.UnbindItem = (datum, element) =>
            {

            };
            ManipulationMenu.menu.onAbstractMenuItemAdded.AddListener(menu =>
            {
                if (menu is not ManipulationMenuItem manip) return;

                ManipulationContainer.AddDatum(manip);
            });
            ManipulationMenu.menu.OnAbstractMenuItemRemoved.AddListener(menu =>
            {
                if (menu is not ManipulationMenuItem manip) return;

                ManipulationContainer.RemoveDatum(manip);
            });
            ManipulationContainer.AnimationTimeIn = 1f;
            ManipulationContainer.AnimationTimeOut = .5f;

            WindowContainer.AddDatum(ManipulationContainer);

            Add(WindowContainer);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Controller = ControllerEnum.MouseAndKeyboard;
            DisplayNotifUsersArea = false;
            LeftHand = m_leftHand;
        }

        protected override void GeometryChanged(GeometryChangedEvent evt)
        {
            base.GeometryChanged(evt);

            if (evt.newRect.height.EqualsEpsilone(evt.oldRect.height, .5f)) return;
            ObjectMenu.style.maxHeight = evt.newRect.height;
            EmoteWindow.style.maxHeight = evt.newRect.height;
            ToolsWindow.style.maxHeight = evt.newRect.height;
        }

        #region Implementation

        public MenuAsset GlobalToolsMenu;
        public MenuAsset ManipulationMenu;

        protected bool m_displayObjectMenu;
        protected bool m_displayEmoteWindow;
        protected bool m_displayToolsWindow;
        protected bool m_displayToolsItemsWindow;

        /// <summary>
        /// Direction of the swipe.
        /// </summary>
        public Vector2 Direction
        {
            get
            {
                if (!m_cameraMoved) return Vector2.zero;
                m_cameraMoved = false;
                m_initialDownPosition = m_localPosition;
                return m_direction;
            }
        }

        protected virtual bool DisplayObjectMenu
        {
            get => m_displayObjectMenu;
            set
            {
                m_displayObjectMenu = value;
                if (value) WindowContainer.AddDatum(ObjectMenu);
                else WindowContainer.RemoveDatum(ObjectMenu);
            }
        }

        protected virtual bool DisplayEmoteWindow
        {
            get
            {
                if (Application.isPlaying) return Game_C.S_displayEmoteWindow;
                else return m_displayEmoteWindow;
            }
            set
            {
                if (!Application.isPlaying) m_displayEmoteWindow = value;
                if (value)
                {
                    WindowContainer.AddDatum(EmoteWindow);
                    EmoteWindow.UpdateFilter();
                }
                else WindowContainer.RemoveDatum(EmoteWindow);
            }
        }

        protected virtual bool DisplayToolsWindow
        {
            get => m_displayToolsWindow;
            set
            {
                m_displayToolsWindow = value;
                if (value) WindowContainer.AddDatum(ToolsWindow);
                else WindowContainer.RemoveDatum(ToolsWindow);
            }
        }

        protected virtual bool DisplayToolsItemsWindow
        {
            get => m_displayToolsItemsWindow;
            set
            {
                m_displayToolsItemsWindow = value;
                if (value) WindowContainer.AddDatum(ToolsItemsWindow);
                else WindowContainer.RemoveDatum(ToolsItemsWindow);
            }
        }

        #endregion
    }
}
