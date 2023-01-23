namespace umi3dbrowser.module.lipsync
{
    /// <summary>
    /// Component that handles facial animations for lip synchronization
    /// </summary>
    public interface ILipSyncController
    {
        /// <summary>
        /// Start the lips synchronization from scratch.
        /// </summary>
        public void StartLipSync();

        /// <summary>
        /// Stop the lips synchronization totally.
        /// </summary>
        /// Walling this method will require to restart the LipSync afterwards.
        public void StopLipSync();

        /// <summary>
        /// Is the lipsync initialized and started?
        /// </summary>
        /// <returns></returns>
        public bool IsLipSyncStarted();

        /// <summary>
        /// Pause the lips synchronization.
        /// </summary>
        public void PauseLipSync();

        /// <summary>
        /// Resume the lips synchronization.
        /// </summary>
        public void ResumeLipSync();

        /// <summary>
        /// Is the controller paused for optimization?
        /// </summary>
        public bool IsLipSyncPaused();
    }
}