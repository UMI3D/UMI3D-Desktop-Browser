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
using System;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.menu
{
    public class Library_C : Visual_C
    {
        public new class UxmlFactory : UxmlFactory<Library_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlLocaliseAttributeDescription m_title = new UxmlLocaliseAttributeDescription
            {
                name = "title"
            };
            UxmlLocaliseAttributeDescription m_size = new UxmlLocaliseAttributeDescription
            {
                name = "size"
            };
            UxmlLocaliseAttributeDescription m_date = new UxmlLocaliseAttributeDescription
            {
                name = "date"
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
            protected UxmlBoolAttributeDescription m_allowDeletion = new UxmlBoolAttributeDescription
            {
                name = "allow-deletion",
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
                var custom = ve as Library_C;

                custom.Title = m_title.GetValueFromBag(bag, cc);
                custom.Size = m_size.GetValueFromBag(bag, cc);
                custom.Date = m_date.GetValueFromBag(bag, cc);
                custom.Message = m_message.GetValueFromBag(bag, cc);
                custom.DisplayMessage = m_displayMessage.GetValueFromBag(bag, cc);
                custom.AllowDeletion = m_allowDeletion.GetValueFromBag(bag, cc);
            }
        }

        public virtual LocalisationAttribute Title
        {
            get => TitleLabel.LocaliseText;
            set
            {
                if (value.IsEmpty) TitleLabel.RemoveFromHierarchy();
                else DropDown_Button_Background.Insert(0, TitleLabel);
                TitleLabel.LocaliseText = value;
            }
        }
        public virtual LocalisationAttribute Size
        {
            get => SizeLabel.LocaliseText;
            set
            {
                if (value.IsEmpty) SizeLabel.RemoveFromHierarchy();
                else DropDown_Button_Background.Insert(1, SizeLabel);
                SizeLabel.LocaliseText = value;
            }
        }
        public virtual LocalisationAttribute Date
        {
            get => DropDown_Date.LocaliseText;
            set
            {
                if (value.IsEmpty) DropDown_Date.RemoveFromHierarchy();
                else
                {
                    DropDown_Field.Insert(0, DropDown_Date);
                    if (DropDown_Field.Contains(DropDown_Message)) DropDown_Date.PlaceBehind(DropDown_Message);
                }
                DropDown_Date.LocaliseText = value;
            }
        }
        public virtual LocalisationAttribute Message
        {
            get => DropDown_Message.LocaliseText;
            set
            {
                if (value.IsEmpty) DropDown_Message.RemoveFromHierarchy();
                else
                {
                    DropDown_Field.Insert(0, DropDown_Message);
                    if (DropDown_Field.Contains(DropDown_Date)) DropDown_Message.PlaceInFront(DropDown_Date);
                }
                DropDown_Message.LocaliseText = value;
            }
        }
        public virtual string Path
        {
            get;
            set;
        }
        public virtual bool DisplayMessage
        {
            get => m_displayMessage;
            set
            {
                m_displayMessage = value;
                if (m_displayMessage) this.AddIfNotInHierarchy(Overlay);

                DropDown_Button_Icon.AddAnimation
                (
                    this,
                    () => DropDown_Button_Icon.style.rotate = new Rotate(90),
                    () => DropDown_Button_Icon.style.rotate = new Rotate(180),
                    "rotate",
                    AnimatorManager.DropdownDuration,
                    revert: !m_displayMessage
                );

                Overlay.schedule.Execute(() =>
                {
                    Overlay.WaitUntil
                    (
                        () => !float.IsNaN(DropDown_Field.layout.height),
                        () =>
                        {
                            var fieldTotalHeight = DropDown_Field.layout.height + DropDown_Field.resolvedStyle.marginTop + DropDown_Field.resolvedStyle.marginBottom;
                            Overlay.AddAnimation
                            (
                                this,
                                () => Overlay.style.height = 0f,
                                () => Overlay.style.height = fieldTotalHeight,
                                "height",
                                AnimatorManager.DropdownDuration,
                                revert: !m_displayMessage,
                                callback: m_displayMessage ? null : Overlay.RemoveFromHierarchy
                            );
                        }
                    );
                });
            }
        }

        public virtual bool AllowDeletion
        {
            get => m_allowDeletion;
            set
            {
                m_allowDeletion = value;
                if (value) Main.Add(Delete);
                else Delete.RemoveFromHierarchy();
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
        public virtual string USSCustomClassSize => $"{UssCustomClass_Emc}-size";
        public virtual string USSCustomClassDelete => $"{UssCustomClass_Emc}-delete";
        public virtual string USSCustomClassDelete_Background => $"{UssCustomClass_Emc}-delete__background";
        public virtual string USSCustomClassDelete_Icon => $"{UssCustomClass_Emc}-delete__icon";
        public virtual string USSCustomClassDropDown_Field => $"{UssCustomClass_Emc}-drop_down__field";
        public virtual string USSCustomClassDropDown_Date => $"{UssCustomClass_Emc}-drop_down__date";
        public virtual string USSCustomClassDropDown_Message => $"{UssCustomClass_Emc}-drop_down__message";

        public VisualElement Overlay = new VisualElement { name = "overlay" };
        public VisualElement Main = new VisualElement { name = "main" };
        public Button_C DropDown_Button = new Button_C { name = "dropdown" };
        public VisualElement DropDown_Button_Icon_Background = new VisualElement { name = "dropdown-icon-background" };
        public VisualElement DropDown_Button_Icon = new VisualElement { name = "dropdown-icon" };
        public VisualElement DropDown_Button_Background = new VisualElement { name = "dropdown-background" };
        public Text_C TitleLabel = new Text_C { name = "title" };
        public Text_C SizeLabel = new Text_C { name = "size" };
        public Button_C Delete = new Button_C { name = "delete" };
        public VisualElement Delete_Background = new VisualElement { name = "delete-background" };
        public VisualElement Delete_Icon = new VisualElement { name = "delete-icon" };
        public VisualElement DropDown_Field = new VisualElement { name = "field" };
        public Text_C DropDown_Date = new Text_C { name = "date" };
        public Text_C DropDown_Message = new Text_C { name = "message" };

        protected bool m_displayMessage;
        protected bool m_allowDeletion;

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
            SizeLabel.AddToClassList(USSCustomClassSize);
            Delete.AddToClassList(USSCustomClassDelete);
            Delete_Background.AddToClassList(USSCustomClassDelete_Background);
            Delete_Icon.AddToClassList(USSCustomClassDelete_Icon);
            DropDown_Field.AddToClassList(USSCustomClassDropDown_Field);
            DropDown_Date.AddToClassList(USSCustomClassDropDown_Date);
            DropDown_Message.AddToClassList(USSCustomClassDropDown_Message);
        }

        protected override void InitElement()
        {
            base.InitElement();
            DropDown_Button.Size = ElementSize.Small;
            DropDown_Button.Type = ButtonType.Invisible;
            DropDown_Button.clicked += DropDownClicked;
            DropDown_Button.Front.RemoveFromHierarchy();
            Delete.Size = ElementSize.Small;
            Delete.Type = ButtonType.Invisible;

            Add(Main);
            Main.Add(DropDown_Button);
            DropDown_Button.Add(DropDown_Button_Icon_Background);
            DropDown_Button_Icon_Background.Add(DropDown_Button_Icon);
            DropDown_Button.Add(DropDown_Button_Background);

            Delete.Add(Delete_Background);
            Delete_Background.Add(Delete_Icon);
            Overlay.Add(DropDown_Field);
            DropDown_Field.Add(DropDown_Date);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Title = null;
            Size = null;
            Date = null;
            Message = null;
            DisplayMessage = false;
            AllowDeletion = false;
        }


        protected virtual void DropDownClicked() => DisplayMessage = !m_displayMessage;

        public Action<int> DeleteCallback;
    }
}