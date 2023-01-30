namespace VCM_PickAndPlace.Processing
{
    public enum ETrayPlaceStep
    {
        Start,
        /// <summary>
        /// If current tray is load tray, move directly to End step
        /// </summary>
        TrayType_Check,

        HeadZ_ReadyWait,

        YAxis_WorkPosition_Move,
        YAxis_WorkPosition_MoveWait,

        Place_Wait,

        End,
    }
}