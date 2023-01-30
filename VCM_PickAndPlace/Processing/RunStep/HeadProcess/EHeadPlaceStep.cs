namespace VCM_PickAndPlace.Processing
{
    public enum EHeadPlaceStep
    {
        Start,

        ZAxis_ReadyPosition_Move,
        ZAxis_ReadyPosition_MoveWait,

        Transfer_Pplace_PositionWait,
        Tray_Work_PositionMoveWait,

        YAxisTAxis_Place_PositionMove,
        YAxisTAxis_Place_PositionMoveWait,
        ZAxis_PlacePosition1_Move,
        ZAxis_PlacePosition1_MoveWait,
        ZAxis_PlacePosition2_Move,
        ZAxis_PlacePosition2_MoveWait,

        Head_Purge_On,
        Head_Purge_Delay,

        /// <summary>
        /// Z Axis up position is ReadyPosition
        /// </summary>
        ZAxis_UpPosition_Move,
        ZAxis_UpPosition_MoveWait,

        StatusUpdate,

        End,
    }
}
