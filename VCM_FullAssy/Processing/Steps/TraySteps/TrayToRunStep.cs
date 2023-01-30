namespace VCM_FullAssy.Processing
{
    public enum TrayToRunStep
    {
        ToRunStart,
        ZAxis_ReadyWait,
        TrayStatus_Check,
        YAxis_LoadVisionMove,
        YAxis_LoadVisionWait,
        ToRunDecision,
        ToRunEnd,
    }
}
