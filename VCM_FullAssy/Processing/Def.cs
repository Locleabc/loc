using TopUI.Define;
using VCM_FullAssy.Define;

namespace VCM_FullAssy.Processing
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
                if (CTray.CurrentLoadTray == null)
                {
                    return false;
                }
                else
                {
                    bool result = false;

                    int startCell = CTray.CurrentLoadTray.WorkStartIndex - CTray.CurrentLoadTray.FirstIndex;
                    startCell /= CTray.CurrentLoadTray.HeadCount;
                    startCell *= CTray.CurrentLoadTray.HeadCount;
                    startCell += CTray.CurrentLoadTray.FirstIndex;

                    for (int i = 0; i < CTray.CurrentLoadTray.HeadCount; i++)
                    {
                        result |= CTray.CurrentLoadTray.GetCellStatus(startCell + i) >= ECellStatus.NGPickOrPlace;
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
                if (CTray.CurrentUnloadTray == null)
                {
                    return false;
                }
                else
                {
                    bool result = false;

                    int startCell = CTray.CurrentUnloadTray.WorkStartIndex - CTray.CurrentUnloadTray.FirstIndex;
                    startCell /= CTray.CurrentUnloadTray.HeadCount;
                    startCell *= CTray.CurrentUnloadTray.HeadCount;
                    startCell += CTray.CurrentUnloadTray.FirstIndex;

                    for (int i = 0; i < CTray.CurrentUnloadTray.HeadCount; i++)
                    {
                        result |= CTray.CurrentUnloadTray.GetCellStatus(startCell + i) >= ECellStatus.NGPickOrPlace;
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

        Manual_LoadVision,
        Manual_Pick,
        Manual_UnderVision,
        Manual_UnloadVision,
        Manual_Place,

        Manual_TrayChange,
    }
}
