namespace VCM_PickAndPlace.Processing
{
    public enum EHeadToRunStep
    {
        Start,
        ZAxis_ReadyPosition_Move,
        ZAxis_ReadyPosition_MoveWait,
        AllZAxis_ReadyWait,
        YAxisTAxis_ReadyPosition_Move,
        YAxisTAxis_ReadyPosition_MoveWait,
        End,
    }
}
