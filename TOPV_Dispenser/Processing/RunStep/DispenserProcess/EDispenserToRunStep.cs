namespace TOPV_Dispenser.Processing
{
    public enum EDispenserToRunStep
    {
        Start,
        ZAxis_ReadyPosition_Move,
        ZAxis_ReadyPosition_MoveWait,

        XAxisYAxis_VisionPosition_Move,
        XAxisYAxis_VisionPosition_MoveWait,

        End
    }
}