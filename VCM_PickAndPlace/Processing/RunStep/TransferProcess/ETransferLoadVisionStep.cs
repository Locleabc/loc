namespace VCM_PickAndPlace.Processing
{
    public enum ETransferLoadVisionStep
    {
        Start,
        /// <summary>
        /// If load vision processed finish, skip (jump to End step)
        /// </summary>
        LoadVision_DoneCheck,
        HeadZAxis_ReadyPosition_Wait,
        /// <summary>
        /// If transfer is not in avoid position then move to avoid position.
        /// </summary>
        XAxis_VisionAvoid_PositionCheck,
        XAxis_VisionAvoid_PositionMove,
        XAxis_VisionAvoid_PositionMoveWait,
        LoadVision_DoneWait,
        End,
    }
}
