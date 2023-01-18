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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using umi3d.baseBrowser.ui.viewController;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class CustomScrollableDataCollection<D> : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlEnumAttributeDescription<ElementCategory> m_category = new UxmlEnumAttributeDescription<ElementCategory>
        {
            name = "category",
            defaultValue = ElementCategory.Menu
        };

        protected UxmlEnumAttributeDescription<ScrollViewMode> m_ScrollViewMode = new UxmlEnumAttributeDescription<ScrollViewMode>
        {
            name = "mode",
            defaultValue = ScrollViewMode.Vertical
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomScrollableDataCollection<D>;

            custom.Set
                (
                    m_category.GetValueFromBag(bag, cc),
                    m_ScrollViewMode.GetValueFromBag(bag, cc)
                 );
        }
    }

    public virtual ElementCategory Category
    {
        get => m_category;
        set
        {
            RemoveFromClassList(USSCustomClassCategory(m_category));
            AddToClassList(USSCustomClassCategory(value));
            m_category = value;
            ScrollView.Category = m_category;
        }
    }
    public virtual ScrollViewMode Mode
    {
        get => ScrollView.mode;
        set
        {
            RemoveFromClassList(USSCustomClassMode(ScrollView.mode));
            AddToClassList(USSCustomClassMode(value));
            ScrollView.mode = value;
            UpdateSize();
        }
    }

    public virtual string StyleSheetContainerPath => $"USS/container";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetContainersFolderPath}/scrollableDataCollection";
    public virtual string USSCustomClassName => "sdc";
    public virtual string USSCustomClassCategory(ElementCategory category) => $"{USSCustomClassName}-{category}".ToLower();
    public virtual string USSCustomClassMode(ScrollViewMode mode) => $"{USSCustomClassName}-{mode}".ToLower();
    public virtual string USSCustomClassBox => $"{USSCustomClassName}-box";
    public virtual string USSCustomClassDragger => $"{USSCustomClassName}-dragger";

    public CustomScrollView ScrollView;

    protected ElementCategory m_category;
    protected bool m_hasBeenInitialized;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetContainerPath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);

        Add(ScrollView);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual void Set() => Set(ElementCategory.Menu, ScrollViewMode.Vertical);

    public virtual void Set(ElementCategory category, ScrollViewMode mode)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Category = category;
        Mode = mode;
    }

    #region Implementation

    class DraggerData
    {
        public TouchManipulator2 manipulator;
        public Vector2 startPosition;
    }

    /// <summary>
    /// Collection of the data.
    /// </summary>
    public List<D> Data = new List<D>();
    /// <summary>
    /// Collection of the Data and its corresponding visualElement in the scroll view.
    /// </summary>
    public Dictionary<D, VisualElement> DataToItem = new Dictionary<D, VisualElement>();

    protected List<VisualElement> m_waitingItems = new List<VisualElement>();
    protected List<VisualElement> m_waintingBoxes = new List<VisualElement>();
    protected List<VisualElement> m_waintingDragger = new List<VisualElement>();

    protected float m_size;
    protected bool m_isReorderable;

    /// <summary>
    /// If <see cref="Mode"/> is Vertical : width of each element. Else heigth.
    /// </summary>
    public virtual float Size 
    { 
        get => m_size;
        set
        {
            m_size = value;
            UpdateSize();
        }

    }

    /// <summary>
    /// whether or not the collection can be reorderable with drag&drop.
    /// </summary>
    public virtual bool IsReorderable 
    {
        get => m_isReorderable;
        set
        {
            m_isReorderable = value;
            UpdateReorderableState();
        }
    }

    /// <summary>
    /// Make a VisualElement to add to the scroll view.
    /// </summary>
    public virtual Func<VisualElement> MakeItem { get; set; }

    /// <summary>
    /// Bind a datum to a visualElement.
    /// </summary>
    public virtual Action<D, VisualElement> BindItem { get; set; }

    /// <summary>
    /// Unbind a visualElement from its datum.
    /// </summary>
    public virtual Action<VisualElement> UnbindItem { get; set; }

    /// <summary>
    /// Add this <paramref name="datum"/> at the end of the collection of data and make an item and add it in the scroll view.
    /// </summary>
    /// <param name="datum"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    public virtual void AddDatum(D datum)
        => InsertAt(Data == null ? 0 : Data.Count, datum); 

    /// <summary>
    /// Insert this <paramref name="datum"/> at <paramref name="index"/> in the collection of data and make an item and add it in the scroll view.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="datum"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    public virtual void InsertAt(int index, D datum)
    {
        if (datum == null) throw new ArgumentNullException("Try to add a datum null");
        if (Data == null) throw new NullReferenceException("Data is null");
        if (Data.Contains(datum)) return;
        if (MakeItem == null) throw new NullReferenceException("MakeItem is null");
        if (BindItem == null) throw new NullReferenceException("BindItem is null");

        VisualElement item;
        if (m_waitingItems.Count == 0) item = MakeItem();
        else
        {
            item = m_waitingItems[0];
            m_waitingItems.RemoveAt(0);
        }

        BindItem(datum, item);

        VisualElement box;
        if (m_waintingBoxes.Count == 0)
        {
            box = new VisualElement { name = "box-item" };
            box.AddToClassList(USSCustomClassBox);
        }
        else
        {
            box = m_waintingBoxes[0];
            m_waintingBoxes.RemoveAt(0);
        }
        switch (Mode)
        {
            case ScrollViewMode.Vertical:
                box.style.height = Size;
                break;
            case ScrollViewMode.Horizontal:
                box.style.width = Size;
                break;
            case ScrollViewMode.VerticalAndHorizontal:
                break;
            default:
                break;
        }
        box.Add(item);

        if (IsReorderable) AddDragger(datum, box);

        Data.Insert(index, datum);
        DataToItem.Add(datum, item);

        ScrollView.Insert(index, box);
    }

    /// <summary>
    /// Remove this <paramref name="datum"/> for the collection of data and remove the corresponding item from the scroll view.
    /// </summary>
    /// <param name="datum"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    public virtual void RemoveDatum(D datum)
    {
        if (datum == null) throw new ArgumentNullException("Try to add a datum null");
        if (Data == null) throw new NullReferenceException("Data is null");
        if (!Data.Contains(datum)) return;
        if (UnbindItem == null) throw new NullReferenceException("UnbindItem is null");

        Data.Remove(datum);
        var item = DataToItem[datum];
        var box = item.parent;

        box.RemoveFromHierarchy();
        m_waintingBoxes.Add(box);
        item.RemoveFromHierarchy();
        m_waitingItems.Add(item);

        UnbindItem(item);

        DataToItem.Remove(datum);
    }

    /// <summary>
    /// Reordered this <paramref name="datum"/> at the <paramref name="indexNew"/>.
    /// </summary>
    /// <param name="indexNew"></param>
    /// <param name="datum"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual void ReorderedDatum(int indexNew, D datum)
    {
        if (datum == null) throw new ArgumentNullException("Try to add a datum null");
        if (Data == null) throw new NullReferenceException("Data is null");
        if (!Data.Contains(datum)) return;

        Data.Remove(datum);
        Data.Insert(indexNew, datum);
        ScrollView.Insert(indexNew, DataToItem[datum].parent);
    }

    /// <summary>
    /// Clears the collection view, recreates all visible visual elements, and rebinds all items.
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual void Rebuild()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Rebinds all items.
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual void Rebind()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Update the size of all items.
    /// </summary>
    public virtual void UpdateSize()
    {
        foreach (var item in DataToItem.Values)
        {
            var box = item.parent;
            box.style.width = StyleKeyword.Null;
            box.style.height = StyleKeyword.Null;

            switch (Mode)
            {
                case ScrollViewMode.Vertical:
                    box.style.height = Size;
                    break;
                case ScrollViewMode.Horizontal:
                    box.style.width = Size;
                    break;
                case ScrollViewMode.VerticalAndHorizontal:
                    break;
                default:
                    break;
            }
        }
    }

    public virtual void UpdateReorderableState()
    {
        foreach (var DataAnditem in DataToItem)
        {
            var box = DataAnditem.Value.parent;

            if (IsReorderable) AddDragger(DataAnditem.Key, box);
            else
            {
                var dragger = box.Q("dragger");
                if (dragger == null) return;
                dragger.RemoveFromHierarchy();
                m_waintingDragger.Add(dragger);
            }
        }
    }

    protected virtual void AddDragger(D datum, VisualElement box)
    {
        VisualElement dragger;
        if (m_waintingDragger.Count == 0)
        {
            dragger = new VisualElement { name = "dragger" };
            dragger.AddToClassList(USSCustomClassDragger);
            TouchManipulator2 draggerManipulator = new TouchManipulator2();
            dragger.AddManipulator(draggerManipulator);

            dragger.userData = new DraggerData { manipulator = draggerManipulator };

            draggerManipulator.ClickedDownWithInfo += (evt, localPosition) =>
            {
                var draggerData = dragger.userData as DraggerData;
                draggerData.startPosition = localPosition;
                dragger.userData = draggerData;
            };
            draggerManipulator.ClickedUp += () =>
            {
                var draggerData = dragger.userData as DraggerData;
                draggerData.startPosition = Vector2.zero;
                dragger.userData = draggerData;

                dragger.parent.transform.position = Vector3.zero;
            };
            draggerManipulator.MovedWithInfo += (evt, localPosition) => {
                var draggerData = dragger.userData as DraggerData;
                Vector3 delta = localPosition - draggerData.startPosition;
                switch (Mode)
                {
                    case ScrollViewMode.Vertical:
                        delta.x = 0;
                        break;
                    case ScrollViewMode.Horizontal:
                        delta.y = 0;
                        break;
                    case ScrollViewMode.VerticalAndHorizontal:
                        break;
                    default:
                        break;
                }
                dragger.parent.transform.position = dragger.parent.transform.position + delta;

                float position = 0;
                switch (Mode)
                {
                    case ScrollViewMode.Vertical:
                        position = dragger.parent.transform.position.y;
                        break;
                    case ScrollViewMode.Horizontal:
                        position = dragger.parent.transform.position.x;
                        break;
                    case ScrollViewMode.VerticalAndHorizontal:
                        break;
                    default:
                        break;
                }
                if (Math.Sign(position) < 0 && Math.Abs(position) > Size / 2f)
                {
                    dragger.parent.transform.position = Vector3.zero;
                    var oldindex = Data.IndexOf(datum);
                    if (oldindex != 0) ReorderedDatum(oldindex - 1, datum);
                }
                else if (Math.Sign(position) > 0 && Math.Abs(position) > Size / 2f)
                {
                    dragger.parent.transform.position = Vector3.zero;
                    var oldindex = Data.IndexOf(datum);
                    if (oldindex != Data.Count - 1) ReorderedDatum(oldindex + 1, datum);
                }
            };
        }
        else
        {
            dragger = m_waintingDragger[0];
            m_waintingDragger.RemoveAt(0);
        }

        box.Add(dragger);
    }

    #endregion
}
