namespace VCM_PickAndPlace.Processing
{
    public enum ETransferInspectStep
    {
        Start,

        HeadZ_ReadyWait,

        Transfer_InspectPosition_Move,
        Transfer_InspectPosition_MoveWait,

        CheckIfInspectAllDone,

        End
    }
}
