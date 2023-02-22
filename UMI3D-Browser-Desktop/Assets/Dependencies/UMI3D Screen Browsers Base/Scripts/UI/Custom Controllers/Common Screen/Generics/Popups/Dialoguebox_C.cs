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
using System.Collections.Generic;
using umi3d.commonScreen.Container;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class Dialoguebox_C : BaseVisual_C
    {
        public new class UxmlFactory : UxmlFactory<Dialoguebox_C, UxmlTraits> { }

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
            UxmlEnumAttributeDescription<DialogueboxType> m_type = new UxmlEnumAttributeDescription<DialogueboxType>
            {
                name = "type",
                defaultValue = DialogueboxType.Default
            };
            UxmlLocaliseAttributeDescription m_title = new UxmlLocaliseAttributeDescription
            {
                name = "title"
            };
            UxmlLocaliseAttributeDescription m_message = new UxmlLocaliseAttributeDescription
            {
                name = "message",
            };
            UxmlLocaliseAttributeDescription m_choiceA = new UxmlLocaliseAttributeDescription
            {
                name = "choice-a-text"
            };
            UxmlLocaliseAttributeDescription m_choiceB = new UxmlLocaliseAttributeDescription
            {
                name = "choice-b-text"
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
                var custom = ve as Dialoguebox_C;

                custom.Category = m_category.GetValueFromBag(bag, cc);
                custom.Size = m_size.GetValueFromBag(bag, cc);
                custom.Type = m_type.GetValueFromBag(bag, cc);
                custom.Title = m_title.GetValueFromBag(bag, cc);
                custom.Message = m_message.GetValueFromBag(bag, cc);
                custom.ChoiceAText = m_choiceA.GetValueFromBag(bag, cc);
                custom.ChoiceBText = m_choiceB.GetValueFromBag(bag, cc);
            }
        }

        public override string StyleSheetPath_MainTheme => $"USS/displayer";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/dialogueBox";

        public override string UssCustomClass_Emc => "dialoguebox";
        public virtual string USSCustomClassCategory(ElementCategory category) => $"{UssCustomClass_Emc}-{category}".ToLower();
        public virtual string USSCustomClassSize(ElementSize size) => $"{UssCustomClass_Emc}-{size}".ToLower();
        public virtual string USSCustomClassType(DialogueboxType type) => $"{UssCustomClass_Emc}-{type}".ToLower();
        public virtual string USSCustomClassBody => $"{UssCustomClass_Emc}__body".ToLower();
        public virtual string USSCustomClassTitle => $"{UssCustomClass_Emc}__title".ToLower();
        public virtual string USSCustomClassMain => $"{UssCustomClass_Emc}__main".ToLower();
        public virtual string USSCustomClassMessage => $"{UssCustomClass_Emc}__message".ToLower();
        public virtual string USSCustomClassContainer => $"{UssCustomClass_Emc}__container".ToLower();
        public virtual string USSCustomClassChoiceContainer => $"{UssCustomClass_Emc}__choices-container".ToLower();
        public virtual string USSCustomClassChoice => $"{UssCustomClass_Emc}__choice".ToLower();
        public virtual string USSCustomClassChoiceA => $"{UssCustomClass_Emc}__choice-a".ToLower();
        public virtual string USSCustomClassChoiceB => $"{UssCustomClass_Emc}__choice-b".ToLower();

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
                Container.Category = value;
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
                ChoiceA.Size = value;
                ChoiceB.Size = value;
                switch (value)
                {
                    case ElementSize.Small:
                        TitleLabel.TextStyle = TextStyle.Subtitle;
                        MessageLabel.TextStyle = TextStyle.Caption;
                        break;
                    case ElementSize.Medium:
                        TitleLabel.TextStyle = TextStyle.LowTitle;
                        MessageLabel.TextStyle = TextStyle.Body;
                        break;
                    case ElementSize.Large:
                        TitleLabel.TextStyle = TextStyle.Title;
                        MessageLabel.TextStyle = TextStyle.Body;
                        break;
                    default:
                        break;
                }
            }
        }
        public virtual DialogueboxType Type
        {
            get => m_type;
            set
            {
                RemoveFromClassList(USSCustomClassType(m_type));
                AddToClassList(USSCustomClassType(value));
                m_type = value;
                switch (value)
                {
                    case DialogueboxType.Default:
                        ChoicesContainer.Insert(0, ChoiceA);
                        ChoiceB.RemoveFromHierarchy();
                        ChoiceA.Type = ButtonType.Default;
                        break;
                    case DialogueboxType.Confirmation:
                        ChoicesContainer.Insert(0, ChoiceA);
                        ChoiceA.Type = ButtonType.Primary;
                        ChoicesContainer.Insert(1, ChoiceB);
                        ChoiceB.Type = ButtonType.Danger;
                        break;
                    case DialogueboxType.Error:
                        ChoicesContainer.Insert(0, ChoiceA);
                        ChoicesContainer.Insert(1, ChoiceB);
                        break;
                    default:
                        break;
                }
            }
        }
        public virtual LocalisationAttribute Title
        {
            get => TitleLabel.LocaliseText;
            set
            {
                IsSet = false;
                if (value.IsEmpty) TitleLabel.RemoveFromHierarchy();
                else Body.Insert(0, TitleLabel);
                TitleLabel.LocaliseText = value;
                IsSet = true;
            }
        }
        public virtual LocalisationAttribute Message
        {
            get => MessageLabel.LocaliseText;
            set
            {
                IsSet = false;
                if (value.IsEmpty) MessageLabel.RemoveFromHierarchy();
                else Container.Insert(0, MessageLabel);
                MessageLabel.LocaliseText = value;
                IsSet = true;
            }
        }
        public virtual LocalisationAttribute ChoiceAText
        {
            get => ChoiceA.LocaliseText;
            set => ChoiceA.LocaliseText = value;
        }
        public virtual LocalisationAttribute ChoiceBText
        {
            get => ChoiceB.LocaliseText;
            set => ChoiceB.LocaliseText = value;
        }

        protected ElementCategory m_category;
        protected ElementSize m_size;
        protected DialogueboxType m_type;

        public Visual_C Body = new Visual_C { name = "body" };
        public Text_C TitleLabel = new Text_C { name = "title" };
        public Visual_C Main = new Visual_C { name = "main" };
        public Text_C MessageLabel = new Text_C { name = "message" };
        public ScrollView_C Container = new ScrollView_C { name = "container" };
        public Visual_C ChoicesContainer = new Visual_C { name = "choice-container" };
        public Button_C ChoiceA = new Button_C { name = "choice-a" };
        public Button_C ChoiceB = new Button_C { name = "choice-b" };

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            Body.AddToClassList(USSCustomClassBody);
            TitleLabel.AddToClassList(USSCustomClassTitle);
            Main.AddToClassList(USSCustomClassMain);
            MessageLabel.AddToClassList(USSCustomClassMessage);
            Container.AddToClassList(USSCustomClassContainer);
            ChoicesContainer.AddToClassList(USSCustomClassChoiceContainer);
            ChoiceA.AddToClassList(USSCustomClassChoice);
            ChoiceB.AddToClassList(USSCustomClassChoice);
            ChoiceA.AddToClassList(USSCustomClassChoiceA);
            ChoiceB.AddToClassList(USSCustomClassChoiceB);
        }

        protected override void InitElement()
        {
            base.InitElement();
            ChoiceA.clicked += () =>
            {
                Callback?.Invoke(0);
                this.RemoveFromHierarchy();
                DisplayNextDialogueBox();
            };

            ChoiceB.clicked += () =>
            {
                Callback?.Invoke(1);
                this.RemoveFromHierarchy();
                DisplayNextDialogueBox();
            };

            Add(Body);
            Body.Add(Main);
            Main.Add(Container);
            Body.Add(ChoicesContainer);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Category = ElementCategory.Menu;
            Size = ElementSize.Medium;
            Type = DialogueboxType.Default;
            Title = null;
            Message = null;
            ChoiceAText = null;
            ChoiceBText = null;
        }

        public override VisualElement contentContainer => IsSet ? Container.contentContainer : this;

        #region Implementation

        public Action<int> Callback;
        public static Queue<(Dialoguebox_C, VisualElement)> Queue = new Queue<(Dialoguebox_C, VisualElement)>();
        public static Queue<(Dialoguebox_C, VisualElement)> PriorityQueue = new Queue<(Dialoguebox_C, VisualElement)>();

        /// <summary>
        /// Clear Queue and PriorityQueue.
        /// </summary>
        public static void ResetAllQueue()
        {
            Queue.Clear();
            PriorityQueue.Clear();
        }

        /// <summary>
        /// Enqueue this. It will then be displayed at the root of this visual [element].
        /// </summary>
        /// <param name="element"></param>
        public virtual void Enqueue(VisualElement element)
        {
            Queue.Enqueue((this, element));
            if (Queue.Count > 1 || PriorityQueue.Count > 1) return;

            AddToTheRoot(element);
        }

        /// <summary>
        /// Enqueue this. It will be displayed before the simple dialogue box and after the priority dialogue box that have beeen added earlyer.
        /// </summary>
        /// <param name="element"></param>
        public virtual void EnqueuePriority(VisualElement element)
        {
            PriorityQueue.Enqueue((this, element));
            if (PriorityQueue.Count > 1) return;

            if (Queue.Count > 0)
            {
                var (currentDialogueBox, elt) = Queue.Peek();
                currentDialogueBox.RemoveFromHierarchy();
            }

            AddToTheRoot(element);
        }

        /// <summary>
        /// Add this to the root of the visual [element].
        /// </summary>
        /// <param name="element"></param>
        protected virtual void AddToTheRoot(VisualElement element)
        {
            if (element == null) return;
            var root = element.FindRoot();
            root.Add(this);
        }

        /// <summary>
        /// Display the first simple dialogue box in the [Queue].
        /// <see cref="Queue"/>
        /// </summary>
        protected virtual void DisplayFirstInQueue()
        {
            if (Queue.Count == 0) return;
            var (nextDialogueBox, elt) = Queue.Peek();
            if (nextDialogueBox != null) nextDialogueBox.AddToTheRoot(elt);
        }

        /// <summary>
        /// Display the first priority dialogue box in the [PriorityQueue].
        /// <see cref="PriorityQueue"/>
        /// </summary>
        protected virtual void DisplayFirstInPriorityQueue()
        {
            if (PriorityQueue.Count == 0) return;
            var (nextDialogueBox, elt) = PriorityQueue.Peek();
            if (nextDialogueBox != null) nextDialogueBox.AddToTheRoot(elt);
        }

        /// <summary>
        /// Remove the current dialogue box from the right Queue ([Queue] or [PriorityQueue]). Then display the next dialogue box (eather a priority one if any or a simple one).
        /// </summary>
        public virtual void DisplayNextDialogueBox()
        {
            if (PriorityQueue.Count > 0)
            {
                //Dequeue the current dialogue box.
                PriorityQueue.Dequeue();

                if (PriorityQueue.Count > 0) DisplayFirstInPriorityQueue();
                else if (Queue.Count > 0) DisplayFirstInQueue();
            }
            else if (Queue.Count > 0)
            {
                //Dequeue the current dialogue box.
                Queue.Dequeue();

                if (Queue.Count > 0) DisplayFirstInQueue();
            }
        }

        #endregion
    }
}
