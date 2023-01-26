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
using System.Collections.Generic;
using System.Linq;
using umi3d.baseBrowser.ui.viewController;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomScrollableDataCollection<D> : VisualElement, ICustomElement
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

        protected UxmlEnumAttributeDescription<SelectionType> m_selectionType = new UxmlEnumAttributeDescription<SelectionType>
        {
            name = "selection-type",
            defaultValue = SelectionType.None
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
                    m_ScrollViewMode.GetValueFromBag(bag, cc),
                    m_selectionType.GetValueFromBag(bag, cc)
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
    public virtual SelectionType SelectionType
    {
        get => m_selectionType;
        set
        {
            m_selectionType = value;
            //TODO update selected data collection when updated.
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
    protected SelectionType m_selectionType;

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
    public virtual void Set() => Set(ElementCategory.Menu, ScrollViewMode.Vertical, SelectionType.None);

    public virtual void Set(ElementCategory category, ScrollViewMode mode, SelectionType selectionType)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Category = category;
        Mode = mode;
        SelectionType = selectionType;
    }

    #region Implementation

    class DraggerData
    {
        public D Datum;
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
    /// <summary>
    /// Collection of all the data that are selected.
    /// </summary>
    public List<D> SelectedData = new List<D>();

    protected List<VisualElement> m_waitingItems = new List<VisualElement>();
    protected List<VisualElement> m_waintingBoxes = new List<VisualElement>();
    protected List<VisualElement> m_waintingDragger = new List<VisualElement>();

    protected float m_size;
    protected bool m_isReorderable;
    protected ReorderableMode m_reorderableMode;

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
    /// Whether or not a dragger should be added to drag&drop.
    /// </summary>
    public virtual ReorderableMode ReorderableMode
    {
        get => m_reorderableMode;
        set
        {
            m_reorderableMode = value;
            UpdateReorderableState();
        }
    }

    /// <summary>
    /// Make a VisualElement to add to the scroll view.
    /// </summary>
    public virtual Func<D, VisualElement> MakeItem { get; set; }

    /// <summary>
    /// Predicate use to find available item matching this datum.
    /// </summary>
    public virtual Predicate<(D, VisualElement)> FindItem { get; set; }

    /// <summary>
    /// Bind a datum to a visualElement.
    /// </summary>
    public virtual Action<D, VisualElement> BindItem { get; set; }

    /// <summary>
    /// Unbind a visualElement from its datum.
    /// </summary>
    public virtual Action<D, VisualElement> UnbindItem { get; set; }

    /// <summary>
    /// Action raised when a datum is selected.
    /// </summary>
    public virtual System.Action<D, VisualElement> SelectItem { get; set; }

    /// <summary>
    /// Action raised when a datum is unselected.
    /// </summary>
    public virtual System.Action<D, VisualElement> UnselectItem { get; set; }

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
        if (m_waitingItems.Count == 0) item = MakeItem(datum);
        else
        {
            int i = -1;
            bool isFound = false;
            bool find(VisualElement v)
            {
                i++;
                isFound = FindItem == null ? true : FindItem.Invoke((datum, v));
                return isFound;
            }
            
            item = m_waitingItems.Find(find);
            if (isFound) m_waitingItems.RemoveAt(i);
            else item = MakeItem(datum);
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
                if (Size != 0) box.style.height = Size;
                break;
            case ScrollViewMode.Horizontal:
                if (Size != 0) box.style.width = Size;
                break;
            case ScrollViewMode.VerticalAndHorizontal:
                break;
            default:
                break;
        }
        box.Add(item);

        if (IsReorderable)
        {
            switch (ReorderableMode)
            {
                case ReorderableMode.Dragger:
                    AddDragger(datum, box);
                    break;
                case ReorderableMode.Element:
                    AddDraggerAsElement(datum, item);
                    break;
                default:
                    break;
            }
        }

        Data.Insert(index, datum);
        DataToItem.Add(datum, item);

        ScrollView.Insert(index, box);

        if (Data.Count == 2) UpdateReorderableState();
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

        var item = DataToItem[datum];
        var box = item.parent;

        RemoveDragger(box);
        RemoveDraggerAsElement(item);

        box.RemoveFromHierarchy();
        m_waintingBoxes.Add(box);
        item.RemoveFromHierarchy();
        m_waitingItems.Add(item);

        UnbindItem(datum, item);

        DataToItem.Remove(datum);
        Data.Remove(datum);
        SelectedData.Remove(datum);
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
    /// Clear data, remove item from scrollview, unbind item.
    /// </summary>
    public virtual void ClearSDC()
    {
        UnselectAll();
        for (int i = Data.Count - 1; i >= 0 ; i--)
            RemoveDatum(Data[i]);
    }


    /// <summary>
    /// Select <paramref name="datum"/> if <see cref="SelectionType"/> is <see cref="SelectionType.Single"/> or <see cref="SelectionType.Multiple"/>.
    /// </summary>
    /// <param name="datum"></param>
    public virtual void Select(D datum)
    {
        if (datum == null || !Data.Contains(datum)) return;

        switch (SelectionType)
        {
            case SelectionType.None:
                return;
            case SelectionType.Single:
                UnselectAll(datum);
                break;
            case SelectionType.Multiple:
                break;
            default:
                break;
        }

        SelectItem?.Invoke(datum, DataToItem[datum]);

        SelectedData.Add(datum);
    }

    /// <summary>
    /// Select the collection <paramref name="data"/> if <see cref="SelectionType"/> is <see cref="SelectionType.Multiple"/>.
    /// </summary>
    /// <param name="data"></param>
    /// <exception cref="Exception"></exception>
    public virtual void Select(List<D> data)
    {
        if (SelectionType != SelectionType.Multiple) throw new Exception("Selection type is not set to Multiple");

        foreach (var datum in data) Select(datum);
    }

    /// <summary>
    /// Unselect <paramref name="datum"/>.
    /// </summary>
    /// <param name="datum"></param>
    public virtual void Unselect(D datum)
    {
        if (datum == null || !Data.Contains(datum) || !SelectedData.Contains(datum)) return;

        SelectedData.Remove(datum);

        UnselectItem?.Invoke(datum, DataToItem[datum]);
    }
    
    /// <summary>
    /// Unselect the collection <paramref name="data"/>.
    /// </summary>
    /// <param name="data"></param>
    public virtual void Unselect(List<D> data)
    {
        foreach (var datum in data) Unselect(datum);
    }

    /// <summary>
    /// Select all the <see cref="Data"/> except <paramref name="exceptions"/> if <see cref="SelectionType"/> is <see cref="SelectionType.Multiple"/>.
    /// </summary>
    /// <param name="exceptions"></param>
    /// <exception cref="Exception"></exception>
    public virtual void SelectAll(params D[] exceptions)
    {
        if (SelectionType != SelectionType.Multiple) throw new Exception("Selection type is not set to Multiple");

        foreach (var datum in Data)
        {
            bool isException = false;
            foreach (var exception in exceptions)
            {
                if (datum.Equals(exception)) isException = true;
                break;
            }
            if (!isException) Select(datum);
        }
    }

    /// <summary>
    /// Unselect all the <see cref="Data"/> except <paramref name="exceptions"/>
    /// </summary>
    /// <param name="exceptions"></param>
    public virtual void UnselectAll(params D[] exceptions)
    {
        foreach (var datum in Data)
        {
            bool isException = false;
            foreach (var exception in exceptions)
            {
                if (datum.Equals(exception)) isException = true;
                break;
            }
            if (!isException) Unselect(datum);
        }
    }

    /// <summary>
    /// Update the size of all items.
    /// </summary>
    protected virtual void UpdateSize()
    {
        foreach (var item in DataToItem.Values)
        {
            var box = item.parent;
            box.style.width = StyleKeyword.Null;
            box.style.height = StyleKeyword.Null;

            switch (Mode)
            {
                case ScrollViewMode.Vertical:
                    if (Size != 0) box.style.height = Size;
                    break;
                case ScrollViewMode.Horizontal:
                    if (Size != 0) box.style.width = Size;
                    break;
                case ScrollViewMode.VerticalAndHorizontal:
                    break;
                default:
                    break;
            }
        }
    }

    protected virtual void UpdateReorderableState()
    {
        if (IsReorderable && Data.Count >= 2)
        {
            foreach (var DataAnditem in DataToItem)
            {
                switch (ReorderableMode)
                {
                    case ReorderableMode.Dragger:
                        AddDragger(DataAnditem.Key, DataAnditem.Value.parent);
                        break;
                    case ReorderableMode.Element:
                        AddDraggerAsElement(DataAnditem.Key, DataAnditem.Value);
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            foreach (var DataAnditem in DataToItem)
            {
                switch (ReorderableMode)
                {
                    case ReorderableMode.Dragger:
                        RemoveDragger(DataAnditem.Value.parent);
                        break;
                    case ReorderableMode.Element:
                        RemoveDraggerAsElement(DataAnditem.Value);
                        break;
                    default:
                        break;
                }
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

            CreateDraggerData(datum, dragger);
        }
        else
        {
            dragger = m_waintingDragger[0];
            m_waintingDragger.RemoveAt(0);

            (dragger.userData as DraggerData).Datum = datum;
        }

        box.Add(dragger);
    }

    protected virtual void AddDraggerAsElement(D datum, VisualElement item)
    {
        if (item.userData == null || item.userData is not DraggerData draggerData)
            CreateDraggerData(datum, item);
        else item.AddManipulator(draggerData.manipulator);
    }

    protected virtual void RemoveDragger(VisualElement box)
    {
        var dragger = box.Query("dragger").Last();
        if (dragger == null) return;
        dragger.RemoveFromHierarchy();
        m_waintingDragger.Add(dragger);
    }

    protected virtual void RemoveDraggerAsElement(VisualElement item)
    {
        if (item.userData == null) return;
        var manipulator = (item.userData as DraggerData).manipulator;
        item.RemoveManipulator(manipulator);
    }

    /// <summary>
    /// 1) create a draggerManipulator.
    /// 2) create a draggerData add add it to <paramref name="dragger"/>.
    /// 3) implement the click down, click up and move phase.
    /// </summary>
    /// <param name="datum"></param>
    /// <param name="dragger"></param>
    /// <returns></returns>
    protected virtual TouchManipulator2 CreateDraggerData(D datum, VisualElement dragger)
    {
        TouchManipulator2 draggerManipulator = new TouchManipulator2();
        dragger.AddManipulator(draggerManipulator);

        dragger.userData = new DraggerData 
        {
            Datum = datum,
            manipulator = draggerManipulator 
        };

        draggerManipulator.ClickedDownWithInfo += (evt, localPosition) =>
        {
            var draggerData = dragger.userData as DraggerData;
            draggerData.startPosition = localPosition;
        };
        draggerManipulator.ClickedUp += () =>
        {
            var draggerData = dragger.userData as DraggerData;
            draggerData.startPosition = Vector2.zero;

            dragger.parent.transform.position = Vector3.zero;
        };
        draggerManipulator.MovedWithInfo += (evt, localPosition) => {
            var draggerData = dragger.userData as DraggerData;
            var box = dragger.parent;

            //Update the position of the box.
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
            box.transform.position = box.transform.position + delta;

            //Reorder the item if needed.
            var draggerDatum = draggerData.Datum;
            var oldindex = Data.IndexOf(draggerDatum);
            float position = 0;
            float previousSize = 0;
            float nextSize = 0;
            switch (Mode)
            {
                case ScrollViewMode.Vertical:
                    position = box.transform.position.y;
                    if (oldindex != 0) previousSize = DataToItem[Data[oldindex - 1]].parent.resolvedStyle.height;
                    if (oldindex != Data.Count - 1) nextSize = DataToItem[Data[oldindex + 1]].parent.resolvedStyle.height;
                    break;
                case ScrollViewMode.Horizontal:
                    position = box.transform.position.x;
                    if (oldindex != 0) previousSize = DataToItem[Data[oldindex - 1]].parent.resolvedStyle.width;
                    if (oldindex != Data.Count - 1) nextSize = DataToItem[Data[oldindex + 1]].parent.resolvedStyle.width;
                    break;
                case ScrollViewMode.VerticalAndHorizontal:
                    break;
                default:
                    break;
            }

            if (oldindex != 0 && Math.Sign(position) < 0 && Math.Abs(position) > previousSize / 2f)
            {
                box.transform.position = Vector3.zero;
                ReorderedDatum(oldindex - 1, draggerDatum);
            }
            else if (oldindex != Data.Count - 1 && Math.Sign(position) > 0 && Math.Abs(position) > nextSize / 2f)
            {
                box.transform.position = Vector3.zero;
                ReorderedDatum(oldindex + 1, draggerDatum);
            }
        };

        return draggerManipulator;
    }

    #endregion
}
