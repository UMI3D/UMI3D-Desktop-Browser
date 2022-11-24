using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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


    public static Dictionary<VisualElement, List<Animation>> Animations = new Dictionary<VisualElement, List<Animation>>();

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
            if (!revert) setEndValue?.Invoke();
            else setInitialValue?.Invoke();
            callback?.Invoke();
            return;
        }

        var animation = new Animation()
        {
            PropertyName = propertyName,
            Duration = duration,
            EasingMode = easingMode,
            Delay = delay,
        };

        var delayBeforeCallback = (long)(duration.value * 1000 + (delay == null ? 0 : delay.value * 1000));
        var scheduledItem = persistentVisual.schedule.Execute(() =>
        {
            callback?.Invoke();
            ve.RemoveAnimation(animation);
        });
        scheduledItem.ExecuteLater(delayBeforeCallback);
        animation.Callback = scheduledItem;

        if (!Animations.TryGetValue(ve, out var oldAnimations))
        {
            if (!revert) setInitialValue?.Invoke();
            else setEndValue?.Invoke();
            ve.style.transitionProperty = new List<StylePropertyName> { propertyName };
            ve.style.transitionDuration = new List<TimeValue> { duration };
            ve.style.transitionTimingFunction = new List<EasingFunction> { EasingMode.EaseInOut };
            ve.style.transitionDelay = new List<TimeValue> { delay };
            if (!revert) setEndValue?.Invoke();
            else setInitialValue?.Invoke();

            Animations.Add(ve, new List<Animation> { animation });

            return;
        }

        var isNew = true;
        for (int i = 0; i < oldAnimations.Count; ++i)
        {
            if (oldAnimations[i].PropertyName != propertyName) continue;

            oldAnimations[i].Callback?.Pause();
            oldAnimations[i] = animation;
            isNew = false;
            break;
        }
        if (isNew)
        {
            if (!revert) setInitialValue();
            else setEndValue?.Invoke();
            oldAnimations.Add(animation);
        }

        var properties = new List<StylePropertyName>();
        var durations = new List<TimeValue>();
        var easingModes = new List<EasingFunction>();
        var delays = new List<TimeValue>();
        foreach (var animation_ in oldAnimations)
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

        if (!revert) setEndValue?.Invoke();
        else setInitialValue?.Invoke();
    }

    public static void RemoveAnimation(this VisualElement ve, Animation animation)
    {
        if (!Animations.TryGetValue(ve, out var animations)) return;

        if (!animations.Contains(animation)) return;

        animations.Remove(animation);

        var properties = new List<StylePropertyName>();
        var durations = new List<TimeValue>();
        var easingModes = new List<EasingFunction>();
        var delays = new List<TimeValue>();
        foreach (var animation_ in animations)
        {
            properties.Add(animation.PropertyName);
            durations.Add(animation_.Duration);
            easingModes.Add(animation_.EasingMode);
            delays.Add(animation_.Delay);
        }

        ve.style.transitionProperty = properties;
        ve.style.transitionDuration = durations;
        ve.style.transitionTimingFunction = easingModes;
        ve.style.transitionDelay = delays;

        if (animations.Count == 0) Animations.Remove(ve);
    }

    public static void RemoveAnimation(this VisualElement ve, StylePropertyName propertyName)
    {
        if (!Animations.TryGetValue(ve, out var animations)) return;

        if (!animations.Exists(animation => animation.PropertyName == propertyName)) return;
        var animation = animations.Find(animation => animation.PropertyName == propertyName);
        ve.RemoveAnimation(animation);
    }
}
