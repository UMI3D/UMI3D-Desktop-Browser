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
using System.Linq;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;

public class EmoteManager : MonoBehaviour
{
    public RuntimeAnimatorController EmoteAnimatorController;
    private RuntimeAnimatorController cachedAnimatorController;
    private Animator animator;

    [HideInInspector]
    public AnimatorOverrideController animatorOverride;

    public AnimationClip idleAnimation;

    public List<Emote> emotes;

    [System.Serializable]
    public struct Emote
    {
        public Sprite icon;
        public AnimationClip anim;
        public int id;
    }

    void Awake()
    {
        Settingbox_E.Instance.Emote.ClickedDown += ManageEmoteTab;

        animator = GetComponent<Animator>();

        // set up overrider
        animatorOverride = new AnimatorOverrideController(EmoteAnimatorController);
        var baseAnims = animatorOverride.animationClips;
        var animMapping = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        animMapping.Add(new KeyValuePair<AnimationClip, AnimationClip>(idleAnimation, idleAnimation));
        for (var i=0; i< emotes.Count && i+1<baseAnims.Length; i++)
        {
             animMapping.Add(new KeyValuePair<AnimationClip, AnimationClip>(baseAnims[i+1], emotes[i].anim));;
        }
        animatorOverride.ApplyOverrides(animMapping);
        animatorOverride.name = "EmoteOverride";
    }

    private void ManageEmoteTab()
    {
        if (!EmoteWindow_E.Instance.IsDisplaying)
            OpenEmoteTab();
        else
            EmoteWindow_E.Instance.Hide();
    }

    private void OnDestroy()
    {
        Settingbox_E.Instance.Emote.ClickedDown -= ManageEmoteTab;
    }


    /// <summary>
    /// Open emote window UI
    /// </summary>
    private void OpenEmoteTab()
    {
        if (!EmoteWindow_E.Instance.AreButtonsLoaded)
        {
            EmoteWindow_E.Instance.LoadButtons(emotes.Select(x => x.icon).ToList());
            foreach(var emote in emotes)
            {
                EmoteWindow_E.Instance.EmoteButtons[emote.id].Button.ClickedDown += delegate { TriggerAnimation(emote.id); };
            }
        }
            
        EmoteWindow_E.Instance.Display();
    }

    /// <summary>
    /// Loads the emotes in the animator
    /// </summary>
    public void LoadEmotes()
    {
        animator.runtimeAnimatorController = animatorOverride;
    }

    public void TriggerAnimation(int emoteId)
    {
        StartCoroutine(PlayEmoteAnimation(emotes[emoteId]));
    }

    /// <summary>
    /// Play the emote of given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IEnumerator PlayEmoteAnimation(Emote emote)
    {
        cachedAnimatorController = animator.runtimeAnimatorController;
        animator.runtimeAnimatorController = EmoteAnimatorController;
        LoadEmotes();
        animator.SetTrigger($"trigger{emote.id}");
        yield return new WaitForSeconds(emote.anim.length);
        animator.runtimeAnimatorController = cachedAnimatorController;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.AltGr))
            TriggerAnimation(0);
    }
}

