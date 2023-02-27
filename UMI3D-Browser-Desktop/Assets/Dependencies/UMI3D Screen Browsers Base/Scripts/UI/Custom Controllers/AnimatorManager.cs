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
        /// Name of the property to animate.
        /// </summary>
        public StylePropertyName PropertyName;
        /// <summary>
        /// Duration time of this animation. (In second).
        /// </summary>
        public TimeValue Duration;
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
        /// Action raised just before playing this animation.
        /// </summary>
        public Action Callin;
        /// <summary>
        /// Action raised just after playin this animation. (If this animaiton was not canceled).
        /// </summary>
        public IVisualElementScheduledItem Callback;
        /// <summary>
        /// Action raised just after playin this animation. (If this animaiton was canceled).
        /// </summary>
        public IVisualElementScheduledItem Callcancel;
        /// <summary>
        /// Whether or not this initial and end values should be inverted.
        /// </summary>
        public bool IsReverted;
        /// <summary>
        /// Whether or not this animation should play when <see cref="ReduceAnimation"/> is true.
        /// </summary>
        public bool IsForcedAnimation;
        public Coroutine Coroutine;

        /// <summary>
        /// Copy animation properties from <paramref name="other"/> to this.
        /// </summary>
        /// <param name="other"></param>
        public void Copy(Animation other)
        {
            Duration = other.Duration;
            EasingMode = other.EasingMode;
            Delay = other.Delay;
            SetInitialValue = other.SetInitialValue;
            SetEndValue = other.SetEndValue;
            Callin = other.Callin;
            Callback = other.Callback;
            Callcancel = other.Callcancel;
            IsReverted = other.IsReverted;
            IsForcedAnimation = other.IsForcedAnimation;
            Coroutine = other.Coroutine;
        }
    }

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
    /// Add an animation.
    /// </summary>
    /// <param name="ve">The visual that will have an animation.</param>
    /// <param name="persistentVisual">The visual that should be attached to a panel until the end of the animation.</param>
    /// <param name="setInitialValue">The initial value.</param>
    /// <param name="setEndValue">The end value.</param>
    /// <param name="propertyName">Name of the property that will be animated.</param>
    /// <param name="duration">Duration of the animation, in second.</param>
    /// <param name="easingMode">Animation easing mode.</param>
    /// <param name="delay">Delay before playing the animation, in second.</param>
    /// <param name="callin">Action raised just before playing this animation.</param>
    /// <param name="callcancel">Action raised just after playing this animation. (If this animaiton was canceled)</param>
    /// <param name="callback">Callback raised when the animation end. This callback is raised only when the animation end properly.</param>
    /// <param name="forceAnimation">Whether or not playing this animation when <see cref="ReduceAnimation"/> is true.</param>
    /// <param name="revert">Should the animation be played reverted.</param>
    public static void AddAnimation<T>
    (
        this T ve,
        VisualElement persistentVisual,
        Action setInitialValue, 
        Action setEndValue, 
        StylePropertyName propertyName, 
        TimeValue duration,
        EasingMode easingMode = EasingMode.EaseInOut, 
        TimeValue delay = new TimeValue(), 
        Action callin = null,
        Action callback = null, 
        Action callcancel = null,
        bool forceAnimation = false,
        bool revert = false
    ) where T: VisualElement, IPanelBindable, ITransitionable
    {
        var animation = new Animation()
        {
            PropertyName = propertyName,
            Duration = duration,
            EasingMode = easingMode,
            Delay = delay,
            SetInitialValue = setInitialValue,
            SetEndValue = setEndValue,
            Callin = callin,
            IsReverted = revert,
            IsForcedAnimation = forceAnimation,
        };

        var scheduledItemBack = persistentVisual.schedule.Execute(() =>
        {
            callback?.Invoke();
            ve.RemoveAnimation(animation);
        });
        // Will be resume when animation end event will be trigger.
        scheduledItemBack.Pause();
        animation.Callback = scheduledItemBack;

        var scheduledItemCancel = persistentVisual.schedule.Execute(() =>
        {
            callcancel?.Invoke();
            ve.RemoveAnimation(animation);
        });
        // Will be resume when animation end event will be trigger.
        scheduledItemCancel.Pause();
        animation.Callcancel = scheduledItemCancel;

        ve.InsertAnimationInAnimationsList(animation, out bool isNew);

        if (ve.IsListeningForTransition) ve.PlayAnimation(animation, isNew);
    }

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
        for (int i = animations.Count - 1; i >= 0 ; i--) ve.PlayAnimation(animations[i]);
    }

    /// <summary>
    /// Play <paramref name="animation"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ve"></param>
    /// <param name="animation"></param>
    /// <param name="isNew"></param>
    public static void PlayAnimation<T>(this T ve, Animation animation, bool isNew = true)
        where T : VisualElement, IPanelBindable, ITransitionable
    {
        if (!ve.IsListeningForTransition) return;

        // Raise the callin action. This action should not update the property that is animated.
        animation.Callin?.Invoke();

        if (ReduceAnimation && !animation.IsForcedAnimation)
        {
            // Play this action without animation and call the callback
            if (!animation.IsReverted) animation.SetEndValue?.Invoke();
            else animation.SetInitialValue?.Invoke();
            animation.Callback.Resume();
            return;
        }

        if (isNew)
        {
            animation.IsPlaying = true;
            if (!animation.IsReverted) animation.SetInitialValue?.Invoke();
            else animation.SetEndValue?.Invoke();

            ve.UpdateTransitionList(Animations[ve]);

            animation.Coroutine = UIManager.StartCoroutine(ve.WaitOneFrameAndSetEndValue(animation));
        }
        else
        {
            //if (!animation.IsReverted) animation.SetInitialValue?.Invoke();
            //else animation.SetEndValue?.Invoke();
            ////ve.UpdateTransitionList(Animations[ve]);

            //if (animation.Coroutine != null) UIManager.StopCoroutine(animation.Coroutine);
            //animation.Coroutine = UIManager.StartCoroutine(ve.WaitOneFrameAndSetEndValue(animation));
        }
    }

    private static IEnumerator WaitOneFrameAndSetEndValue(this VisualElement ve, Animation animation)
    {
        yield return null;

        // Set the end value.
        if (!animation.IsReverted) animation.SetEndValue?.Invoke();
        else animation.SetInitialValue?.Invoke();
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
        animation.Callback?.Resume();
    }

    /// <summary>
    /// Trigger the animation callcancel with the name <paramref name="property"/>.
    /// </summary>
    /// <param name="ve"></param>
    /// <param name="property"></param>
    public static void TriggerAnimationCallcancel(this VisualElement ve, StylePropertyName property)
    {
        if (!Animations.TryGetValue(ve, out var animations)) return;

        var animation = animations.Find(_animation => _animation.PropertyName == property);
        animation.Callcancel?.Resume();
    }

    private static void InsertAnimationInAnimationsList(this VisualElement ve, Animation animation, out bool isNew)
    {
        if (!Animations.TryGetValue(ve, out var animations))
        {
            Animations.Add(ve, new List<Animation> { animation });
            isNew = true;
        }
        else
        {
            for (int i = 0; i < animations.Count; ++i)
            {
                if (animations[i].PropertyName != animation.PropertyName) continue;

                isNew = !animations[i].IsPlaying;
                animations[i].Copy(animation);
                return;
            }
            animations.Add(animation);
            isNew = true;
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

    private static Animation AddAnimation<T>
    (
        this T ve,
        Action setInitialValue,
        Action setEndValue,
        StylePropertyName propertyName
    ) where T : VisualElement, IPanelBindable, ITransitionable
    {
        var animation = new Animation()
        {
            Visual = ve,
            PropertyName = propertyName,
            SetInitialValue = setInitialValue,
            SetEndValue = setEndValue,
        };

        var scheduledItemBack = ve.schedule.Execute(() =>
        {
            //callback?.Invoke();
            ve.RemoveAnimation(animation);
        });
        // Will be resume when animation end event will be trigger.
        scheduledItemBack.Pause();
        animation.Callback = scheduledItemBack;

        var scheduledItemCancel = ve.schedule.Execute(() =>
        {
            //callcancel?.Invoke();
            //ve.RemoveAnimation(animation);
        });
        // Will be resume when animation end event will be trigger.
        scheduledItemCancel.Pause();
        animation.Callcancel = scheduledItemCancel;

        ve.InsertAnimationInAnimationsList(animation, out bool isNew);
        if (ve.IsListeningForTransition) ve.PlayAnimation(animation, isNew);

        return animation;
    }

    public static Animation WithAnimation(this Animation animation, int duration = 1, EasingMode easingMode = EasingMode.EaseInOut)
    {
        animation.Duration = duration;
        animation.EasingMode = easingMode;

        animation.Visual.UpdateTransitionList(Animations[animation.Visual]);

        return animation;
    }

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
            propertyName: "width"
        );

    #endregion
}
