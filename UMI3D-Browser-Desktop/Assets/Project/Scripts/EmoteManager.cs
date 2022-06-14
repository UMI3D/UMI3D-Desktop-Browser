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
using umi3d.baseBrowser.ui.viewController;
using umi3d.cdk;
using umi3d.cdk.userCapture;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;

public class EmoteManager : MonoBehaviour
{
    public RuntimeAnimatorController emoteAnimatorController;
    private RuntimeAnimatorController cachedAnimatorController;
    private Animator avatarAnimator;

    [HideInInspector]
    public AnimatorOverrideController animatorOverride;

    public AnimationClip idleAnimation;

    [HideInInspector]
    public List<Emote> emotesAvailable = new List<Emote>();

    public class Emote
    {
        public Sprite icon;
        public AnimationClip anim;
        public int id;
    }

    public Sprite defaultSprite;

    void Awake()
    {
        Settingbox_E.Instance.Emote.ClickedDown += ManageEmoteTab;

        avatarAnimator = GetComponent<Animator>();

        UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(delegate
        {
            StartCoroutine(GetEmotes());
        }); 
    }

    private void OnDestroy()
    {
        Settingbox_E.Instance.Emote.ClickedDown -= ManageEmoteTab;
    }

    /// <summary>
    /// Waits bundle loading and retreives emotes
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetEmotes() //wait for bundle loading
    {
        var id = UMI3DClientServer.Instance.GetUserId();
        var avatar = UMI3DClientUserTracking.Instance.embodimentDict[id];

        while (avatar.transform.childCount == 0
            || (avatar.transform.childCount == 1 && avatar.transform.GetChild(0).transform.childCount == 0))
        {
            yield return null;
        }
        
        var emoteFromBundleAnimator = avatar.GetComponentInChildren<Animator>();
        emoteFromBundleAnimator.enabled = false; //disabled because it causes interferences with avatar bindings
        if (emoteFromBundleAnimator != null)
        {
            animatorOverride = new AnimatorOverrideController(emoteAnimatorController);
            var importedEmotesAnim = emoteFromBundleAnimator.runtimeAnimatorController.animationClips.Where(e => !e.name.Contains("Idle"));
            var holderEmotesAnim = emoteAnimatorController.animationClips.Where(e => !e.name.Contains("Idle")).ToList();

            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            var i = 0;
            foreach (var anim in importedEmotesAnim)
            {
                if (i < holderEmotesAnim.Count)
                {
                    var holderToOverride = holderEmotesAnim[i];
                    var emote = new Emote()
                    {
                        icon = defaultSprite,
                        anim = anim,
                        id = i
                    };
                    emotesAvailable.Add(emote);
                    overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(holderToOverride, anim));
                    i++;
                }
                else
                    Debug.LogWarning("Not enough emote holders to receive all emotes from server");
            }
            animatorOverride.ApplyOverrides(overrides);
            emoteAnimatorController = animatorOverride;
        }
    }

    /// <summary>
    /// Toggle Emote window
    /// </summary>
    private void ManageEmoteTab()
    {
        if (!EmoteWindow_E.Instance.IsDisplaying)
            OpenEmoteTab();
        else
            EmoteWindow_E.Instance.Hide();
    }

    private Dictionary<Button_E, int> dict;
    /// <summary>
    /// Open emote window UI
    /// </summary>
    private void OpenEmoteTab()
    {
        if (!EmoteWindow_E.Instance.AreButtonsLoaded)
        {
            EmoteWindow_E.Instance.LoadButtons(emotesAvailable.Select(x => x.icon).ToList());
            dict = EmoteWindow_E.Instance.MapButtons(ClickButton);
        }
        EmoteWindow_E.Instance.Display();
    }

    /// <summary>
    /// Loads the emotes in the animator
    /// </summary>
    private void LoadEmotes()
    {
        cachedAnimatorController = avatarAnimator.runtimeAnimatorController;
        avatarAnimator.runtimeAnimatorController = emoteAnimatorController;
    }

    private void UnloadEmotes()
    {
        avatarAnimator.runtimeAnimatorController = cachedAnimatorController;
    }

    public void ClickButton(Button_E button)
    {
        TriggerEmote(dict[button]);
    }

    public void TriggerEmote(int emoteId)
    {
        StartCoroutine(PlayEmoteAnimation(emotesAvailable[emoteId]));
    }


    /// <summary>
    /// Play the emote of given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IEnumerator PlayEmoteAnimation(Emote emote)
    {
        LoadEmotes();
        avatarAnimator.SetTrigger($"trigger{emote.id}");
        yield return new WaitForSeconds(emote.anim.length);
        UnloadEmotes();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.AltGr))
            TriggerEmote(0);
    }
}

