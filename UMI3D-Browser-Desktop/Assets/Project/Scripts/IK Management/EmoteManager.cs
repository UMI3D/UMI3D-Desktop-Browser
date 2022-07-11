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

using inetum.unityUtils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3d.baseBrowser.ui.viewController;
using umi3d.cdk;
using umi3d.cdk.userCapture;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;
using UnityEngine.Events;

namespace umi3dDesktopBrowser.emotes
{
    /// <summary>
    /// Manager that handles emotes
    /// </summary>
    public class EmoteManager : SingleBehaviour<EmoteManager>
    {
        /// <summary>
        /// AnimatorController that manages emotes from a bundle
        /// </summary>
        [HideInInspector]
        public RuntimeAnimatorController emoteAnimatorController;

        /// <summary>
        /// Cache to keep previous animator controller during emote animation
        /// </summary>
        private RuntimeAnimatorController cachedAnimatorController;

        /// <summary>
        /// Reference to the avatar animator
        /// </summary>
        private Animator avatarAnimator;

        /// <summary>
        /// True when a bundle with emotes has been loaded
        /// </summary>
        [HideInInspector]
        public bool hasReceivedEmotes = false;

        /// <summary>
        /// Default idle animation
        /// </summary>
        public AnimationClip idleAnimation;

        /// <summary>
        /// Describes an emote from the client side
        /// </summary>
        public class Emote
        {
            /// <summary>
            /// Icon of the emote in the UI
            /// </summary>
            public Sprite icon;

            /// <summary>
            /// Animation of the emote
            /// </summary>
            public AnimationClip anim;

            /// <summary>
            /// Emote id
            /// </summary>
            public int id;
        }

        /// <summary>
        /// Available emotes from bundle
        /// </summary>
        [HideInInspector]
        public List<Emote> EmotesAvailable = new List<Emote>();

        /// <summary>
        /// List of icons to be associated with emotes
        /// </summary>
        public List<Sprite> availableIcons;

        /// <summary>
        /// Default icon used when no corresponding emote is found
        /// </summary>
        public Sprite defaultIcon;

        /// <summary>
        /// True when an emote is currently playing
        /// </summary>
        public bool IsPlayingEmote = false;

        public static UnityEvent PlayingEmote = new UnityEvent();

        protected override void Awake()
        {
            base.Awake();
            BottomBar_E.Instance.Emotes.ClickedDown += PrepareEmoteWindow;

            avatarAnimator = GetComponent<Animator>();

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(delegate
            {
                StartCoroutine(GetEmotes());
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            BottomBar_E.Instance.Emotes.ClickedDown -= PrepareEmoteWindow;
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
            if (emoteFromBundleAnimator != null)
            {
                emoteFromBundleAnimator.enabled = false; //disabled because it causes interferences with avatar bindings
                if (emoteFromBundleAnimator.runtimeAnimatorController == null)
                {
                    DisableEmoteSystem();
                    yield break;
                }

                var importedEmoteController = emoteFromBundleAnimator.runtimeAnimatorController.animationClips.Where(e => !e.name.Contains("Idle"));

                var i = 0;
                foreach (var anim in importedEmoteController)
                {
                    var icon = availableIcons.Where(x => x.name.ToUpper().Contains(anim.name.ToUpper())).FirstOrDefault();
                    if (icon == default)
                        icon = defaultIcon;

                    var emote = new Emote()
                    {
                        icon = icon,
                        anim = anim,
                        id = i
                    };
                    EmotesAvailable.Add(emote);
                    i++;
                }
                emoteAnimatorController = emoteFromBundleAnimator.runtimeAnimatorController;
                hasReceivedEmotes = true;
            }
            if (EmotesAvailable.Count == 0)
                DisableEmoteSystem();
        }

        /// <summary>
        /// Prepare Emote window
        /// </summary>
        private void PrepareEmoteWindow()
        {
            if (!EmoteWindow_E.Instance.AreButtonsLoaded)
            {
                EmoteWindow_E.Instance.LoadButtons(EmotesAvailable.Select(x => x.icon).ToList());
                buttonEmotesMapping = EmoteWindow_E.Instance.MapButtons(ClickButton);
                BottomBar_E.Instance.Emotes.ClickedDown -= PrepareEmoteWindow;
            }
        }

        /// <summary>
        /// Link between buttons and emotes indexing
        /// </summary>
        private Dictionary<Button_E, int> buttonEmotesMapping;

        /// <summary>
        /// Loads the emotes in the animator
        /// </summary>
        private void LoadEmotes()
        {
            cachedAnimatorController = avatarAnimator.runtimeAnimatorController;
            avatarAnimator.runtimeAnimatorController = emoteAnimatorController;
            avatarAnimator.Update(0);
        }

        /// <summary>
        /// Put back the normal animator of the avatar
        /// </summary>
        private void UnloadEmotes()
        {
            avatarAnimator.runtimeAnimatorController = cachedAnimatorController;
            avatarAnimator.Update(0);
        }

        /// <summary>
        /// Triggered action when clicked on <paramref name="button"/>
        /// </summary>
        /// <param name="button"></param>
        private void ClickButton(Button_E button)
        {
            TriggerEmote(buttonEmotesMapping[button]);
        }

        /// <summary>
        /// Starts the coroutine associated to the emote
        /// </summary>
        /// <param name="emoteId"></param>
        public void TriggerEmote(int emoteId)
        {
            StartCoroutine(PlayEmoteAnimation(EmotesAvailable[emoteId]));
        }

        /// <summary>
        /// Play the emote
        /// </summary>
        /// <param name="emote"></param>
        /// <returns></returns>
        public IEnumerator PlayEmoteAnimation(Emote emote)
        {
            IsPlayingEmote = true;
            PlayingEmote.Invoke();

            LoadEmotes();
            avatarAnimator.SetTrigger($"trigger{emote.id}");

            var interruptionAction = new UnityAction(delegate { InterruptEmote(emote); });
            FpsNavigation.PlayerMoved.AddListener(interruptionAction);
            PlayingEmote.AddListener(interruptionAction); //used if another emote is played in the meanwhile

            yield return new WaitForSeconds(emote.anim.length); //wait for emote end of animation

            FpsNavigation.PlayerMoved.RemoveListener(interruptionAction);
            PlayingEmote.RemoveListener(interruptionAction);
            IsPlayingEmote = false;

            UnloadEmotes();
        }

        private void InterruptEmote(Emote emote)
        {
            StopCoroutine(PlayEmoteAnimation(emote));
            IsPlayingEmote = false;
            UnloadEmotes();
        }

        /// <summary>
        /// Definitively disable the emote system for the session
        /// </summary>
        public void DisableEmoteSystem()
        {
            BottomBar_E.Instance.Emotes.Reset();
            BottomBar_E.Instance.Emotes.Hide();
        }
    }
}