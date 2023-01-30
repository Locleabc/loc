using System;
using TopUI.Define;
using PLV_BracketAssemble.Define;

namespace PLV_BracketAssemble.Processing
{
    public enum EPickers
    {
        /// <summary>
        /// Head left in front view
        /// </summary>
        Picker1,
        /// <summary>
        /// Head right in front view
        /// </summary>
        Picker2,
    }

    public enum ERunMode
    {
        Stop,
        AutoRun,

        Manual_Change,
        Manual_Pick,
        Manual_PreAlign,
        Manual_UnderVision,
        Manual_Place,
    }
}
