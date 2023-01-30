namespace VCM_PickAndPlace.Processing
{
    public enum ETrayPickStep
    {
        Start,

        /// <summary>
        /// If current tray is unload tray, move directly to End step
        /// </summary>
        TrayType_Check,

        NewIndex_PickPlace_Wait,

        HeadZ_ReadyWait,

        YAxis_WorkPosition_Move,
        YAxis_WorkPosition_MoveWait,

        Pick_Wait,

        End,
    }
}