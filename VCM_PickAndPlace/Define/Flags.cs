using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VCM_PickAndPlace.Define
{
    public static class Flags
    {
        public static bool AllHead_ZReady
        {
            get
            {
                return CDef.RootProcess.Head1Process.Flag_ZAxisReady && CDef.RootProcess.Head2Process.Flag_ZAxisReady;
            }
        }

        public static bool AllZAxis_Ready
        {
            get
            {
#if USPCUTTING
                return AllHead_ZReady && CDef.RootProcess.PressProcess.Flag_PAxisReady;
#else
                return AllHead_ZReady;
#endif
            }
        }

        #region Press
#if USPCUTTING
        public static bool PressAxis_Ready
        {
            get
            {
                return CDef.RootProcess.PressProcess.Flag_PAxisReady;
            }
        }
#endif
        #endregion

        #region Tray
#if USPCUTTING
        public static bool TrayPress_Position
        {
            get
            {
                return CDef.RootProcess.LoadTrayProcess.Flag_TrayPress_Position;
            }
        }
#endif

        public static bool TrayLoad_WorkPosition
        {
            get
            {
                return CDef.RootProcess.LoadTrayProcess.Flag_TrayWork_Position;
            }
        }

        public static bool TrayUnload_WorkPosition
        {
            get
            {
                return CDef.RootProcess.UnloadTrayProcess.Flag_TrayWork_Position;
            }
        }

        public static bool TrayLoad_PickDone
        {
            get
            {
                return CDef.RootProcess.LoadTrayProcess.Flag_Tray_PickPlace_Done;
            }
            set
            {
                CDef.RootProcess.LoadTrayProcess.Flag_Tray_PickPlace_Done = value;
            }
        }

        public static bool TrayUnload_PlaceDone
        {
            get
            {
                return CDef.RootProcess.UnloadTrayProcess.Flag_Tray_PickPlace_Done;
            }
            set
            {
                CDef.RootProcess.UnloadTrayProcess.Flag_Tray_PickPlace_Done = value;
            }
        }
        #endregion

        #region Transfer
        public static bool Transfer_LoadVision_Avoid_Position
        {
            get
            {
                return CDef.RootProcess.TransferProcess.Flag_Transfer_LoadVision_Avoid_Position;
            }
        }

        public static bool Transfer_UnloadVision_Avoid_Position
        {
            get
            {
                return CDef.RootProcess.TransferProcess.Flag_Transfer_UnloadVision_Avoid_Position;
            }
        }

        public static bool Transfer_Pick_Position
        {
            get
            {
                return CDef.RootProcess.TransferProcess.Flag_Transfer_Pick_Position;
            }
        }

        public static bool Transfer_UnderVision_Head1_Position
        {
            get
            {
                return CDef.RootProcess.TransferProcess.Flag_Transfer_UnderVision1_Position;
            }
        }

        public static bool Transfer_UnderVision_Head2_Position
        {
            get
            {
                return CDef.RootProcess.TransferProcess.Flag_Transfer_UnderVision2_Position;
            }
        }

        public static bool Transfer_Place_Position
        {
            get
            {
                return CDef.RootProcess.TransferProcess.Flag_Transfer_Place_Position;
            }
        }

        public static bool Transfer_PickDone
        {
            get
            {
                return CDef.RootProcess.TransferProcess.Flag_Transfer_Pick_Done;
            }
            set
            {
                CDef.RootProcess.TransferProcess.Flag_Transfer_Pick_Done = value;
            }
        }

        public static bool Transfer_PlaceDone
        {
            get
            {
                return CDef.RootProcess.TransferProcess.Flag_Transfer_Place_Done;
            }
            set
            {
                CDef.RootProcess.TransferProcess.Flag_Transfer_Place_Done = value;
            }
        }
        #endregion

        #region UpperVision
        public static bool LoadInspect_Done
        {
            get { return CDef.RootProcess.UpperVisionProcess.Flag_LoadInspect_Done; }
        }

        public static bool UnloadInspect_Done
        {
            get { return CDef.RootProcess.UpperVisionProcess.Flag_UnloadInspect_Done; }
        }

        public static bool LoadVision_AvoidRequest
        {
            get
            {
                if (CDef.GlobalRecipe.SkipLoadVision) return false;
                else
                {
                    return CDef.RootProcess.UpperVisionProcess.Flag_LoadVision_AvoidRequest;
                }
            }
        }

        public static bool UnloadVision_AvoidRequest
        {
            get
            {
                if (CDef.GlobalRecipe.SkipUnloadVision) return false;
                else
                {
                    return CDef.RootProcess.UpperVisionProcess.Flag_UnloadVision_AvoidRequest;
                }
            }
        }
        #endregion

        #region HEAD
        public static bool Head_UnderVision_Position
        {
            get { return CDef.RootProcess.Head1Process.Flag_UnderVision_Position & CDef.RootProcess.Head2Process.Flag_UnderVision_Position; }
        }
        #endregion

        #region UnderVision
        public static bool UnderVision_Inspect_Done
        {
            get
            {
                return UnderVision_Inspect_Head1_Done && UnderVision_Inspect_Head2_Done;
            }
            set
            {
                CDef.RootProcess.UnderVisionProcess.Flag_UnderVision_Inspect_Head1_Done = value;
                CDef.RootProcess.UnderVisionProcess.Flag_UnderVision_Inspect_Head2_Done = value;
            }
        }

        public static bool UnderVision_Inspect_Head1_Done
        {
            get
            {
                return CDef.RootProcess.UnderVisionProcess.Flag_UnderVision_Inspect_Head1_Done;
            }
        }

        public static bool UnderVision_Inspect_Head2_Done
        {
            get
            {
                return CDef.RootProcess.UnderVisionProcess.Flag_UnderVision_Inspect_Head2_Done;
            }
        }
        #endregion

        #region Inspect
        public static class Inspect
        {
            public static bool Tray_InspectPosition
            {
                get
                {
                    return CDef.RootProcess.UnloadTrayProcess.Flag_Tray_InspectPosition;
                }
            }

            public static bool Transfer_InspectPosition
            {
                get
                {
                    return CDef.RootProcess.TransferProcess.Flag_Transfer_Inspect_Position;
                }
            }

            public static bool IsAllCellInspectDone
            {
                get
                {
                    return InspectIndex == -1;
                }
            }

            /// <summary>
            /// TRAY 2
            /// <br>72 70 68 71 69 67</br>
            /// <br>...</br>
            /// <br>48 46 44 47 45 43</br>
            /// <br>42 40 38 41 39 37</br>
            /// <br/>
            /// <br>TRAY 1</br>
            /// <br>36 34 32 35 33 31</br>
            /// <br>...</br>
            /// <br>12 10  8 11  9  7</br>
            /// <br> 6  4  2  5  3  1</br>
            /// </summary>
            public static int InspectIndex
            {
                get
                {
                    int index = -1;

                    bool inspectIndexDetected = false;

                    try
                    {
                        if (CDef.UnloadingTrays[0].IsEnable &&
                            CDef.UnloadingTrays[0].Cells.Any(cell => cell.CellInfo.CellStatus <= TopUI.Define.ECellStatus.OK))
                        {
                            int firstCellID = CDef.UnloadingTrays[0].Cells.OrderByDescending(c => c.CellInfo.CellID).First(cell => cell.CellInfo.CellStatus <= TopUI.Define.ECellStatus.OK).CellInfo.CellIndex;

                            index = firstCellID;
                            inspectIndexDetected = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        TopCom.LOG.UILog.Error(ex.Message);
                    }

                    if (inspectIndexDetected == false)
                    {
                        try
                        {
                            if (CDef.UnloadingTrays[1].IsEnable &&
                                CDef.UnloadingTrays[1].Cells.Any(cell => cell.CellInfo.CellStatus <= TopUI.Define.ECellStatus.OK))
                            {
                                int firstCellID = CDef.UnloadingTrays[1].Cells.OrderByDescending(c => c.CellInfo.CellID).First(cell => cell.CellInfo.CellStatus <= TopUI.Define.ECellStatus.OK).CellInfo.CellIndex;

                                index = firstCellID + CDef.UnloadingTrays[1].RowCount * CDef.UnloadingTrays[1].ColumnCount;
                            }
                        }
                        catch (Exception ex)
                        {
                            TopCom.LOG.UILog.Error(ex.Message);
                        }
                    }

                    return index;
                }
            }

            /// <summary>
            /// 0 -> (ColumnCount - 1)
            /// </summary>
            public static int InspectColumn
            {
                get
                {
                    int index = InspectIndex;
                    int columnCount = CDef.UnloadingTrays[0].ColumnCount;

                    if (index == -1) return -1;

                    return (((index - 1) % columnCount) / 2)
                         + (1 - (index % 2)) * (columnCount / CDef.UnloadingTrays[0].HeadCount);
                }
            }

            /// <summary>
            /// Range [-1] or [1 -> 12]
            /// <br/> [-1] all tray is inspected
            /// <br/>[0 -> 5] <see cref="CDef.UnloadingTray1"/>
            /// <br/>[6 -> 11] <see cref="CDef.UnloadingTray2"/>
            /// </summary>
            public static int InspectRow
            {
                get
                {
                    int index = InspectIndex;

                    if (index == -1) return -1;

                    return (index - 1) / CDef.UnloadingTrays[0].ColumnCount;
                }
            }
        }
        #endregion
    }
}
