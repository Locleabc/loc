namespace VCM_PickAndPlace.Processing
{
    public enum ETransferPlaceStep
    {
        Start,
        HeadZAxis_ReadyPosition_Wait,
        XAxisXXAxis_Place_PositionMove,
        XAxisXXAxis_Place_PositionMoveWait,
        End,
    }
}
