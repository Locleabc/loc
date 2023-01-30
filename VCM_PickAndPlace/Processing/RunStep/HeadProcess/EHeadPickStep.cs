namespace VCM_PickAndPlace.Processing
{
    public enum EHeadPickStep
    {
        Start,

        ZAxis_ReadyPosition_Move,
        ZAxis_ReadyPosition_MoveWait,

        PickPlace_DoneCheck,

        YAxisTAxis_Pick_PositionMove,
        YAxisTAxis_Pick_PositionMoveWait,

        Transfer_Pick_PositionWait,
        Tray_Work_PositionMoveWait,

        ZAxis_PickPosition1_Move,
        ZAxis_PickPosition1_MoveWait,
        ZAxis_PickPosition2_Move,
        ZAxis_PickPosition2_MoveWait,

        Head_VAC_On,
        Head_VAC_Delay,

        /// <summary>
        /// Z Axis up position is ReadyPosition
        /// </summary>
        ZAxis_UpPosition_Move,
        ZAxis_UpPosition_MoveWait,

        StatusUpdate,

        Wait_TrayAndTransfer_ReachToEnd,

        End,
    }
}
