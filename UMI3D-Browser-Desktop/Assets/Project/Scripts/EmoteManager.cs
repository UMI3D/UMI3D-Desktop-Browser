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
using System.Collections;
using System.Collections.Generic;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;

public class EmoteManager : MonoBehaviour
{
    public RuntimeAnimatorController EmoteAnimatorController;
    private RuntimeAnimatorController cachedAnimatorController;
    private Animator animator;

    [HideInInspector]
    public AnimatorOverrideController animatorOverride;

    public List<AnimationClip> availableEmotes;
    public AnimationClip idleAnimation;

    void Awake()
    {
        Settingbox_E.Instance.Emote.ClickedDown += TriggerAnimation;
        animator = GetComponent<Animator>();

        // set up overrider
        animatorOverride = new AnimatorOverrideController(EmoteAnimatorController);
        var baseAnims = animatorOverride.animationClips;
        var animMapping = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        animMapping.Add(new KeyValuePair<AnimationClip, AnimationClip>(idleAnimation, idleAnimation));
        for (var i=0; i< availableEmotes.Count; i++)
        {
             animMapping.Add(new KeyValuePair<AnimationClip, AnimationClip>(baseAnims[i+1], availableEmotes[i]));;
        }
        animatorOverride.ApplyOverrides(animMapping);
        animatorOverride.name = "EmoteOverride";
    }

    public void LoadEmotes()
    {
        animator.runtimeAnimatorController = animatorOverride;
    }

    public void TriggerAnimation()
    {
        var emoteId = 0;
        StartCoroutine(PlayEmoteAnimation(emoteId));
    }

    public IEnumerator PlayEmoteAnimation(int id)
    {
        cachedAnimatorController = animator.runtimeAnimatorController;
        animator.runtimeAnimatorController = EmoteAnimatorController;
        LoadEmotes();
        animator.SetTrigger($"trigger{id}");
        yield return new WaitForSeconds(availableEmotes[id].length);
        animator.runtimeAnimatorController = cachedAnimatorController;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.AltGr))
            TriggerAnimation();
    }
}

