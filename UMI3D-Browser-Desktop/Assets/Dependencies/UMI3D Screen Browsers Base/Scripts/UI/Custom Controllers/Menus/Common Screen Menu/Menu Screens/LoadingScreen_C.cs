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
using umi3d.commonScreen.Displayer;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.menu
{
    public class LoadingScreen_C : BaseMenuScreen_C
    {
        public new class UxmlFactory : UxmlFactory<LoadingScreen_C, UxmlTraits> { }

        public new class UxmlTraits : BaseMenuScreen_C.UxmlTraits
        {
            UxmlLocaliseAttributeDescription m_message = new UxmlLocaliseAttributeDescription
            {
                name = "message"
            };
            UxmlFloatAttributeDescription m_value = new UxmlFloatAttributeDescription
            {
                name = "value",
                defaultValue = 0f
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
                var custom = ve as LoadingScreen_C;

                custom.Message = m_message.GetValueFromBag(bag, cc);
                custom.Value = m_value.GetValueFromBag(bag, cc);
            }
        }

        public override string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/LoadingScreen";

        public override string UssCustomClass_Emc => "loading-screen";
        public virtual string USSCustomClassMain => $"{UssCustomClass_Emc}__main";
        public virtual string USSCustomClassLoadingBar => $"{UssCustomClass_Emc}__loading-bar";

        public VisualElement Main = new VisualElement();
        public LoadingBar_C LoadingBar = new LoadingBar_C();

        public virtual LocalisationAttribute Message
        {
            get => LoadingBar.LocalisedMessage;
            set => LoadingBar.LocalisedMessage = value;
        }

        public virtual float Value
        {
            get => LoadingBar.value;
            set => LoadingBar.value = value;
        }

        public LoadingScreen_C() { }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            Main.AddToClassList(USSCustomClassMain);
            LoadingBar.AddToClassList(USSCustomClassLoadingBar);
        }

        protected override void InitElement()
        {
            base.InitElement();
            Add(Main);
            Main.Add(LoadingBar);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Message = null;
            Value = 0.0f;
        }
    }
}
