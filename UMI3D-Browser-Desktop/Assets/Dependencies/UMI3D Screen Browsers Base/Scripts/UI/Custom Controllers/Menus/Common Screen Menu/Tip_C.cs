/*
Copyright 2019 - 2023 Inetum

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
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.menu
{
    public class Tip_C : BaseVisual_C
    {
        public new class UxmlFactory : UxmlFactory<Tip_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlLocaliseAttributeDescription m_title = new UxmlLocaliseAttributeDescription
            {
                name = "title"
            };
            UxmlLocaliseAttributeDescription m_message = new UxmlLocaliseAttributeDescription
            {
                name = "message"
            };
            protected UxmlBoolAttributeDescription m_displayMessage = new UxmlBoolAttributeDescription
            {
                name = "display-message",
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
                var custom = ve as Tip_C;

                custom.Title = m_title.GetValueFromBag(bag, cc);
                custom.Message = m_message.GetValueFromBag(bag, cc);
                custom.DisplayMessage = m_displayMessage.GetValueFromBag(bag, cc);
            }
        }

        public virtual LocalisationAttribute Title
        {
            get => TitleLabel.LocalisedText;
            set
            {
                if (value.IsEmpty) TitleLabel.RemoveFromHierarchy();
                else DropDown_Button_Background.Insert(0, TitleLabel);
                TitleLabel.LocalisedText = value;
            }
        }
        public virtual LocalisationAttribute Message
        {
            get => DropDown_Message.LocalisedText;
            set
            {
                if (value.IsEmpty) DropDown_Message.RemoveFromHierarchy();
                else
                {
                    DropDown_Field.Insert(0, DropDown_Message);
                }
                DropDown_Message.LocalisedText = value;
            }
        }
        public virtual bool DisplayMessage
        {
            get => m_displayMessage;
            set
            {
                m_displayMessage = value;
                if (m_displayMessage) this.AddIfNotInHierarchy(Overlay);

                DropDown_Button_Icon
                    .SetRotate(m_displayMessage ? new Rotate(180) : new Rotate(90))
                    .WithAnimation(AnimatorManager.DropdownDuration);

                this.WaitUntil
                    (
                        () => !float.IsNaN(DropDown_Field.layout.height),
                        () =>
                        {
                            var fieldTotalHeight = DropDown_Field.layout.height + DropDown_Field.resolvedStyle.marginTop + DropDown_Field.resolvedStyle.marginBottom;

                            Overlay
                                .SetHeight(m_displayMessage ? fieldTotalHeight : 0f)
                                .WithAnimation(AnimatorManager.DropdownDuration)
                                .SetCallback(m_displayMessage ? null : Overlay.RemoveFromHierarchy);
                        }
                    );
            }
        }
        public override string StyleSheetPath_MainTheme => $"USS/menu";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetMenusFolderPath}/library";

        public override string UssCustomClass_Emc => "library";
        public virtual string USSCustomClassOverlay => $"{UssCustomClass_Emc}-overlay";
        public virtual string USSCustomClassMain => $"{UssCustomClass_Emc}-main";
        public virtual string USSCustomClassDropDown_Button => $"{UssCustomClass_Emc}-drop_down__button";
        public virtual string USSCustomClassDropDown_Button_background => $"{UssCustomClass_Emc}-drop_down__button__background";
        public virtual string USSCustomClassDropDown_Button_Icon_Background => $"{UssCustomClass_Emc}-drop_down__button__icon__background";
        public virtual string USSCustomClassDropDown_Button_Icon => $"{UssCustomClass_Emc}-drop_down__button__icon";
        public virtual string USSCustomClassTitle => $"{UssCustomClass_Emc}-title";
        public virtual string USSCustomClassDropDown_Field => $"{UssCustomClass_Emc}-drop_down__field";
        public virtual string USSCustomClassDropDown_Message => $"{UssCustomClass_Emc}-drop_down__message";

        public Visual_C Overlay = new Visual_C { name = "overlay" }; public Text_C TitleLabel = new Text_C { name = "title" };
        public Visual_C Main = new Visual_C { name = "main" }; 
        public Button_C DropDown_Button = new Button_C { name = "dropdown" };
        public Visual_C DropDown_Button_Icon_Background = new Visual_C { name = "dropdown-icon-background" };
        public Visual_C DropDown_Button_Icon = new Visual_C { name = "dropdown-icon" };
        public Visual_C DropDown_Button_Background = new Visual_C { name = "dropdown-background" };
        public Visual_C DropDown_Field = new Visual_C { name = "field" };
        public Text_C DropDown_Message = new Text_C { name = "message" };

        protected bool m_displayMessage;

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            Overlay.AddToClassList(USSCustomClassOverlay);
            Main.AddToClassList(USSCustomClassMain);
            DropDown_Button.AddToClassList(USSCustomClassDropDown_Button);
            DropDown_Button_Icon_Background.AddToClassList(USSCustomClassDropDown_Button_Icon_Background);
            DropDown_Button_Icon.AddToClassList(USSCustomClassDropDown_Button_Icon);
            DropDown_Button_Background.AddToClassList(USSCustomClassDropDown_Button_background);
            TitleLabel.AddToClassList(USSCustomClassTitle);
            DropDown_Field.AddToClassList(USSCustomClassDropDown_Field);
            DropDown_Message.AddToClassList(USSCustomClassDropDown_Message);
        }

        protected override void InitElement()
        {
            base.InitElement();
            DropDown_Button.Height = ElementSize.Small;
            DropDown_Button.Type = ButtonType.Invisible;
            DropDown_Button.clicked += DropDownClicked;
            DropDown_Button.Front.RemoveFromHierarchy();

            DropDown_Button_Icon.style.rotate = new Rotate(90);

            Add(Main);
            Main.Add(DropDown_Button);
            DropDown_Button.Add(DropDown_Button_Icon_Background);
            DropDown_Button_Icon_Background.Add(DropDown_Button_Icon);
            DropDown_Button.Add(DropDown_Button_Background);

            Overlay.Add(DropDown_Field);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Title = null;
            Message = null;
            DisplayMessage = false;
        }

        protected virtual void DropDownClicked() => DisplayMessage = !m_displayMessage;
        public Tip Tip;
    }
}