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
using umi3d.commonScreen;
using UnityEngine;
using UnityEngine.UIElements;

public static class AnimatorManager
{
    public class Animation
    {
        public VisualElement Visual;

        /// <summary>
        /// Is this animation currently playing.
        /// </summary>
        public bool IsPlaying;
        /// <summary>
        /// Is this animation animated.
        /// </summary>
        public bool IsAnimating;
        /// <summary>
        /// Name of the property to animate.
        /// </summary>
        public StylePropertyName PropertyName;
        /// <summary>
        /// Duration time of the previous animation that has been canceled. (in second).
        /// </summary>
        public float PreviousDuration;
        /// <summary>
        /// Duration time of this animation. (in second).
        /// </summary>
        public float Duration;
        /// <summary>
        /// Easing mode of this animation.
        /// </summary>
        public EasingFunction EasingMode;
        /// <summary>
        /// Delay before playing this animation. (In second).
        /// </summary>
        public TimeValue Delay;
        /// <summary>
        /// Action that will be used to set the initila value.
        /// </summary>
        public Action SetInitialValue;
        /// <summary>
        /// Action that will be used to set the end value.
        /// </summary>
        public Action SetEndValue;
        /// <summary>
        /// Check if current value equals to end value.
        /// </summary>
        public Func<bool> IsCurrentValueEqualToEndValue;
        /// <summary>
        /// Action raised just before playing this animation.
        /// </summary>
        public Action Callin;
        /// <summary>
        /// Action raised just after playing this animation. (If this animaiton was not canceled).
        /// </summary>
        public Action Callback;
        /// <summary>
        /// Action raised just after playin this animation. (If this animaiton was canceled).
        /// </summary>
        public Action Callcancel;
        /// <summary>
        /// Action raised just after playin this animation. (If this animaiton was not canceled).
        /// </summary>
        public IVisualElementScheduledItem ScheduledCallback;
        /// <summary>
        /// Action raised just after playin this animation. (If this animaiton was canceled).
        /// </summary>
        public IVisualElementScheduledItem ScheduledCallcancel;
        /// <summary>
        /// Whether or not this animation should play when <see cref="ReduceAnimation"/> is true.
        /// </summary>
        public bool IsForcedAnimation;
        /// <summary>
        /// The Coroutine that set the initial value.
        /// </summary>
        public Coroutine InitialValueCoroutine;
        /// <summary>
        /// The Coroutine that set the end value.
        /// </summary>
        public Coroutine EndValueCoroutine;
    }

    public class AnimationSet: List<Animation> { }

    /// <summary>
    /// Whether or not the animations should play.
    /// </summary>
    public static bool ReduceAnimation;
    public const float NavigationScreenDuration = 0.3f;
    public const float DropdownDuration = 0.5f;
    public const float MainDuration = 1f;
    public const float TextFadeDuration = 1f;

    public static Dictionary<VisualElement, List<Animation>> Animations = new Dictionary<VisualElement, List<Animation>>();

    /// <summary>
    /// Remove <paramref name="animation"/> from the list of animations.
    /// </summary>
    /// <param name="ve"></param>
    /// <param name="animation"></param>
    public static void RemoveAnimation(this VisualElement ve, Animation animation)
    {
        if (!Animations.TryGetValue(ve, out var animations)) return;

        if (!animations.Contains(animation)) return;

        animations.Remove(animation);

        ve.UpdateTransitionList(animations);

        if (animations.Count == 0) Animations.Remove(ve);
    }

    /// <summary>
    /// Remove the animation with the name <paramref name="propertyName"/> from the list of animation.
    /// </summary>
    /// <param name="ve"></param>
    /// <param name="propertyName"></param>
    public static void RemoveAnimation(this VisualElement ve, StylePropertyName propertyName)
    {
        if (!Animations.TryGetValue(ve, out var animations)) return;

        if (!animations.Exists(animation => animation.PropertyName == propertyName)) return;
        var animation = animations.Find(animation => animation.PropertyName == propertyName);
        ve.RemoveAnimation(animation);
    }

    /// <summary>
    /// Whether or not animaitons are waitting to be played.
    /// </summary>
    /// <param name="ve"></param>
    /// <returns></returns>
    public static bool AreAnimationsWaiting(this VisualElement ve)
    {
        if (!Animations.TryGetValue(ve, out var animations)) return false;
        if (animations.Count == 0) return false;
        else return true;
    }

    /// <summary>
    /// Play all the animations that are waitting.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    public static void PlayAllAnimations<T>(this T ve)
        where T : VisualElement, IPanelBindable, ITransitionable
    {
        if (!Animations.TryGetValue(ve, out var animations)) return;

        if (!ve.IsListeningForTransition) return;
        for (int i = animations.Count - 1; i >= 0; i--)
        {
            if (animations[i].InitialValueCoroutine != null) UIManager.StopCoroutine(animations[i].InitialValueCoroutine);
            animations[i].InitialValueCoroutine = UIManager.StartCoroutine(ve.PlayAnimation(animations[i]));
        }
    }

    /// <summary>
    /// Play <paramref name="animation"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="animation"></param>
    /// <param name="isNew"></param>
    public static IEnumerator PlayAnimation<T>(this T ve, Animation animation, bool isNew = true)
        where T : VisualElement, IPanelBindable, ITransitionable
    {
        yield return null;

        if (!ve.IsListeningForTransition) yield break;

        // Raise the callin action. This action should not update the property that is animated.
        animation.Callin?.Invoke();

        if (ReduceAnimation && !animation.IsForcedAnimation)
        {
            // Play this action without animation and call the callback
            animation.SetEndValue?.Invoke();
            animation.ScheduledCallback?.Resume();
            yield break;
        }

        animation.SetInitialValue?.Invoke();

        // Do not start end value coroutine if the previous animation was animated.
        if (isNew || animation.PreviousDuration == 0)
        {
            animation.IsPlaying = true;

            animation.EndValueCoroutine = UIManager.StartCoroutine(ve.WaitOneFrameAndSetEndValue(animation));
        }
    }

    private static IEnumerator WaitOneFrameAndSetEndValue(this VisualElement ve, Animation animation)
    {
        yield return null;
        if (!Animations.ContainsKey(ve)) yield break;

        ve.UpdateTransitionList(Animations[ve]);

        var isEndValueEqualToEnd = animation.IsCurrentValueEqualToEndValue();

        // Set the end value.
        // If the current value is equal to the end value then no need to play the animaiton.
        if (!isEndValueEqualToEnd) animation.SetEndValue?.Invoke();
        // If the current value is equal to the end value or if it is not animated then callback.
        if (isEndValueEqualToEnd || !animation.IsAnimating) animation.ScheduledCallback?.Resume();
    }

    /// <summary>
    /// Trigger the animation callback with the name <paramref name="property"/>
    /// </summary>
    /// <param name="ve"></param>
    /// <param name="property"></param>
    public static void TriggerAnimationCallback(this VisualElement ve, StylePropertyName property)
    {
        if (!Animations.TryGetValue(ve, out var animations)) return;

        var animation = animations.Find(_animation => _animation.PropertyName == property);

        if (animation.InitialValueCoroutine != null) UIManager.StopCoroutine(animation.InitialValueCoroutine);
        if (animation.EndValueCoroutine != null) UIManager.StopCoroutine(animation.EndValueCoroutine);
        animation.ScheduledCallback?.Resume();
    }

    /// <summary>
    /// Trigger the animation callcancel with the name <paramref name="property"/>.
    /// </summary>
    /// <param name="ve"></param>
    /// <param name="property"></param>
    public static void TriggerAnimationCallcancel(this VisualElement ve, StylePropertyName property, TransitionCancelEvent evt)
    {
        if (!Animations.TryGetValue(ve, out var animations)) return;

        var animation = animations.Find(_animation => _animation.PropertyName == property);
        animation.ScheduledCallcancel?.Resume();

        if (animation.PreviousDuration != 0)
        {
            float previousDurationPercentage = (float)evt.elapsedTime * 100f / animation.PreviousDuration;
            animation.Duration = previousDurationPercentage * animation.Duration / 100f;
        }

        if (animation.EndValueCoroutine != null) UIManager.StopCoroutine(animation.EndValueCoroutine);
        animation.EndValueCoroutine = UIManager.StartCoroutine(ve.WaitOneFrameAndSetEndValue(animation));
    }

    private static void InsertAnimationInAnimationsList(this VisualElement ve, StylePropertyName propertyName, out Animation animation, out bool isNew, out bool isAnimated)
    {
        if (!Animations.TryGetValue(ve, out var animations))
        {
            animation = new Animation
            {
                Visual = ve,
                PropertyName = propertyName
            };

            Animations.Add(ve, new List<Animation> { animation });
            isNew = true;
            isAnimated = false;
        }
        else
        {
            for (int i = 0; i < animations.Count; ++i)
            {
                if (animations[i].PropertyName != propertyName) continue;

                isAnimated = animations[i].IsAnimating;
                isNew = isAnimated && !animations[i].IsPlaying;
                animation = animations[i];
                return;
            }

            animation = new Animation
            {
                Visual = ve,
                PropertyName = propertyName
            };
            animations.Add(animation);
            isNew = true;
            isAnimated = false;
        }
    }

    private static void UpdateTransitionList(this VisualElement ve, List<Animation> animations)
    {
        if (animations == null) return;

        var properties = new List<StylePropertyName>();
        var durations = new List<TimeValue>();
        var easingModes = new List<EasingFunction>();
        var delays = new List<TimeValue>();
        foreach (var animation_ in animations)
        {
            properties.Add(animation_.PropertyName);
            durations.Add(animation_.Duration);
            easingModes.Add(animation_.EasingMode);
            delays.Add(animation_.Delay);
        }

        ve.style.transitionProperty = properties;
        ve.style.transitionDuration = durations;
        ve.style.transitionTimingFunction = easingModes;
        ve.style.transitionDelay = delays;
    }

    #region Set Properties

    public enum BorderColorAndWidthEnum { All, Left, Right, Top, Bottom }
    public enum BorderRadiusEnum { All, TopLeft, TopRight, BottomLeft, BottomRight }
    public enum MarginAndPaddingEnum { All, Left, Right, Top, Bottom }

    private static Animation AddAnimation<T>
    (
        this T ve,
        Action setInitialValue,
        Action setEndValue,
        Func<bool> isCurrentValueEqualToEndValue,
        StylePropertyName propertyName
    ) where T : VisualElement, IPanelBindable, ITransitionable
    {
        ve.InsertAnimationInAnimationsList(propertyName, out Animation animation, out bool isNew, out bool isAnimated);

        animation.IsAnimating = false;
        animation.PreviousDuration = animation.Duration;
        animation.Duration = 0;
        animation.EasingMode = EasingMode.EaseInOut;
        animation.Delay = 0;

        if (animation.InitialValueCoroutine != null) UIManager.StopCoroutine(animation.InitialValueCoroutine);
        if (animation.EndValueCoroutine != null) UIManager.StopCoroutine(animation.EndValueCoroutine);

        // Is new if this animation was not previously in the list or if the previous animation was animated and not playing.
        if (isNew)
        {
            var scheduledItemBack = ve.schedule.Execute(() =>
            {
                animation.Callback?.Invoke();
                ve.RemoveAnimation(animation);
                animation.ScheduledCallback.Pause();
            });
            // Will be resume when animation end event will be trigger.
            scheduledItemBack.Pause();
            animation.ScheduledCallback = scheduledItemBack;

            var scheduledItemCancel = ve.schedule.Execute(() =>
            {
                animation.Callcancel?.Invoke();
                animation.ScheduledCallcancel.Pause();
            });
            // Will be resume when animation end event will be trigger.
            scheduledItemCancel.Pause();
            animation.ScheduledCallcancel = scheduledItemCancel;

            animation.InitialValueCoroutine = UIManager.StartCoroutine(ve.PlayAnimation(animation, isNew));
        }
        else
        {
            if (!isAnimated)
            {
                animation.Callin?.Invoke();
                animation.SetEndValue?.Invoke();
                animation.Callcancel?.Invoke();
                animation.Callback?.Invoke();

                animation.InitialValueCoroutine = UIManager.StartCoroutine(ve.PlayAnimation(animation, isNew));
            }
            else
            {
                ve.UpdateTransitionList(Animations[ve]);
                setInitialValue?.Invoke();
            }
        }

        animation.SetInitialValue = setInitialValue;
        animation.SetEndValue = setEndValue;
        animation.IsCurrentValueEqualToEndValue = isCurrentValueEqualToEndValue;
        animation.Callin = null;
        animation.Callcancel = null;
        animation.Callback = null;

        return animation;
    }

    #region Modificators

    /// <summary>
    /// Set the animation properties.
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="duration"></param>
    /// <param name="easingMode"></param>
    /// <returns></returns>
    public static Animation WithAnimation(this Animation animation, float duration = 1, EasingMode easingMode = EasingMode.EaseInOut)
    {
        animation.IsAnimating = true;
        animation.Duration = duration;
        animation.EasingMode = easingMode;

        return animation;
    }

    /// <summary>
    /// Set the call in action.
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="callin"></param>
    /// <returns></returns>
    public static Animation SetCallin(this Animation animation, Action callin)
    {
        animation.Callin = callin;
        return animation;
    }

    /// <summary>
    /// Set the call cancel action.
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="callcancel"></param>
    /// <returns></returns>
    public static Animation SetCallcancel(this Animation animation, Action callcancel)
    {
        animation.Callcancel = callcancel;
        return animation;
    }

    /// <summary>
    /// Set the call back action.
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static Animation SetCallback(this Animation animation, Action callback)
    {
        animation.Callback = callback;
        return animation;
    }

    #endregion

    /// <summary>
    /// Set the opacity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="opacity"></param>
    /// <returns></returns>
    public static Animation SetOpacity<T>(this T ve, StyleFloat opacity)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () => ve.style.opacity = ve.resolvedStyle.opacity,
            setEndValue: () => ve.style.opacity = opacity,
            isCurrentValueEqualToEndValue: () => ve.style.opacity == opacity,
            propertyName: "opacity"
        );

    #region Color

    /// <summary>
    /// Set the background color.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Animation SetBackgroundColor<T>(this T ve, Color color)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () => ve.style.backgroundColor = ve.resolvedStyle.backgroundColor,
            setEndValue: () => ve.style.backgroundColor = color,
            isCurrentValueEqualToEndValue: () => ve.style.backgroundColor == color,
            propertyName: "background-color"
        );

    /// <summary>
    /// Set the text color.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Animation SetColor<T>(this T ve, Color color)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () => ve.style.color = ve.resolvedStyle.color,
            setEndValue: () => ve.style.color = color,
            isCurrentValueEqualToEndValue: () => ve.style.color == color,
            propertyName: "color"
        );

    #endregion

    #region Border color

    /// <summary>
    /// Set the border Color of <paramref name="border"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="color"></param>
    /// <param name="border"></param>
    /// <returns></returns>
    public static AnimationSet SetBorderColor<T>(this T ve, Color color, BorderColorAndWidthEnum border)
        where T : VisualElement, IPanelBindable, ITransitionable
    {
        var animations = new AnimationSet();
        switch (border)
        {
            case BorderColorAndWidthEnum.All:
                animations.Add(ve.SetBorderLeftColor(color));
                animations.Add(ve.SetBorderRightColor(color));
                animations.Add(ve.SetBorderTopColor(color));
                animations.Add(ve.SetBorderBottomColor(color));
                break;
            case BorderColorAndWidthEnum.Left:
                animations.Add(ve.SetBorderLeftColor(color));
                break;
            case BorderColorAndWidthEnum.Right:
                animations.Add(ve.SetBorderRightColor(color));
                break;
            case BorderColorAndWidthEnum.Top:
                animations.Add(ve.SetBorderTopColor(color));
                break;
            case BorderColorAndWidthEnum.Bottom:
                animations.Add(ve.SetBorderBottomColor(color));
                break;
            default:
                break;
        }
        return animations;
    }

    private static Animation SetBorderLeftColor<T>(this T ve, Color color)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () => ve.style.borderLeftColor = ve.resolvedStyle.borderLeftColor,
            setEndValue: () => ve.style.borderLeftColor = color,
            isCurrentValueEqualToEndValue: () => ve.style.borderLeftColor == color,
            propertyName: "border-left-color"
        );

    private static Animation SetBorderRightColor<T>(this T ve, Color color)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () => ve.style.borderRightColor = ve.resolvedStyle.borderRightColor,
            setEndValue: () => ve.style.borderRightColor = color,
            isCurrentValueEqualToEndValue: () => ve.style.borderRightColor == color,
            propertyName: "border-right-color"
        );

    private static Animation SetBorderTopColor<T>(this T ve, Color color)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () => ve.style.borderTopColor = ve.resolvedStyle.borderTopColor,
            setEndValue: () => ve.style.borderTopColor = color,
            isCurrentValueEqualToEndValue: () => ve.style.borderTopColor == color,
            propertyName: "border-top-color"
        );

    private static Animation SetBorderBottomColor<T>(this T ve, Color color)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () => ve.style.borderBottomColor = ve.resolvedStyle.borderBottomColor,
            setEndValue: () => ve.style.borderBottomColor = color,
            isCurrentValueEqualToEndValue: () => ve.style.borderBottomColor == color,
            propertyName: "border-bottom-color"
        );

    #endregion

    #region Border width

    /// <summary>
    /// Set the border width of <paramref name="border"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="length"></param>
    /// <param name="border"></param>
    /// <returns></returns>
    public static AnimationSet SetBorderWidth<T>(this T ve, StyleFloat length, BorderColorAndWidthEnum border)
        where T : VisualElement, IPanelBindable, ITransitionable
    {
        var animations = new AnimationSet();
        switch (border)
        {
            case BorderColorAndWidthEnum.All:
                animations.Add(ve.SetBorderLeftWidth(length));
                animations.Add(ve.SetBorderRightWidth(length));
                animations.Add(ve.SetBorderTopWidth(length));
                animations.Add(ve.SetBorderBottomWidth(length));
                break;
            case BorderColorAndWidthEnum.Left:
                animations.Add(ve.SetBorderLeftWidth(length));
                break;
            case BorderColorAndWidthEnum.Right:
                animations.Add(ve.SetBorderRightWidth(length));
                break;
            case BorderColorAndWidthEnum.Top:
                animations.Add(ve.SetBorderTopWidth(length));
                break;
            case BorderColorAndWidthEnum.Bottom:
                animations.Add(ve.SetBorderBottomWidth(length));
                break;
            default:
                break;
        }
        return animations;
    }

    private static Animation SetBorderLeftWidth<T>(this T ve, StyleFloat length)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () => ve.style.borderLeftWidth = ve.resolvedStyle.borderLeftWidth,
            setEndValue: () => ve.style.borderLeftWidth = length,
            isCurrentValueEqualToEndValue: () => ve.style.borderLeftWidth == length,
            propertyName: "border-left-width"
        );

    private static Animation SetBorderRightWidth<T>(this T ve, StyleFloat length)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () => ve.style.borderRightWidth = ve.resolvedStyle.borderRightWidth,
            setEndValue: () => ve.style.borderRightWidth = length,
            isCurrentValueEqualToEndValue: () => ve.style.borderRightWidth == length,
            propertyName: "border-right-width"
        );

    private static Animation SetBorderTopWidth<T>(this T ve, StyleFloat length)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () => ve.style.borderTopWidth = ve.resolvedStyle.borderTopWidth,
            setEndValue: () => ve.style.borderTopWidth = length,
            isCurrentValueEqualToEndValue: () => ve.style.borderTopWidth == length,
            propertyName: "border-top-width"
        );

    private static Animation SetBorderBottomWidth<T>(this T ve, StyleFloat length)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () => ve.style.borderBottomWidth = ve.resolvedStyle.borderBottomWidth,
            setEndValue: () => ve.style.borderBottomWidth = length,
            isCurrentValueEqualToEndValue: () => ve.style.borderBottomWidth == length,
            propertyName: "border-bottom-width"
        );

    #endregion

    #region Border radius

    /// <summary>
    /// Set the border radius of <paramref name="border"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="length"></param>
    /// <param name="border"></param>
    /// <returns></returns>
    public static AnimationSet SetBorderRadius<T>(this T ve, StyleLength length, BorderRadiusEnum border)
        where T : VisualElement, IPanelBindable, ITransitionable
    {
        var animations = new AnimationSet();
        switch (border)
        {
            case BorderRadiusEnum.All:
                animations.Add(ve.SetBorderTopLeftRadius(length));
                animations.Add(ve.SetBorderTopRightRadius(length));
                animations.Add(ve.SetBorderBottomLeftRadius(length));
                animations.Add(ve.SetBorderBottomRightRadius(length));
                break;
            case BorderRadiusEnum.TopLeft:
                animations.Add(ve.SetBorderTopLeftRadius(length));
                break;
            case BorderRadiusEnum.TopRight:
                animations.Add(ve.SetBorderTopRightRadius(length));
                break;
            case BorderRadiusEnum.BottomLeft:
                animations.Add(ve.SetBorderBottomLeftRadius(length));
                break;
            case BorderRadiusEnum.BottomRight:
                animations.Add(ve.SetBorderBottomRightRadius(length));
                break;
            default:
                break;
        }
        return animations;
    }

    private static Animation SetBorderTopLeftRadius<T>(this T ve, StyleLength length)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () => ve.style.borderTopLeftRadius = ve.resolvedStyle.borderTopLeftRadius,
            setEndValue: () => ve.style.borderTopLeftRadius = length,
            isCurrentValueEqualToEndValue: () => ve.style.borderTopLeftRadius == length,
            propertyName: "border-top-left-radius"
        );

    private static Animation SetBorderTopRightRadius<T>(this T ve, StyleLength length)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () => ve.style.borderTopRightRadius = ve.resolvedStyle.borderTopRightRadius,
            setEndValue: () => ve.style.borderTopRightRadius = length,
            isCurrentValueEqualToEndValue: () => ve.style.borderTopRightRadius == length,
            propertyName: "border-top-right-radius"
        );

    private static Animation SetBorderBottomLeftRadius<T>(this T ve, StyleLength length)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () => ve.style.borderBottomLeftRadius = ve.resolvedStyle.borderBottomLeftRadius,
            setEndValue: () => ve.style.borderBottomLeftRadius = length,
            isCurrentValueEqualToEndValue: () => ve.style.borderBottomLeftRadius == length,
            propertyName: "border-bottom-left-radius"
        );

    private static Animation SetBorderBottomRightRadius<T>(this T ve, StyleLength length)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () => ve.style.borderBottomRightRadius = ve.resolvedStyle.borderBottomRightRadius,
            setEndValue: () => ve.style.borderBottomRightRadius = length,
            isCurrentValueEqualToEndValue: () => ve.style.borderBottomRightRadius == length,
            propertyName: "border-top-right-radius"
        );

    #endregion

    #region Size

    /// <summary>
    /// Set the width.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public static Animation SetWidth<T>(this T ve, StyleLength width)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () =>
            {
                if (width.value.unit == LengthUnit.Pixel) ve.style.width = ve.resolvedStyle.width;
                else
                {
                    var parentWidth = ve.parent.resolvedStyle.width;
                    var currentWidth = ve.resolvedStyle.width;

                    ve.style.width = Length.Percent(currentWidth * 100f / parentWidth);
                }
            },
            setEndValue: () => ve.style.width = width,
            isCurrentValueEqualToEndValue: () => ve.style.width == width,
            propertyName: "width"
        );

    /// <summary>
    /// Set the height.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Animation SetHeight<T>(this T ve, StyleLength height)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () =>
            {
                if (height.value.unit == LengthUnit.Pixel) ve.style.height = ve.resolvedStyle.height;
                else
                {
                    var parentHeight = ve.parent.resolvedStyle.height;
                    var currentHeight = ve.resolvedStyle.height;

                    ve.style.height = Length.Percent(currentHeight * 100f / parentHeight);
                }
            },
            setEndValue: () => ve.style.height = height,
            isCurrentValueEqualToEndValue: () => ve.style.height == height,
            propertyName: "height"
        );

    #endregion

    #region Position

    /// <summary>
    /// Set the left position.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="left"></param>
    /// <returns></returns>
    public static Animation SetLeft<T>(this T ve, StyleLength left)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () =>
            {
                if (left.value.unit == LengthUnit.Pixel) ve.style.left = ve.resolvedStyle.left;
                else
                {
                    var parentWidth = ve.parent.resolvedStyle.width;
                    var currentLeft = ve.resolvedStyle.left;

                    ve.style.left = Length.Percent(currentLeft * 100f / parentWidth);
                }
            },
            setEndValue: () => ve.style.left = left,
            isCurrentValueEqualToEndValue: () => ve.style.left == left,
            propertyName: "left"
        );

    /// <summary>
    /// Set the right position.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Animation SetRight<T>(this T ve, StyleLength right)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () =>
            {
                if (right.value.unit == LengthUnit.Pixel) ve.style.right = ve.resolvedStyle.right;
                else
                {
                    var parentWidth = ve.parent.resolvedStyle.width;
                    var currentRight = ve.resolvedStyle.right;

                    ve.style.right = Length.Percent(currentRight * 100f / parentWidth);
                }
            },
            setEndValue: () => ve.style.right = right,
            isCurrentValueEqualToEndValue: () => ve.style.right == right,
            propertyName: "right"
        );

    /// <summary>
    /// Set the top position.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="top"></param>
    /// <returns></returns>
    public static Animation SetTop<T>(this T ve, StyleLength top)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () =>
            {
                if (top.value.unit == LengthUnit.Pixel) ve.style.top = ve.resolvedStyle.top;
                else
                {
                    var parentHeight = ve.parent.resolvedStyle.height;
                    var currentTop = ve.resolvedStyle.top;

                    ve.style.top = Length.Percent(currentTop * 100f / parentHeight);
                }
            },
            setEndValue: () => ve.style.top = top,
            isCurrentValueEqualToEndValue: () => ve.style.top == top,
            propertyName: "top"
        );

    /// <summary>
    /// Set the bottom position.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="bottom"></param>
    /// <returns></returns>
    public static Animation SetBottom<T>(this T ve, StyleLength bottom)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () =>
            {
                if (bottom.value.unit == LengthUnit.Pixel) ve.style.bottom = ve.resolvedStyle.bottom;
                else
                {
                    var parentHeight = ve.parent.resolvedStyle.height;
                    var currentBottom = ve.resolvedStyle.bottom;

                    ve.style.bottom = Length.Percent(currentBottom * 100f / parentHeight);
                }
            },
            setEndValue: () => ve.style.bottom = bottom,
            isCurrentValueEqualToEndValue: () => ve.style.bottom == bottom,
            propertyName: "bottom"
        );

    #endregion

    #region Margin & Padding

    /// <summary>
    /// Set the margin of <paramref name="margin"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="length"></param>
    /// <param name="margin"></param>
    /// <returns></returns>
    public static AnimationSet SetMargin<T>(this T ve, StyleLength length, MarginAndPaddingEnum margin)
        where T : VisualElement, IPanelBindable, ITransitionable
    {
        var animations = new AnimationSet();
        switch (margin)
        {
            case MarginAndPaddingEnum.All:
                animations.Add(ve.SetMarginLeft(length));
                animations.Add(ve.SetMarginRight(length));
                animations.Add(ve.SetMarginTop(length));
                animations.Add(ve.SetMarginBottom(length));
                break;
            case MarginAndPaddingEnum.Left:
                animations.Add(ve.SetMarginLeft(length));
                break;
            case MarginAndPaddingEnum.Right:
                animations.Add(ve.SetMarginRight(length));
                break;
            case MarginAndPaddingEnum.Top:
                animations.Add(ve.SetMarginTop(length));
                break;
            case MarginAndPaddingEnum.Bottom:
                animations.Add(ve.SetMarginBottom(length));
                break;
            default:
                break;
        }
        return animations;
    }

    private static Animation SetMarginLeft<T>(this T ve, StyleLength length)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () =>
            {
                if (length.value.unit == LengthUnit.Pixel) ve.style.marginLeft = ve.resolvedStyle.marginLeft;
                else
                {
                    var parentWidth = ve.parent.resolvedStyle.width;
                    var currentMargLeft = ve.resolvedStyle.marginLeft;

                    ve.style.marginLeft = Length.Percent(currentMargLeft * 100f / parentWidth);
                }
            },
            setEndValue: () => ve.style.marginLeft = length,
            isCurrentValueEqualToEndValue: () => ve.style.marginLeft == length,
            propertyName: "margin-left"
        );

    private static Animation SetMarginRight<T>(this T ve, StyleLength length)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () =>
            {
                if (length.value.unit == LengthUnit.Pixel) ve.style.marginRight = ve.resolvedStyle.marginRight;
                else
                {
                    var parentWidth = ve.parent.resolvedStyle.width;
                    var currentMargRight = ve.resolvedStyle.marginRight;

                    ve.style.marginRight = Length.Percent(currentMargRight * 100f / parentWidth);
                }
            },
            setEndValue: () => ve.style.marginRight = length,
            isCurrentValueEqualToEndValue: () => ve.style.marginRight == length,
            propertyName: "margin-right"
        );

    private static Animation SetMarginTop<T>(this T ve, StyleLength length)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () =>
            {
                if (length.value.unit == LengthUnit.Pixel) ve.style.marginTop = ve.resolvedStyle.marginTop;
                else
                {
                    var parentHeigth = ve.parent.resolvedStyle.height;
                    var currentMargTop = ve.resolvedStyle.marginTop;

                    ve.style.marginTop = Length.Percent(currentMargTop * 100f / parentHeigth);
                }
            },
            setEndValue: () => ve.style.marginTop = length,
            isCurrentValueEqualToEndValue: () => ve.style.marginTop == length,
            propertyName: "margin-top"
        );

    private static Animation SetMarginBottom<T>(this T ve, StyleLength length)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () =>
            {
                if (length.value.unit == LengthUnit.Pixel) ve.style.marginBottom = ve.resolvedStyle.marginBottom;
                else
                {
                    var parentHeigth = ve.parent.resolvedStyle.height;
                    var currentMargBottom = ve.resolvedStyle.marginBottom;

                    ve.style.marginBottom = Length.Percent(currentMargBottom * 100f / parentHeigth);
                }
            },
            setEndValue: () => ve.style.marginBottom = length,
            isCurrentValueEqualToEndValue: () => ve.style.marginBottom == length,
            propertyName: "margin-bottom"
        );

    #endregion

    #region Rotate & Scale

    /// <summary>
    /// Set the rotate property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="rotate"></param>
    /// <returns></returns>
    public static Animation SetRotate<T>(this T ve, StyleRotate rotate)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () =>
            {
                switch (rotate.value.angle.unit)
                {
                    case AngleUnit.Degree:
                        ve.style.rotate = ve.resolvedStyle.rotate;
                        break;
                    case AngleUnit.Gradian:
                        UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"convert gradian to degree");
                        break;
                    case AngleUnit.Radian:
                        UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"convert radian to degree");
                        break;
                    case AngleUnit.Turn:
                        break;
                    default:
                        break;
                }
            },
            setEndValue: () => ve.style.rotate = rotate,
            isCurrentValueEqualToEndValue: () => ve.style.rotate == rotate,
            propertyName: "rotate"
        );

    /// <summary>
    /// Set the scale property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public static Animation SetScale<T>(this T ve, StyleScale scale)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () => ve.style.scale = ve.resolvedStyle.scale,
            setEndValue: () => ve.style.scale = scale,
            isCurrentValueEqualToEndValue: () => ve.style.scale == scale,
            propertyName: "scale"
        );
    /// <summary>
    /// Set the scale property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public static Animation SetScale<T>(this T ve, Vector3 scale)
        where T : VisualElement, IPanelBindable, ITransitionable
        => ve.AddAnimation
        (
            setInitialValue: () => ve.style.scale = ve.resolvedStyle.scale,
            setEndValue: () => ve.style.scale = new Scale(scale),
            isCurrentValueEqualToEndValue: () => ve.style.scale == new Scale(scale),
            propertyName: "scale"
        );

    #endregion

    #endregion
}
