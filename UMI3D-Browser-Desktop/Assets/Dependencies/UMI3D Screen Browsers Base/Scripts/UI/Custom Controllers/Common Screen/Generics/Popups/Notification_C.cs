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
using umi3d.common;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class Notification_C : BaseVisual_C
    {
        public new class UxmlFactory : UxmlFactory<Notification_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlEnumAttributeDescription<ElementCategory> m_category = new UxmlEnumAttributeDescription<ElementCategory>
            {
                name = "category",
                defaultValue = ElementCategory.Menu
            };
            UxmlEnumAttributeDescription<ElementSize> m_size = new UxmlEnumAttributeDescription<ElementSize>
            {
                name = "size",
                defaultValue = ElementSize.Medium
            };
            UxmlEnumAttributeDescription<NotificationType> m_type = new UxmlEnumAttributeDescription<NotificationType>
            {
                name = "type",
                defaultValue = NotificationType.Default
            };
            UxmlLocaliseAttributeDescription m_title = new UxmlLocaliseAttributeDescription
            {
                name = "title"
            };
            UxmlLocaliseAttributeDescription m_author = new UxmlLocaliseAttributeDescription
            {
                name = "author"
            };
            UxmlLocaliseAttributeDescription m_timestamp = new UxmlLocaliseAttributeDescription
            {
                name = "timestamp"
            };
            UxmlLocaliseAttributeDescription m_message = new UxmlLocaliseAttributeDescription
            {
                name = "message"
            };
            UxmlLocaliseAttributeDescription m_choiceALabel = new UxmlLocaliseAttributeDescription
            {
                name = "choice-a-label"
            };
            UxmlLocaliseAttributeDescription m_choiceBLabel = new UxmlLocaliseAttributeDescription
            {
                name = "choice-b-label"
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
                var custom = ve as Notification_C;

                custom.Category = m_category.GetValueFromBag(bag, cc);
                custom.Size = m_size.GetValueFromBag(bag, cc);
                custom.Type = m_type.GetValueFromBag(bag, cc);
                custom.Title = m_title.GetValueFromBag(bag, cc);
                custom.Author = m_author.GetValueFromBag(bag, cc);
                custom.Timestamp = m_timestamp.GetValueFromBag(bag, cc);
                custom.Message = m_message.GetValueFromBag(bag, cc);
                custom.ChoiceALabel = m_choiceALabel.GetValueFromBag(bag, cc);
                custom.ChoiceBLabel = m_choiceBLabel.GetValueFromBag(bag, cc);
            }
        }

        public virtual ElementCategory Category
        {
            get => m_category;
            set
            {
                RemoveFromClassList(USSCustomClassCategory(m_category));
                AddToClassList(USSCustomClassCategory(value));
                m_category = value;
                ChoiceA.Category = value;
                ChoiceB.Category = value;
            }
        }
        public virtual ElementSize Size
        {
            get => m_size;
            set
            {
                RemoveFromClassList(USSCustomClassSize(m_size));
                AddToClassList(USSCustomClassSize(value));
                m_size = value;
                TitleLabel.TextStyle = TextStyle.Subtitle;
                TimestampLabel.TextStyle = TextStyle.Caption;
                AuthorLabel.TextStyle = TextStyle.Caption;
            }
        }
        public virtual NotificationType Type
        {
            get => m_type;
            set
            {
                RemoveFromClassList(USSCustomClassType(m_type));
                AddToClassList(USSCustomClassType(value));
                m_type = value;
                switch (value)
                {
                    case NotificationType.Default:
                        ChoicesContainer.Insert(0, ChoiceA);
                        ChoiceB.RemoveFromHierarchy();
                        ChoiceA.Type = ButtonType.Default;
                        break;
                    case NotificationType.Temporary:
                        ChoiceA.RemoveFromHierarchy();
                        ChoiceB.RemoveFromHierarchy();
                        break;
                    case NotificationType.Confirmation:
                        ChoicesContainer.Insert(0, ChoiceA);
                        ChoiceA.Type = ButtonType.Primary;
                        ChoicesContainer.Insert(1, ChoiceB);
                        ChoiceB.Type = ButtonType.Danger;
                        break;
                    default:
                        break;
                }
            }
        }
        public virtual LocalisationAttribute Title
        {
            get => TitleLabel.LocalisedText;
            set
            {
                if (value.IsEmpty) TitleLabel.RemoveFromHierarchy();
                else Header.Insert(0, TitleLabel);
                TitleLabel.LocalisedText = value;
            }
        }
        public virtual LocalisationAttribute Author
        {
            get => AuthorLabel.LocalisedText;
            set
            {
                if (value.IsEmpty) AuthorLabel.RemoveFromHierarchy();
                else
                {
                    Header.Insert(0, AuthorLabel);
                    if (Header.Contains(TitleLabel)) AuthorLabel.PlaceInFront(TitleLabel);
                }
                AuthorLabel.LocalisedText = value;
            }
        }
        public virtual LocalisationAttribute Timestamp
        {
            get => TimestampLabel.LocalisedText;
            set
            {
                if (value.IsEmpty) TimestampLabel.RemoveFromHierarchy();
                else
                {
                    Header.Insert(0, TimestampLabel);
                    if (Header.Contains(AuthorLabel)) TimestampLabel.PlaceInFront(AuthorLabel);
                    else if (Header.Contains(TitleLabel)) TimestampLabel.PlaceInFront(TitleLabel);
                }
                TimestampLabel.LocalisedText = value;
            }
        }
        public virtual LocalisationAttribute Message
        {
            get => MessageLabel.LocalisedText;
            set
            {
                if (value.IsEmpty) MessageLabel.RemoveFromHierarchy();
                else Main.Insert(0, MessageLabel);
                MessageLabel.LocalisedText = value;
            }
        }
        public virtual LocalisationAttribute ChoiceALabel
        {
            get => ChoiceA.LocaliseText;
            set => ChoiceA.LocaliseText = value;
        }
        public virtual LocalisationAttribute ChoiceBLabel
        {
            get => ChoiceB.LocaliseText;
            set => ChoiceB.LocaliseText = value;
        }

        public override string StyleSheetPath_MainTheme => $"USS/displayer";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/notification";

        public override string UssCustomClass_Emc => "notification";
        public virtual string USSCustomClassCategory(ElementCategory category) => $"{UssCustomClass_Emc}-{category}".ToLower();
        public virtual string USSCustomClassSize(ElementSize size) => $"{UssCustomClass_Emc}-{size}".ToLower();
        public virtual string USSCustomClassType(NotificationType type) => $"{UssCustomClass_Emc}-{type}".ToLower();
        public virtual string USSCustomClassHeader => $"{UssCustomClass_Emc}__header";
        public virtual string USSCustomClassTitle => $"{UssCustomClass_Emc}__title";
        public virtual string USSCustomClassAuthor => $"{UssCustomClass_Emc}__author";
        public virtual string USSCustomClassTimestamp => $"{UssCustomClass_Emc}__timestamp";
        public virtual string USSCustomClassMain => $"{UssCustomClass_Emc}__main";
        public virtual string USSCustomClassMessage => $"{UssCustomClass_Emc}__message";
        public virtual string USSCustomClassChoicesContainer => $"{UssCustomClass_Emc}__choices-container";
        public virtual string USSCustomClassChoice => $"{UssCustomClass_Emc}__choice";

        public Visual_C Header = new Visual_C { name = "header" };
        public Text_C TitleLabel = new Text_C { name = "title" };
        public Text_C AuthorLabel = new Text_C { name = "author" };
        public Text_C TimestampLabel = new Text_C { name = "timestamp" };
        public Visual_C Main = new Visual_C { name = "main" };
        public Text_C MessageLabel = new Text_C { name = "message" };
        public Visual_C ChoicesContainer = new Visual_C { name = "choices-container" };
        /// <summary>
        /// Primary button.
        /// </summary>
        public Button_C ChoiceA = new Button_C { name = "choice-a" };
        /// <summary>
        /// Danger button.
        /// </summary>
        public Button_C ChoiceB = new Button_C { name = "choice-b" };

        protected ElementCategory m_category;
        protected ElementSize m_size;
        protected NotificationType m_type;

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            Header.AddToClassList(USSCustomClassHeader);
            TitleLabel.AddToClassList("text-low-title");
            TitleLabel.AddToClassList(USSCustomClassTitle);
            AuthorLabel.AddToClassList("text-body");
            AuthorLabel.AddToClassList(USSCustomClassAuthor);
            TimestampLabel.AddToClassList("text-body");
            TimestampLabel.AddToClassList(USSCustomClassTimestamp);
            Main.AddToClassList(USSCustomClassMain);
            MessageLabel.AddToClassList("text-body");
            MessageLabel.AddToClassList(USSCustomClassMessage);
            ChoicesContainer.AddToClassList(USSCustomClassChoicesContainer);
            ChoiceA.AddToClassList(USSCustomClassChoice);
            ChoiceB.AddToClassList(USSCustomClassChoice);
            ChoiceA.AddToClassList($"{UssCustomClass_Emc}__choice-a");
            ChoiceB.AddToClassList($"{UssCustomClass_Emc}__choice-b");
        }

        protected override void InitElement()
        {
            base.InitElement();
            Add(Header);
            Add(Main);
            Add(ChoicesContainer);

            ChoiceA.Height = ElementSize.Small;
            ChoiceB.Height = ElementSize.Small;

            ChoiceA.clicked += () =>
            {
                Callback?.Invoke(0);
                this.RemoveFromHierarchy();
            };

            ChoiceB.clicked += () =>
            {
                Callback?.Invoke(1);
                this.RemoveFromHierarchy();
            };
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Category = ElementCategory.Menu;
            Size = ElementSize.Medium;
            Type = NotificationType.Default;
            Title = null;
            Author = null;
            Timestamp = null;
            Message = null;
            ChoiceALabel = null;
            ChoiceBLabel = null;
        }

        #region Implementation

        public NotificationDto DTO;
        public Action<int> Callback;

        #endregion
    }
}
