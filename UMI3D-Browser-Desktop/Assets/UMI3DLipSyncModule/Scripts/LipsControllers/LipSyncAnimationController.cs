using inetum.unityUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3dbrowser.module.lipsync
{
    /// <summary>
    /// Control lips synchronization for lips animation based on the <see cref="UnityEngine.Animation"/> component.
    /// </summary>
    public class LipSyncAnimationController : AbstractLipSyncController
    {
        /// <summary>
        /// Extracted animation component.
        /// </summary>
        private Animation lipsAnimation;

        /// <summary>
        /// Available visemes.
        /// </summary>
        internal Dictionary<Viseme, AnimationState> VisemesAnimation { get; private protected set; } = new Dictionary<Viseme, AnimationState>();

        /// <inheritdoc/>
        protected override void Init()
        {
            Visemes = Viseme.GetOVRVisemes();
            foreach (var viseme in Visemes)
                VisemesAnimation.Add(viseme, lipsAnimation[viseme.DimimoName]);

            if (OVRLipSyncContextHandler == null)
                OVRLipSyncContextHandler = gameObject.GetOrAddComponent<OVRLipSyncContext>();
            isInited = true;
        }

        /// <inheritdoc/>
        public override void StartLipSync()
        {
            if (lipsAnimation == null)
                lipsAnimation = GetComponent<Animation>();

            if (!isInited)
            {
                Init();
                foreach (AnimationState state in lipsAnimation)
                {
                    state.wrapMode = WrapMode.ClampForever;
                    state.blendMode = AnimationBlendMode.Blend;
                    state.weight = 1f;
                    lipsAnimation.Blend(state.name, 0, 0);
                    state.normalizedTime = 1;
                }
            }

            TakeLipsControl();
        }

        /// <inheritdoc/>
        public override void StopLipSync()
        {
            ReleaseLipsControl();
            lipsAnimation.enabled = false;
            lipsAnimation = null;
            isInited = false;
        }

        /// <inheritdoc/>
        protected override void ReleaseLipsControl()
        {
            ResetLips();
            StartCoroutine(ReleasingLips());
            areLipsControlled = false;
        }

        private IEnumerator ReleasingLips()
        {
            yield return new WaitForEndOfFrame(); //Wait for animation component to reset the lips
            lipsAnimation.enabled = false;
        }

        /// <inheritdoc/>
        protected override void TakeLipsControl()
        {
            lipsAnimation.enabled = true;
            ResetLips();
            areLipsControlled = true;
        }

        /// <inheritdoc/>
        protected override void ResetLips()
        {
            foreach (var viseme in Visemes)
            {
                viseme.value = 0;
                VisemesAnimation[viseme].weight = 0;
                VisemesAnimation[viseme].enabled = true;
            }

            Visemes[0].value = 1;
            VisemesAnimation[Visemes[0]].weight = 1;
        }

        /// <inheritdoc/>
        protected override void AnimateLips()
        {
            OVRLipSync.Frame frame = OVRLipSyncContextHandler.GetCurrentPhonemeFrame();

            if (!areLipsControlled && frame.Visemes[0] != 1) // not 100% sil
                TakeLipsControl();
            else if (areLipsControlled && frame.Visemes[0] == 1) // 100% sil
                ReleaseLipsControl();

            if (!areLipsControlled) // lips animation control has been released
                return;

            for (var i = 0; i < frame.Visemes.Length; i++)
            {
                Visemes[i].value = frame.Visemes[i];
                VisemesAnimation[Visemes[i]].weight = Visemes[i].value;
            }
        }
    }
}