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
using umi3d.baseBrowser.preferences;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class CustomServerButton : CustomButton
{
    public new class UxmlTraits : CustomButton.UxmlTraits
    {
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomServerButton;

            custom.Set
                (
                    m_category.GetValueFromBag(bag, cc),
                    m_size.GetValueFromBag(bag, cc),
                    m_shape.GetValueFromBag(bag, cc),
                    m_type.GetValueFromBag(bag, cc),
                    m_label.GetValueFromBag(bag, cc),
                    m_labelDirection.GetValueFromBag(bag, cc),
                    m_iconAlignment.GetValueFromBag(bag, cc)
                );
        }
    }

    public virtual string StyleSheetMenuPath => $"USS/menu";
    public virtual string StyleSheetServerPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/serverButton";
    public virtual string USSCustomClassServerButton => "server-button";
    public virtual string USSCutomClassIcon => $"{USSCustomClassServerButton}__icon";
    public virtual string USSCustomClassServerName => $"{USSCustomClassServerButton}__server-name";

    public VisualElement Icon = new VisualElement();
    public ServerPreferences.ServerData Data;

    public override void InitElement()
    {
        base.InitElement();

        try
        {
            this.AddStyleSheetFromPath(StyleSheetMenuPath);
            this.AddStyleSheetFromPath(StyleSheetServerPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }

        AddToClassList(USSCustomClassServerButton);
        Icon.AddToClassList(USSCutomClassIcon);
        LabelVisual.AddToClassList(USSCustomClassServerName);
        Front.RemoveFromHierarchy();
    }

    public override void Set(ElementCategory category, ElementSize size, ButtonShape shape, ButtonType type, string label, ElemnetDirection direction, ElementAlignment alignment)
    {
        base.Set(category, size, shape, type, label, direction, alignment);

        Add(Icon);

        LabelDirection = ElemnetDirection.Bottom;
        Type = ButtonType.Invisible;
    }
}
