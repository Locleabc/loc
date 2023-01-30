namespace VCM_PickAndPlace.Processing
{
    public enum EUpperVisionInspectStep
    {
        Start,
        Vision_InspectPosion_Move,
        Vision_InspectPosion_MoveWait,
        TrayHeadTransfer_InspectPosition_MoveWait,

        GrabImage,
        GrabImageResult,
        Inspect,
        Inspect_Wait,

        Inspect_ResultUpdate,

        CheckIfInspectAllDone,
        End
    }
}