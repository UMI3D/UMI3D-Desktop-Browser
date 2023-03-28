using UnityEngine;

namespace umi3dbrowser.module.lipsync
{
    /// <summary>
    /// Control lips synchronization for lips animation based on the <see cref="UnityEngine.SkinnedMeshRenderer"/> component.
    /// </summary>
    public class LipSyncBlendshapeController : AbstractLipSyncController
    {
        /// <summary>
        /// Extracted renderer component with OVR blend shapes included.
        /// </summary>
        private SkinnedMeshRenderer skinnedMeshRenderer;

        /// <inheritdoc/>
        public override void StartLipSync()
        {
            if (!isInited)
                Init();

            TakeLipsControl();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            base.Init();
        }

        /// <inheritdoc/>
        public override void StopLipSync()
        {
            ReleaseLipsControl();
            isInited = false;
        }

        /// <inheritdoc/>
        protected override void ResetLips()
        {
            foreach (var viseme in Visemes)
            {
                viseme.value = 0;
                skinnedMeshRenderer.SetBlendShapeWeight(viseme.index, 0f);
            }
            Visemes[0].value = 1f;
            skinnedMeshRenderer.SetBlendShapeWeight(0, 1f);
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
                skinnedMeshRenderer.SetBlendShapeWeight(Visemes[i].index, Visemes[i].value);
            }
        }
    }
}