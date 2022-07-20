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
using umi3d.common;
using umi3d.common.userCapture;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;
using UnityEngine.Events;

namespace umi3dDesktopBrowser.emotes
{
    /// <summary>
    /// Manager that handles emotes
    /// </summary>
    public class EmoteManager : MonoBehaviour
    {
        #region Fields

        #region AnimatorManagement

        /// <summary>
        /// AnimatorController that manages emotes from a bundle
        /// </summary>
        [HideInInspector]
        public RuntimeAnimatorController emoteAnimatorController;

        /// <summary>
        /// Cache to keep previous animator controller during emote animationMf
        /// </summary>
        private RuntimeAnimatorController cachedAnimatorController;

        /// <summary>
        /// Reference to the avatar animator
        /// </summary>
        private Animator avatarAnimator;

        /// <summary>
        /// Default idle animation
        /// </summary>
        public AnimationClip idleAnimation;

        #endregion AnimatorManagement

        #region EmotesConfigManagement

        /// <summary>
        /// Describes an emote from the client side
        /// </summary>
        [System.Serializable]
        public class Emote
        {
            /// <summary>
            /// Emote's name
            /// </summary>
            public string name;

            /// <summary>
            /// Icon of the emote in the UI
            /// </summary>
            public Sprite icon;

            /// <summary>
            /// Animation of the emote
            /// </summary>
            public AnimationClip anim;

            /// <summary>
            /// Should the emote be available or not
            /// </summary>
            public bool available;

            /// <summary>
            /// Emote id
            /// </summary>
            public int id;

            /// <summary>
            /// Emote dto
            /// </summary>
            public UMI3DEmoteDto dto;
        }

        /// <summary>
        /// Available emotes from bundle
        /// </summary>
        [ReadOnly]
        public List<Emote> Emotes = new List<Emote>();

        /// <summary>
        /// Last received dto reference
        /// </summary>
        private UMI3DEmotesConfigDto emoteConfigDto;

        /// <summary>
        /// Default icon used when no corresponding emote is found
        /// </summary>
        public Sprite defaultIcon;

        /// <summary>
        /// True when a bundle with emotes has been loaded
        /// </summary>
        [HideInInspector]
        public bool hasReceivedEmotes = false;

        #endregion EmotesConfigManagement

        #region EmotePlayingManagement

        /// <summary>
        /// True when an emote is currently playing
        /// </summary>
        [HideInInspector]
        public bool IsPlayingEmote = false;

        /// <summary>
        /// Triggered when an emote starts playing
        /// </summary>
        public static UnityEvent PlayingEmote = new UnityEvent();

        /// <summary>
        /// Link between buttons and emotes indexing
        /// </summary>
        private Dictionary<Button_E, int> buttonTriggerEmotesMapping;

        #endregion EmotePlayingManagement

        #endregion Fields

        protected void Awake()
        {
            UMI3DClientUserTracking.Instance.EmotesLoadedEvent.AddListener(ConfigEmotes);
            BottomBar_E.Instance.Emotes.Hide();
        }

        protected void OnDestroy()
        {
            EmoteWindow_E.Instance.DestroyButtons();
            EmoteWindow_E.Instance.Hide();
        }

        #region Emote Config

        /// <summary>
        /// Load and configure emotes from an <see cref="UMI3DEmotesConfigDto"/>
        /// and try to get the animations.
        /// </summary>
        /// <param name="dto"></param>
        private void ConfigEmotes(UMI3DEmotesConfigDto dto)
        {
            emoteConfigDto = dto;

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(delegate
            {
                avatarAnimator = GetComponentInChildren<Animator>();
                StartCoroutine(GetEmotes());
            });
        }

        /// <summary>
        /// Change the availability of an emote based on the received <see cref="UMI3DEmoteDto"/>
        /// </summary>
        /// <param name="dto"></param>
        private void UpdateEmote(UMI3DEmoteDto dto)
        {
            if (!hasReceivedEmotes)
                return;
            var emote = Emotes.Find(x => x.name == dto.name);

            emote.available = dto.available;

            UpdateEmoteWindow(emote);
        }

        /// <summary>
        /// Waits bundle loading attached to avatar, retreives emotes and their icon
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
                    DisableEmoteSystem(); //no emotes support in the scene
                    yield break;
                }

                var importedEmoteControllerAnims = emoteFromBundleAnimator.runtimeAnimatorController.animationClips.Where(e => !e.name.Contains("Idle"));

                var i = 0;
                foreach (var anim in importedEmoteControllerAnims)
                {
                    var emoteRefInConfig = emoteConfigDto.emotes.Find(x => anim.name == x.name);

                    if (emoteRefInConfig != null)
                    {
                        Sprite icon = default;
                        if (emoteRefInConfig.iconResource != null)
                        {
                            IResourcesLoader loader = UMI3DEnvironmentLoader.Parameters.SelectLoader(emoteRefInConfig.iconResource.extension);
                            UMI3DResourcesManager.LoadFile(
                                emoteConfigDto.id,
                                emoteRefInConfig.iconResource,
                                loader.UrlToObject,
                                loader.ObjectFromCache,
                                (image) =>
                                {
                                    var tex = (Texture2D)image;
                                    icon = Sprite.Create((Texture2D)image, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                    return;
                                },
                                e => UMI3DLogger.LogWarning(e, DebugScope.CDK),
                                loader.DeleteObject
                                );
                        }

                        if (icon == default)
                            icon = defaultIcon;

                        var emote = new Emote()
                        {
                            name = emoteRefInConfig.name,
                            available = emoteConfigDto.allAvailableByDefault ? true : emoteRefInConfig.available,
                            icon = icon,
                            anim = anim,
                            id = i,
                            dto = emoteRefInConfig
                        };
                        Emotes.Add(emote);
                    }
                    i++;
                }
                emoteAnimatorController = emoteFromBundleAnimator.runtimeAnimatorController;
                hasReceivedEmotes = true;
                BottomBar_E.Instance.Emotes.Display();
                PrepareEmoteWindow();
                UMI3DClientUserTracking.Instance.EmoteChangedEvent.AddListener(UpdateEmote);
            }
            if (Emotes.Count == 0)
                DisableEmoteSystem();
        }

        #endregion Emote Config

        #region UI-related

        /// <summary>
        /// Prepare Emote window
        /// </summary>
        private void PrepareEmoteWindow()
        {
            if (EmoteWindow_E.Instance.AreButtonsLoaded)
                return;

            EmoteWindow_E.Instance.LoadButtons(Emotes.Select(x => x.icon).ToList());
            buttonTriggerEmotesMapping = EmoteWindow_E.Instance.MapButtons(ClickButton);
            UpdateAllEmoteWindow();
        }

        /// <summary>
        /// Update all the appearing buttons according to availability
        /// </summary>
        private void UpdateAllEmoteWindow()
        {
            if (!EmoteWindow_E.Instance.AreButtonsLoaded)
                return;

            for (var i = 0; i < Emotes.Count; i++)
            {
                EmoteWindow_E.Instance.UpdateButtonVisibility(i, Emotes[i].available);
            }
        }

        /// <summary>
        /// Update the appearing button linked to the emote based on availability
        /// </summary>
        private void UpdateEmoteWindow(Emote emote)
        {
            if (!EmoteWindow_E.Instance.AreButtonsLoaded)
                return;

            int i = Emotes.FindIndex(x => x == emote);
            EmoteWindow_E.Instance.UpdateButtonVisibility(i, emote.available);
        }

        /// <summary>
        /// Triggered action when clicked on <paramref name="button"/>
        /// </summary>
        /// <param name="button"></param>
        private void ClickButton(Button_E button)
        {
            TriggerEmote(buttonTriggerEmotesMapping[button]);
        }

        /// <summary>
        /// Starts the coroutine associated to the emote
        /// </summary>
        /// <param name="emoteId"></param>
        public void TriggerEmote(int emoteId)
        {
            StartCoroutine(PlayEmoteAnimation(Emotes[emoteId]));
        }

        /// <summary>
        /// Definitively disable the emote system for the session
        /// </summary>
        public void DisableEmoteSystem()
        {
            BottomBar_E.Instance.Emotes.Reset();
            BottomBar_E.Instance.Emotes.Hide();
        }

        #endregion UI-related

        #region Emote Playing

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

            yield return new WaitWhile(() => avatarAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1); //wait for emote end of animation
            //? Possible to improve using a StateMachineBehaviour attached to the EmoteController & trigger events on OnStateExit on anim/OnStateEnter on AnyState

            FpsNavigation.PlayerMoved.RemoveListener(interruptionAction);
            PlayingEmote.RemoveListener(interruptionAction);
            IsPlayingEmote = false;

            UnloadEmotes();
        }

        /// <summary>
        /// Interrupts an emote animation
        /// </summary>
        /// <param name="emote"></param>
        private void InterruptEmote(Emote emote)
        {
            StopCoroutine(PlayEmoteAnimation(emote));
            IsPlayingEmote = false;
            UnloadEmotes();
        }

        #endregion Emote Playing
    }
}