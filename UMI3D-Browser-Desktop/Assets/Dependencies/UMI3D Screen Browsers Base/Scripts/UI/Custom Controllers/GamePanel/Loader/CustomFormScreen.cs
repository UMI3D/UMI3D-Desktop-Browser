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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomFormScreen : CustomMenuScreen
{
    public new class UxmlTraits : CustomMenuScreen.UxmlTraits
    {
        protected UxmlBoolAttributeDescription m_displaySubmitButton = new UxmlBoolAttributeDescription
        {
            name = "display-submit-button",
            defaultValue = false
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as CustomFormScreen;

            custom.Set
                (
                    m_displaySubmitButton.GetValueFromBag(bag, cc)
                 );
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
    public override string USSCustomClassName => "form__screen";
    public virtual string USSCustomClassForm => $"{USSCustomClassName}-form";
    public virtual string USSCustomClassButton_Navigation => $"{USSCustomClassName}-button__navigation";
    public virtual string USSCustomClassButton_Submit => $"{USSCustomClassName}-button__submit";

    public event System.Action SubmitClicked;

    public CustomScrollView ScrollView;
    public CustomButton Buttond_Submit;

    protected bool m_displaySubmitButton;

    public virtual void ResetSubmitEvent() => SubmitClicked = null;

    public override void InitElement()
    {
        base.InitElement();

        ScrollView.AddToClassList(USSCustomClassForm);
        Buttond_Submit.AddToClassList(USSCustomClassButton_Navigation);
        Buttond_Submit.AddToClassList(USSCustomClassButton_Submit);

        Buttond_Submit.Type = ButtonType.Primary;
        Buttond_Submit.ClickedDown += () => SubmitClicked?.Invoke();

        Add(ScrollView);
    }

    public override void Set() => Set(false);

    public virtual void Set(bool displaySubmitButton)
    {
        Set(null);

        DisplaySubmitButton = displaySubmitButton;
    }

    public override VisualElement contentContainer => m_isSet ? ScrollView.contentContainer : this;
}
