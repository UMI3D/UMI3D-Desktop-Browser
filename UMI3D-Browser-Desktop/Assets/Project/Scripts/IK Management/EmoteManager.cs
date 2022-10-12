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
using System.Drawing;
using System.Linq;
using umi3d.baseBrowser.ui.viewController;
using umi3d.cdk;
using umi3d.cdk.collaboration;
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
        /// Cache to keep previous animator controller during emote animation
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
            /// Emote's label
            /// </summary>
            public string Label => dto.label;

            /// <summary>
            /// Icon of the emote in the UI
            /// </summary>
            public Sprite icon;

            /// <summary>
            /// Should the emote be available or not
            /// </summary>
            public bool available;

            /// <summary>
            /// Emote order in UI
            /// </summary>
            public int uiOrder;

            /// <summary>
            /// Emote dto
            /// </summary>
            public UMI3DEmoteDto dto;
        }

        /// <summary>
        /// Available emotes from bundle
        /// </summary>
        [HideInInspector]
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
        private bool hasReceivedEmotes = false;

        #endregion EmotesConfigManagement

        #region UIEmoteManagement

        /// <summary>
        /// Link between buttons and emotes indexing
        /// </summary>
        private Dictionary<Button_E, int> buttonTriggerEmotesMapping;

        #endregion UIEmoteManagement

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
            UMI3DCollaborationClientServer.Instance.OnRedirection.AddListener(() =>
            {
                StopAllCoroutines();
                ResetEmoteSystem();
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
            var emote = Emotes.Find(x => x.dto.stateName == dto.stateName);

            emote.available = dto.available;
            emote.dto = dto;

            UpdateEmoteWindow(emote);
        }

        /// <summary>
        /// Waits bundle loading attached to avatar, retreives emotes and their icon
        /// </summary>
        /// <returns></returns>
        private IEnumerator GetEmotes() 
        {
            var id = UMI3DClientServer.Instance.GetUserId();
            var avatar = UMI3DClientUserTracking.Instance.embodimentDict[id];

            while (avatar.transform.childCount == 0
                || (avatar.transform.childCount == 1 && avatar.transform.GetChild(0).transform.childCount == 0)) //wait for bundle loading
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

                var i = 0;
                foreach (UMI3DEmoteDto emoteRefInConfig in emoteConfigDto.emotes)
                {
                    if (emoteRefInConfig != null)
                    {
                        var emote = new Emote()
                        {
                            available = emoteConfigDto.allAvailableByDefault ? true : emoteRefInConfig.available,
                            icon = defaultIcon,
                            uiOrder = i,
                            dto = emoteRefInConfig
                        };

                        if (emoteRefInConfig.iconResource != null)
                        {
                            LoadFile(emoteRefInConfig, emote);
                        }

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

        async void LoadFile(UMI3DEmoteDto emoteRefInConfig, Emote emote)
        {
            IResourcesLoader loader = UMI3DEnvironmentLoader.Parameters.SelectLoader(emoteRefInConfig.iconResource.extension);
            var image = await UMI3DResourcesManager.LoadFile(
                emoteConfigDto.id,
                emoteRefInConfig.iconResource,
                loader );
            var tex = (Texture2D)image;
            emote.icon = Sprite.Create((Texture2D)image, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            return;
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
        /// Disable the emote system
        /// </summary>
        public void DisableEmoteSystem()
        {
            BottomBar_E.Instance.Emotes.Hide();
        }

        /// <summary>
        /// Reset all variables and disable the system
        /// </summary>
        public void ResetEmoteSystem()
        {
            if (hasReceivedEmotes)
            {
                EmoteWindow_E.Instance.DestroyButtons();
                BottomBar_E.Instance.Emotes.Hide();
                Emotes.Clear();
                buttonTriggerEmotesMapping.Clear();
                emoteConfigDto = null;
                emoteAnimatorController = null;
                hasReceivedEmotes = false;
            }
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

        private UnityAction currentInterruptionAction;

        /// <summary>
        /// Play the emote
        /// </summary>
        /// <param name="emote"></param>
        /// <returns></returns>
        public IEnumerator PlayEmoteAnimation(Emote emote)
        {
            UMI3DClientUserTracking.Instance.EmotePlayedSelfEvent.Invoke();
            // send the emote triggerring info to other browsers through the server
            var emoteRequest = new EmoteRequest()
            {
                emoteId = emote.dto.id,
                shouldTrigger = true,
                sendingUserId = UMI3DClientServer.Instance.GetUserId()
            };
            UMI3DClientServer.SendData(emoteRequest, true);

            LoadEmotes();


            currentInterruptionAction = new UnityAction(delegate { InterruptEmote(emote); });
            FpsNavigation.PlayerMoved.AddListener(currentInterruptionAction);

            avatarAnimator.Play(emote.dto.stateName);
            UMI3DClientUserTracking.Instance.EmotePlayedSelfEvent.AddListener(currentInterruptionAction); //used if another emote is played in the meanwhile

            yield return new WaitWhile(() => avatarAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1); //wait for emote end of animation
            //? Possible to improve using a StateMachineBehaviour attached to the EmoteController & trigger events on OnStateExit on anim/OnStateEnter on AnyState
            StopEmotePlayMode();
        }

        /// <summary>
        /// Stop the emote playing process.
        /// </summary>
        /// <param name="emote"></param>
        private void StopEmotePlayMode()
        {
            if (UMI3DClientUserTracking.Instance.IsEmotePlaying)
            {
                FpsNavigation.PlayerMoved.RemoveListener(currentInterruptionAction);
                UMI3DClientUserTracking.Instance.EmotePlayedSelfEvent.RemoveListener(currentInterruptionAction);
                currentInterruptionAction = null;
            }
            UnloadEmotes();
            UMI3DClientUserTracking.Instance.EmoteEndedSelfEvent.Invoke();
        }

        /// <summary>
        /// Interrupts an emote animation
        /// </summary>
        /// <param name="emote"></param>
        private void InterruptEmote(Emote emote)
        {
            StopCoroutine(PlayEmoteAnimation(emote));
            StopEmotePlayMode();
            // send the emote interruption info to other browsers through the server
            var emoteRequest = new EmoteRequest()
            {
                emoteId = emote.dto.id,
                shouldTrigger = false,
                sendingUserId = UMI3DClientServer.Instance.GetUserId()
            };
            UMI3DClientServer.SendData(emoteRequest, true);
        }

        #endregion Emote Playing
    }
}