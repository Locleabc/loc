namespace VCM_PickAndPlace.Processing
{
    public enum ETransferUnderVisionStep
    {
        Start,
        HeadZAxis_ReadyPosition_Wait,

        /// <summary>
        /// If unload vision need to avoid, move to <see cref="XAxis_AvoidPosition_Check"/>; Else Move to <see cref="XAxisXXAxis_UnderVision2_PositionMove"/>
        /// </summary>
        CheckFlag_UnloadVision_AvoidRequest,
        XAxis_AvoidPosition_Check,
        XAxis_AvoidPosition_Move,
        XAxis_AvoidPosition_Wait,
        Vision_DoneWait,

        XAxisXXAxis_UnderVision2_PositionMove,
        XAxisXXAxis_UnderVision2_PositionMoveWait,

        UnderVision2_InspectDoneWait,

        XAxisXXAxis_UnderVision1_PositionMove,
        XAxisXXAxis_UnderVision1_PositionMoveWait,

        UnderVision1_InspectDoneWait,

        End,
    }
}
