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
using System.Collections.Generic;
using UnityEngine.UIElements;

public abstract class CustomText : Label, ICustomElement
{
    public new class UxmlTraits : Label.UxmlTraits
    {
        UxmlEnumAttributeDescription<TextStyle> m_style = new UxmlEnumAttributeDescription<TextStyle>
        {
            name = "text-style",
            defaultValue = TextStyle.Body
        };

        UxmlEnumAttributeDescription<TextColor> m_color = new UxmlEnumAttributeDescription<TextColor>
        {
            name = "color",
            defaultValue = TextColor.White
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomText;

            custom.Set
                (
                    m_style.GetValueFromBag(bag, cc),
                    m_color.GetValueFromBag(bag, cc)
                );
        }
    }

    public virtual string StyleSheetPath => "USS/Displayers/text";
    public virtual string USSCustomClassName => "text";
    public virtual string USSCustomClassStyle(TextStyle style) => $"{USSCustomClassName}-{style}".ToLower();
    public virtual string USSCustomClassColor(TextColor color) => $"{USSCustomClassName}-{color}".ToLower();

    public virtual TextStyle TextStyle
    {
        get => m_style;
        set
        {
            RemoveFromClassList(USSCustomClassStyle(m_style));
            AddToClassList(USSCustomClassStyle(value));
            m_style = value;
        }
    }
    public virtual TextColor Color
    {
        get => m_color;
        set
        {
            RemoveFromClassList(USSCustomClassColor(m_color));
            AddToClassList(USSCustomClassColor(value));
            m_color = value;
        }
    }


    protected TextStyle m_style;
    protected TextColor m_color;
    protected bool m_hasBeenInitialized;

    public void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {

            throw e;
        }
        AddToClassList(USSCustomClassName);
    }

    public virtual void Set() => Set(TextStyle.Body, TextColor.White);
    
    public virtual void Set(TextStyle style, TextColor color)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        TextStyle = style;
        Color = color;
    }
}
