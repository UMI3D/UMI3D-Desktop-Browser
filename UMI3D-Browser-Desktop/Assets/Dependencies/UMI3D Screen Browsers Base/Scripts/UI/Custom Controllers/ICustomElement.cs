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
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public interface ICustomElement
{
    /// <summary>
    /// Should be call as many times unity need.
    /// </summary>
    void Set();
    /// <summary>
    /// Should be call just once.
    /// </summary>
    void InitElement();
}

public interface IPanelBindable
{
    /// <summary>
    /// Whether or not this element is attached to a panel.
    /// </summary>
    bool IsAttachedToPanel { get; }
    /// <summary>
    /// Method called when this element is attached to a panel.
    /// </summary>
    /// <param name="evt"></param>
    void AttachedToPanel(AttachToPanelEvent evt);
    /// <summary>
    /// Method called when this element is detached from a panel.
    /// </summary>
    /// <param name="evt"></param>
    void DetachedFromPanel(DetachFromPanelEvent evt);
}

public interface ITransitionable
{
    /// <summary>
    /// Whether or not this visual is listening for transition.
    /// </summary>
    bool IsListeningForTransition { get; }
    /// <summary>
    /// Method called when a property of this element will be animated.
    /// </summary>
    /// <param name="evt"></param>
    void TransitionRun(TransitionRunEvent evt);
    /// <summary>
    /// Method called when a property of this element has started to be animated.
    /// </summary>
    /// <param name="evt"></param>
    void TransitionStarted(TransitionStartEvent evt);
    /// <summary>
    /// Method called when a property of this element has finished to be animated. (End properly without being canceled).
    /// </summary>
    /// <param name="evt"></param>
    void TransitionEnded(TransitionEndEvent evt);
    /// <summary>
    /// Method called when a property of this element has finished to be animated. (When canceled).
    /// </summary>
    /// <param name="evt"></param>
    void TransitionCanceled(TransitionCancelEvent evt);
}

public interface IDisplayer
{
    /// <summary>
    /// Height size of this element.
    /// </summary>
    ElementSize HeightSize { get; set; }
    /// <summary>
    /// Width size of this element.
    /// </summary>
    ElementSize WidthSize { get; set; }

    /// <summary>
    /// Direction of the label and the input.
    /// </summary>
    ElemnetDirection LabelAndInputDirection { get; set; }
    /// <summary>
    /// Alignment of the text of the lable.
    /// </summary>
    ElementAlignment LabelAlignment { get; set; }
}

public interface IGameView
{
    void TransitionIn(VisualElement persistentVisual);
    void TransitionOut(VisualElement persistentVisual);
}

public enum ControllerEnum
{
    MouseAndKeyboard,
    Touch,
    GameController
}

public enum ElementPseudoState
{
    Enabled,
    Disabled,
    Hover,
    Active,
    Focus
}
public enum ElementCategory { Menu, Game }
public enum ElementSize { Small, Medium, Large, Custom }
public enum ElemnetDirection { Leading, Trailing, Top, Bottom }
public enum ElementVerticalAlignment { Top, Center, Bottom }
public enum ElementHorizontalAlignment { Leading, Center, Trailing }
public enum ElementAlignment
{
    Top,
    Leading, Center, Trailing,
    Bottom
}

public enum TextStyle
{
    Title,
    LowTitle,
    Subtitle,
    Body,
    Caption
}

public enum TextColor
{
    White,
    Menu,
    Game,
    Custom
}

public enum ButtonType
{
    Default,
    ButtonGroupEnable, // To be added in a button group.
    ButtonGroupSelected, // To be added in a button group.
    Navigation, // To navigate through screens.
    Primary, // To emphasize the highest priority.
    Danger, // To warn that an action is potentially dangerous or destructive.
    Invisible // Only show text of icon
}
public enum ButtonShape { Square, Round }

public enum DialogueboxType
{
    Default,
    Confirmation,
    Error
}

public enum NotificationType
{
    Default,
    Temporary,
    Confirmation,
}

public enum NotificationPriority
{
    All, Low, Medium, High
}

public enum NotificationFilter
{
    All,
    New
}

public enum ToolType
{
    Unknown, //unknown type.
    ToolboxMenu, //A tool that display or hide the ToolsWindow.
    Tool, //A tool display or hide parameters.
    Toolbox //A toolbox display or hide subtools.
}
public enum ToolboxType
{
    Unknown, //unknown type.
    Main, //Toolbox displayed in the toolsWindow and pinned area.
    Sub //Toolbox displayed in as sub toolbox in the toolsWindow.
}

public enum ReorderableMode
{
    Dragger, //Add a dragger to drag&drop the elements.
    Element, //Use the element as a dragger to drag&drop the elements.
}
public enum SelectionType
{
    None, //No item can be selected.
    Single, //Only one item can be selected.
    Multiple //Several items can be selected.
}

public enum ExpansionMode
{
    Vertical,
    Horizontal
}

public static class ElementExtensions
{
    public static string StyleSheetContainersFolderPath => "USS/Containers";
    public static string StyleSheetDisplayersFolderPath => "USS/Displayers";
    public static string StyleSheetMenusFolderPath => "USS/Menus";
    public static string StyleSheetGamesFolderPath => "USS/Games";

    /// <summary>
    /// Display [ve].
    /// </summary>
    public static void Display(this VisualElement ve) => ve.style.display = DisplayStyle.Flex;
    /// <summary>
    /// Hide [ve].
    /// </summary>
    public static void Hide(this VisualElement ve) => ve.style.display = DisplayStyle.None;
    /// <summary>
    /// Get the VisualTreeAsset from [resourcePath] and return the TemplateContainer.
    /// </summary>
    /// <param name="resourcePath"></param>
    /// <returns></returns>
    public static TemplateContainer GetTemplateFromPath(this VisualElement ve, string resourcePath)
    {
        if (string.IsNullOrEmpty(resourcePath)) throw new System.Exception("resourcePath null or empty");
        VisualTreeAsset visualTA = UnityEngine.Resources.Load<VisualTreeAsset>(resourcePath);
        if (visualTA == null) throw new System.NullReferenceException($"[{resourcePath}] return a null visual tree asset.");
        return visualTA.CloneTree();
    }
    /// <summary>
    /// Add the style sheet located at <paramref name="resourcePath"/> to <paramref name="ve"/>
    /// </summary>
    /// <param name="ve"></param>
    /// <param name="resourcePath"></param>
    /// <exception cref="System.Exception"></exception>
    /// <exception cref="System.NullReferenceException"></exception>
    public static void AddStyleSheetFromPath(this VisualElement ve, string resourcePath)
    {
        if (string.IsNullOrEmpty(resourcePath)) throw new System.Exception($"resourcePath null or empty for visual {ve.GetType()}");
        StyleSheet styleSheet = UnityEngine.Resources.Load<StyleSheet>(resourcePath);
        if (styleSheet == null) throw new System.NullReferenceException($"[{resourcePath}] return a null Style Sheet for visual {ve.GetType()}.");
        ve.styleSheets.Add(styleSheet);
    }
    /// <summary>
    /// Clear [this] style classes and copy and add the style classes from [ve] to [this]
    /// </summary>
    /// <param name="from"></param>
    public static void ClearAndCopyStyleClasses(this VisualElement ve, VisualElement from)
    {
        if (from == null) return;
        ve.ClearClassList();
        foreach (var style in from.GetClasses()) ve.AddToClassList(style);
    }
    /// <summary>
    /// Copy and add the style classes from [ve] to [this]
    /// </summary>
    /// <param name="from"></param>
    public static void CopyStyleClasses(this VisualElement ve, VisualElement from)
    {
        if (from == null) return;
        foreach (var style in from.GetClasses()) ve.AddToClassList(style);
    }
    /// <summary>
    /// Remove <paramref name="from"/> from the list of style classes and add <paramref name="to"/>.
    /// </summary>
    /// <param name="ve"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public static void SwitchStyleclasses(this VisualElement ve, string from, string to)
    {
        ve.RemoveFromClassList(from);
        ve.AddToClassList(to);
    }

    /// <summary>
    /// Find the root visualElement of <paramref name="ve"/>. If the root is not found return null.
    /// </summary>
    /// <param name="ve"></param>
    /// <returns></returns>
    public static VisualElement FindRoot(this VisualElement ve)
    {
        var parent = ve;

        while (parent != null && !parent.ClassListContains("unity-ui-document__root"))
            parent = parent.parent;

        return parent;
    }

    /// <summary>
    /// Wait until <paramref name="condition"/> is true before doing <paramref name="action"/>.
    /// </summary>
    /// <param name="ve"></param>
    /// <param name="condition"></param>
    /// <param name="action"></param>
    public static IVisualElementScheduledItem WaitUntil(this VisualElement ve, System.Func<bool> condition, System.Action action)
    {
        if (condition())
        {
            action();
            return null;
        }

        var scheduleItem = ve.schedule.Execute(() =>
        {
            if (!condition()) return;
            action();
        }).Until(condition);

        return scheduleItem;
    }

    /// <summary>
    /// Try to get the length <paramref name="value"/> custom style with the name <paramref name="propertyName"/> from <paramref name="ve"/>.
    /// </summary>
    /// <param name="ve"></param>
    /// <param name="propertyName"></param>
    /// <param name="value"></param>
    public static void TryGetCustomStyle(this VisualElement ve, string propertyName, out Length value)
    {
        ve.customStyle.TryGetValue(new CustomStyleProperty<string>(propertyName), out var stringValue);
        try
        {
            if (stringValue.Contains('%'))
            {
                value = Length.Percent(float.Parse(stringValue.Substring(0, stringValue.Length - 1)));
                return;
            }
            if (stringValue.EndsWith("px"))
            {
                value = float.Parse(stringValue.Substring(0, stringValue.Length - 2));
                return;
            }

            throw new System.Exception($"--try get custom style \"{propertyName}\"-- \nCustom style \"{stringValue}\" is not a length.");
        }
        catch (System.Exception e)
        {
            throw e;
        }
    }

    /// <summary>
    /// Add <paramref name="child"/> visualElement to <paramref name="parent"/> if <paramref name="parent"/> doesn't contain already <paramref name="child"/>.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="child"></param>
    public static void AddIfNotInHierarchy(this VisualElement parent, VisualElement child)
    {
        if (parent.Contains(child)) return;
        parent.Add(child);
    }

    /// <summary>
    /// Insert <paramref name="child"/> visualElement to <paramref name="parent"/> at index <paramref name="index"/> if <paramref name="parent"/> doesn't contain already <paramref name="child"/>.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="index"></param>
    /// <param name="child"></param>
    public static void InsertIfNotInHierarchy(this VisualElement parent, int index, VisualElement child)
    {
        if (parent.Contains(child)) return;
        parent.Insert(index, child);
    }

    /// <summary>
    /// Remove <paramref name="ve"/> from the hierarchy if <paramref name="ve"/> is in the hierarchy.
    /// </summary>
    /// <param name="ve"></param>
    public static void RemoveIfIsInHierarchy(this VisualElement ve)
    {
        if (ve.parent == null) return; 
        ve.RemoveFromHierarchy();
    }

    /// <summary>
    /// Whether or not this visual can be consider as listening for transition.
    /// </summary>
    /// <remarks>To be sure if this visual is listening for transition check <see cref="ITransitionable.IsListeningForTransition"/></remarks>
    /// <param name="ve"></param>
    /// <returns></returns>
    public static bool CanBeConsiderAsListeningForTransition(this VisualElement ve)
        => !float.IsNaN(ve.resolvedStyle.width) 
            && !float.IsNaN(ve.resolvedStyle.height);
}

public static class FloatExtentions
{
    /// <summary>
    /// Whether or not <paramref name="f1"/> equals <paramref name="f2"/> with an approximation of <paramref name="e"/>.
    /// </summary>
    /// <param name="f1"></param>
    /// <param name="f2"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public static bool EqualsEpsilone(this float f1, float f2, float e = .01f)
        => Mathf.Abs(f1 - f2) <= e;
}
