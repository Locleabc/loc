namespace VCM_PickAndPlace.Processing
{
    public enum ETrayToRunStep
    {
        Start,
        AllZAxis_ReadyWait,

        RootProcess_ModeCheck,

        /// <summary>
        /// If Load tray, jump to YAxis_PressPosition_Move -> Wait -> Then end
        /// Else jump to YAxis_WorkPosition_Move
        /// </summary>
        TrayType_Check,

#if USPCUTTING
        /// <summary>
        /// If Press Done -> Jump directly to "YAxis_WorkPosition_Move"
        /// </summary>
        PressCheck,
        YAxis_PressPosition_Move,
        YAxis_PressPosition_MoveWait,
#endif

        YAxis_WorkPosition_Move,
        YAxis_WorkPosition_MoveWait,
        End,
    }
}