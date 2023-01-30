namespace VCM_PickAndPlace.Processing
{
    public enum EHeadUnloadVisionStep
    {
        Start,
        /// <summary>
        /// If load vision processed finish, skip (jump to End step)
        /// </summary>
        UnloadVision_DoneCheck,
        ZAxis_ReadyPosition_Check,
        ZAxis_ReadyPosition_Move,
        ZAxis_ReadyPosition_MoveWait,
        End,
    }
}
