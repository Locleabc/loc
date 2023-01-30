namespace VCM_PickAndPlace.Processing
{
    public enum ETransferUnloadVisionStep
    {
        Start,
        /// <summary>
        /// If unload vision processed finish, skip (jump to End step)
        /// </summary>
        UnloadVision_DoneCheck,
        HeadZAxis_ReadyPosition_Wait,
        /// <summary>
        /// If transfer is not in avoid position then move to avoid position.
        /// </summary>
        XAxis_VisionAvoid_PositionCheck,
        XAxis_VisionAvoid_PositionMove,
        XAxis_VisionAvoid_PositionMoveWait,
        End,
    }
}
