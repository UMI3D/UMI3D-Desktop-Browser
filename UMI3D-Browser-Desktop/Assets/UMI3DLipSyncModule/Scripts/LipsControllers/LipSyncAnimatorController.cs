using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace umi3dbrowser.module.lipsync
{
    /// <summary>
    /// Control lips synchronization for lips animation based on the <see cref="UnityEngine.Animator"/> component.
    /// </summary>
    /// Require to design a specific animator. Not recommended.
    public class LipSyncAnimatorController : AbstractLipSyncController
    {
        /// <summary>
        /// Designed animator of lips
        /// </summary>
        private Animator lipsAnimator;

        /// <summary>
        /// Lips overriden animations
        /// </summary>
        public AnimatorOverrideController overrideController;

        protected void SetupAnimatorOverride(Animation animation)
        {
            var animationClips = new Dictionary<string, AnimationClip>();
            foreach (AnimationState state in animation)
            {
                animationClips.Add(state.name, state.clip);
            }

            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            overrideController.GetOverrides(overrides);

            var newOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var overrideAnimation in overrides)
            {
                var correspondingViseme = Visemes.Where(x => x.DimimoName == overrideAnimation.Key.name).FirstOrDefault();
                newOverrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(overrideAnimation.Key, animationClips[correspondingViseme.DimimoName]));
            }

            overrideController.ApplyOverrides(newOverrides);

            lipsAnimator.runtimeAnimatorController = overrideController;
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            Visemes = Viseme.GetOVRVisemes();

            var animationComponent = GetComponent<Animation>();
            SetupAnimatorOverride(animationComponent);
            animationComponent.enabled = false;
            Destroy(animationComponent);

            if (OVRLipSyncContextHandler == null)
                gameObject.AddComponent<OVRLipSyncContext>();
            isInited = true;
        }

        /// <inheritdoc/>
        public override void StartLipSync()
        {
            if (lipsAnimator == null)
                lipsAnimator = gameObject.AddComponent<Animator>();

            if (!isInited)
                Init();

            TakeLipsControl();
        }

        /// <inheritdoc/>
        protected override void AnimateLips()
        {
            OVRLipSync.Frame frame = OVRLipSyncContextHandler.GetCurrentPhonemeFrame();

            for (var i = 0; i < frame.Visemes.Length; i++)
            {
                Visemes[i].value = frame.Visemes[i];
                if (Visemes[i].code != "sil") // not for sil
                    lipsAnimator.SetFloat(Visemes[i].code, Visemes[i].value);
            }
        }

        /// <inheritdoc/>
        public override void PauseLipSync()
        {
            isPaused = true;
            OVRLipSyncContextHandler.enabled = false;
        }

        /// <inheritdoc/>
        public override void ResumeLipSync()
        {
            isPaused = false;
            OVRLipSyncContextHandler.enabled = true;
        }

        /// <inheritdoc/>
        public override void StopLipSync()
        {
            ReleaseLipsControl();
            lipsAnimator.enabled = false;
            lipsAnimator = null;
            isInited = false;
        }
    }
}