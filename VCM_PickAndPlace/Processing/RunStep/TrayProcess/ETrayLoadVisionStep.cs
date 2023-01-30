namespace VCM_PickAndPlace.Processing
{
    public enum ETrayLoadVisionStep
    {
        Start,
        AllZAxis_ReadyPosition_Wait,
        /// <summary>
        /// If current tray is unload tray, move directly to End step
        /// </summary>
        TrayType_Check,
        YAxis_WorkPosition_Move,
        YAxis_WorkPosition_MoveWait,
        End,
    }
}