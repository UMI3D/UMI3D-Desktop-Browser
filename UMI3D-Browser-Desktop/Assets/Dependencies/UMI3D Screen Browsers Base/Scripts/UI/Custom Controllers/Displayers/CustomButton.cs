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
using umi3d.baseBrowser.ui.viewController;
using UnityEngine.UIElements;

public abstract class CustomButton : Button, ICustomElement
{
    public new class UxmlTraits : Button.UxmlTraits
    {
        protected UxmlEnumAttributeDescription<ElementCategory> m_category = new UxmlEnumAttributeDescription<ElementCategory>
        {
            name = "category",
            defaultValue = ElementCategory.Menu
        };
        protected UxmlEnumAttributeDescription<ElementSize> m_size = new UxmlEnumAttributeDescription<ElementSize>
        {
            name = "size",
            defaultValue = ElementSize.Medium
        };
        protected UxmlEnumAttributeDescription<ButtonShape> m_shape = new UxmlEnumAttributeDescription<ButtonShape>
        {
            name = "shape",
            defaultValue = ButtonShape.Square
        };
        protected UxmlEnumAttributeDescription<ButtonType> m_type = new UxmlEnumAttributeDescription<ButtonType>
        {
            name = "type",
            defaultValue = ButtonType.Default
        };
        protected UxmlStringAttributeDescription m_label = new UxmlStringAttributeDescription
        {
            name = "label",
            defaultValue = null
        };
        protected UxmlEnumAttributeDescription<ElemnetDirection> m_labelDirection = new UxmlEnumAttributeDescription<ElemnetDirection>
        {
            name = "label-direction",
            defaultValue = ElemnetDirection.Leading
        };
        protected UxmlEnumAttributeDescription<ElementAlignment> m_iconAlignment = new UxmlEnumAttributeDescription<ElementAlignment>
        {
            name = "icon-alignment",
            defaultValue = ElementAlignment.Center
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomButton;

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

    public virtual string StyleSheetDisplayerPath => $"USS/displayer";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/button";
    public virtual string USSCustomClassName => "button";
    public virtual string USSCustomClassCategory(ElementCategory category) => $"{USSCustomClassName}-{category}".ToLower();
    public virtual string USSCustomClassSize(ElementSize size) => $"{USSCustomClassName}-{size}".ToLower();
    public virtual string USSCustomClassShape(ButtonShape shape) => $"{USSCustomClassName}-{shape}".ToLower();
    public virtual string USSCustomClassType(ButtonType type) => $"{USSCustomClassName}-{type}".ToLower();
    public virtual string USSCustomClassDirection(ElemnetDirection direction) => $"{USSCustomClassName}-{direction}-direction".ToLower();
    public virtual string USSCustomClassAlignment(ElementAlignment alignment) => $"{USSCustomClassName}-{alignment}-alignment".ToLower();
    public virtual string USSCustomClassLabel => $"{USSCustomClassName}__label";
    public virtual string USSCustomClassBody => $"{USSCustomClassName}__body";
    public virtual string USSCustomClassText => $"{USSCustomClassName}__text";
    public virtual string USSCustomClassContainer => $"{USSCustomClassName}__content-container";
    public virtual string USSCustomClassFront => $"{USSCustomClassName}__front";

    public virtual ElementCategory Category
    {
        get => m_category;
        set
        {
            RemoveFromClassList(USSCustomClassCategory(m_category));
            AddToClassList(USSCustomClassCategory(value));
            m_category = value;
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
        }
    }
    public virtual ButtonShape Shape
    {
        get => m_shape;
        set
        {
            RemoveFromClassList(USSCustomClassShape(m_shape));
            AddToClassList(USSCustomClassShape(value));
            m_shape = value;
        }
    }
    public virtual ButtonType Type
    {
        get => m_type;
        set
        {
            RemoveFromClassList(USSCustomClassType(m_type));
            AddToClassList(USSCustomClassType(value));
            m_type = value;
        }
    }
    public override string text
    {
        get => TextVisual.text;
        set
        {
            m_isSet = false;
            if (string.IsNullOrEmpty(value)) TextVisual.RemoveFromHierarchy();
            else Body.Insert(1, TextVisual);
            TextVisual.text = value;
            m_isSet = true;
        }
    }
    public virtual string Label
    {
        get => LabelVisual.text;
        set
        {
            m_isSet = false;
            if (string.IsNullOrEmpty(value)) LabelVisual.RemoveFromHierarchy();
            else Insert(0, LabelVisual);
            LabelVisual.text = value;
            m_isSet = true;
        }
    }
    public ElemnetDirection LabelDirection
    {
        get => m_labelDirection;
        set
        {
            RemoveFromClassList(USSCustomClassDirection(m_labelDirection));
            AddToClassList(USSCustomClassDirection(value));
            m_labelDirection = value;
        }
    }
    public ElementAlignment IconAlignment
    {
        get => m_iconAlignment;
        set
        {
            RemoveFromClassList(USSCustomClassAlignment(m_iconAlignment));
            AddToClassList(USSCustomClassAlignment(value));
            m_iconAlignment = value;
        }
    }

    protected ElementCategory m_category;
    protected ElementSize m_size;
    protected ButtonShape m_shape;
    protected ButtonType m_type;
    protected ElemnetDirection m_labelDirection;
    protected ElementAlignment m_iconAlignment;

    public CustomText LabelVisual;
    public VisualElement Body = new VisualElement { name = "body" };
    public CustomText TextVisual;
    public VisualElement Container = new VisualElement { name = "content-container" };
    public VisualElement Front = new VisualElement { name = "front" };

    /// <summary>
    /// Event raised when the interaction is up if delay and interval is 0. 
    /// Otherwise it will be raised once when down then repeatedly after [delay] at [interval] rate.
    /// </summary>
    public event System.Action<EventBase> ClickedWithInfo
    {
        add => TouchManipulator.clickedWithEventInfo += value;
        remove => TouchManipulator.clickedWithEventInfo -= value;
    }
    /// <summary>
    /// Event raised when the interaction is down.
    /// </summary>
    public event System.Action<EventBase, UnityEngine.Vector2> ClickedDownWithInfo
    {
        add => TouchManipulator.ClickedDownWithInfo += value;
        remove => TouchManipulator.ClickedDownWithInfo -= value;
    }
    /// <summary>
    /// Event raised when the interaction is up.
    /// </summary>
    public event System.Action<EventBase, UnityEngine.Vector2> ClickedUpWithInfo
    {
        add => TouchManipulator.ClickedUpWithInfo += value;
        remove => TouchManipulator.ClickedUpWithInfo -= value;
    }
    public event System.Action<EventBase, UnityEngine.Vector2> ClickedLongWithInfo
    {
        add => TouchManipulator.ClickedLongWithInfo += value;
        remove => TouchManipulator.ClickedLongWithInfo -= value;
    }
    public event System.Action<EventBase, UnityEngine.Vector2> MovedWithInfo
    {
        add => TouchManipulator.MovedWithInfo += value;
        remove => TouchManipulator.MovedWithInfo -= value;
    }
    /// <summary>
    /// Event raised when the interaction is up if delay and interval is 0. 
    /// Otherwise it will be raised once when down then repeatedly after [delay] at [interval] rate.
    /// </summary>
    public new event System.Action clicked
    {
        add => TouchManipulator.clicked += value;
        remove => TouchManipulator.clicked -= value;
    }
    /// <summary>
    /// Event raised when the interaction is down.
    /// </summary>
    public event System.Action ClickedDown
    {
        add => TouchManipulator.ClickedDown += value;
        remove => TouchManipulator.ClickedDown -= value;
    }
    /// <summary>
    /// Event raised when the interaction is up.
    /// </summary>
    public event System.Action ClickedUp
    {
        add => TouchManipulator.ClickedUp += value;
        remove => TouchManipulator.ClickedUp -= value;
    }
    public event System.Action ClickedLong
    {
        add => TouchManipulator.ClickedLong += value;
        remove => TouchManipulator.ClickedLong -= value;
    }
    public event System.Action Moved
    {
        add => TouchManipulator.Moved += value;
        remove => TouchManipulator.Moved -= value;
    }

    public TouchManipulator2 TouchManipulator = new TouchManipulator2(null, 0, 0);
    protected bool m_isSet = false;
    protected bool m_hasBeenInitialized;

    public virtual void Set() => Set(ElementCategory.Menu, ElementSize.Medium, ButtonShape.Square, ButtonType.Default, null, ElemnetDirection.Leading, ElementAlignment.Center);
    public virtual void Set(ElementCategory category, ElementSize size, ButtonShape shape, ButtonType type, string label, ElemnetDirection direction, ElementAlignment alignment)
    {
        m_isSet = false;
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Category = category;
        Size = size;
        Shape = shape;
        Type = type;
        Label = label;
        LabelDirection = direction;
        IconAlignment = alignment;
        
        m_isSet = true;
    }
    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetDisplayerPath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);
        LabelVisual.AddToClassList(USSCustomClassLabel);
        Body.AddToClassList(USSCustomClassBody);
        TextVisual.AddToClassList(USSCustomClassText);
        Container.AddToClassList(USSCustomClassContainer);
        Front.AddToClassList(USSCustomClassFront);

        clickable = TouchManipulator;

        LabelVisual.name = "label";

        Add(Body);
        Body.Add(Container);
        Body.Add(Front);
    }

    public override VisualElement contentContainer => m_isSet ? Container : this;
}
 