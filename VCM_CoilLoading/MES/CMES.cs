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
using VCM_CoilLoading.Define;
using XSmNetW;
using XValueNet;

namespace VCM_CoilLoading.MES
{
    public class CMES : CMESBase
    {
        public override void Send_PCInfoData()
        {
            XValueClr xValue = new XValueClr();
            XValueClr xList1 = new XValueClr();
            XValueClr xList2 = new XValueClr();
            XValueClr xList3 = new XValueClr();
            XValueClr xTemp = new XValueClr();

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

            xValue.MakeSECS2Stream(ref sendData, len);

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
                string strFileName = Path.Combine(ProgramFolder.FolderMESEquip, strDate, $"PD_{strDateTime}_Z{headNo}.csv");
                Directory.CreateDirectory(Path.GetDirectoryName(strFileName));

                StreamWriter sw = new StreamWriter(strFileName, true, Encoding.Default);

                sw.WriteLine($"VER,VER-001");
                sw.WriteLine($"EQUIP_ID,{MESInfo.Head1.EQPCode}");
                sw.WriteLine($"DEVICE,{MESInfo.Head1.DeviceCode}");
                sw.WriteLine($"OPERATION,{MESInfo.Head1.OperationCode}");
                sw.WriteLine($"PROD_TYPE,{MESInfo.Head1.ProductTypeCode}");
                sw.WriteLine($"BARCODE,NA");
                sw.WriteLine($"EQUIP_ZONE,{headNo + 1}");
                sw.WriteLine($"TRANS_TIME,{strDateTime}");
                sw.WriteLine($"RESULT,OK");
                sw.WriteLine($"NG_CODE,NA");
                sw.WriteLine($"START_TEST_RESULT");
                sw.WriteLine($"LOADVISIONRESULT_{headNo + 1},,,{Datas.Data_LoadingInspect_Result[headNo].Judge}");
                sw.WriteLine($"UNDERVISIONRESULT_{headNo + 1},,,{Datas.Data_UnderInspect_Result[headNo].Judge}");
                sw.WriteLine($"UNLOADVISIONRESULT_{headNo + 1},,,{Datas.Data_UnloadingInspect_Result[headNo].Judge}");
                sw.WriteLine($"END_TEST_RESULT");

                sw.Close();
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
                string strFileName = Path.Combine(ProgramFolder.FolderMESEquip, strDate, $"TT_{strDateTime}_Z{headNo}.csv");
                Directory.CreateDirectory(Path.GetDirectoryName(strFileName));

                StreamWriter sw = new StreamWriter(strFileName, true, Encoding.Default);

                sw.WriteLine($"VER,VER-001");
                sw.WriteLine($"EQUIP_ID,{MESInfo.Head1.EQPCode}");
                sw.WriteLine($"DEVICE,{MESInfo.Head1.DeviceCode}");
                sw.WriteLine($"OPERATION,{MESInfo.Head1.OperationCode}");
                sw.WriteLine($"PROD_TYPE,{MESInfo.Head1.ProductTypeCode}");
                sw.WriteLine($"BARCODE,NA");
                sw.WriteLine($"EQUIP_ZONE,{headNo + 1}");
                sw.WriteLine($"TRANS_TIME,{strDateTime}");
                sw.WriteLine($"RESULT,OK");
                sw.WriteLine($"START_TEST_RESULT");
                sw.WriteLine($"TOTALTACTTIME,{Datas.WorkData.TaktTime.Total},OK");
                sw.WriteLine($"PICKTACTTIME,{Datas.WorkData.TaktTime.Pick},OK");
                sw.WriteLine($"VISIONTACTTIME,{Datas.WorkData.TaktTime.Vision},OK");
                sw.WriteLine($"PLACETACTTIME,{Datas.WorkData.TaktTime.Place},OK");
                sw.WriteLine($"LOADVISIONPROCESSTACTTIME_{headNo + 1},{Datas.WorkData.TaktTime.LoadVision[headNo].ProcessTime},OK");
                sw.WriteLine($"UNDERVISIONTACTTIME_{headNo + 1},{Datas.WorkData.TaktTime.BotVision[headNo].ProcessTime},OK");
                sw.WriteLine($"UNLOADVISIONTACTTIME_{headNo + 1},{Datas.WorkData.TaktTime.UnloadVision[headNo].ProcessTime},OK");
                sw.WriteLine($"END_TEST_RESULT");

                sw.Close();
            }
            catch { }
        }
    }
}
