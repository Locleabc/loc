namespace VCM_FullAssy.Processing
{
    public enum TrayPlaceStep
    {
        PlaceStart,
        ZAxis_ReadyWait,
        YAxis_PlaceMove,
        YAxis_PlaceMoveWait,
        Place_ProcessWait,
        PlaceEnd,
    }
}
