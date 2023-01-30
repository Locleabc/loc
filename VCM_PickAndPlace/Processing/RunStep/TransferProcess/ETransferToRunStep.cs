namespace VCM_PickAndPlace.Processing
{
    public enum ETransferToRunStep
    {
        Start,
        AllZAxis_ReadyPosition_Wait,
        /// <summary>
        /// X Axis and XX Axis Move
        /// </summary>
        XAxis_WorkPosition_Move,
        XAxis_WorkPosition_MoveWait,
        End,
    }
}
