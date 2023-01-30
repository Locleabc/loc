#if USPCUTTING
namespace VCM_PickAndPlace.Processing
{
    public enum EPressToRunStep
    {
        Start,
        PAxis_ReadyPosition_Move,
        PAxis_ReadyPosition_MoveWait,
        End,
    }
}
#endif