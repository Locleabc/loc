namespace VCM_PickAndPlace.Processing
{
    public enum ETransferHomeStep
    {
        Start,
        AllZAxis_HomeWait,
        /// <summary>
        /// X Axis and XX Axis Move
        /// </summary>
        XAxis_HomeSearch,
        XAxis_HomeSearchWait,
        End,
    }
}
