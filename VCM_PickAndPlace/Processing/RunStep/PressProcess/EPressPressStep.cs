#if USPCUTTING
namespace VCM_PickAndPlace.Processing
{
    public enum EPressPressStep
    {
        Start,
        PressDone_Check,
        PAxis_ReadyPosition_Check,
        PAxis_ReadyPosition_PrepareMove,
        PAxis_ReadyPosition_PrepareMoveWait,
        /// <summary>
        /// If PressDone -> Jump to <see cref="End"/>
        /// </summary>

        PressStart,
        Tray_PressPosition_Wait,
        PAxis_PressPosition1_Move,
        PAxis_PressPosition1_MoveWait,
        PAxis_PressPosition2_Move,
        PAxis_PressPosition2_MoveWait,
        PAxis_ReadyPosition_Move,
        PAxis_ReadyPosition_MoveWait,
        /// <summary>
        /// Update load tray PrepareDone status
        /// </summary>
        PressStatus_Update,
        PressEnd,

        End,
    }
}
#endif