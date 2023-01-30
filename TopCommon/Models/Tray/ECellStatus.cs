namespace TopCom
{
    public enum ECellSimpleStatus
    {
        /// <summary>
        /// This cell is ready to work
        /// </summary>
        Ready,

        /// <summary>
        /// There is no material in the cell
        /// </summary>
        Skip,
        /// <summary>
        /// Work have done success
        /// </summary>
        Pass,
        /// <summary>
        /// Work have done with fail
        /// </summary>
        Fail,
    }

    public enum ECellAdvancedStatus
    {
        Empty = 0,
        /// <summary>
        /// Cutting status, etc
        /// </summary>
        PrepareDone,
        Processing,
        NGVision,
        OKVision,
        NGPickOrPlace,
        OK,
        InspNG,
        InspOK,
    }
}
