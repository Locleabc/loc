namespace VCM_PickAndPlace.Processing
{
    public enum EHeadUnderVisionStep
    {
        Start,
        ZAxis_ReadyPosition_Check,
        ZAxis_ReadyPosition_Move,
        ZAxis_ReadyPosition_MoveWait,
        YTAxis_UnderVision_Move,
        YTAxis_UnderVision_MoveWait,

        Transfer_UnderVision2_PositionMoveWait,

        ZAxis_UnderVision_Move,
        ZAxis_UnderVision_MoveWait,

        UnderVision1_InspectDoneWait,

        End,
    }
}
