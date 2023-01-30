namespace VCM_FullAssy.Processing
{
    public enum TrayTrayChangeStep
    {
        TrayChangeStart,
        YAxis_TrayChangeMove,
        YAxis_TrayChangeMoveWait,
        CheckBothTrayStatus,
        TrayChangeMessage_Display,
        Tray_Clear,
        YAxis_WorkPositionMove,
        YAxis_WorkPositionMoveWait,
        TrayChangeEnd,
    }
}
