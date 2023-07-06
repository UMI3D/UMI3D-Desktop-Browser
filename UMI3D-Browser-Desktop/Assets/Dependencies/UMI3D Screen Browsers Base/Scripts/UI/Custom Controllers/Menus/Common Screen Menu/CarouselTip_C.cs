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
    public class CarouselTip_C : BaseVisual_C
    {
        public new class UxmlFactory : UxmlFactory<CarouselTip_C, UxmlTraits> { }

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
                var custom = ve as CarouselTip_C;

                custom.Setup(m_title.GetValueFromBag(bag, cc), m_message.GetValueFromBag(bag, cc));
            }
        }

        public override string StyleSheetPath_MainTheme => $"USS/menu";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetMenusFolderPath}/tip";

        public virtual string UssCustomClass => "tip-carousel";
        public virtual string USSCustomClassMain => $"{UssCustomClass}-main";
        public virtual string USSCustomClassTitle => $"{UssCustomClass}-title";
        public virtual string USSCustomClassMessage => $"{UssCustomClass}-message";

        public Visual_C Main = new Visual_C { name = "main" };
        public Text_C Title = new Text_C { name = "title" };
        public Text_C Message = new Text_C { name = "message" };

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            Main.AddToClassList(USSCustomClassMain);
            Title.AddToClassList(USSCustomClassTitle);
            Message.AddToClassList(USSCustomClassMessage);
        }

        protected override void InitElement()
        {
            base.InitElement();

            Title.TextAlignment = ElementAlignment.Center;
            Title.Color = TextColor.Custom;

            Message.TextAlignment = ElementAlignment.Center;

            Main.Add(Title);
            Main.Add(Message);

            Add(Main);
        }

        public void Setup(string title, string message)
        {
            Title.LocalisedText = title;
            Message.LocalisedText = message;
        }

        protected override void SetProperties()
        {
            base.SetProperties();
        }
    }
}