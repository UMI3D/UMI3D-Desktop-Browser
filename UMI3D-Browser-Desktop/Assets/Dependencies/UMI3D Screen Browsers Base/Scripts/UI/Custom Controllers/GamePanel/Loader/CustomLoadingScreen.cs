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

public abstract class CustomLoadingScreen : CustomMenuScreen
{
    public new class UxmlTraits : CustomMenuScreen.UxmlTraits
    {
        UxmlStringAttributeDescription m_message = new UxmlStringAttributeDescription
        {
            name = "message",
            defaultValue = null
        };
        UxmlFloatAttributeDescription m_value = new UxmlFloatAttributeDescription
        {
            name = "value",
            defaultValue = 0f
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomLoadingScreen;

            custom.Set
                (
                    m_title.GetValueFromBag(bag, cc),
                    m_message.GetValueFromBag(bag, cc),
                    m_value.GetValueFromBag(bag, cc)
                 );
        }
    }

    public override string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/LoadingScreen";
    public override string USSCustomClassName => "loading-screen";
    public virtual string USSCustomClassMain => $"{USSCustomClassName}__main";
    public virtual string USSCustomClassLoadingBar => $"{USSCustomClassName}__loading-bar";

    public VisualElement Main = new VisualElement();
    public CustomLoadingBar LoadingBar;

    public virtual string Message
    {
        get => LoadingBar.Message;
        set => LoadingBar.Message = value;
    }

    public virtual float Value
    {
        get => LoadingBar.value;
        set => LoadingBar.value = value;
    }

    public override void InitElement()
    {
        base.InitElement();

        Main.AddToClassList(USSCustomClassMain);
        LoadingBar.AddToClassList(USSCustomClassLoadingBar);

        Add(Main);
        Main.Add(LoadingBar);
    }

    public override void Set() => Set(null, null, 0f);

    public virtual void Set(string title, string message, float value)
    {
        Set(title);
        Message = message;
        Value = value;
    }
}
