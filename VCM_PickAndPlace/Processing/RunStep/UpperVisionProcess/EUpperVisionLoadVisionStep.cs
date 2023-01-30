namespace VCM_PickAndPlace.Processing
{
    public enum EUpperVisionLoadVisionStep
    {
        Start,
        /// <summary>
        /// If load vision processed finish, skip (jump to End step)
        /// </summary>
        LoadVision_DoneCheck,

        Set_Flag_Vision_AvoidRequest,

        XAxis_Head1_LoadVision_PositionMove,
        XAxis_Head1_LoadVision_PositionMoveWait,
        Transfer_VisionAvoid_PositionWait,
        Tray_Work_PositionWait,
        ZAxis_LoadVision_Position,
        ZAxis_LoadVision_PositionWait,

        GrabImage,
        GrabImageResult,
        VisionInspect,
        VisionInspectWait,
        VisionInspectResultApply,

        Check_CurrentHead,

        XAxis_Head2_LoadVision_PositionMove,
        XAxis_Head2_LoadVision_PositionMoveWait,

        StatusUpdate,

        End,
    }
}