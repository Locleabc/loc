namespace VCM_FullAssy.Processing
{
    public enum TrayLoadVisionStep
    {
        LoadVisionStart,
        ZAxis_ReadyWait,
        Wait_TrayChangeOrPickPlace,
        YAxis_LoadVisionMove,
        YAxis_LoadVisionWait,
        LoadVision_ProcessWait,
        LoadVisionEnd,
    }
}
