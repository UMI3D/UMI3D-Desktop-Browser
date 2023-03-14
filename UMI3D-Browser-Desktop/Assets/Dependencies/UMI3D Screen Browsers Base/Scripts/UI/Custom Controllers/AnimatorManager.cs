using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static AnimatorManager;

public static class AnimatorManager
{
    public struct Animation
    {
        public StylePropertyName PropertyName;
        public TimeValue Duration;
        public EasingFunction EasingMode;
        public TimeValue Delay;
        public IVisualElementScheduledItem Callback;
    }

    public static bool ReduceAnimation;
    public const float NavigationScreenDuration = 0.3f;
    public const float DropdownDuration = 0.5f;
    public const float MainDuration = 1f;
    public const float TextFadeDuration = 1f;
    public const float CallbackDelay = .1f;

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
    /// <param name="callback">Callback raised when the animation end. This callback is raised only when the animation end properly.</param>
    /// <param name="forceAnimation">Whether or not playing this animation when <see cref="ReduceAnimation"/> is true.</param>
    /// <param name="revert">Should the animation be played reverted.</param>
    public static void AddAnimation
    (
        this VisualElement ve,
        VisualElement persistentVisual,
        Action setInitialValue, 
        Action setEndValue, 
        StylePropertyName propertyName, 
        TimeValue duration,
        EasingMode easingMode = EasingMode.EaseInOut, 
        TimeValue delay = new TimeValue(), 
        Action callback = null, 
        bool forceAnimation = false,
        bool revert = false
    )
    {
        if (ReduceAnimation && !forceAnimation)
        {
            // Play this action without animation and call the callback
            if (!revert) setEndValue?.Invoke();
            else setInitialValue?.Invoke();
            persistentVisual.schedule.Execute(() => callback?.Invoke());
            return;
        }

        var animation = new Animation()
        {
            PropertyName = propertyName,
            Duration = duration,
            EasingMode = easingMode,
            Delay = delay,
        };

        var delayBeforeCallback = (long)(duration.value * 1000 + (delay == null ? 0 : delay.value * 1000) + CallbackDelay);
        var scheduledItem = persistentVisual.schedule.Execute(() =>
        {
            callback?.Invoke();
            ve.RemoveAnimation(animation);
        });
        scheduledItem.ExecuteLater(delayBeforeCallback);
        animation.Callback = scheduledItem;

        if (!Animations.TryGetValue(ve, out var animations))
        {
            // Add this animation.
            if (!revert) setInitialValue?.Invoke();
            else setEndValue?.Invoke();
            ve.style.transitionProperty = new List<StylePropertyName> { propertyName };
            ve.style.transitionDuration = new List<TimeValue> { duration };
            ve.style.transitionTimingFunction = new List<EasingFunction> { EasingMode.EaseInOut };
            ve.style.transitionDelay = new List<TimeValue> { delay };
            if (!revert) setEndValue?.Invoke();
            else setInitialValue?.Invoke();

            Animations.Add(ve, new List<Animation> { animation });
        }
        else
        {
            // Check if there is a similar animation.
            var isNew = true;
            for (int i = 0; i < animations.Count; ++i)
            {
                if (animations[i].PropertyName != propertyName) continue;

                // if not new: Pause the old animation callback as we don't want it to be called.
                animations[i].Callback?.Pause();
                animations[i] = animation;
                isNew = false;
                break;
            }
            if (isNew)
            {
                // Set the initial value.
                if (!revert) setInitialValue();
                else setEndValue?.Invoke();

                animations.Add(animation);

                // Insert this animation among the others.
                ve.UpdateTransitionList(animations);
            }

            // Set the end value.
            if (!revert) setEndValue?.Invoke();
            else setInitialValue?.Invoke();
        }
    }

    public static void RemoveAnimation(this VisualElement ve, Animation animation)
    {
        if (!Animations.TryGetValue(ve, out var animations)) return;

        if (!animations.Contains(animation)) return;

        animations.Remove(animation);

        ve.UpdateTransitionList(animations);

        if (animations.Count == 0) Animations.Remove(ve);
    }

    public static void RemoveAnimation(this VisualElement ve, StylePropertyName propertyName)
    {
        if (!Animations.TryGetValue(ve, out var animations)) return;

        if (!animations.Exists(animation => animation.PropertyName == propertyName)) return;
        var animation = animations.Find(animation => animation.PropertyName == propertyName);
        ve.RemoveAnimation(animation);
    }

    private static void UpdateTransitionList(this VisualElement ve, List<Animation> animations)
    {
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
}
