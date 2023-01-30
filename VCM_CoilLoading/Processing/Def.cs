using TopUI.Define;
using VCM_CoilLoading.Define;

namespace VCM_CoilLoading.Processing
{
    public static class PickerWorkStatus
    {
        public static bool Head1PickDone { get; }
        public static bool Head2PickDone { get; }

        public static bool PickDone
        {
            //get { return Head1PickDone | Head2PickDone; }
            get
            {
                if (CDef.CurrentLoadingTray == null)
                {
                    return false;
                }
                else
                {
                    bool result = false;

                    int startCell = CDef.CurrentLoadingTray.WorkStartIndex - CDef.CurrentLoadingTray.FirstIndex;
                    startCell /= CDef.CurrentLoadingTray.HeadCount;
                    startCell *= CDef.CurrentLoadingTray.HeadCount;
                    startCell += CDef.CurrentLoadingTray.FirstIndex;

                    for (int i = 0; i < CDef.CurrentLoadingTray.HeadCount; i++)
                    {
                        result |= CDef.CurrentLoadingTray.GetCellStatus(startCell + i) >= ECellStatus.NGPickOrPlace;
                    }

                    return result;
                }
            }
        }

        public static bool Head1UnderInspectDone { get; set; }
        public static bool Head2UnderInspectDone { get; set; }
        public static bool UnderInspectDone
        {
            get { return Head1UnderInspectDone & Head2UnderInspectDone; }
        }

        public static bool Head1PlaceDone { get; }
        public static bool Head2PlaceDone { get; }
        public static bool PlaceDone
        {
            //get { return Head1PlaceDone | Head2PlaceDone; }
            get
            {
                if (CDef.CurrentUnloadingTray == null)
                {
                    return false;
                }
                else
                {
                    bool result = false;

                    int startCell = CDef.CurrentUnloadingTray.WorkStartIndex - CDef.CurrentUnloadingTray.FirstIndex;
                    startCell /= CDef.CurrentUnloadingTray.HeadCount;
                    startCell *= CDef.CurrentUnloadingTray.HeadCount;
                    startCell += CDef.CurrentUnloadingTray.FirstIndex;

                    for (int i = 0; i < CDef.CurrentUnloadingTray.HeadCount; i++)
                    {
                        result |= CDef.CurrentUnloadingTray.GetCellStatus(startCell + i) >= ECellStatus.NGPickOrPlace;
                    }

                    return result;
                }
            }
        }

        public static void Clear()
        {
            Head1UnderInspectDone = false;
            Head2UnderInspectDone = false;
        }
    }

    public static class UpperVisionWorkStatus
    {
        public static bool LoadHead1InspectDone { get; set; }
        public static bool LoadHead2InspectDone { get; set; }
        public static bool UnloadHead1InspectDone { get; set; }
        public static bool UnloadHead2InspectDone { get; set; }

        public static void Clear()
        {
            LoadHead1InspectDone = false;
            LoadHead2InspectDone = false;
            UnloadHead1InspectDone = false;
            UnloadHead2InspectDone = false;
        }

        public static bool IsLoadInspectDone()
        {
            return LoadHead1InspectDone & LoadHead2InspectDone;
        }

        public static bool IsUnloadInspectDone()
        {
            return UnloadHead1InspectDone & UnloadHead2InspectDone;
        }
    }

    public enum TargetTray
    {
        LoadingTray,
        UnloadingTray,
        Both
    }

    public enum ERunMode
    {
        Stop,
        AutoRun,

        #region Picker
        Manual_Picker_Pick,
        Manual_Picker_Place,
        Manual_UnderVision_Inspect_1,
        Manual_UnderVision_Inspect_2,
        Manual_LoadingTray_Change,
        Manual_UnloadingTray_Change,
        Manual_AllTray_Change,

        Manual_LoadingTray_Load,
        Manual_UnloadingTray_Load,
        Manual_AllTray_Load,
        #endregion

        #region UnderVision
        Manual_UpperVision_Load_Inspect,
        Manual_UpperVision_Unload_Inspect,
        #endregion
    }
}
