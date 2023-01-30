namespace VCM_FullAssy.Processing
{
    public enum TrayUnloadVisionStep
    {
        UnloadVisionStart,
        ZAxis_ReadyWait,
        YAxis_UnloadVisionMove,
        YAxis_UnloadVisionWait,
        UnloadVision_ProcessWait,
        UnloadVisionEnd,
    }
}
