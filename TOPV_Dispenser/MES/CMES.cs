using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopCom;
using TopCom.MES;
using TOPV_Dispenser.Define;
using XSmNetW;
using WindowsFormsApplication1;

namespace TOPV_Dispenser.MES
{
    public class CMES : CMESBase
    {
        public override void Send_PCInfoData()
        {
            XValue xValue = new XValue();
            XValue xList1 = new XValue();
            XValue xList2 = new XValue();
            XValue xList3 = new XValue();
            XValue xTemp = new XValue();

            // 함수테스트
            xValue.SetList();           // LIST 2
            {
                xTemp.SetString("EQUIP_PC_INFO"); xValue.Add(xTemp);             // MESSAGE NAME //고정
                xList1.SetList();                                                   // LIST 3
                {
                    xTemp.SetString(MESInfo.Head1.DeviceCode); xList1.Add(xTemp);      // EQUIPMENT ID //장비 아이디를 1번이라도 못 받으면 알람
                    xList2.SetList();                                               // LIST 2
                    {

                        xTemp.SetString(""); xList2.Add(xTemp);            //OS
                        xTemp.SetString(""); xList2.Add(xTemp);            //BIT
                        xTemp.SetString(""); xList2.Add(xTemp);            //IP
                        xTemp.SetString(""); xList2.Add(xTemp);            //MAC ADDRESS
                        xTemp.SetString(MachineInfor.SoftwareVersion); xList2.Add(xTemp); //핸들러 버전
                        xTemp.SetString(MachineInfor.SoftwareVersion); xList2.Add(xTemp); //비전 버전
                    }
                    xList1.Add(xList2);
                }
                xValue.Add(xList1);
            }

            int len = xValue.GetByteLength();
            byte[] sendData = new byte[len];

            xValue.MakeSECS2Stream(ref sendData, 0, len);

            int ret = this._xcom.SendMsg(sendData, sendData.Length);
        }

        /// <summary>
        /// PD File write function
        /// </summary>
        /// <param name="headNo">Head Number start with 0</param>
        public override void SavePD(int headNo)
        {
            if (MESInfo.UseMES == false) return;

            try
            {
                DateTime now = DateTime.Now;
                string strDate = now.ToString("yyyyMMdd");
                string strDateTime = now.ToString("yyyyMMddHHmmss");

                // D:\MES\EQUIP\<Date>\PD_<DateTime>_Z<headNo>.csv
                string strFileName = Path.Combine(GlobalFolders.FolderMESEquip, strDate, $"PD_{strDateTime}_Z{headNo}.csv");
                Directory.CreateDirectory(Path.GetDirectoryName(strFileName));

                string strRESULT = "OK";
                //if (Datas.Data_UnderInspect_Result[headNo].Judge == TopVision.EVisionJudge.NG ||
                //    Datas.PlaceResult[headNo] == false)
                //{
                //    strRESULT = "NG";
                //}

                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"VER,VER-001");
                sb.AppendLine($"EQUIP_ID,{MESInfo.Head1.EQPCode}");
                sb.AppendLine($"DEVICE,{MESInfo.Head1.DeviceCode}");
                sb.AppendLine($"OPERATION,{MESInfo.Head1.OperationCode}");
                sb.AppendLine($"PROD_TYPE,{MESInfo.Head1.ProductTypeCode}");
                sb.AppendLine($"BARCODE,NA");
                sb.AppendLine($"EQUIP_ZONE,{headNo + 1}");
                sb.AppendLine($"TRANS_TIME,{strDateTime}");
                sb.AppendLine($"RESULT,{strRESULT}");
                sb.AppendLine($"NG_CODE,NA");
                sb.AppendLine($"START_TEST_RESULT");
                //sb.AppendLine($"LOADVISIONRESULT_{headNo + 1},,,{Datas.Data_LoadingInspect_Result[headNo].Judge}");
                sb.AppendLine($"PICKRESULT_{headNo + 1},,,{Datas.PickResult[headNo]}");
                //sb.AppendLine($"UNDERVISIONRESULT_{headNo + 1},,,{Datas.Data_UnderInspect_Result[headNo].Judge}");
                //sb.AppendLine($"UNLOADVISIONRESULT_{headNo + 1},,,{Datas.Data_UnloadingInspect_Result[headNo].Judge}");
                sb.AppendLine($"PLACERESULT_{headNo + 1},,,{Datas.PlaceResult[headNo]}");
                sb.AppendLine($"END_TEST_RESULT");

                using (StreamWriter writer = File.CreateText(strFileName))
                {
                    writer.WriteAsync(sb.ToString());
                }
            }
            catch { }
        }

        /// <summary>
        /// TT File write function
        /// </summary>
        /// <param name="headNo">Head Number start with 0</param>
        public override void SaveTT(int headNo)
        {
            if (MESInfo.UseMES == false) return;

            try
            {
                DateTime now = DateTime.Now;
                string strDate = now.ToString("yyyyMMdd");
                string strDateTime = now.ToString("yyyyMMddHHmmss");

                // D:\MES\EQUIP\<Date>\TT_<DateTime>_Z<headNo>.csv
                string strFileName = Path.Combine(GlobalFolders.FolderMESEquip, strDate, $"TT_{strDateTime}_Z{headNo}.csv");
                Directory.CreateDirectory(Path.GetDirectoryName(strFileName));

                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"VER,VER-001");
                sb.AppendLine($"EQUIP_ID,{MESInfo.Head1.EQPCode}");
                sb.AppendLine($"DEVICE,{MESInfo.Head1.DeviceCode}");
                sb.AppendLine($"OPERATION,{MESInfo.Head1.OperationCode}");
                sb.AppendLine($"PROD_TYPE,{MESInfo.Head1.ProductTypeCode}");
                sb.AppendLine($"BARCODE,NA");
                sb.AppendLine($"EQUIP_ZONE,{headNo + 1}");
                sb.AppendLine($"TRANS_TIME,{strDateTime}");
                sb.AppendLine($"RESULT,OK");
                sb.AppendLine($"START_TEST_RESULT");
                sb.AppendLine($"TOTALTACTTIME,{Datas.WorkData.TaktTime.Total},OK");
#if USPCUTTING
                sb.AppendLine($"PRESSTACTTIME,{Datas.WorkData.TaktTime.Press / 60.0},OK");
#endif
                sb.AppendLine($"PICKTACTTIME,{Datas.WorkData.TaktTime.Pick},OK");
                sb.AppendLine($"VISIONTACTTIME,{Datas.WorkData.TaktTime.UnderVisionProcess},OK");
                sb.AppendLine($"PLACETACTTIME,{Datas.WorkData.TaktTime.Place},OK");
                sb.AppendLine($"LOADVISIONPROCESSTACTTIME_{headNo + 1},{Datas.WorkData.TaktTime.LoadVision[headNo].ProcessTime},OK");
                sb.AppendLine($"UNDERVISIONTACTTIME_{headNo + 1},{Datas.WorkData.TaktTime.BotVision[headNo].ProcessTime},OK");
                sb.AppendLine($"UNLOADVISIONTACTTIME_{headNo + 1},{Datas.WorkData.TaktTime.UnloadVision[headNo].ProcessTime},OK");
                sb.AppendLine($"END_TEST_RESULT");

                using (StreamWriter writer = File.CreateText(strFileName))
                {
                    writer.WriteAsync(sb.ToString());
                }
            }
            catch { }
        }
    }
}
