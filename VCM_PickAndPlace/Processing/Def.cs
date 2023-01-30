using System;
using TopUI.Define;
using VCM_PickAndPlace.Define;

namespace VCM_PickAndPlace.Processing
{
    public static class HeadWorkStatus
    {
        public static bool Head1PickDone
        {
            get
            {
                if (CDef.CurrentLoadingTray == null)
                {
                    return true;
                }
                else
                {
                    try
                    {
                        int startCell = CDef.CurrentLoadingTray.WorkStartIndex - CDef.CurrentLoadingTray.FirstIndex;
                        startCell /= CDef.CurrentLoadingTray.HeadCount;
                        startCell *= CDef.CurrentLoadingTray.HeadCount;
                        startCell += CDef.CurrentLoadingTray.FirstIndex;

                        return CDef.CurrentLoadingTray.GetCellStatus(startCell + 1) >= ECellStatus.NGPickOrPlace;
                    }
                    catch (NullReferenceException)
                    {
                        return true;
                    }
                    catch (Exception)
                    {
                        return true;
                    }
                }
            }
        }

        public static bool Head2PickDone
        {
            get
            {
                if (CDef.CurrentLoadingTray == null)
                {
                    return true;
                }
                else
                {
                    try
                    {
                        int startCell = CDef.CurrentLoadingTray.WorkStartIndex - CDef.CurrentLoadingTray.FirstIndex;
                        startCell /= CDef.CurrentLoadingTray.HeadCount;
                        startCell *= CDef.CurrentLoadingTray.HeadCount;
                        startCell += CDef.CurrentLoadingTray.FirstIndex;

                        return CDef.CurrentLoadingTray.GetCellStatus(startCell) >= ECellStatus.NGPickOrPlace;
                    }
                    catch (NullReferenceException)
                    {
                        return true;
                    }
                    catch (Exception)
                    {
                        return true;
                    }
                }
            }
        }

        public static bool PickDone
        {
            get { return Head1PickDone & Head2PickDone; }
        }

        public static bool Head1UnderInspectDone { get; set; }
        public static bool Head2UnderInspectDone { get; set; }
        public static bool UnderInspectDone
        {
            get { return Head1UnderInspectDone & Head2UnderInspectDone; }
        }

        public static bool Head1PlaceDone
        {
            get
            {
                if (CDef.CurrentUnloadingTray == null)
                {
                    return true;
                }
                else
                {
                    try
                    {
                        int startCell = CDef.CurrentUnloadingTray.WorkStartIndex - CDef.CurrentUnloadingTray.FirstIndex;
                        startCell /= CDef.CurrentUnloadingTray.HeadCount;
                        startCell *= CDef.CurrentUnloadingTray.HeadCount;
                        startCell += CDef.CurrentUnloadingTray.FirstIndex;

                        return CDef.CurrentUnloadingTray.GetCellStatus(startCell + 1) >= ECellStatus.NGPickOrPlace;
                    }
                    catch (NullReferenceException)
                    {
                        return true;
                    }
                    catch (Exception)
                    {
                        return true;
                    }
                }
            }
        }

        public static bool Head2PlaceDone
        {
            get
            {
                if (CDef.CurrentUnloadingTray == null)
                {
                    return true;
                }
                else
                {
                    try
                    {
                        int startCell = CDef.CurrentUnloadingTray.WorkStartIndex - CDef.CurrentUnloadingTray.FirstIndex;
                        startCell /= CDef.CurrentUnloadingTray.HeadCount;
                        startCell *= CDef.CurrentUnloadingTray.HeadCount;
                        startCell += CDef.CurrentUnloadingTray.FirstIndex;

                        return CDef.CurrentUnloadingTray.GetCellStatus(startCell) >= ECellStatus.NGPickOrPlace;
                    }
                    catch (NullReferenceException)
                    {
                        return true;
                    }
                    catch (Exception)
                    {
                        return true;
                    }
                }
            }
        }

        public static bool PlaceDone
        {
            get { return Head1PlaceDone & Head2PlaceDone; }
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

    public enum ETrays
    {
        LoadTray,
        UnloadTray,
    }

    public enum EHeads
    {
        /// <summary>
        /// Head left in front view
        /// </summary>
        Head1,
        /// <summary>
        /// Head right in front view
        /// </summary>
        Head2,
    }

    public enum ERunMode
    {
        Stop,
        AutoRun,

#if USPCUTTING
        Manual_Press,
#endif
        Manual_LoadVision,
        Manual_Pick,
        Manual_UnderVision,
        Manual_UnloadVision,
        Manual_Place,
        Manual_Inspect,

        Manual_TrayChange,
    }
}
