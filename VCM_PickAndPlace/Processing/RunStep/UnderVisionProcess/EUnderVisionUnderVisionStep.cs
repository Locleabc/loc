namespace VCM_PickAndPlace.Processing
{
    public enum EUnderVisionUnderVisionStep
    {
        Start,

        UnderVision_ProcessDoneCheck,

        Head_UnderVision_PositionWait,

        Transfer_UnderVision_PositionWait,
        UnderVision_Start,
        GrabImage,
        GrabImageResult,
        VisionInspect,
        VisionInspectWait,
        UnderVision_EndCheck,
        VisionOffset_Move,
        VisionOffset_MoveDone,
        VisionInspect_Retry,
        VisionInspectResultApply,
        StatusUpdate,

        Check_CurrentHead,

        End,
    }
}