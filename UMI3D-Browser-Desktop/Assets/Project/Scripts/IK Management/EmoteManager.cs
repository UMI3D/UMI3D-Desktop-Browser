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
        public List<Emote> emotesAvailable = new List<Emote>();

        /// <summary>
        /// List of icons to be associated with emotes
        /// </summary>
        public List<Sprite> availableIcons;

        /// <summary>
        /// Default icon used when no corresponding emote is found
        /// </summary>
        public Sprite defaultIcon;

        private FpsNavigation playerFPSNavigation;

        protected override void Awake()
        {
            base.Awake();
            Settingbox_E.Instance.Emote.ClickedDown += ManageEmoteTab;

            avatarAnimator = GetComponent<Animator>();

            playerFPSNavigation = GetComponentInParent<FpsNavigation>();

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(delegate
            {
                StartCoroutine(GetEmotes());
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
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
                if (emoteFromBundleAnimator.runtimeAnimatorController == null)
                    yield break;

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
                    emotesAvailable.Add(emote);
                    i++;
                }
                emoteAnimatorController = emoteFromBundleAnimator.runtimeAnimatorController;
                hasReceivedEmotes = true;
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
            TriggerEmote(dict[button]);
        }

        /// <summary>
        /// Starts the coroutine associated to the emote
        /// </summary>
        /// <param name="emoteId"></param>
        public void TriggerEmote(int emoteId)
        {
            StartCoroutine(PlayEmoteAnimation(emotesAvailable[emoteId]));
        }

        /// <summary>
        /// Play the emote
        /// </summary>
        /// <param name="emote"></param>
        /// <returns></returns>
        public IEnumerator PlayEmoteAnimation(Emote emote)
        {
            LoadEmotes();
            avatarAnimator.SetTrigger($"trigger{emote.id}");
            var interruptionAction = new UnityAction(delegate { InterruptEmote(emote); });
            FpsNavigation.PlayerMoved.AddListener(interruptionAction);
            yield return new WaitForSeconds(emote.anim.length);
            FpsNavigation.PlayerMoved.RemoveListener(interruptionAction);
            UnloadEmotes();
        }

        private void InterruptEmote(Emote emote)
        {
            StopCoroutine(PlayEmoteAnimation(emote));
            UnloadEmotes();
        }
    }
}
