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
using umi3d.baseBrowser.ui.viewController;
using umi3d.commonScreen;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseDataCollection_C<D> : BaseVisual_C
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
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

        protected UxmlFloatAttributeDescription m_size = new UxmlFloatAttributeDescription
        {
            name = "size",
            defaultValue = 0
        };

        protected UxmlBoolAttributeDescription m_isReorderable = new UxmlBoolAttributeDescription
        {
            name = "is-reorderable",
            defaultValue = false
        };

        protected UxmlEnumAttributeDescription<ReorderableMode> m_reorderableMode = new UxmlEnumAttributeDescription<ReorderableMode>
        {
            name = "reorderable-mode",
            defaultValue = ReorderableMode.Dragger
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
            var custom = ve as BaseDataCollection_C<D>;

            custom.Mode = m_ScrollViewMode.GetValueFromBag(bag, cc);
            custom.SelectionType = m_selectionType.GetValueFromBag(bag, cc);
            custom.Size = m_size.GetValueFromBag(bag, cc);
            custom.IsReorderable = m_isReorderable.GetValueFromBag(bag, cc);
            custom.ReorderableMode = m_reorderableMode.GetValueFromBag(bag, cc);
        }
    }

    /// <summary>
    /// Direction in wich the items are added.
    /// </summary>
    public virtual ScrollViewMode Mode
    {
        get => m_mode;
        set
        {
            RemoveFromClassList(USSCustomClassMode(m_mode));
            AddToClassList(USSCustomClassMode(value));
            UpdateMode(m_mode, value);
            m_mode = value;
            UpdateSize();
        }
    }
    /// <summary>
    /// Mode of seletion see: <see cref="SelectionType.SelectionType"/>
    /// </summary>
    public virtual SelectionType SelectionType
    {
        get => m_selectionType;
        set
        {
            m_selectionType = value;
            //TODO update selected data collection when updated.
        }
    }
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

    public virtual string USSCustomClassMode(ScrollViewMode mode) => $"{UssCustomClass_Emc}-{mode}".ToLower();
    public virtual string USSCustomClassBox => $"{UssCustomClass_Emc}-box";
    public virtual string USSCustomClassDragger => $"{UssCustomClass_Emc}-dragger";

    /// <summary>
    /// The VisualElement that contains the data.
    /// </summary>
    public abstract VisualElement DataContainer { get; }

    protected ScrollViewMode m_mode;
    protected SelectionType m_selectionType;
    protected float m_size;
    protected bool m_isReorderable;
    protected ReorderableMode m_reorderableMode;

    #region Implementation

    protected class DraggerData
    {
        public D Datum;
        public VisualElement Dragger;
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

    /// <summary>
    /// Event raised when a new item is added.
    /// </summary>
    public event Action<D> ItemAdded;
    /// <summary>
    /// Event raised when an item is removed.
    /// </summary>
    public event Action<D> ItemRemoved;
    /// <summary>
    /// Event raised when a new item is added or when an item is removed. 
    /// <para>Param: nb of items after change.</para>
    /// </summary>
    public event Action<int> ContentChanged;

    /// <summary>
    /// Make a VisualElement to add to the collection.
    /// </summary>
    public virtual Func<D, VisualElement> MakeItem { get; set; }

    /// <summary>
    /// Predicate use to find available item matching this datum.
    /// </summary>
    public virtual Predicate<(D, VisualElement)> FindItem { get; set; }

    /// <summary>
    /// Bind a datum to a visualElement. This action is called after the datum is added to the collection
    /// </summary>
    public virtual Action<D, VisualElement> BindItem { get; set; }

    /// <summary>
    /// Unbind a visualElement from its datum. This action is called before the datum is removed from the collection.
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
    /// Whether or not this collection is being cleared. When true calling <see cref="ClearDC"/> will do nothing.
    /// </summary>
    public virtual bool IsBeingCleared { get; protected set; }
    /// <summary>
    /// Whether or not all selected item are being unselected. When true calling <see cref="UnselectAll(D[])"/> and <see cref="Unselect(List{D})"/> will do nothing.
    /// </summary>
    public bool IsBeingUnselected { get; set; }

    /// <summary>
    /// Add this <paramref name="datum"/> at the end of the collection of data and make an item and add it to the view.
    /// </summary>
    /// <param name="datum"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    public virtual void AddDatum(D datum)
        => InsertAt(Data == null ? 0 : Data.Count, datum);

    /// <summary>
    /// Insert this <paramref name="datum"/> at <paramref name="index"/> in the collection of data and make an item and add it to the view.
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
                    AddDragger(datum, item);
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

        DataContainer.Insert(index, box);

        if (Data.Count == 2) UpdateReorderableState();

        BindItem(datum, item);

        ItemAdded?.Invoke(datum);
        ContentChanged?.Invoke(Data.Count);
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

        box.RemoveFromHierarchy();
        m_waintingBoxes.Add(box);

        RemoveDragger(box);

        UnbindItem(datum, item);
        item.RemoveFromHierarchy();
        m_waitingItems.Add(item);

        DataToItem.Remove(datum);
        Data.Remove(datum);
        SelectedData.Remove(datum);

        ItemRemoved?.Invoke(datum);
        ContentChanged?.Invoke(Data.Count);
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
        DataContainer.Insert(indexNew, DataToItem[datum].parent);
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
    /// Clear data, remove item from <see cref="DataContainer"/>, unbind item.
    /// </summary>
    /// <remarks>Before clearing make sure to not have side effect in the <see cref="UnselectItem"/> action.</remarks>
    public virtual void ClearDC()
    {
        if (IsBeingCleared) return;

        IsBeingCleared = true;

        UnselectAll();
        for (int i = Data.Count - 1; i >= 0; i--)
            RemoveDatum(Data[i]);

        IsBeingCleared = false;
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

        SelectedData.Add(datum);

        SelectItem?.Invoke(datum, DataToItem[datum]);
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
        if (IsBeingUnselected) return;

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
        if (IsBeingUnselected || SelectedData.Count == 0) return;

        IsBeingUnselected = true;

        foreach (var datum in Data)
        {
            if (!SelectedData.Contains(datum)) continue;
            bool isException = false;
            foreach (var exception in exceptions)
            {
                if (datum.Equals(exception)) isException = true;
                break;
            }
            if (!isException) Unselect(datum);
        }

        IsBeingUnselected = false;
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
        if (Data.Count < 2)
        {
            RemoveAllDragger();
            return;
        }

        switch (ReorderableMode)
        {
            case ReorderableMode.Dragger:
                RemoveAllDragger();
                if (IsReorderable) AddAllDragger();
                break;
            case ReorderableMode.Element:
                RemoveAllDragger();
                if (IsReorderable) AddAllDraggerAsElement();
                break;
            default:
                break;
        }
    }

    protected virtual void AddDragger(D datum, VisualElement item)
    {
        if (item == null) throw new NullReferenceException($"{item} is null");

        VisualElement dragger;
        if (m_waintingDragger.Count == 0)
        {
            dragger = new VisualElement { name = "dragger" };
            dragger.AddToClassList(USSCustomClassDragger);
        }
        else
        {
            dragger = m_waintingDragger[0];
            m_waintingDragger.RemoveAt(0);
        }

        var box = item.parent;
        if (box == null) throw new NullReferenceException($"Box, parent of {item} is null");
        GetOrCreateDraggerData(datum, box, dragger, out TouchManipulator2 manipulator);

        dragger.AddManipulator(manipulator);
        box.Add(dragger);
    }

    protected virtual void AddDraggerAsElement(D datum, VisualElement item)
    {
        if (item == null) throw new NullReferenceException($"{item} is null");

        var box = item.parent;
        if (box == null) throw new NullReferenceException($"Box, parent of {item} is null");
        GetOrCreateDraggerData(datum, box, item, out TouchManipulator2 manipulator);

        item.AddManipulator(manipulator);
    }

    protected virtual void GetOrCreateDraggerData(D datum, VisualElement box, VisualElement dragger, out TouchManipulator2 manipulator)
    {
        if (box.userData == null || box.userData is not DraggerData draggerData)
            manipulator = CreateDraggerData(datum, box, dragger);
        else
        {
            manipulator = draggerData.manipulator;
            draggerData.Datum = datum;
            draggerData.Dragger = dragger;
        }
    }

    protected virtual void RemoveDragger(VisualElement box)
    {
        var dragger = _RemoveDragger(box);

        if (dragger == null || dragger.name != "dragger") return;
        dragger.RemoveFromHierarchy();
        m_waintingDragger.Add(dragger);
    }

    protected virtual VisualElement _RemoveDragger(VisualElement box)
    {
        if (box == null) throw new NullReferenceException($"Box null when removing dragger");

        var draggerData = box.userData as DraggerData;
        if (draggerData == null) return null;

        var dragger = draggerData.Dragger;
        
        dragger.RemoveManipulator(draggerData.manipulator);
        
        return dragger;
    }

    protected virtual void AddAllDragger()
    {
        foreach (var DataAnditem in DataToItem) AddDragger(DataAnditem.Key, DataAnditem.Value);
    }

    protected virtual void AddAllDraggerAsElement()
    {
        foreach (var DataAnditem in DataToItem) AddDraggerAsElement(DataAnditem.Key, DataAnditem.Value);
    }

    protected virtual void RemoveAllDragger()
    {
        foreach (var DataAnditem in DataToItem) RemoveDragger(DataAnditem.Value.parent);
    }

    /// <summary>
    /// 1) create a draggerManipulator.
    /// 2) create a draggerData add add it to <paramref name="box"/>.
    /// 3) implement the click down, click up and move phase.
    /// </summary>
    /// <param name="datum"></param>
    /// <param name="box"></param>
    /// <returns></returns>
    protected virtual TouchManipulator2 CreateDraggerData(D datum, VisualElement box, VisualElement dragger)
    {
        TouchManipulator2 draggerManipulator = new TouchManipulator2();

        box.userData = new DraggerData
        {
            Datum = datum,
            Dragger = dragger,
            manipulator = draggerManipulator
        };

        draggerManipulator.ClickedDownWithInfo += (evt, localPosition) =>
        {
            var draggerData = box.userData as DraggerData;
            draggerData.startPosition = localPosition;
        };
        draggerManipulator.ClickedUp += () =>
        {
            var draggerData = box.userData as DraggerData;
            draggerData.startPosition = Vector2.zero;

            box.transform.position = Vector3.zero;
        };
        draggerManipulator.MovedWithInfo += (evt, localPosition) => {
            var draggerData = box.userData as DraggerData;

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

    protected virtual void UpdateMode(ScrollViewMode from, ScrollViewMode to)
    {

    }

    #endregion
}
