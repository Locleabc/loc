namespace PLV_BracketAssemble.Processing
{
    public enum EHeadToRunStep
    {
        Start,
        CheckIf_CMExistOnPreAlignUnit,
        AllPickerCylinderUp,
        ClampCylinder_Backward,
        AllPickerCylinderUp_Wait,
        ClampCylinder_Backward_Wait,

        Set_PickerPickPlaceDone_Status,

        End,
    }
}