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
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.menu
{
    public class FormScreen_C : BaseMenuScreen_C
    {
        public new class UxmlFactory : UxmlFactory<FormScreen_C, UxmlTraits> { }

        public new class UxmlTraits : BaseMenuScreen_C.UxmlTraits
        {
            protected UxmlBoolAttributeDescription m_displaySubmitButton = new UxmlBoolAttributeDescription
            {
                name = "display-submit-button",
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
                var custom = ve as FormScreen_C;

                custom.DisplaySubmitButton = m_displaySubmitButton.GetValueFromBag(bag, cc);
            }
        }

        public virtual bool DisplaySubmitButton
        {
            get => m_displaySubmitButton;
            set
            {
                m_displaySubmitButton = value;
                if (value) Header__Right.Add(Buttond_Submit);
                else Buttond_Submit.RemoveFromHierarchy();
            }
        }

        public override string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/FormScreen";
        public override string UssCustomClass_Emc => "form__screen";
        public virtual string USSCustomClassForm => $"{UssCustomClass_Emc}-form";
        public virtual string USSCustomClassButton_Navigation => $"{UssCustomClass_Emc}-button__navigation";
        public virtual string USSCustomClassButton_Submit => $"{UssCustomClass_Emc}-button__submit";

        public ScrollView_C ScrollView = new ScrollView_C { name = "scroll-view" };
        public Button_C Buttond_Submit = new Button_C { name = "submit" };

        protected bool m_displaySubmitButton;

        public FormScreen_C() { }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            ScrollView.AddToClassList(USSCustomClassForm);
            Buttond_Submit.AddToClassList(USSCustomClassButton_Navigation);
            Buttond_Submit.AddToClassList(USSCustomClassButton_Submit);
        }

        protected override void InitElement()
        {
            base.InitElement();
            Buttond_Submit.Type = ButtonType.Primary;
            Buttond_Submit.ClickedDown += () => SubmitClicked?.Invoke();

            Add(ScrollView);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            DisplaySubmitButton = false;
        }

        public override VisualElement contentContainer => IsSet ? ScrollView.contentContainer : this;

        #region Implementation

        public event System.Action SubmitClicked;

        /// <summary>
        /// Reset <see cref="SubmitClicked"/> event.
        /// </summary>
        public virtual void ResetSubmitEvent() => SubmitClicked = null;

        #endregion
    }
}
