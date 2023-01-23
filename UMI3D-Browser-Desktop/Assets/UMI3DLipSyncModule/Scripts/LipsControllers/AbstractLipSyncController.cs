using inetum.unityUtils;
using System.Collections.Generic;
using UnityEngine;

namespace umi3dbrowser.module.lipsync
{
    /// <summary>
    /// Control lips synchronization for lips animation.
    /// </summary>
    public abstract class AbstractLipSyncController : MonoBehaviour, ILipSyncController
    {
        public OVRLipSyncContextBase OVRLipSyncContextHandler { get; set; }

        /// <summary>
        /// Available visemes.
        /// </summary>
        internal List<Viseme> Visemes { get; private protected set; } = new List<Viseme>();

        /// <summary>
        /// Were all the visemes mapped?
        /// </summary>
        protected bool isInited;

        /// <summary>
        /// Is the animation control active?
        /// </summary>
        protected bool areLipsControlled;

        /// <summary>
        /// Is the viseme analysis enabled?
        /// </summary>
        protected bool isPaused;

        #region lifecycle

        protected virtual void Update()
        {
            if (isPaused || !isInited)
                return;

            AnimateLips();
        }

        /// <summary>
        /// Set up the internally required elements at runtime.
        /// </summary>
        protected virtual void Init()
        {
            Visemes = Viseme.GetOVRVisemes();
            if (OVRLipSyncContextHandler == null)
                OVRLipSyncContextHandler = gameObject.GetOrAddComponent<OVRLipSyncContext>();
            isInited = true;
        }

        /// <inheritdoc/>
        public abstract void StartLipSync();

        /// <inheritdoc/>
        public abstract void StopLipSync();

        /// <inheritdoc/>
        public virtual bool IsLipSyncStarted() => isInited;

        /// <inheritdoc/>
        public virtual void PauseLipSync()
        {
            isPaused = true;
            ResetLips();
            OVRLipSyncContextHandler.enabled = false;
        }

        /// <inheritdoc/>
        public virtual void ResumeLipSync()
        {
            isPaused = false;
            OVRLipSyncContextHandler.enabled = true;
        }

        /// <summary>
        /// Is the phoneme analysis enabled?
        /// </summary>
        /// <returns></returns>
        public virtual bool IsLipSyncPaused() => isPaused;

        #endregion lifecycle

        /// <summary>
        /// Remove the lips control over the model
        /// </summary>
        protected virtual void ReleaseLipsControl()
        {
            ResetLips();
            areLipsControlled = false;
        }

        /// <summary>
        /// Get the lips control over the model
        /// </summary>
        protected virtual void TakeLipsControl()
        {
            ResetLips();
            areLipsControlled = true;
        }

        /// <summary>
        /// Move lips to default position.
        /// </summary>
        protected virtual void ResetLips()
        {
            foreach (var viseme in Visemes)
            {
                viseme.value = 0;
            }

            Visemes[0].value = 1;
        }

        /// <summary>
        /// Get the current visemes and animate lips.
        /// </summary>
        protected virtual void AnimateLips()
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
            }
        }
    }
}