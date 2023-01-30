namespace VCM_PickAndPlace.Processing
{
    public enum ETrayTrayChangeStep
    {
        Start,

        AllZAxis_ReadyWait,

        YAxis_TrayChangePosition_Move,
        YAxis_TrayChangePosition_MoveWait,

        CheckIf_ManualTrayChange,
        TrayChangeMessage_Display,

        End,
    }
}