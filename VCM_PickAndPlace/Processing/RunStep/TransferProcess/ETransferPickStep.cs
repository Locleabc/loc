namespace VCM_PickAndPlace.Processing
{
    public enum ETransferPickStep
    {
        Start,
        PickPlace_DoneCheck,
        HeadZAxis_ReadyPosition_Wait,

        /// <summary>
        /// If vision need to avoid move to <see cref="XAxis_AvoidPosition_Check"/>; Else Move to <see cref="XAxisXXAxis_Pick_PositionMove"/>
        /// </summary>
        CheckFlag_Vision_AvoidRequest,
        XAxis_AvoidPosition_Check,
        XAxis_AvoidPosition_Move,
        XAxis_AvoidPosition_Wait,
        Vision_DoneWait,

        XAxisXXAxis_Pick_PositionMove,
        XAxisXXAxis_Pick_PositionMoveWait,

        PickPlace_DoneWait,
        XXAxis_UnderVision_PreMove,

        End,
    }
}
