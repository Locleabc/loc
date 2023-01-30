namespace VCM_FullAssy.Processing
{
    public enum TrayWorkStep
    {
        TrayWorkStart,
        ZAxis_ReadyWait,
        /// <summary>
        /// Check if Tray can move to work position (Load, Unload) or need to be changed (after work done)
        /// </summary>
        WorkDecision,

        YAxis_VisionLoadPositionMove,
        YAxis_VisionLoadPositionWait,

        YAxis_VisionUnloadPositionMove,
        YAxis_VisionUnloadPositionWait,
        /// <summary>
        /// Pick not done -> Move to Load position
        /// </summary>
        YAxis_HeadLoadPositionMove,
        YAxis_HeadLoadPositionWait,
        /// <summary>
        /// Pick done -> Move to Unload position
        /// </summary>
        YAxis_HeadUnloadPositionMove,
        YAxis_HeadUnloadPositionWait,
        /// <summary>
        /// Tell another process that tray is move to ready-work position
        /// </summary>
        Tray_WorkFlagSet,
        YAxis_ChangePositionMove,
        YAxis_ChangePositionWait,
        TrayWorkEnd,
    }
}
