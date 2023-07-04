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
using System.Linq;
using umi3d.commonScreen.Displayer;

public abstract class OldCustomCarrousel : VisualElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlEnumAttributeDescription<ElementCategory> m_category = new UxmlEnumAttributeDescription<ElementCategory> 
        { 
            name = "category", 
            defaultValue = ElementCategory.Menu 
        };

        protected UxmlIntAttributeDescription m_nb_elts = new UxmlIntAttributeDescription
        {
            name = "number-of-elements",
            defaultValue = 3
        };

        protected UxmlFloatAttributeDescription m_min_margin = new UxmlFloatAttributeDescription
        {
            name = "min-margin",
            defaultValue = 0
        };
 
        protected UxmlEnumAttributeDescription<EltSize> m_elt_size = new UxmlEnumAttributeDescription<EltSize> 
        { 
            name = "elements-size", 
            defaultValue = EltSize.Maximum
        };

        protected UxmlBoolAttributeDescription m_scroll_all = new UxmlBoolAttributeDescription
        {
            name = "scroll-all",
            defaultValue = true
        };
        
        protected UxmlBoolAttributeDescription m_loop_scroll = new UxmlBoolAttributeDescription
        {
            name = "loop-scroll",
            defaultValue = false
        };
        
        protected UxmlIntAttributeDescription m_auto_scroll = new UxmlIntAttributeDescription
        {
            name = "speed-auto-scroll",
            defaultValue = 0
        };
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;
            base.Init(ve, bag, cc);
            var custom = ve as OldCustomCarrousel;

            custom.Set
                (
                    m_category.GetValueFromBag(bag, cc),
                    m_nb_elts.GetValueFromBag(bag, cc),
                    m_min_margin.GetValueFromBag(bag, cc),
                    m_elt_size.GetValueFromBag(bag, cc),
                    m_scroll_all.GetValueFromBag(bag, cc),
                    m_loop_scroll.GetValueFromBag(bag, cc),
                    m_auto_scroll.GetValueFromBag(bag, cc)
                );
        }
    }

    public virtual string StyleSheetContainerPath => $"USS/container";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetContainersFolderPath}/carrousel";
    public virtual string USSCustomClassName => "carrousel";
    public virtual string USSCustomClassButton => $"{USSCustomClassName}-button";
    public virtual string USSCustomClassContainerBox => $"{USSCustomClassName}-container_box";
    public virtual string USSCustomClassBoxElts => $"{USSCustomClassName}-box_elts";
    public virtual string USSCustomClassBottomButtons => $"{USSCustomClassName}-bottom_buttons";
    public virtual string USSCustomClassPrevIcon => $"{USSCustomClassName}-prev_icon";
    public virtual string USSCustomClassNextIcon => $"{USSCustomClassName}-next_icon";
    public virtual string USSCustomClassSelectHover => $"{USSCustomClassName}-select_hover";
    public virtual string USSCustomClassSelectActive => $"{USSCustomClassName}-select_active";

    public virtual ElementCategory Category
    {
        get => m_category;
        set
        {
            m_category = value;
            Prev.Category = value;
            Next.Category = value;
        }
    }

    public virtual int NbElts
    {
        get => m_nb_elts;
        set
        {
            if (value == 0 || Carrousel_Elts.Count <= 0) return;
            m_nb_elts = value;
            CalculateSizes(m_nb_elts);
        }
    }

    public virtual float MinMargin
    {
        get => m_min_margin;
        set
        {
            m_min_margin = value;
            CalculateSizes(m_nb_elts);
        }
    }

    public virtual EltSize ElementsSize
    {
        get => m_elt_size;
        set
        {
            m_elt_size = value;
            CalculateSizes(m_nb_elts);
        }
    }

    public virtual bool ScrollAll
    {
        get => m_scroll_all;
        set => m_scroll_all = value;
    }

    public virtual bool LoopScroll
    {
        get => m_loop_scroll;
        set => m_loop_scroll = value;
    }

    public virtual int AutoScroll
    {
        get => m_auto_scroll;
        set
        {
            m_auto_scroll = value;
            if (value != 0 && !is_auto_scrolling)
                ScrollAuto();
            if (value == 0)
                is_auto_scrolling = false;
        }
    }

    protected ElementCategory m_category;
    protected int m_nb_elts;
    protected float m_min_margin;
    protected EltSize m_elt_size;
    protected bool m_scroll_all;
    protected bool m_loop_scroll;
    protected int m_auto_scroll;
    protected bool m_hasBeenInitialized;

    protected int curr_elt;
    protected int wanted_elt;
    protected int curr_page;
    protected int nb_pages;
    protected bool is_auto_scrolling;
    public enum EltSize
    {
        Minimun,
        Maximum
    }
    protected List<float> sizesElts = new List<float>();
    protected List<VisualElement> Carrousel_Elts = new List<VisualElement>();

    public Button_C Prev = new Button_C { name = "precedent" };
    public VisualElement Prev_Icon = new VisualElement();
    public VisualElement Container_Box = new VisualElement();
    public VisualElement Box_Elts = new VisualElement();
    public VisualElement Bottom_Buttons = new VisualElement();
    public VisualElement PageSelectIcon = new VisualElement();
    public Button_C Next = new Button_C { name = "next" };
    public VisualElement Next_Icon = new VisualElement();

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
        Next.AddToClassList(USSCustomClassButton);
        Next_Icon.AddToClassList(USSCustomClassNextIcon);
        PageSelectIcon.AddToClassList(USSCustomClassSelectActive);
        Container_Box.AddToClassList(USSCustomClassContainerBox);
        Box_Elts.AddToClassList(USSCustomClassBoxElts);
        Prev_Icon.AddToClassList(USSCustomClassPrevIcon);
        Prev.AddToClassList(USSCustomClassButton);
        Bottom_Buttons.AddToClassList(USSCustomClassBottomButtons);
        AddToClassList(USSCustomClassName);

        Add(Prev);
        Prev.ClickedUp += () => ChangeScrollPrev(m_scroll_all ? -m_nb_elts : -1);
        Prev.Type = ButtonType.Invisible;
        Prev.Add(Prev_Icon);
        Add(Container_Box);
        Container_Box.Add(Box_Elts);
        Container_Box.Add(Bottom_Buttons);
        Bottom_Buttons.Add(PageSelectIcon);
        Add(Next);
        Next.ClickedUp += () => ChangeScrollNext(m_scroll_all ? m_nb_elts : 1);
        Next.Type = ButtonType.Invisible;
        Next.Add(Next_Icon);

        this.RegisterCallback<GeometryChangedEvent>((ec) =>
        {
            CalculateSizes(m_nb_elts);
        });
    }

    public virtual void Set() => Set(ElementCategory.Menu, m_nb_elts, m_min_margin,  m_elt_size, m_scroll_all, m_loop_scroll, m_auto_scroll);

    public virtual void Set(ElementCategory category, int nb_elts, float min_margin, EltSize elt_size, bool scroll_all, bool loop_scroll, int auto_scroll)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }
        
        Category = category;
        NbElts = nb_elts;
        MinMargin = min_margin;
        ElementsSize = elt_size;
        ScrollAll = scroll_all;
        LoopScroll = loop_scroll;
        AutoScroll = auto_scroll;
    }

    public void InsertElement(int index, VisualElement child)
    {
        Carrousel_Elts.Insert(index, child);
        CalculateSizes(m_nb_elts);
    }

    protected void CalculateSizeElts()
    {
        this.WaitUntil(
            () => !float.IsNaN(Carrousel_Elts.ElementAt(0).layout.size.x),
            () =>
            {
                if (Box_Elts.childCount == 0) return;
                
                var max_len = curr_elt + Box_Elts.childCount > Carrousel_Elts.Count ? Carrousel_Elts.Count - curr_elt : Box_Elts.childCount;
                var size_elts_curr = Carrousel_Elts.GetRange(curr_elt, max_len).Select(elt => elt.layout.size.x);
                if (curr_elt + Box_Elts.childCount > Carrousel_Elts.Count)
                    size_elts_curr = size_elts_curr.Concat(Carrousel_Elts.GetRange(0, Box_Elts.childCount - max_len).Select(elt => elt.layout.size.x));
                var size_max = size_elts_curr.Max();
                var size_min = size_elts_curr.Min();

                foreach (var child in Box_Elts.Children())
                {
                    if (m_elt_size == EltSize.Maximum)
                        child.style.width = size_max;
                    else
                        child.style.width = size_min;
                }
            });
    }

    protected void CalculateSizes(int nb_elt)
    {
        this.WaitUntil(
            () => !float.IsNaN(Box_Elts.layout.size.x),
            () => {
                if (Box_Elts.childCount == 0) return;
                if (nb_elt > Carrousel_Elts.Count) nb_elt = Carrousel_Elts.Count;
                while (!m_loop_scroll && !m_scroll_all && nb_elt + curr_elt > Carrousel_Elts.Count) 
                {
                    curr_elt--;
                    Box_Elts.Insert(0, Carrousel_Elts.ElementAt(curr_elt));
                }
                for (var toSupp = nb_elt; toSupp < Box_Elts.childCount; toSupp++)
                    Box_Elts.RemoveAt(toSupp);
                for (var toAdd = curr_elt + Box_Elts.childCount; toAdd < curr_elt + nb_elt; toAdd++)
                    Box_Elts.Add(Carrousel_Elts.ElementAt(toAdd));
                CalculateSizeElts();

                if (Box_Elts.childCount == 0) return;
                var size1elt = Box_Elts.layout.size.x / nb_elt;
                var width_elt = Box_Elts.ElementAt(wanted_elt - curr_elt).layout.size.x;

                if (size1elt < width_elt + m_min_margin * 2)
                {
                    width_elt = size1elt - m_min_margin * 2;
                    foreach(var child in Box_Elts.Children())
                        child.style.width = width_elt;
                }
                else
                {
                    foreach(var child in Box_Elts.Children())
                        child.style.width = StyleKeyword.Null;
                    width_elt = Box_Elts.ElementAt(0).layout.size.x;
                }

                var curr_margin = size1elt - width_elt;
                ChangeMarginAllElements(curr_margin/2);

                if (nb_elt == 0) return;
                var new_curr_page = Mathf.CeilToInt((float)curr_elt / m_nb_elts);
                nb_pages = Mathf.CeilToInt((float)Carrousel_Elts.Count /  m_nb_elts);
                var prev_button = m_scroll_all ? curr_page : Bottom_Buttons.IndexOf(PageSelectIcon);
                var curr_button = m_scroll_all ? new_curr_page : curr_elt;
                var nb_buttons = m_scroll_all ? nb_pages : Carrousel_Elts.Count;

                for (var i = 0; i < prev_button; i++)
                    Bottom_Buttons.RemoveAt(0);
                var tmp_size_box = Bottom_Buttons.childCount;
                for (var i = 1; i < tmp_size_box; i++)
                    Bottom_Buttons.RemoveAt(1);

                for (var i = 0; i < curr_button; i++)
                {
                    var new_page = new VisualElement();
                    new_page.AddToClassList(USSCustomClassSelectHover);
                    var tmp_i = i;
                    new_page.RegisterCallback<ClickEvent>((ce) => ChangeScrollPrev(m_scroll_all ? - m_nb_elts * (new_curr_page - tmp_i) : -curr_elt + tmp_i));
                    Bottom_Buttons.Insert(i, new_page);
                }
                for (var i = curr_button + 1; i < nb_buttons; i++)
                {
                    var new_page = new VisualElement();
                    new_page.AddToClassList(USSCustomClassSelectHover);
                    var tmp_i = i;
                    new_page.RegisterCallback<ClickEvent>((ce) => ChangeScrollNext(m_scroll_all ?  m_nb_elts * (tmp_i - new_curr_page) : tmp_i - curr_elt));
                    Bottom_Buttons.Insert(Bottom_Buttons.childCount, new_page);
                }
                curr_page = new_curr_page;
            }
        );
    }

    protected void ChangeMarginAllElements(float margin)
    {
        foreach (var elt in Box_Elts.Children())
        {
            elt.style.marginLeft = margin;
            elt.style.marginRight = margin;
        }
    }

    protected void ChangeScrollPrev(int new_elt)
    {
        if (!m_loop_scroll)
            new_elt = curr_elt + new_elt < 0 ? -curr_elt : new_elt;
        if (m_scroll_all && curr_elt == 0)
        {
            if (m_loop_scroll)
                ChangeScrollNext((nb_pages - 1) * m_nb_elts);
            return;
        }
        
        if (new_elt == 0) return;

        var border_max = Box_Elts.childCount >= -new_elt ? new_elt : -Box_Elts.childCount;
        for (var toSupp = 0; toSupp > border_max; toSupp--)
            Box_Elts.RemoveAt(Box_Elts.childCount - 1);
        
        var modulo = (curr_elt + new_elt + Carrousel_Elts.Count) % Carrousel_Elts.Count;
        curr_elt = modulo;
        wanted_elt = curr_elt;

        var border_min = -new_elt > m_nb_elts ? -m_nb_elts : new_elt;
        for (var toAdd = border_min + 1; toAdd <= 0; toAdd++)
            Box_Elts.Insert(0, Carrousel_Elts.ElementAt((curr_elt - toAdd) % Carrousel_Elts.Count));

        CalculateSizes(Box_Elts.childCount);
    }

    protected void ChangeScrollNext(int new_elt)
    {
        if (!m_loop_scroll)
            new_elt = curr_elt + new_elt > m_nb_elts-1 ? curr_elt : new_elt;
        if (m_scroll_all && curr_elt == m_nb_elts-1)
        {
            if (m_loop_scroll)
                ChangeScrollPrev(-(nb_pages - 1) * m_nb_elts);
            return;
        }
        if (new_elt == 0) return;

        var border_max = Box_Elts.childCount >= new_elt ? new_elt : Box_Elts.childCount;
        for (var toSupp = 0; toSupp < border_max; toSupp++)
            Box_Elts.RemoveAt(0);
        
        var modulo = (curr_elt + new_elt + Carrousel_Elts.Count) % Carrousel_Elts.Count;
        curr_elt = modulo;
        wanted_elt = curr_elt;

        border_max = border_max > m_nb_elts ? m_nb_elts : border_max;
        for (var toAdd = 0; toAdd < border_max; toAdd++)
            Box_Elts.Add(Carrousel_Elts.ElementAt((curr_elt + toAdd) % Carrousel_Elts.Count));
                
        CalculateSizes(Box_Elts.childCount);
    }

    protected void ScrollAuto()
    {
        is_auto_scrolling = true;
        var initial = System.DateTime.Now;
        this.schedule.Execute(() =>
        {
            if (m_auto_scroll == 0 || (System.DateTime.Now - initial).TotalSeconds < m_auto_scroll) return;
            m_loop_scroll = true;
            ChangeScrollNext(m_scroll_all ? Box_Elts.childCount : 1);
            initial = System.DateTime.Now;
        }).Until(() => (System.DateTime.Now - initial).TotalSeconds > m_auto_scroll);;
        
    }
}
