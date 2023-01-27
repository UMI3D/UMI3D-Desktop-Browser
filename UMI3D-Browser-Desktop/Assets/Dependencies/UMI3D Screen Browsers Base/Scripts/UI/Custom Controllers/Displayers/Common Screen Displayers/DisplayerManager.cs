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
using System.Windows.Forms;
using umi3d.cdk.menu;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public interface IDisplayer<M> where M: AbstractMenuItem
{
    /// <summary>
    /// MenuItem associated with this displayer. See <typeparamref name="M"/>
    /// </summary>
    M DisplayerMenu { get; set; }
    /// <summary>
    /// Bind this displayer with its menuItem. See <see cref="DisplayerMenu"/>
    /// </summary>
    void BindDisplayer();
    /// <summary>
    /// Unbind this displayer with its menuItem. Basicly reset this Displayer.
    /// </summary>
    void UnbindDisplayer();
}

public static class DisplayerManager
{
    /// <summary>
    /// Make the visual displayer according to <paramref name="menuItem"/>.
    /// </summary>
    /// <param name="menuItem"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="Exception"></exception>
    public static VisualElement MakeDisplayer(AbstractMenuItem menuItem)
    {
        if (menuItem == null) new NullReferenceException($"Menu is null");

        if (menuItem is ButtonMenuItem) return new umi3d.commonScreen.Displayer.ButtonDisplayer_C();
        if (menuItem is DropDownInputMenuItem) return new umi3d.commonScreen.Displayer.DropdownDisplayer_C();
        if (menuItem is FloatRangeInputMenuItem) return new umi3d.commonScreen.Displayer.SliderDisplayer_C();
        if (menuItem is TextInputMenuItem) return new umi3d.commonScreen.Displayer.TextfieldDisplayer_C();
        if (menuItem is BooleanInputMenuItem) return new umi3d.commonScreen.Displayer.ToggleDisplayer_C();


        throw new Exception($"Menu {menuItem.GetType()} is not recognized");
    }

    /// <summary>
    /// Bind the menu <paramref name="menuItem"/> to the visualElement <paramref name="item"/>.
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <param name="menuItem"></param>
    /// <param name="item"></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="Exception"></exception>
    public static void BindItem<M>(M menuItem, VisualElement item) where M : AbstractMenuItem
    {
        if (menuItem == null) new NullReferenceException($"Menu is null");
        if (item == null) new NullReferenceException($"Item is null");

        if (menuItem is ButtonMenuItem buttonMenuItem && item is IDisplayer<ButtonMenuItem> buttonDisplayer) 
            BindDisplayer(buttonMenuItem, buttonDisplayer);
        else if (menuItem is DropDownInputMenuItem dropdownMenuItem && item is IDisplayer<DropDownInputMenuItem> dropdownDisplayer)
            BindDisplayer(dropdownMenuItem, dropdownDisplayer);
        else if (menuItem is FloatRangeInputMenuItem sliderMenuItem && item is IDisplayer<FloatRangeInputMenuItem> sliderDisplayer)
            BindDisplayer(sliderMenuItem, sliderDisplayer);
        else if (menuItem is TextInputMenuItem textfieldMenuItem && item is IDisplayer<TextInputMenuItem> textfieldDisplayer)
            BindDisplayer(textfieldMenuItem, textfieldDisplayer);
        else if (menuItem is BooleanInputMenuItem toggleMenuItem && item is IDisplayer<BooleanInputMenuItem> toggleDisplayer)
            BindDisplayer(toggleMenuItem, toggleDisplayer);
        else 
            throw new Exception($"Menu {menuItem.GetType()} and Item {item.GetType()} are not compatible or are not recognized");
    }

    /// <summary>
    /// Bind the menu <paramref name="menuItem"/> to the displayer <paramref name="displayer"/>.
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <param name="menuItem"></param>
    /// <param name="displayer"></param>
    /// <exception cref="NullReferenceException"></exception>
    public static void BindDisplayer<M>(M menuItem, IDisplayer<M> displayer) where M : AbstractMenuItem
    {
        if (menuItem == null) new NullReferenceException($"Menu is null");
        if (displayer == null) new NullReferenceException($"Displayer is null");

        displayer.DisplayerMenu = menuItem;
        displayer.BindDisplayer();
    }

    /// <summary>
    /// Unbind the visualElement from its menu. Basicly reset this element.
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="Exception"></exception>
    public static void UnbindItem(VisualElement item)
    {
        if (item == null) new NullReferenceException($"Item is null");

        if (item is IDisplayer<ButtonMenuItem> buttonDisplayer) UnbindDisplayer(buttonDisplayer);
        else if (item is IDisplayer<DropDownInputMenuItem> dropdownDisplayer) UnbindDisplayer(dropdownDisplayer);
        else if (item is IDisplayer<FloatRangeInputMenuItem> sliderDisplayer) UnbindDisplayer(sliderDisplayer);
        else if (item is IDisplayer<TextInputMenuItem> textfieldDisplayer) UnbindDisplayer(textfieldDisplayer);
        else if (item is IDisplayer<BooleanInputMenuItem> toggleDisplayer) UnbindDisplayer(toggleDisplayer);
        else 
            throw new Exception($"Item {item.GetType()} is not recognized");
    }

    /// <summary>
    /// Unbind the displayer from its menu. Basicly reset this element.
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <param name="displayer"></param>
    public static void UnbindDisplayer<M>(IDisplayer<M> displayer) where M : AbstractMenuItem
    {
        if (displayer == null) new NullReferenceException($"Item is null");

        displayer.DisplayerMenu = null;
        displayer.UnbindDisplayer();
    }
}
