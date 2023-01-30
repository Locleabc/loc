namespace VCM_PickAndPlace.Processing
{
    public enum EUpperVisionUnloadVisionStep
    {
        Start,
        /// <summary>
        /// If load vision processed finish, skip (jump to End step)
        /// </summary>
        UnloadVision_DoneCheck,
        XAxis_Head1_UnloadVision_PositionMove,
        XAxis_Head1_UnloadVision_PositionMoveWait,
        Transfer_VisionAvoid_PositionWait,
        Tray_Work_PositionWait,

        Head1_GrabImage,
        Head1_GrabImageResult,
        Head1_VisionInspect,
        Head1_VisionInspectWait,
        Head1_VisionInspectResultApply,

        XAxis_Head2_UnloadVision_PositionMove,
        XAxis_Head2_UnloadVision_PositionMoveWait,

        Head2_GrabImage,
        Head2_GrabImageResult,
        Head2_VisionInspect,
        Head2_VisionInspectWait,
        Head2_VisionInspectResultApply,

        StatusUpdate,

        End,
    }
}