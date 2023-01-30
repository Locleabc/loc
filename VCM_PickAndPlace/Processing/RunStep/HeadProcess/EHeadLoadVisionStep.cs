namespace VCM_PickAndPlace.Processing
{
    public enum EHeadLoadVisionStep
    {
        Start,
        /// <summary>
        /// If load vision processed finish, skip (jump to End step)
        /// </summary>
        LoadVision_DoneCheck,
        ZAxis_ReadyPosition_Check,
        ZAxis_ReadyPosition_Move,
        ZAxis_ReadyPosition_MoveWait,
        LoadVision_DoneWait,
        End,
    }
}
