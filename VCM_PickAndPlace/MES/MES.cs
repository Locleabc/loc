using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XSmNetW;
using XValueNet;

namespace WpfPlatform.MES
{
    public class MES
    {
        public enum EQ_STATUS
        {
            RUN = 1,    // (Auto 작업 진행 상태)
            IDLE,       // (Auto Mode 대기 상태 – Auto 진행 간 제품 공급을 기다리는 상태에 해당)
            DOWN,       // (Alarm or Warning에 의한 설비 정지 상태)
            SETUP,      // (원점 구동 / Manual 구동 / PM등)
            DISCONNECT, // (Agent와 통신을 끊을 경우, PGM 종료)

            SETUP_HOME,     // SETUP 중 상세 Description. 
            SETUP_START,
            SETUP_PM,
        }
        public enum PM_STATUS
        {
            LOAD_DOWN = 1, // (계획없음)
            MODEL_CHANGE, // (모델변경)
            BM_START, // (BM DOWN)
            MATR_DOWN, // (자재없음)
            PM_EXIT, // (PM 종료, PM 작업 완료 후 반드시 PM_EXIT 상태를 보고해야 한다)
        }

        public enum ALARM_STATUS
        {
            Set = 1,    // (Alarm 발생 時)
            Reset,      // (Alarm Clear 時)
            Clear,      // (중복 Alarm등 모든 alarm에 대해 강제 Clear)
        }

        public static XSmNet4CS _xcom = null;

        public static string mIPAddress = "000.000.000.000";

        public static string mEQP_Message = "";

        public static string mMES_Event = "";

        public static string mTRANS_TIME = DateTime.Now.ToString("yyyyMMddHHmmss");

        public static string mMES_FOLDER = @"D:\MES";
        public static string mEQUIP_FOLDER = mMES_FOLDER + @"\EQUIP";
        public static string mSET_FOLDER = mMES_FOLDER + @"\SET";
        private static string mLINKAGENT_FOLDER = @"D:\Agent\LinkAgent.exe";

        public static string mEEEventString = "";
        public static string mEEEventFileName = "";

        public static bool mAgentConnected = false;    // Agent와의 연결 상태
        public static bool mMesConnected = false;      // Agent로 부터 MES 연결 상태를 수신받으면 상태 변함.

        public static PM_STATUS mPmStatus;
        public static EQ_STATUS mEqStatus;

        private static bool b1stSendData = false;   // 통신 연결 됐을때 정보를 1회만 보내주기 위해 사용
        private static bool bPowerOn = true;       // 설비 초기 가동시 EQ State를 보내주기 위해 사용

        public static void Initial()
        {
            try
            {
                ExecuteLinkAgent();

                // 공유메모리 변수 instance
                _xcom = new XSmNet4CS();

                // 공유메모리 변수 이벤트 핸들러 등록
                _xcom.OnConnected += _xcom_OnConnected;
                _xcom.OnDisconnected += _xcom_OnDisconnected;
                _xcom.OnReceived += _xcom_OnReceived;

                Stop();
                UnInitialize();
                System.Threading.Thread.Sleep(100);

                // 공유메모리 Initialize 및 Start 까지 해야함.
                Initialize();
                Start();
                LogSave(true, "Request Connect");
            }
            catch { }
        }

        /// <summary>
        /// 공유메모리 연결 이벤트
        /// </summary>
        public static void _xcom_OnConnected()
        {
            b1stSendData = true;
            mAgentConnected = true;
        }

        /// <summary>
        /// 공유메모리 연결종료 이벤트 (아래 코드를 넣어준다. 그렇지 않을 경우 연결이 끊어졌을 때 이벤트 계속 발생)
        /// </summary>
        public static void _xcom_OnDisconnected()
        {
            LogSave(false, "Disconnect");
            mAgentConnected = false;
            Stop();
            UnInitialize();

            Thread.Sleep(5000);

            Initialize();
            Start();
        }

        /// <summary>
        /// XValueNet dll 을 이용하여 Parsing
        /// </summary>
        /// <param name="pcValue"></param>
        /// <param name="nSize"></param>
        public static void _xcom_OnReceived(byte[] pcValue, int nSize)
        {
            LogSave(false, "Receive Message");
            int count = 0;
            string msgName = "";
            string temp = "";

            // XValue 변수 선언
            XValue xValue = new XValue();

            // XValue 을 이용하여 전송받은 byte[] 를 XValue 클래스에 Load
            int ret = xValue.LoadFromSECS2(ref pcValue, 0, nSize);

            // 사양서에 맞게 코딩
            count = xValue.GetCount();          // LIST 2
            {
                XValue val1 = xValue.GetFirstItem(); msgName = val1.GetString();

                // 메시지 이름으로 분류하여 Parsing
                if (msgName == "WORK_POSSIBLE_RESULT")
                {
                    xValue.GetNextItem(); count = val1.GetCount();              // LIST 3
                    {
                        XValue val2 = new XValue();
                        val1.GetFirstItem(); temp = val2.GetString();        // EQUIPMENT_ID
                        val1.GetNextItem(); temp = val2.GetString();        // MATERIAL_NAME
                        val1.GetNextItem(); temp = val2.GetString();        // BARCODE
                    }
                }
                else if (msgName == "RECIPE_REGISTER")
                {
                    xValue.GetNextItem(); count = val1.GetCount();            // LIST 2
                    {
                        XValue val2 = new XValue();
                        val1.GetFirstItem(); temp = val2.GetString();        // EQUIPMENT_ID
                        val1.GetNextItem(); count = val2.GetCount();           // LIST 1
                        {
                            XValue val3 = new XValue();
                            val2.GetFirstItem(); temp = val3.GetString();    // RESULT
                        }
                    }
                }
                else if (msgName == "MES_STANDARD_INFO")
                {
                    val1 = xValue.GetNextItem(); count = val1.GetCount();
                    {
                        XValue val2 = val1.GetFirstItem(); MESStandardInfo.str_Device_Code = val2.GetString();
                        val2 = val1.GetNextItem(); MESStandardInfo.str_Operation_Code = val2.GetString();
                        val2 = val1.GetNextItem(); MESStandardInfo.str_Equip_ID_Code = val2.GetString();
                        val2 = val1.GetNextItem(); MESStandardInfo.str_Product_Type_Code = val2.GetString();

                        val2 = val1.GetNextItem(); MESStandardInfo.str_Device_Name = val2.GetString();
                        val2 = val1.GetNextItem(); MESStandardInfo.str_Operation_Name = val2.GetString();
                        val2 = val1.GetNextItem(); MESStandardInfo.str_Equip_ID_Name = val2.GetString();
                        val2 = val1.GetNextItem(); MESStandardInfo.str_Product_Type_Name = val2.GetString();
                    }

                    if (MESStandardInfo.IsContainNullField())
                    {
                        MESStandardInfo.LoadStandardInfoFromFile();
                    }
                    else
                    {
                        MESStandardInfo.SaveStandardInfoToFile();
                    }

                    string[] sTemp = new string[] {
                        "MsgName", msgName,
                        "Device", MESStandardInfo.str_Device_Code,
                        "Operation", MESStandardInfo.str_Operation_Code,
                        "EqpID", MESStandardInfo.str_Equip_ID_Code,
                        "ProductType", MESStandardInfo.str_Product_Type_Code,
                        "Device_Name", MESStandardInfo.str_Device_Name,
                        "Operation_Name", MESStandardInfo.str_Operation_Name,
                        "EqpID_Name", MESStandardInfo.str_Equip_ID_Name,
                        "ProductType_Name", MESStandardInfo.str_Product_Type_Name
                    };
                    LogSave(false, sTemp);

                    if (bPowerOn)           // 전원 ON시 1회만 보냄
                    {
                        bPowerOn = false;
                        b1stSendData = false;
                        SendMesOption(Data.Option.mOptionData.MESUse);
                        SendPCInfo();
                        SendEqStatus(EQ_STATUS.SETUP_START, true);
                    }
                    else if (b1stSendData)   // 최초 연결시에만 보냄 
                    {
                        b1stSendData = false;
                        SendMesOption(Data.Option.mOptionData.MESUse);
                        SendPCInfo();
                        SendEqStatus(mEqStatus, true);

                    }
                }
                else if (msgName == "MES_CONNECTION_STATUS")
                {
                    val1 = xValue.GetNextItem(); count = val1.GetCount();
                    {
                        XValue val2 = val1.GetFirstItem(); MESStandardInfo.str_Equip_ID_Code = val2.GetString();
                        val2 = val1.GetNextItem(); byte[] byTemp = val2.GetU1();
                        mMesConnected = (byTemp[0] == 1 ? true : false);
                    }
                    string[] sTemp = new string[] { "MsgName", msgName, "EqpID", MESStandardInfo.str_Equip_ID_Code, "MesConnected", mMesConnected.ToString() };
                    LogSave(false, sTemp);
                }
            }
        }

        /// <summary>
        /// 공유메모리 dll initialize
        /// </summary>
        public static void Initialize()
        {
            // cfg 파일이 있어야 한다. 프로젝트에 포함시켰음. (EQ1.cfg)
            string path = Application.StartupPath + "\\EQ1.cfg";

            int res = _xcom.Initialize(path);
            _xcom.BinaryType = true;
        }

        /// <summary>
        /// 공유메모리 dll 종료
        /// </summary>
        public static void UnInitialize()
        {
            int ret = _xcom.Terminate();
        }

        /// <summary>
        /// 공유메모리 dll Start
        /// </summary>
        public static void Start()
        {
            int ret = _xcom.Start();
        }

        public static void Stop()
        {
            int ret = _xcom.Stop();
        }

        #region Send Message
        /// <summary>
        /// Equipment Status 보고
        /// </summary>
        public static void SendEqStatus(EQ_STATUS EqStatus, bool ForcedSend)
        {
            string sMsgName = "COLLECTION_EVENT";
            int iCEID = 1;
            int iReportID = 100;

            string sDescription;
            switch (EqStatus)
            {
                case EQ_STATUS.RUN: sDescription = "AUTO"; break;
                case EQ_STATUS.IDLE: sDescription = "IDLE"; break;
                case EQ_STATUS.DOWN: sDescription = "EQ DOWN"; break;
                case EQ_STATUS.DISCONNECT: sDescription = "PROGRAM END"; break;
                case EQ_STATUS.SETUP_HOME: sDescription = "HOME"; break;
                case EQ_STATUS.SETUP_START: sDescription = "PROGRAM START"; break;
                case EQ_STATUS.SETUP_PM: sDescription = "PM"; break;
                default: return;
            }

            if (ForcedSend == false && mEqStatus == EqStatus) return;
            mEqStatus = EqStatus;

            EQ_STATUS EqState = (EqStatus < EQ_STATUS.SETUP_HOME ? EqStatus : EQ_STATUS.SETUP);

            XValue xValue = new XValue();   // ASCII Message Name
            XValue xList1 = new XValue();   // ASCII EqID, U1 CEID
            XValue xList2 = new XValue();   // U1 ReportID
            XValue xList3 = new XValue();   // U1 NewState, ASCII Description
            XValue xTemp = new XValue();

            // 함수테스트
            xValue.SetList();           // LIST 2
            {
                xTemp.SetString(sMsgName); xValue.Add(xTemp);               // MESSAGE NAME
                xList1.SetList();                                       // LIST 3
                {
                    xTemp.SetString(MESStandardInfo.str_Equip_ID_Code); xList1.Add(xTemp);            // EQUIPMENT ID
                    xTemp.SetU1((byte)iCEID); xList1.Add(xTemp);            // CEID = 1 
                    xList2.SetList();                                   // LIST 2
                    {
                        xTemp.SetU1((byte)iReportID); xList2.Add(xTemp);       // ReportID
                        xList3.SetList();                               // LIST 1
                        {
                            xTemp.SetU1((byte)EqState); xList3.Add(xTemp);              // NEW_STATE
                            xTemp.SetString(sDescription); xList3.Add(xTemp);              // DESCRIPTION
                        }
                        xList2.Add(xList3);
                    }
                    xList1.Add(xList2);
                }
                xValue.Add(xList1);
            }

            int len = xValue.GetByteLength();
            byte[] sendData = new byte[len];

            xValue.MakeSECS2Stream(ref sendData, 0, len);

            int ret = _xcom.SendMsg(sendData, sendData.Length);

            string[] sTemp = new string[] { "MsgName", sMsgName, "EqpID", MESStandardInfo.str_Equip_ID_Code, "CEID", iCEID.ToString(), "ReportID", iReportID.ToString(), "State", ((EQ_STATUS)EqState).ToString(), "Descriptioni", sDescription };
            LogSave(true, sTemp);
        }

        /// <summary>
        /// PM Status 보고
        /// </summary>
        public static void SendPMStatus(PM_STATUS PmStatus)
        {
            if (mPmStatus == PmStatus) return;
            else mPmStatus = PmStatus;

            string sMsgName = "COLLECTION_EVENT";

            int iCEID = 2;
            int iReportID = 101;
            string sDescription = ((PM_STATUS)PmStatus).ToString();

            XValue xValue = new XValue();   // ASCII MsgName
            XValue xList1 = new XValue();   // ASCII EqpID, U1 CEID
            XValue xList2 = new XValue();   // U1 ReportID
            XValue xList3 = new XValue();   // U1 NewState, ASCII Description
            XValue xTemp = new XValue();

            // 함수테스트
            xValue.SetList();
            {
                xTemp.SetString(sMsgName); xValue.Add(xTemp);       // MESSAGE NAME
                xList1.SetList();                                       // LIST 3
                {
                    xTemp.SetString(MESStandardInfo.str_Equip_ID_Code); xList1.Add(xTemp);        // EQUIPMENT ID
                    xTemp.SetU1((byte)iCEID); xList1.Add(xTemp);        // CEID  

                    xList2.SetList();                                       // LIST 2
                    {
                        xTemp.SetU1((byte)iReportID); xList2.Add(xTemp);    // ReportID
                        xList3.SetList();                                       // LIST 1
                        {
                            xTemp.SetU1((byte)PmStatus); xList3.Add(xTemp);         // NEW_STATE
                            xTemp.SetString(sDescription); xList3.Add(xTemp);       // DESCRIPTION
                        }
                        xList2.Add(xList3);
                    }
                    xList1.Add(xList2);
                }
                xValue.Add(xList1);
            }

            int len = xValue.GetByteLength();
            byte[] sendData = new byte[len];

            xValue.MakeSECS2Stream(ref sendData, 0, len);

            int ret = _xcom.SendMsg(sendData, sendData.Length);

            string[] sTemp = new string[] { "MsgName", sMsgName, "EqpID", MESStandardInfo.str_Equip_ID_Code, "CEID", iCEID.ToString(), "ReportID", iReportID.ToString(), "State", sDescription };
            LogSave(true, sTemp);
        }

        #region - 미사용
        /// <summary>
        /// Master Recipe 보고
        /// </summary>
        public static void SendMasterRecipe()
        {
            string sMsgName = "MASTER_RECIPE";
            string sFilePath = "D:\\" + MESStandardInfo.str_Equip_ID_Code + "\\MasterRecipe.txt";

            XValue xValue = new XValue();
            XValue xList1 = new XValue();
            XValue xList2 = new XValue();
            XValue xList3 = new XValue();
            XValue xTemp = new XValue();

            // 함수테스트
            xValue.SetList();           // LIST 2
            {
                xTemp.SetString(sMsgName); xValue.Add(xTemp);                // MESSAGE NAME
                xList1.SetList();                                                   // LIST 2
                {
                    xTemp.SetString(MESStandardInfo.str_Equip_ID_Code); xList1.Add(xTemp);      // EQUIPMENT_ID
                    xList2.SetList();                                               // LIST 1
                    {
                        xTemp.SetString(sFilePath); xList2.Add(xTemp);   // FILE PATH
                    }
                    xList1.Add(xList2);
                }
                xValue.Add(xList1);
            }

            int len = xValue.GetByteLength();
            byte[] sendData = new byte[len];

            xValue.MakeSECS2Stream(ref sendData, 0, len);

            int ret = _xcom.SendMsg(sendData, sendData.Length);
        }
        #endregion

        // TODO: Functionalization this method
        /*
        /// <summary>
        /// Alarm Set/Reset 보고
        /// </summary>
        public static void SendAlarm(ALARM_STATUS AlarmStatus, int ErrorCode)
        {
            string sMsgName = "COLLECTION_EVENT";
            int iCEID = 3;
            int iReportID = 102;
            int iAlarmStatus = (int)AlarmStatus;
            int iAlarmID = 0;
            string sAlarmDescription = "";
            int nErrorCode = 0;

            if (AlarmStatus != ALARM_STATUS.Clear)
            {
                iAlarmID = ErrorCode;
                //nErrorCode = Error.ErrorCode[iAlarmID];
                sAlarmDescription = Error.ErrorMessage[iAlarmID];
            }

            XValue xValue = new XValue();   // ASCII MsgName    
            XValue xList1 = new XValue();   // ACSII EqpID, U1 CEID
            XValue xList2 = new XValue();   // U1 ReportID
            XValue xList3 = new XValue();   // U1 AlarmStatus, U2 AlarmID, ASCII AlarmDescription
            XValue xTemp = new XValue();

            // 함수테스트
            xValue.SetList();
            {
                xTemp.SetString(sMsgName); xValue.Add(xTemp);
                xList1.SetList();
                {
                    xTemp.SetString(MESStandardInfo.str_Equip_ID_Code); xList1.Add(xTemp);
                    xTemp.SetU1((byte)iCEID); xList1.Add(xTemp);
                    xList2.SetList();
                    {
                        xTemp.SetU1((byte)iReportID); xList2.Add(xTemp);
                        xList3.SetList();
                        {
                            xTemp.SetU1((byte)iAlarmStatus); xList3.Add(xTemp);
                            xTemp.SetU2((ushort)nErrorCode); xList3.Add(xTemp);
                            xTemp.SetString(sAlarmDescription); xList3.Add(xTemp);
                        }
                        xList2.Add(xList3);
                    }
                    xList1.Add(xList2);
                }
                xValue.Add(xList1);
            }

            int len = xValue.GetByteLength();
            byte[] sendData = new byte[len];

            xValue.MakeSECS2Stream(ref sendData, 0, len);

            int ret = _xcom.SendMsg(sendData, sendData.Length);

            string[] sTemp = new string[]
                { "MsgName", sMsgName,
                    "EqpID", MESStandardInfo.str_Equip_ID_Code,
                    "CEID", iCEID.ToString(),
                    "ReportID", iReportID.ToString(),
                    "AlarmStatus", ((ALARM_STATUS)iAlarmStatus).ToString(),
                    "AlarmID", iAlarmID.ToString(),
                    "AlarmDescription", sAlarmDescription};
            LogSave(true, sTemp);
        }
        */

        /// <summary>
        /// PC Info Data 보고
        /// </summary>
        public static void SendPCInfo()
        {
            string sMsgName = "EQUIP_PC_INFO";// "COLLECTION_EVENT";

            //mEQP_ID
            string sOS = "";// "WIN7";
            string sBit = "";//"32";
            string sIPAddress = "";
            string sMacAddress = "";//"00-00-00-00-00";
            string sSoftVer = ProgramInformation.ProgramVersion;
            string sVisionVer = "6.2";

            XValue xValue = new XValue();   // ASCII MsgName
            XValue xList1 = new XValue();   // ASCII EqpID
            XValue xList2 = new XValue();   // ASCII OS, Bit, IP, Mac, Soft, Vision
            XValue xTemp = new XValue();

            // 함수테스트
            xValue.SetList();           // LIST 2
            {
                xTemp.SetString(sMsgName); xValue.Add(xTemp);
                xList1.SetList();
                {
                    xTemp.SetString(MESStandardInfo.str_Equip_ID_Code); xList1.Add(xTemp);
                    xList2.SetList();
                    {
                        xTemp.SetString(sOS); xList2.Add(xTemp);
                        xTemp.SetString(sBit); xList2.Add(xTemp);
                        xTemp.SetString(sIPAddress); xList2.Add(xTemp);
                        xTemp.SetString(sMacAddress); xList2.Add(xTemp);
                        xTemp.SetString(sSoftVer); xList2.Add(xTemp);
                        xTemp.SetString(sVisionVer); xList2.Add(xTemp);
                    }
                    xList1.Add(xList2);
                }
                xValue.Add(xList1);
            }

            int len = xValue.GetByteLength();
            byte[] sendData = new byte[len];

            xValue.MakeSECS2Stream(ref sendData, 0, len);

            int ret = _xcom.SendMsg(sendData, sendData.Length);

            string[] sTemp = new string[]
                { "MsgName", sMsgName,
                    "EqpID", MESStandardInfo.str_Equip_ID_Code,
                    "SoftVersion", sSoftVer,
                    "VisionVersion", sVisionVer};
            LogSave(true, sTemp);
        }

        /// <summary>
        /// MES Option 보고
        /// </summary>
        public static void SendMesOption(int iOption)
        {
            string sMsgName = "MES_USE_OPTION";
            //mEQP_ID

            XValue xValue = new XValue();   // ASCII MsgName
            XValue xList1 = new XValue();   // ASCII EqpID
            XValue xList2 = new XValue();   // U1 Option
            XValue xTemp = new XValue();

            // 함수테스트
            xValue.SetList();           // LIST 2
            {
                xTemp.SetString(sMsgName); xValue.Add(xTemp);
                xList1.SetList();
                {
                    xTemp.SetString(MESStandardInfo.str_Equip_ID_Code); xList1.Add(xTemp);
                    xTemp.SetU1((byte)iOption); xList1.Add(xTemp);
                }
                xValue.Add(xList1);
            }

            int len = xValue.GetByteLength();
            byte[] sendData = new byte[len];

            xValue.MakeSECS2Stream(ref sendData, 0, len);

            int ret = _xcom.SendMsg(sendData, sendData.Length);

            string[] sTemp = new string[]
                { "MsgName", sMsgName,
                    "EqpID", MESStandardInfo.str_Equip_ID_Code,
                    "Option", (iOption == 1 ? "USE" : "NOT_USE")};
            LogSave(true, sTemp);
        }
        public static void SendRecipeChange()
        {
            string sMsgName = "RECIPE_CHANGE_EVENT";
            XValue xValue = new XValue();   // ASCII MsgName
            XValue xList1 = new XValue();   // ASCII EqpID
            XValue xList2 = new XValue();   // U1 Option
            XValue xTemp = new XValue();
            xValue.SetList();           // LIST 2
            {
                xTemp.SetString(sMsgName); xValue.Add(xTemp);
                xList1.SetList();
                {
                    xTemp.SetString(MESStandardInfo.str_Equip_ID_Code); xList1.Add(xTemp);
                }
                xValue.Add(xList1);
            }
            int len = xValue.GetByteLength();
            byte[] sendData = new byte[len];
            xValue.MakeSECS2Stream(ref sendData, 0, len);
            int ret = _xcom.SendMsg(sendData, sendData.Length);
            string[] sTemp = new string[]
                { "MsgName", sMsgName,
                    "EqpID", MESStandardInfo.str_Equip_ID_Code,
                };
            LogSave(true, sTemp);
        }
        #endregion

        public static void PDFileSave(int nHeadNo, string sResult, string sNgComment)
        {
            try
            {
                DateTime datetime = new DateTime();

                datetime = DateTime.Now;
                string sDateTime = datetime.ToString("yyyyMMddHHmmss");
                string sDate = datetime.ToString("yyyyMMdd");

                //string sFileName = MES.mEQUIP_FOLDER + "\\" + sDate + "\\PD_" + sDateTime + ".CSV";
                string sFileName = MES.mEQUIP_FOLDER + "\\PD_" + sDateTime + "_Z" + (nHeadNo + 1) + ".CSV";

                ProgramFolder.ProgramFolderCheck();  // 해당 폴더가 없는 경우 생성

                System.IO.Directory.CreateDirectory(mEQUIP_FOLDER + "\\" + sDate);

                bool bFileFind = !File.Exists(sFileName);

                System.IO.StreamWriter streamwriter = new System.IO.StreamWriter(sFileName, true, Encoding.Default);

                if (sResult == "OK") sNgComment = "NA";
                if (bFileFind)
                {
                    //streamwriter.WriteLine("Header,값 Field1,값 Field2,값 Field3");

                    string sWriteLine = "";// sDate + "," + sTime + ",Error," + pErrorNo.ToString();
                    sWriteLine = string.Format("VER,VER-001"); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("EQUIP_ID,{0}", MESStandardInfo.str_Equip_ID_Code); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("DEVICE,{0}", MESStandardInfo.str_Device_Code); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("OPERATION,{0}", MESStandardInfo.str_Operation_Code); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("PROD_TYPE,{0}", MESStandardInfo.str_Product_Type_Code); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("BARCODE,NA"); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("EQUIP_ZONE,{0}", nHeadNo + 1); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("TRANS_TIME,{0}", sDateTime); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("RESULT,{0}", sResult); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("NG_CODE,{0}", sNgComment); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("START_TEST_RESULT"); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("RESULT,{0}", sResult); streamwriter.WriteLine(sWriteLine);             // 201219 Result 추가 hdshim
                    sWriteLine = string.Format("END_TEST_RESULT"); streamwriter.WriteLine(sWriteLine);
                }

                streamwriter.Close();
            }
            catch { }
        }

        public static void TTFileSave(int nHeadNo, double dTactTime, string sResult)
        {
            try
            {
                DateTime datetime = new DateTime();

                datetime = DateTime.Now;
                string sDateTime = datetime.ToString("yyyyMMddHHmmss");
                string sDate = datetime.ToString("yyyyMMdd");

                //string sFileName = MES.mEQUIP_FOLDER + "\\" + sDate + "\\TT_" + sDateTime + ".CSV";
                string sFileName = MES.mEQUIP_FOLDER + "\\TT_" + sDateTime + "_Z" + (nHeadNo + 1) + ".CSV";

                ProgramFolder.ProgramFolderCheck();  // 해당 폴더가 없는 경우 생성

                //System.IO.Directory.CreateDirectory(mEQUIP_FOLDER + "\\" + sDate);
                System.IO.Directory.CreateDirectory(mEQUIP_FOLDER);

                bool bFileFind = !File.Exists(sFileName);

                System.IO.StreamWriter streamwriter = new System.IO.StreamWriter(sFileName, true, Encoding.Default);

                if (bFileFind)
                {
                    //streamwriter.WriteLine("Header,값 Field1,값 Field2,값 Field3");

                    string sWriteLine = "";// sDate + "," + sTime + ",Error," + pErrorNo.ToString();
                    sWriteLine = string.Format("VER,VER-001"); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("EQUIP_ID,{0}", MESStandardInfo.str_Equip_ID_Code); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("DEVICE,{0}", MESStandardInfo.str_Device_Code); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("OPERATION,{0}", MESStandardInfo.str_Operation_Code); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("PROD_TYPE,{0}", MESStandardInfo.str_Product_Type_Code); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("BARCODE,NA"); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("EQUIP_ZONE,{0}", nHeadNo + 1); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("TRANS_TIME,{0}", sDateTime); streamwriter.WriteLine(sWriteLine);
                    sWriteLine = string.Format("RESULT,{0}", sResult); streamwriter.WriteLine(sWriteLine);

                    sWriteLine = string.Format("START_TEST_RESULT"); streamwriter.WriteLine(sWriteLine);
                    if (nHeadNo == 0)
                    {
                        sWriteLine = string.Format("HEADLTACTTIME,{0:N2},{1}", (double)dTactTime / 1000, sResult); streamwriter.WriteLine(sWriteLine);
                        sWriteLine = string.Format("HEADLFILTERTIME,{0:N2},{1}", (double)WAFER.WaferCamera.dHeadLFilterTime / 1000, sResult); streamwriter.WriteLine(sWriteLine);
                        sWriteLine = string.Format("HEADLACTUATORVISIONTIME,{0:N2},{1}", (double)VISION.HeadL.nActuatorTime / 1000, sResult); streamwriter.WriteLine(sWriteLine);
                        sWriteLine = string.Format("HEADLUNDERVISIONTIME,{0:N2},{1}", (double)VISION.UnderL.nActuatorTime / 1000, sResult); streamwriter.WriteLine(sWriteLine);
                    }
                    else
                    {
                        sWriteLine = string.Format("HEADRTACTTIME,{0:N2},{1}", (double)dTactTime / 1000, sResult); streamwriter.WriteLine(sWriteLine);
                        sWriteLine = string.Format("HEADRFILTERTIME,{0:N2},{1}", (double)WAFER.WaferCamera.dHeadRFilterTime / 1000, sResult); streamwriter.WriteLine(sWriteLine);
                        sWriteLine = string.Format("HEADRACTUATORVISIONTIME,{0:N2},{1}", (double)VISION.HeadR.nActuatorTime / 1000, sResult); streamwriter.WriteLine(sWriteLine);
                        sWriteLine = string.Format("HEADRUNDERVISIONTIME,{0:N2},{1}", (double)VISION.UnderR.nActuatorTime / 1000, sResult); streamwriter.WriteLine(sWriteLine);
                    }
                    sWriteLine = string.Format("END_TEST_RESULT"); streamwriter.WriteLine(sWriteLine);
                }

                streamwriter.Close();
            }
            catch { }
        }

        public static void SETFileSave()
        {
            try
            {
                DateTime datetime = new DateTime();

                datetime = DateTime.Now;
                string sDateTime = datetime.ToString("yyyyMMddHHmmss");
                string sDate = datetime.ToString("yyyyMMdd");

                // string sFileName = MES.mSET_FOLDER + "\\" + sDate + "\\SET_" + sDateTime + ".CSV";
                string sFileName = MES.mSET_FOLDER + "\\SET_" + sDateTime + ".CSV";

                ProgramFolder.ProgramFolderCheck();  // 해당 폴더가 없는 경우 생성

                //System.IO.Directory.CreateDirectory(mSET_FOLDER + "\\" + sDate);
                System.IO.Directory.CreateDirectory(mSET_FOLDER);

                bool bFileFind = !File.Exists(sFileName);

                System.IO.StreamWriter streamwriter = new System.IO.StreamWriter(sFileName, true, Encoding.Default);

                if (bFileFind)
                {
                    //streamwriter.WriteLine("EQUIPMENT,OPERATION,DEVICE,PROD_TYPE");

                    string sWriteLine = MESStandardInfo.str_Equip_ID_Code + "," + MESStandardInfo.str_Operation_Code + "," + MESStandardInfo.str_Device_Code + "," + MESStandardInfo.str_Product_Type_Code;
                    streamwriter.WriteLine(sWriteLine);
                }

                streamwriter.Close();
            }
            catch { }
        }

        #region Delete Files
        private static Thread threads;
        private static int DeleteDay = 0;
        private static string Path = "";

        public static void Delete(int iDeleteDay, string sPath)
        {
            DeleteDay = iDeleteDay;
            Path = sPath;

            threads = new Thread(DeleteMethod);
            threads.Start();
        }

        private static void DeleteMethod()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(Path);
                if (di.Exists)
                {
                    DirectoryInfo[] dirinfo = di.GetDirectories();

                    for (int i = 0; i < dirinfo.Length; i++)
                    {
                        // Empty Folder
                        string[] file = Directory.GetFiles(dirinfo[i].FullName);
                        if (file.Length < 1)
                        {
                            dirinfo[i].Delete();
                        }
                        // File
                        else
                        {
                            DeleteFile(DeleteDay, dirinfo[i].FullName);
                        }
                    }
                }
            }
            catch { }
        }

        private static void DeleteFile(int iDeleteDay, string sPath)
        {
            try
            {
                string[] file = Directory.GetFiles(sPath);

                for (int i = 0; i < file.Length; i++)
                {
                    FileInfo fi = new FileInfo(file[i]);
                    if (fi.CreationTime <= DateTime.Now.AddDays(-iDeleteDay)
                        || fi.LastWriteTime <= DateTime.Now.AddDays(-iDeleteDay))
                        fi.Delete();
                }
            }
            catch { }
        }
        #endregion

        private static void LogSave(bool bSend, string sMsg)
        {
            try
            {
                DateTime datetime = new DateTime();

                datetime = DateTime.Now;
                string sDate = datetime.ToString("yyyy/MM/dd");
                string sTime = datetime.ToString("HH:mm:ss");

                string sFileName = ProgramFolder.FolderMesLog + @"\MesLog_" + datetime.ToString("yyyyMMdd") + ".CSV";

                ProgramFolder.ProgramFolderCheck();  // 해당 폴더가 없는 경우 생성


                bool bFileFind = !File.Exists(sFileName);

                System.IO.StreamWriter streamwriter = new System.IO.StreamWriter(sFileName, true, Encoding.Default);

                //if (bFileFind)
                //{
                //    streamwriter.WriteLine("일자,시간,구분,번호,내용,설명");
                //}

                string sSend = (bSend ? "E→A" : "A→E");

                string sWriteLine = sDate + "," + sTime + ","
                    + sSend + ","
                    + sMsg;

                streamwriter.WriteLine(sWriteLine);

                streamwriter.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    "ErrorLogSave ..." + ex.Message,
                    ProgramInformation.ProgramTitleVersion,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static void LogSave(bool bSend, string[] sMsg)
        {
            try
            {
                DateTime datetime = new DateTime();

                datetime = DateTime.Now;
                string sDate = datetime.ToString("yyyy/MM/dd");
                string sTime = datetime.ToString("HH:mm:ss");

                string sFileName = ProgramFolder.FolderMesLog + @"\MesLog_" + datetime.ToString("yyyyMMdd") + ".CSV";

                ProgramFolder.ProgramFolderCheck();  // 해당 폴더가 없는 경우 생성

                bool bFileFind = !File.Exists(sFileName);

                System.IO.StreamWriter streamwriter = new System.IO.StreamWriter(sFileName, true, Encoding.Default);

                //if (bFileFind)
                //{
                //    streamwriter.WriteLine("일자,시간,구분,번호,내용,설명");
                //}

                string sSend = (bSend ? "E→A" : "A→E");

                string sMessage = "";
                for (int i = 0; i < sMsg.Length; i++)
                {
                    if (i == 0) sMessage = sMsg[0];
                    else
                    {
                        int iRem = 0;
                        Math.DivRem(i, 2, out iRem);
                        if (iRem == 1) sMessage += ":" + sMsg[i];
                        else sMessage += "," + sMsg[i];
                    }
                }

                string sWriteLine = sDate + "," + sTime + ","
                    + sSend + ","
                    + sMessage;

                streamwriter.WriteLine(sWriteLine);

                streamwriter.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    "ErrorLogSave ..." + ex.Message,
                    ProgramInformation.ProgramTitleVersion,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public static void ExecuteLinkAgent()
        {
            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName.Equals("LinkAgent"))
                {
                    process.Kill();
                }
            }

            try
            {
                Process.Start(mLINKAGENT_FOLDER);
            }
            catch
            {
                MessageBox.Show("Not Available LinkAgent Processing File at " + mLINKAGENT_FOLDER + "!", "Error");
            }
        }
    }
}
