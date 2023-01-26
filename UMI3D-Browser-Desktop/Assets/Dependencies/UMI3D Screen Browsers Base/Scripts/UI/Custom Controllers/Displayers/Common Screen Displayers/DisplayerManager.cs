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
using System.Collections;
using System.Collections.Generic;
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
    public static VisualElement MakeDisplayer(AbstractMenuItem menuItem)
    {
        if (menuItem is ButtonMenuItem) return new umi3d.commonScreen.Displayer.ButtonDisplayer_C();
        if (menuItem is DropDownInputMenuItem) return new umi3d.commonScreen.Displayer.DropdownDisplayer_C();
        if (menuItem is FloatRangeInputMenuItem) return new umi3d.commonScreen.Displayer.SliderDisplayer_C();
        if (menuItem is TextInputMenuItem) return new umi3d.commonScreen.Displayer.TextfieldDisplayer_C();
        if (menuItem is BooleanInputMenuItem) return new umi3d.commonScreen.Displayer.ToggleDisplayer_C();
        //if (menuItem is LocalInfoRequestInputMenuItem) return new umi3d.commonScreen.Displayer.Button_C();

        return null;
    }

    public static void BindItem<M>(M menuItem, VisualElement item) where M : AbstractMenuItem
    {
        if (menuItem is ButtonMenuItem buttonMenuItem && item is IDisplayer<ButtonMenuItem> buttonDisplayer) 
            BindDisplayer(buttonMenuItem, buttonDisplayer);
        if (menuItem is DropDownInputMenuItem dropdownMenuItem && item is IDisplayer<DropDownInputMenuItem> dropdownDisplayer)
            BindDisplayer(dropdownMenuItem, dropdownDisplayer);
        if (menuItem is FloatRangeInputMenuItem sliderMenuItem && item is IDisplayer<FloatRangeInputMenuItem> sliderDisplayer)
            BindDisplayer(sliderMenuItem, sliderDisplayer);
        if (menuItem is TextInputMenuItem textfieldMenuItem && item is IDisplayer<TextInputMenuItem> textfieldDisplayer)
            BindDisplayer(textfieldMenuItem, textfieldDisplayer);
        if (menuItem is BooleanInputMenuItem toggleMenuItem && item is IDisplayer<BooleanInputMenuItem> toggleDisplayer)
            BindDisplayer(toggleMenuItem, toggleDisplayer);
    }

    public static void BindDisplayer<M>(M menuItem, IDisplayer<M> displayer) where M : AbstractMenuItem
    {
        displayer.DisplayerMenu = menuItem;
        displayer.BindDisplayer();
    }

    public static void UnbindItem(VisualElement item)
    {
        if (item is IDisplayer<ButtonMenuItem> buttonDisplayer) UnbindDisplayer(buttonDisplayer);
        if (item is IDisplayer<DropDownInputMenuItem> dropdownDisplayer) UnbindDisplayer(dropdownDisplayer);
        if (item is IDisplayer<FloatRangeInputMenuItem> sliderDisplayer) UnbindDisplayer(sliderDisplayer);
        if (item is IDisplayer<TextInputMenuItem> textfieldDisplayer) UnbindDisplayer(textfieldDisplayer);
        if (item is IDisplayer<BooleanInputMenuItem> toggleDisplayer) UnbindDisplayer(toggleDisplayer);
    }

    public static void UnbindDisplayer<M>(IDisplayer<M> displayer) where M : AbstractMenuItem
    {
        displayer.DisplayerMenu = null;
        displayer.UnbindDisplayer();
    }
}
