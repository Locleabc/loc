namespace VCM_FullAssy.Processing
{
    public enum TrayPickStep
    {
        TrayPickStart,
        ZAxis_ReadyWait,
        YAxis_PickMove,
        YAxis_PickMoveWait,
        Pick_ProcessWait,
        TrayPickEnd,
    }
}
