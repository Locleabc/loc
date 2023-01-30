namespace VCM_PickAndPlace.Processing
{
    public enum ETrayPressStep
    {
        Start,
        AllZAxis_ReadyPosition_Wait,
        /// <summary>
        /// If Current tray is Unload tray -> Jump directly to "YAxis_WorkPosition_Move"
        /// </summary>
        TrayType_Check,
        /// <summary>
        /// If Press Done -> Jump directly to "YAxis_WorkPosition_Move"
        /// </summary>
        PressDone_Check,

        /// <summary>
        /// 
        /// </summary>
        PressStart,
        PressAxis_ReadyPosition_Wait,
        YAxis_PressPosition_Move,
        YAxis_PressPosition_Wait,
        PressWait,
        /// <summary>
        /// Check tray status, if Press is not done yet, move to "PressAxis_ReadyPosition_Wait" step
        /// </summary>
        PressStatus_Check,
        YAxis_ChangePosition_Move,
        YAxis_ChangePosition_MoveWait,
        /// <summary>
        /// Dummy sheet remove message display
        /// </summary>
        Message_Display,
        /// <summary>
        /// Set RootProcess RunMode to STOP, ignore next step(s)
        /// </summary>
        PressEnd,

        YAxis_WorkPosition_Move,
        YAxis_WorkPosition_MoveWait,
        End,
    }
}