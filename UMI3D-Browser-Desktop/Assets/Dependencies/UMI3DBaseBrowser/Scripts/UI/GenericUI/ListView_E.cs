/*
Copyright 2019 - 2021 Inetum

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
using umi3DBrowser.UICustomStyle;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.ui.viewController
{
    public class ListView_item_E : View_E
    {
        protected bool m_isBinded = false;

        public virtual void Bind(VisualElement element)
        {
            if (m_isBinded) Unbind(element);
            m_isBinded = true;
        }
        public virtual void Unbind(VisualElement element) => m_isBinded = false;
    }


    public partial class ListView_E<Item> where Item : ListView_item_E
    {

        List<Item> items;
        int ItemHeight = 50;
        Func<VisualElement> makeFunction;

        public ListView List_View { get; protected set; } = null;


        public virtual void AddRange(params Item[] items)
        {
            this.items.AddRange(items);
            List_View.itemsSource = this.items;
            List_View.Refresh();
        }

        /// <summary>
        /// Clear the scrollview.
        /// </summary>
        public virtual void Clear()
        {
            this.items.Clear();
            List_View.itemsSource = this.items;
            List_View.Refresh();
        }
    }

    public partial class ListView_E<Item> : View_E
    {
        #region Constructors

        public ListView_E() :
            this(new ListView(),0,null)
        { }
        public ListView_E(ListView scrollView,int itemHeight,Func<VisualElement> make) :
            this(scrollView, null, null,itemHeight,make)
        { }

        public ListView_E(ListView scrollView, string partialStylePath, StyleKeys keys, int itemHeight, Func<VisualElement> make) :
            base(scrollView, partialStylePath, keys)
        {
            this.ItemHeight = itemHeight;
            List_View.itemHeight = itemHeight;
            this.makeFunction = make;
            this.items = new List<Item>();
        }
        public ListView_E(string visualResourcePath, int itemHeight, Func<VisualElement> make) :
            this(visualResourcePath, null, null, itemHeight, make)
        { }

        public ListView_E(string visualResourcePath, string partialStylePath, StyleKeys keys, int itemHeight, Func<VisualElement> make) :
            base(visualResourcePath, partialStylePath, keys)
        {
            this.ItemHeight = itemHeight;
            List_View.itemHeight = itemHeight;
            this.makeFunction = make;
            this.items = new List<Item>();
        }

        #endregion

        public override void Add(View_E child)
        {
        }

        public override void Insert(int index, View_E item)
        {
        }


        public override void Remove(View_E item)
        {
        }

        public void Add(Item item) 
        { 
            items.Add(item);
            List_View.Refresh();
        }
        public void Insert(int index, Item item) {
            items.Insert(index,item);
            List_View.Refresh();
        }

        public void Remove(Item item)
        {
            if(items.Remove(item))
                List_View.Refresh();
        }

        public override void Reset()
        {
            base.Reset();
            Clear();
        }

        protected override void Initialize()
        {
            base.Initialize();
            List_View = Root.Q<ListView>();
            List_View.bindItem = Bind;
            List_View.unbindItem = Unbind;
            List_View.itemHeight = ItemHeight;
            List_View.itemsSource = items;
            List_View.makeItem = Make;
            List_View.Refresh();
        }

        void Bind(VisualElement element,int index) {
            items[index].Bind(element);
        }
        void Unbind(VisualElement element, int index) {
            items[index].Unbind(element);
        }
        VisualElement Make()
        {
            return makeFunction?.Invoke() ?? new VisualElement();
        }

        protected override CustomStyle_SO GetStyleSO(string resourcePath)
        {
            var path = (resourcePath == null) ? null : $"UI/Style/Scrollviews/{resourcePath}";
            return base.GetStyleSO(path);
        }
    }
}

