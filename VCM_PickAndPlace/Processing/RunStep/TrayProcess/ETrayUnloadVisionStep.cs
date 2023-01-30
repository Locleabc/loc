namespace VCM_PickAndPlace.Processing
{
    public enum ETrayUnloadVisionStep
    {
        Start,
        /// <summary>
        /// If current tray is load tray, move directly to End step
        /// </summary>
        TrayType_Check,
        YAxis_WorkPosition_Move,
        YAxis_WorkPosition_MoveWait,
        End,
    }
}