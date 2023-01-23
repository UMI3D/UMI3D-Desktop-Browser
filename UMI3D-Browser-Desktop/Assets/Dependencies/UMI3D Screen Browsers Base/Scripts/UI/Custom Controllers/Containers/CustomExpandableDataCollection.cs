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
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomExpandableDataCollection<D> : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlEnumAttributeDescription<ExpansionMode> m_expansionMode = new UxmlEnumAttributeDescription<ExpansionMode>
        {
            name = "mode",
            defaultValue = ExpansionMode.Vertical
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomExpandableDataCollection<D>;

            custom.Set
                (
                    m_expansionMode.GetValueFromBag(bag, cc)
                 );
        }
    }

    public virtual ExpansionMode Mode
    {
        get => m_mode;
        set
        {
            RemoveFromClassList(USSCustomClassMode(m_mode));
            AddToClassList(USSCustomClassMode(value));
            m_mode = value;
        }
    }

    public virtual string StyleSheetContainerPath => $"USS/container";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetContainersFolderPath}/expandableDataCollection";
    public virtual string USSCustomClassName => "edc";
    public virtual string USSCustomClassMode(ExpansionMode mode) => $"{USSCustomClassName}-{mode}".ToLower();
    public virtual string USSCustomClassBox => $"{USSCustomClassName}-box";

    protected bool m_hasBeenInitialized;
    protected ExpansionMode m_mode;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
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
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual void Set() => Set(ExpansionMode.Vertical);

    /// <summary>
    /// Set this UI element.
    /// </summary>
    /// <param name="mode"></param>
    public virtual void Set(ExpansionMode mode)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Mode = mode;
    }

    #region Implementation

    /// <summary>
    /// Collection of the data.
    /// </summary>
    public List<D> Data = new List<D>();
    /// <summary>
    /// Collection of the Data and its corresponding visualElement.
    /// </summary>
    public Dictionary<D, VisualElement> DataToItem = new Dictionary<D, VisualElement>();

    protected List<VisualElement> m_waitingItems = new List<VisualElement>();
    protected List<VisualElement> m_waintingBoxes = new List<VisualElement>();

    /// <summary>
    /// Make a VisualElement to add to this container.
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
    /// Define the size of this visualElement.
    /// </summary>
    public virtual Action<ExpansionMode, D, VisualElement> DefineSize { get; set; }

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
        DefineSize?.Invoke(Mode, datum, box);
        box.Add(item);

        Data.Insert(index, datum);
        DataToItem.Add(datum, item);

        //ScrollView.Insert(index, box);
    }

    protected virtual void UpdateSize()
    {
        foreach (var item in DataToItem) DefineSize?.Invoke(Mode, item.Key, item.Value.parent);
    }

    #endregion
}
