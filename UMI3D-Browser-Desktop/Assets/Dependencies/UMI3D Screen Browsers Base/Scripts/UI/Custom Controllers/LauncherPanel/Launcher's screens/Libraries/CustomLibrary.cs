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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class CustomLibrary : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlStringAttributeDescription m_title = new UxmlStringAttributeDescription
        {
            name = "title",
            defaultValue = null
        };
        UxmlStringAttributeDescription m_size = new UxmlStringAttributeDescription
        {
            name = "size",
            defaultValue = null
        };
        UxmlStringAttributeDescription m_date = new UxmlStringAttributeDescription
        {
            name = "date",
            defaultValue = null
        };
        UxmlStringAttributeDescription m_message = new UxmlStringAttributeDescription
        {
            name = "message",
            defaultValue = null
        };
        protected UxmlBoolAttributeDescription m_displayMessage = new UxmlBoolAttributeDescription
        {
            name = "display-message",
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
            var custom = ve as CustomLibrary;

            custom.Set
                (
                    m_title.GetValueFromBag(bag, cc),
                    m_size.GetValueFromBag(bag, cc),
                    m_date.GetValueFromBag(bag, cc),
                    m_message.GetValueFromBag(bag, cc),
                    m_displayMessage.GetValueFromBag(bag, cc)
                 );
        }
    }

    public virtual string StyleSheetMenuPath => $"USS/menu";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/library";
    public virtual string USSCustomClassName => "library";
    public virtual string USSCustomClassOverlay => $"{USSCustomClassName}-overlay";
    public virtual string USSCustomClassMain => $"{USSCustomClassName}-main";
    public virtual string USSCustomClassDropDown_Button => $"{USSCustomClassName}-drop_down__button";
    public virtual string USSCustomClassDropDown_Button_background => $"{USSCustomClassName}-drop_down__button__background";
    public virtual string USSCustomClassDropDown_Button_Icon_Background => $"{USSCustomClassName}-drop_down__button__icon__background";
    public virtual string USSCustomClassDropDown_Button_Icon => $"{USSCustomClassName}-drop_down__button__icon";
    public virtual string USSCustomClassTitle => $"{USSCustomClassName}-title";
    public virtual string USSCustomClassSize => $"{USSCustomClassName}-size";
    public virtual string USSCustomClassDelete => $"{USSCustomClassName}-delete";
    public virtual string USSCustomClassDelete_Background => $"{USSCustomClassName}-delete__background";
    public virtual string USSCustomClassDelete_Icon => $"{USSCustomClassName}-delete__icon";
    public virtual string USSCustomClassDropDown_Field => $"{USSCustomClassName}-drop_down__field";
    public virtual string USSCustomClassDropDown_Date => $"{USSCustomClassName}-drop_down__date";
    public virtual string USSCustomClassDropDown_Message => $"{USSCustomClassName}-drop_down__message";

    public VisualElement Overlay = new VisualElement { name = "overlay" };
    public VisualElement Main = new VisualElement { name = "main" };
    public CustomButton DropDown_Button;
    public VisualElement DropDown_Button_Icon_Background = new VisualElement { name = "dropdown-icon-background" };
    public VisualElement DropDown_Button_Icon = new VisualElement { name = "dropdown-icon" };
    public VisualElement DropDown_Button_Background = new VisualElement { name = "dropdown-background" };
    public CustomText TitleLabel;
    public CustomText SizeLabel;
    public CustomButton Delete;
    public VisualElement Delete_Background = new VisualElement { name = "delete-background" };
    public VisualElement Delete_Icon = new VisualElement { name = "delete-icon" };
    public VisualElement DropDown_Field = new VisualElement { name = "field" };
    public CustomText DropDown_Date;
    public CustomText DropDown_Message;

    protected bool m_displayMessage;
    protected bool m_hasBeenInitialized;

    public virtual string Title
    {
        get => TitleLabel.text;
        set
        {
            if (string.IsNullOrEmpty(value)) TitleLabel.RemoveFromHierarchy();
            else
            {
                DropDown_Button_Background.Insert(0, TitleLabel);
                TitleLabel.text = value;
            }
        }
    }
    public virtual string Size
    {
        get => SizeLabel.text;
        set
        {
            if (string.IsNullOrEmpty(value)) SizeLabel.RemoveFromHierarchy();
            else
            {
                DropDown_Button_Background.Insert(1, SizeLabel);
                SizeLabel.text = value;
            }
        }
    }
    public virtual string Date
    {
        get => DropDown_Date.text;
        set
        {
            if (string.IsNullOrEmpty(value)) DropDown_Date.RemoveFromHierarchy();
            else
            {
                DropDown_Field.Insert(0, DropDown_Date);
                DropDown_Date.text = value;
                if (DropDown_Field.Contains(DropDown_Message)) DropDown_Date.PlaceBehind(DropDown_Message);
            }
        }
    }
    public virtual string Message
    {
        get => DropDown_Message.text;
        set
        {
            if (string.IsNullOrEmpty(value)) DropDown_Message.RemoveFromHierarchy();
            else
            {
                DropDown_Field.Insert(0, DropDown_Message);
                DropDown_Message.text = value;
                if (DropDown_Field.Contains(DropDown_Date)) DropDown_Message.PlaceInFront(DropDown_Date);
            }
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

    public virtual void Set() => Set(null, null, null, null, false);

    public virtual void Set(string title, string size, string date, string message, bool displayMessage)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Title = title;
        Size = size;
        Date = date;
        Message = message;
        DisplayMessage = displayMessage;
    }

    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetMenuPath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);
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

        Add(Main);
        Main.Add(DropDown_Button);
        DropDown_Button.Add(DropDown_Button_Icon_Background);
        DropDown_Button_Icon_Background.Add(DropDown_Button_Icon);
        DropDown_Button.Add(DropDown_Button_Background);
        Main.Add(Delete);
        Delete.Add(Delete_Background);
        Delete_Background.Add(Delete_Icon);
        Overlay.Add(DropDown_Field);
        DropDown_Field.Add(DropDown_Date);

        DropDown_Button.Size = ElementSize.Small;
        DropDown_Button.Type = ButtonType.Invisible;
        DropDown_Button.clicked += DropDownClicked;
        DropDown_Button.Front.RemoveFromHierarchy();
        Delete.Size = ElementSize.Small;
        Delete.Type = ButtonType.Invisible;
    }
    protected virtual void DropDownClicked() => DisplayMessage = !m_displayMessage;

    protected abstract CustomDialoguebox CreateDialogueBox();

    public Action<int> DeleteCallback;
}
