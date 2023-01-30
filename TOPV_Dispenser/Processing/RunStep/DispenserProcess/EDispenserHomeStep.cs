namespace TOPV_Dispenser.Processing
{
    public enum EDispenserHomeStep
    {
        Start,
        ZAxis_HomeSearch,
        ZAxis_HomeWait,

        XAxisYAxis_HomeSearch,
        XAxisYAxis_HomeSearchWait,

        End
    }
}