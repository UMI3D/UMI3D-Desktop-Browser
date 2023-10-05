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
using umi3d.commonDesktop.game;
using umi3d.commonMobile.game;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public class LeadingArea_C : BaseVisual_C
    {
        public new class UxmlFactory : UxmlFactory<LeadingArea_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            protected UxmlEnumAttributeDescription<ControllerEnum> m_controller = new UxmlEnumAttributeDescription<ControllerEnum>
            {
                name = "controller",
                defaultValue = ControllerEnum.MouseAndKeyboard
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
                var custom = ve as LeadingArea_C;

                custom.Controller = m_controller.GetValueFromBag(bag, cc);
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
                        Add(InteractableMapping);
                        JoystickArea.RemoveFromHierarchy();
                        break;
                    case ControllerEnum.Touch:
                        Add(JoystickArea);
                        InteractableMapping.RemoveFromHierarchy();
                        break;
                    case ControllerEnum.GameController:
                        Add(InteractableMapping);
                        JoystickArea.RemoveFromHierarchy();
                        break;
                    default:
                        break;
                }
                InteractableMapping.Controller = value;
            }
        }

        public bool LeftHand
        {
            get => m_leftHand;
            set
            {
                m_leftHand = value;
                JoystickArea.LeftHand = value;
                if (value) AddToClassList(USSCustomClassNameReverse);
                else RemoveFromClassList(USSCustomClassNameReverse);
            }
        }

        public override string StyleSheetPath_MainTheme => $"USS/game";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetGamesFolderPath}/leadingArea";

        public override string UssCustomClass_Emc => "leading__area";
        public virtual string USSCustomClassNameReverse => "leading__area-reverse";

        public PinnedToolsArea_C PinnedToolsArea;
        public InteractableMapping_C InteractableMapping = new InteractableMapping_C { name = "interaction-mapping" };
        public JoystickArea_C JoystickArea = new JoystickArea_C { name = "joystick-area" };

        protected ControllerEnum m_controller;
        protected bool m_leftHand;

        protected override void InstanciateChildren()
        {
            base.InstanciateChildren();
            if (PinnedToolsArea == null)
            {
                if (Application.isPlaying) PinnedToolsArea = PinnedToolsArea_C.Instance;
                else PinnedToolsArea = new PinnedToolsArea_C();
                PinnedToolsArea.name = "pinned-tools";
            }
        }

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void InitElement()
        {
            base.InitElement();
            PinnedToolsArea.Mode = ScrollViewMode.Vertical;

            //InteractableMapping
            //    .SetLeft(Length.Percent(-50f));
            UnityEngine.Debug.Log("<color=red>Fix for Laval: </color>" + $"To be updated");

            pickingMode = PickingMode.Ignore;
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Controller = ControllerEnum.MouseAndKeyboard;
            LeftHand = m_leftHand;
        }

        protected override void AttachedToPanel(AttachToPanelEvent evt)
        {
            base.AttachedToPanel(evt);
            PinnedToolsArea.SDC.ContentChanged += PinnedToolsAreaContentChanged;
        }

        protected override void DetachedFromPanel(DetachFromPanelEvent evt)
        {
            base.DetachedFromPanel(evt);
            PinnedToolsArea.SDC.ContentChanged -= PinnedToolsAreaContentChanged;
        }

        #region Implementation

        protected virtual void PinnedToolsAreaContentChanged(int count)
        {
            if (count == 1) Insert(0, PinnedToolsArea);
            else if (count == 0) PinnedToolsArea.RemoveFromHierarchy();
        }

        #endregion
    }
}
