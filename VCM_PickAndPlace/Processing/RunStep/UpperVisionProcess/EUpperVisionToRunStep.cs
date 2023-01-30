namespace VCM_PickAndPlace.Processing
{
    public enum EUpperVisionToRunStep
    {
        Start,
        ZAxis_ReadyPosition_Move,
        ZAxis_ReadyPosition_MoveWait,
        XAxis_WorkPosition_Move,
        XAxis_WorkPosition_MoveWait,
        End,
    }
}