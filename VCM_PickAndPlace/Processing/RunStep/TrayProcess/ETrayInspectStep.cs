namespace VCM_PickAndPlace.Processing
{
    public enum ETrayInspectStep
    {
        Start,
        HeadZ_ReadyWait,

        YAxis_InspectPosition_Move,
        YAxis_InspectPosition_MoveWait,

        CheckIfInspectAllDone,

        End
    }
}