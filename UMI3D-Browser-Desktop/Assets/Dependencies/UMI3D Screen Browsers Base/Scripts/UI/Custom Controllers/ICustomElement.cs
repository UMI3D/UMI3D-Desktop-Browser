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
    Main, //Toolbox displayed in the toolsWindow.
    Pinned //Toolbox displayed in the pinnedToolsArea.
}
public enum ReorderableMode
{
    Dragger, //Add a dragger to drag&drop the elements.
    Element, //Use the element as a dragger to drag&drop the elements.
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
    public static void AddStyleSheetFromPath(this VisualElement ve, string resourcePath)
    {
        if (string.IsNullOrEmpty(resourcePath)) throw new System.Exception("resourcePath null or empty");
        StyleSheet styleSheet = UnityEngine.Resources.Load<StyleSheet>(resourcePath);
        if (styleSheet == null) throw new System.NullReferenceException($"[{resourcePath}] return a null Style Sheet.");
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

    public static VisualElement FindRoot(this VisualElement ve)
    {
        var parent = ve;

        while (parent != null && !parent.ClassListContains("unity-ui-document__root"))
            parent = parent.parent;

        return parent;
    }

    public static void WaitUntil(this VisualElement ve, System.Func<bool> condition, System.Action action)
    {
        if (condition())
        {
            action();
            return;
        }

        ve.schedule.Execute(() =>
        {
            if (!condition()) return;
            action();
        }).Until(condition);
    }

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

    public static void AddIfNotInHierarchy(this VisualElement parent, VisualElement child)
    {
        if (parent.Contains(child)) return;
        parent.Add(child);
    }

    public static void InsertIfNotInHierarchy(this VisualElement parent, int index, VisualElement child)
    {
        if (parent.Contains(child)) return;
        parent.Insert(index, child);
    }
}
